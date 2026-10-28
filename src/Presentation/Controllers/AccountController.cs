using Application.DTOs;
using Application.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Customer)]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;
        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpPost]
        public ActionResult<AccountResponse> Create([FromBody] CreateAccountRequest request)
        {
            try
            {
                // The owner is always the logged-in customer, whatever CustomerId the body says.
                var createdAccount = accountService.Create(request with { CustomerId = User.GetUserId() });
                return CreatedAtAction(nameof(GetById), new { id = createdAccount.Id }, createdAccount);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

        }

        [HttpGet("{id}")]
        public ActionResult<AccountResponse> GetById([FromRoute] Guid id)
        {
            var account = accountService.GetById(id);
            if (account is null)
                return NotFound($"There is no account with id {id}.");

            // A customer can only see their own accounts.
            if (account.CustomerId != User.GetUserId())
                return Forbid();

            return Ok(account);

        }

        [HttpGet]
        public ActionResult<IReadOnlyList<AccountResponse>> GetAll()
        {
            var userId = User.GetUserId();

            // Only the accounts of the logged-in customer.
            var accounts = accountService.GetAll()
                .Where(a => a.CustomerId == userId)
                .ToList();

            return Ok(accounts);
        }

        [HttpPatch("{id}")]
        public ActionResult<AccountResponse> Update([FromRoute] Guid id, [FromBody] UpdateAccountRequest request)
        {
            var account = accountService.GetById(id);
            if (account is null)
                return NotFound($"There is no account with id {id}.");

            if (account.CustomerId != User.GetUserId())
                return Forbid();

            try
            {
                var updatedAccount = accountService.Update(id, request);
                return Ok(updatedAccount);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
