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
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService customerService;

        public CustomerController(ICustomerService customerService)
        {
            this.customerService = customerService;
        }

        // Public: anyone can register without a session.
        [HttpPost]
        [AllowAnonymous]
        public ActionResult<CustomerResponse> Create([FromBody] CreateCustomerRequest request) //revisar
        {
            try
            {
                var customer = customerService.Create(request);

                return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer); //devuelve 201 si salio bien
            }
            catch (ArgumentException ex) // valida que los datos sean correctos
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex) // valida que no exista un cliente con ese email
            {
                return Conflict(ex.Message);
            }
        }

        // Only administrators can list every customer.
        [HttpGet]
        [Authorize(Roles = Roles.Administrator)]
        public ActionResult<IReadOnlyList<CustomerResponse>> GetAll()
        {
            var customers = customerService.GetAll();

            if (!customers.Any())
            {
                return NotFound("No hay clientes registrados.");
            }

            return Ok(customers);
        }

        // A customer can only see their own data; an administrator can see anyone's.
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<CustomerResponse> GetById([FromRoute] Guid id)
        {
            if (!User.IsInRole(Roles.Administrator) && User.GetUserId() != id)
                return Forbid();

            var customer = customerService.GetById(id);

            if (customer == null)
            {
                return NotFound($"No existe un cliente con el id {id}.");
            }

            return Ok(customer);
        }
    }
}
