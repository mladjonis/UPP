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

        public NotifyUserHandler(IEmailService emailService, BpmnService bpmnService, UserManager<User> userManager)
        {
            this._bpmnService = bpmnService;
            this._emailService = emailService;
            this._userManager = userManager;
        }

        public async override Task<IExecutionResult> Process(ExternalTask externalTask)
        {
            try
            {
                var processInstanceResource = _bpmnService.GetProcessInstanceResource(externalTask.ProcessInstanceId);
                //izvuci varijablu
                var userEmail = processInstanceResource.Variables.Get("userEmail").Result.GetValue<string>();
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (externalTask.Variables["message"].Value.ToString().Contains("material"))
                {
                    var link = "http://localhost:3000/upload-more";
                    await _emailService.SendAsync(userEmail, $"{externalTask.Variables["message"].Value}", $"<a href=\"{link}\">Go to</a>", true);
                }
                else if (externalTask.Variables["message"].Value.ToString().Contains("approved"))
                {
                    var link = "http://localhost:3000/payment";
                    await _emailService.SendAsync(userEmail, $"{externalTask.Variables["message"].Value}", $"Follow link for payment <a href=\"{link}\">Go to</a>", true);
                }
                else if (externalTask.Variables["message"].Value.ToString().Contains("payment"))
                {
                    var link = "http://localhost:3000/";
                    var paymentStatus = processInstanceResource.Variables.Get("paymentStatus").Result.GetValue<string>();
                    await _emailService.SendAsync(userEmail, $"{externalTask.Variables["message"].Value} + {paymentStatus}", $"Home page <a href=\"{link}\">Go to home</a>", true);
                }
                else if (externalTask.Variables["message"].Value.ToString().Contains("editor"))
                {
                    var link = "http://localhost:3000/books";
                    await _emailService.SendAsync(userEmail, $"{externalTask.Variables["message"].Value}", $"Books page <a href=\"{link}\">Go to books</a>", true);
                }
                else if (externalTask.Variables["message"].Value.ToString().Contains("Send all writing"))
                {
                    var link = "http://localhost:3000/send-writing";
                    await _emailService.SendAsync(userEmail, $"{externalTask.Variables["message"].Value}", $"Send writing page <a href=\"{link}\">Go to send writing</a>", true);
                }
                else
                {
                    await _emailService.SendAsync(userEmail, $"Notify user", $"{externalTask.Variables["message"].Value}");
                }
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
