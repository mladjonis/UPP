using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PublishingCompany.Camunda.BPMN;
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
        private readonly BpmnService bpmnServiceTasks;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, BpmnService bpmnService)
        {
            _logger = logger;
            bpmnServiceTasks = bpmnService;
        }

        [HttpGet("asa")]
        public async Task<ActionResult> Get()
        {
            var asa = await bpmnServiceTasks.GetFormData("Process_Probe_12", "registration_task");
            return Ok(asa);
        }
    }
}
