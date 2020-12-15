using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PublishingCompany.Camunda.CQRS.GetDormData;
using PublishingCompany.Camunda.CQRS.RegisterUser;
using PublishingCompany.Camunda.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IMediator _mediator;
        public RegistrationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetFormData")]
        public async Task<ActionResult> Get()
        {
            var response = await _mediator.Send(new GetFormDataRequest());
            return Ok(response);
        }

        [HttpPost("RegisterUser")]
        public async Task<ActionResult> RegisterUser(RegisterUserRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}
