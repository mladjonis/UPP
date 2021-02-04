using Camunda.Worker;
using NETCore.MailKit.Core;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Handlers
{
    [HandlerTopics("Topic_NotifyMainEditorHandler", LockDuration = 10_000)]
    public class NotifyMainEditorHandler : ExternalTaskHandler
    {
        private readonly BpmnService _bpmnService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public NotifyMainEditorHandler(BpmnService bpmnService, IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _bpmnService = bpmnService;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public override Task<IExecutionResult> Process(ExternalTask externalTask)
        {
            try
            {
                var processInstanceResource = _bpmnService.GetProcessInstanceResource(externalTask.ProcessInstanceId);
                var username = processInstanceResource.Variables.Get("main_editor_username").Result.GetValue<string>();
                var user = _unitOfWork.Users.GetUserByUsername(username);
                var link = "http://localhost:3000/choose-editors";
                await _emailService.SendAsync(user.Email, $"Notify main editor", $"{externalTask.Variables["message"].Value}", $"Go to select new editors <a href=\"{link}\">Go</a>", true);
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
