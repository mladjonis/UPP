using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PublishingCompany.Camunda.CQRS.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Writer")]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("PaymentRequest")]
        public async Task<ActionResult> UploadDocuments([FromBody] PaymentRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}
