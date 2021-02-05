using Camunda.Worker;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.Domain;
using PublishingCompany.Camunda.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Handlers
{
    [HandlerTopics("Topic_ChoosePossiblePlagiatedHandler", LockDuration = 10_000)]
    public class ChoosePossiblePlagiatedHandler : ExternalTaskHandler
    {
        private readonly BpmnService _bpmnService;
        private readonly IUnitOfWork _unitOfWork;

        public ChoosePossiblePlagiatedHandler(BpmnService bpmnService, IUnitOfWork unitOfWork)
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
                var book = _unitOfWork.Books.GetByName(bookName);
                var books = _unitOfWork.Books.GetAll().ToList();
                Random random = new Random();
                int plagiarismIndex = random.Next(0, books.Count);
                var plagiatedBook = books[plagiarismIndex];
                ///editors_books_to_review
                var toModelList = new List<Book>() { book, plagiatedBook };
                await _bpmnService.SetProcessVariableByProcessInstanceId("editors_books_to_review", externalTask.ProcessInstanceId, toModelList);
            }
            catch (Exception e)
            {
                return new CompleteResult()
                {
                    Variables = new Dictionary<string, Variable>
                    {
                        ["ChoosePossiblePlagiatedHandlerError"] = new Variable(e.Message, VariableType.String)
                    }
                };
            }
            return new CompleteResult() { };
        }
    }
}
