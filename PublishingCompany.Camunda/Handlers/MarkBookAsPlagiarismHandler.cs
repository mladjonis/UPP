using Camunda.Worker;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Handlers
{
    [HandlerTopics("Topic_MarkBookAsPlagiarismHandler", LockDuration = 10_000)]
    public class MarkBookAsPlagiarismHandler : ExternalTaskHandler
    {
        private readonly BpmnService _bpmnService;
        private readonly IUnitOfWork _unitOfWork;

        public MarkBookAsPlagiarismHandler(BpmnService bpmnService, IUnitOfWork unitOfWork)
        {
            _bpmnService = bpmnService;
            _unitOfWork = unitOfWork;
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
                book.IsPlagiarism = true;
                _unitOfWork.Books.Update(book);
                await _unitOfWork.CompleteAsync();

                if (writer == null || book == null )
                {
                    return new CompleteResult()
                    {
                        Variables = new Dictionary<string, Variable>
                        {
                            ["MarkBookAsPlagiarismHandler"] = new Variable("Error something went wrong writer or book or was not found", VariableType.String)
                        }
                    };
                }
            }
            catch (Exception e)
            {
                return new CompleteResult()
                {
                    Variables = new Dictionary<string, Variable>
                    {
                        ["MarkBookAsPlagiarismHandler"] = new Variable(e.Message, VariableType.String)
                    }
                };
            }
            return new CompleteResult() { };
        }
    }
}
