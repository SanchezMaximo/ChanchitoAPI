using Application.DTOs;
using Application.Interfaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/exchange-rate")]
    [ApiController]
    [AllowAnonymous]
    public class ExchangeRateController : ControllerBase
    {
        private readonly IExchangeRateService exchangeRateService;

        public ExchangeRateController(IExchangeRateService exchangeRateService)
        {
            this.exchangeRateService = exchangeRateService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ExchangeRateResponse>>> GetCurrent(CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await exchangeRateService.GetCurrentAsync(cancellationToken));
            }
            catch (ExchangeRateUnavailableException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
        }
    }
}
