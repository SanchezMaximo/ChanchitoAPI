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
    [Route("api/Goal")]
    public class GoalController : ControllerBase
    {

        private readonly IGoalService goalService;
        public GoalController(IGoalService goalService)
        {
            this.goalService = goalService;
        }

        [HttpPost]
        public ActionResult<GoalResponse> Create([FromBody] CreateGoalRequest request)
        {
            try
            {
                var createdGoal = goalService.Create(User.GetUserId(), request);
                return CreatedAtAction(nameof(GetById), new { id= createdGoal.Id },createdGoal);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

        }
        
        [HttpPost("{id}/contribute")]
        public async Task<ActionResult<GoalResponse>> Contribute(
            [FromRoute] Guid id,
            [FromBody] ContributeRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var goal = await goalService.ContributeAsync(User.GetUserId(), id, request, cancellationToken);
                return Ok(goal);
            }
            catch (ExchangeRateUnavailableException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpGet("{id}")]
        public ActionResult<GoalResponse> GetById([FromRoute] Guid id)
        {
            var goal = goalService.GetById(id);
            if (goal is null)
                return NotFound($"There is no goal with id {id}.");
            
            if (goal.CustomerId != User.GetUserId())
                return Forbid();
            
            return Ok(goal);
        }
        
        
        [HttpGet]
        public ActionResult<IReadOnlyList<GoalResponse>> GetAll()
        {
            var goals = goalService.GetByCustomerId(User.GetUserId());
            if (goals.Count == 0)
                return NotFound("You have no goals yet.");
            
            return Ok(goals);
        }
    }
}