using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.CQRS.PlagiarismProposal;
using PublishingCompany.Camunda.Domain;
using PublishingCompany.Camunda.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlagiarismController : ControllerBase
    {
        private readonly BpmnService _bpmnService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IMediator _mediator;

        public PlagiarismController(BpmnService bpmnService, IUnitOfWork unitOfWork, UserManager<User> userManager, IMediator mediator)
        {
            _bpmnService = bpmnService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mediator = mediator;
        }

        [HttpPost("SubmitProposal")]
        public async Task<ActionResult> SubmitCometeeForm(PlagiarismProposalRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}
