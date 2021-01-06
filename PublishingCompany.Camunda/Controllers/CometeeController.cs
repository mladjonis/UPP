using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.CQRS.CometeeProcessing;
using PublishingCompany.Camunda.CQRS.GetDormData;
using PublishingCompany.Camunda.Domain;
using PublishingCompany.Camunda.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Controllers
{
    //[Authorize(Roles ="Cometee")]
    [Route("api/[controller]")]
    [ApiController]
    public class CometeeController : ControllerBase
    {
        private readonly BpmnService _bpmnService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IMediator _mediator;

        public CometeeController(BpmnService bpmnService, IUnitOfWork unitOfWork, UserManager<User> userManager, IMediator mediator)
        {
            _bpmnService = bpmnService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mediator = mediator;
        }

        [HttpGet("GetUsersToApprove")]
        public ActionResult<IEnumerable<User>> GetUsersToApprove()
        {
            var users = _unitOfWork.Users.Find(u => u.ApprovalStatus != Domain.Enums.ApprovalStatus.Approved && u.ApprovalStatus != Domain.Enums.ApprovalStatus.Rejected);
            return Ok(users);
        }

        [HttpGet("GetFormData")]
        public async Task<ActionResult> Get([FromQuery] GetFormDataRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("SubmitCometeeForm")]
        public async Task<ActionResult> SubmitCometeeForm(CometeeProcessingRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}
