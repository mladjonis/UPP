using Camunda.Worker;
using Microsoft.AspNetCore.Identity;
using NETCore.MailKit.Core;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Handlers
{
    [HandlerTopics("Topic_NotifyUserHandler", LockDuration = 10_000)]
    public class NotifyUserHandler : ExternalTaskHandler
    {
        private readonly BpmnService _bpmnService;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;

        public NotifyUserHandler(IEmailService emailService, BpmnService bpmnService, UserManager<User> _userManager)
        {
            this._bpmnService = bpmnService;
            this._emailService = emailService;
        }

        public async override Task<IExecutionResult> Process(ExternalTask externalTask)
        {
            try
            {
                var processInstanceResource = _bpmnService.GetProcessInstanceResource(externalTask.ProcessInstanceId);
                //izvuci varijablu
                var userEmail = processInstanceResource.Variables.Get("userEmail").Result.GetValue<string>();
                var user = await _userManager.FindByEmailAsync(userEmail);
                var link = "http://localhost:3000/upload";
                await _emailService.SendAsync(userEmail, $"{externalTask.Variables["message"].Value}", $"<a href=\"{link}\">Go to</a>", true);
            }
            catch (Exception e)
            {
                return new CompleteResult()
                {
                    Variables = new Dictionary<string, Variable>
                    {
                        ["UserApprovalError"] = new Variable(e.Message, VariableType.String)
                    }
                };
            }
            return new CompleteResult() { };
        }
    }
}
