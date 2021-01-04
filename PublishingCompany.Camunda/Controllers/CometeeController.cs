using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.Domain;
using PublishingCompany.Camunda.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Controllers
{
    //[Authorize(Roles ="Writer")]
    [Route("api/[controller]")]
    [ApiController]
    public class CometeeController : ControllerBase
    {
        private readonly BpmnService _bpmnService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public CometeeController(BpmnService bpmnService, IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _bpmnService = bpmnService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [HttpGet("GetUsersToApprove")]
        public ActionResult<IEnumerable<User>> GetUsersToApprove()
        {
            var users = _unitOfWork.Users.Find(u => u.ApprovalStatus != Domain.Enums.ApprovalStatus.Approved && u.ApprovalStatus != Domain.Enums.ApprovalStatus.Rejected);
            return Ok(users);
        }

        [HttpPost("ClaimTask")]
        public ActionResult ClaimTask()
        {
            return Ok();
        }

        [HttpPost("CompleteTask")]
        public ActionResult CompleteTask()
        {
            return Ok();
        }
    }
}
