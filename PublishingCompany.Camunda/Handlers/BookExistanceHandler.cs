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
    [HandlerTopics("Topic_NotifyUserHandler", LockDuration = 10_000)]
    public class BookExistanceHandler : ExternalTaskHandler
    {
        private readonly BpmnService _bpmnService;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public BookExistanceHandler(BpmnService bpmnService, IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _bpmnService = bpmnService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async override Task<IExecutionResult> Process(ExternalTask externalTask)
        {
            try
            {
                var processInstanceResource = _bpmnService.GetProcessInstanceResource(externalTask.ProcessInstanceId);
                var bookName = processInstanceResource.Variables.Get("book_headline").Result.GetValue<string>();
                var writerName = processInstanceResource.Variables.Get("writer_name").Result.GetValue<string>();

                var writer = _unitOfWork.Users.GetUserByName(writerName);
                var book = _unitOfWork.Books.GetByName(bookName);
                if(writer == null || book == null)
                {
                    await _bpmnService.SetProcessVariableByProcessInstanceId("book_exist", externalTask.ProcessInstanceId, false);
                    return new CompleteResult()
                    {
                        Variables = new Dictionary<string, Variable>
                        {
                            ["BookExistanceHandler"] = new Variable("Book or writer doesnt exist", VariableType.String)
                        }
                    };
                }
                await _bpmnService.SetProcessVariableByProcessInstanceId("book_exist", externalTask.ProcessInstanceId, true);
            }
            catch (Exception e)
            {
                return new CompleteResult()
                {
                    Variables = new Dictionary<string, Variable>
                    {
                        ["BookExistanceHandler"] = new Variable(e.Message, VariableType.String)
                    }
                };
            }
            return new CompleteResult() { };
        }
    }
}
