using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.CQRS.RegisterUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IMediator _mediator;
        private readonly BpmnService bpmnServiceTasks;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IMediator mediator, BpmnService bpmnService)
        {
            _logger = logger;
            _mediator = mediator;
            bpmnServiceTasks = bpmnService;
        }

        [HttpGet("asa")]
        public async Task<ActionResult> Get()
        {
            var asa = await bpmnServiceTasks.GetFormData("Process_Probe_12", "registration_task");
            //var response = await _mediator.Send(new RegisterUserRequest());
            return Ok(asa);
        }
    }
}
