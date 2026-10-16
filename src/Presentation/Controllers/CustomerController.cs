using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public ActionResult<CustomerResponse> Create([FromBody] CreateCustomerRequest request) //revisar
        {
            try
            {
                var customer = customerService.Create(request);

                return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer); //explicacion: setea un id y el email y pass?
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

        [HttpGet]
        public ActionResult<IReadOnlyList<CustomerResponse>> GetAll()
        {
            var customers = customerService.GetAll();

            if (!customers.Any())
            {
                return NotFound("No hay clientes registrados.");
            }

            return Ok(customers);
        }

        [HttpGet("{id}")]
        public ActionResult<CustomerResponse> GetById([FromRoute] Guid id)
        {
            var customer = customerService.GetById(id);

            if (customer == null)
            {
                return NotFound($"No existe un cliente con el id {id}.");
            }

            return Ok(customer);
        }
    }
}