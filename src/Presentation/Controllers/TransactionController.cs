using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    
    [ApiController]
    [Route("api/Account/{accountId}/Transaction")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService transactionService;
        public TransactionController(ITransactionService transactionService)
        {
            this.transactionService = transactionService;
        }

        [HttpPost]
        public ActionResult<TransactionResponse> Register([FromRoute]Guid accountId,[FromBody] CreateTransactionRequest request)
        {
            try
            {
                var transaction = transactionService.Register(accountId, request);
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
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<TransactionResponse>>  GetByAccount([FromRoute] Guid accountId)
        {
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
    }
}