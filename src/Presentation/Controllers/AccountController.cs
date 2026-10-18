using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
                var createdAccount = accountService.Create(request);
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
            try
            {
                var account = accountService.GetById(id);
                return Ok(account);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }

        }

        [HttpGet]
        public ActionResult<AccountResponse> GetAll()
        {
            var accounts = accountService.GetAll();
            return Ok(accounts);
        }

        [HttpPatch("{id}")]
        public ActionResult<AccountResponse> Update([FromRoute] Guid id, [FromBody] UpdateAccountRequest request)
        {
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