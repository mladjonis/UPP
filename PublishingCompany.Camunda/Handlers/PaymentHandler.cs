using Camunda.Worker;
using Microsoft.AspNetCore.Identity;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.Domain;
using PublishingCompany.Camunda.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Handlers
{
    [HandlerTopics("Topic_PaymentHandler", LockDuration = 10_000)]
    public class PaymentHandler : ExternalTaskHandler
    {
        private readonly BpmnService _bpmnService;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentHandler(BpmnService bpmnService, UserManager<User> userManager, IUnitOfWork unitOfWork)
        {
            _bpmnService = bpmnService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async override Task<IExecutionResult> Process(ExternalTask externalTask)
        {
            try
            {
                var processInstanceResource = _bpmnService.GetProcessInstanceResource(externalTask.ProcessInstanceId);
                var userEmail = processInstanceResource.Variables.Get("userEmail").Result.GetValue<string>();
                var user = await _userManager.FindByEmailAsync(userEmail);
                if(user == null)
                {
                    return new CompleteResult()
                    {
                        Variables = new Dictionary<string, Variable>
                        {
                            ["PaymentError"] = new Variable("User does not exists", VariableType.String)
                        }
                    };
                }
                var price = processInstanceResource.Variables.Get("price").Result.GetValue<long>();
                if(user.Amount < price)
                {
                    user.Amount -= price;
                    _unitOfWork.Users.Update(user);
                    _unitOfWork.Complete();
                    await _bpmnService.SetProcessVariableByProcessInstanceId("paymentStatus", externalTask.ProcessInstanceId, "Success");
                }else
                {
                    await _bpmnService.SetProcessVariableByProcessInstanceId("paymentStatus", externalTask.ProcessInstanceId, "Failed not enough money");
                }
            }
            catch (Exception e)
            {
                return new CompleteResult()
                {
                    Variables = new Dictionary<string, Variable>
                    {
                        ["PaymentError"] = new Variable(e.Message, VariableType.String)
                    }
                };
            }
            return new CompleteResult() { };
        }
    }
}
