using Camunda.Worker;
using Microsoft.AspNetCore.Identity;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Handlers
{
    [HandlerTopics("Topic_RegistrationFinishHandler", LockDuration = 10_000)]
    public class RegistrationFinishHandler : ExternalTaskHandler
    {
        private readonly BpmnService _bpmnService;
        private readonly UserManager<User> _userManager;

        public RegistrationFinishHandler(UserManager<User> userManager, BpmnService bpmnService)
        {
            _bpmnService = bpmnService;
            _userManager = userManager;
        }

        public async override Task<IExecutionResult> Process(ExternalTask externalTask)
        {
            try
            {
                var processInstanceResource = _bpmnService.GetProcessInstanceResource(externalTask.ProcessInstanceId);
                var userEmail = processInstanceResource.Variables.Get("userEmail").Result.GetValue<string>();
                var user = await _userManager.FindByEmailAsync(userEmail);
                var token = processInstanceResource.Variables.Get("token").Result.GetValue<string>();
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (!result.Succeeded)
                {
                    return new CompleteResult()
                    {
                        Variables = new Dictionary<string, Variable>
                        {
                            ["RegistrationFinishError"] = new Variable(result.Errors, VariableType.Object)
                        }
                    };
                    //bla bla 
                }
            }catch(Exception e)
            {
                return new CompleteResult()
                {
                    Variables = new Dictionary<string, Variable>
                    {
                        ["RegistrationFinishError"] = new Variable(e.Message, VariableType.Object)
                    }
                };
            }
            return new CompleteResult()
            {
                Variables = new Dictionary<string, Variable>
                {
                    ["RegistrationFinishError"] = new Variable("", VariableType.String)
                }
            };

        }
    }
}
