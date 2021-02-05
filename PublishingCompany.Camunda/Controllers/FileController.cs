using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.CQRS.PdfUpload;
using PublishingCompany.Camunda.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Writer")]
    public class FileController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly UserManager<User> _userManager;
        private readonly BpmnService _bpmnService;

        public FileController(IMediator mediator, UserManager<User> userManager, BpmnService bpmnService)
        {
            _userManager = userManager;
            _mediator = mediator;
            _bpmnService = bpmnService;
        }

        [HttpPost("UploadDocuments")]
        public async Task<ActionResult> UploadDocuments([FromForm]PdfUploadRequest request)
        {
            var currentUser = await GetCurrentUserAsync();
            request.User = currentUser;
            request.ProcessInstanceId = _bpmnService.processInstanceId;
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        private async Task<User> GetCurrentUserAsync() => await _userManager.GetUserAsync(HttpContext.User);
    }
}
