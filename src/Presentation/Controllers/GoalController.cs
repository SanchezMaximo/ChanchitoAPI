using Application.DTOs;
using Application.Interfaces;
using Domain.Constants;
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