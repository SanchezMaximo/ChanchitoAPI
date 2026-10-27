using Application.DTOs;
using Application.Interfaces;
using Domain.Constants;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers
{
    
    [ApiController]
    [Authorize(Roles = Roles.Customer)]
    [Route("api/Account/{accountId}/Transaction")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService transactionService;
        private readonly IAccountService accountService;

        public TransactionController(ITransactionService transactionService, IAccountService accountService)
        {
            this.transactionService = transactionService;
            this.accountService = accountService;
        }

        [HttpPost]
        public async Task<ActionResult<TransactionResponse>> Register([FromRoute]Guid accountId,[FromBody] CreateTransactionRequest request, CancellationToken cancellationToken)
        {
            var denied = CheckOwnership(accountId);
            if (denied is not null)
                return denied;

            try
            {
                var transaction = await transactionService.RegisterAsync(accountId, request, cancellationToken);
                return CreatedAtAction(nameof(GetByAccount), new {accountId} , transaction);
            }
                catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (ExchangeRateUnavailableException ex) // DolarAPI is down and there is no saved rate
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<TransactionResponse>>  GetByAccount([FromRoute] Guid accountId)
        {
            var denied = CheckOwnership(accountId);
            if (denied is not null)
                return denied;

            var transactions = transactionService.GetByAccount(accountId);
            if (transactions is null)
            {
                return NotFound($"There is no account with id {accountId}.");
            }
            return Ok(transactions);
        }

        [HttpPatch("{transactionId}")]
        public ActionResult<TransactionResponse> ChangeAmount([FromRoute] Guid accountId, [FromRoute] Guid transactionId,  [FromBody] UpdateTransactionRequest request)
        {
            var denied = CheckOwnership(accountId);
            if (denied is not null)
                return denied;

            try
            {
                var transaction = transactionService.ChangeAmount(accountId, transactionId, request);
                return Ok(transaction);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpDelete("{transactionId}")]
        public ActionResult<TransactionResponse> Cancel([FromRoute] Guid accountId, [FromRoute] Guid transactionId)
        {
            var denied = CheckOwnership(accountId);
            if (denied is not null)
                return denied;

            try
            {
                var transaction = transactionService.Cancel(accountId, transactionId);
                return Ok(transaction);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        // Returns null when the account exists and belongs to the logged-in customer.
        // Otherwise returns the error to send back (404 if it does not exist, 403 if it is someone else's).
        private ActionResult? CheckOwnership(Guid accountId)
        {
            var account = accountService.GetById(accountId);

            if (account is null)
                return NotFound($"There is no account with id {accountId}.");

            if (account.CustomerId != User.GetUserId())
                return Forbid();

            return null;
        }
    }
}
