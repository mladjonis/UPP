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
    [HandlerTopics("Topic_NotifyPlagiarismProposalUserHandler", LockDuration = 10_000)]
    public class NotifyPlagiarismProposalUserHandler : ExternalTaskHandler
    {
        private readonly BpmnService _bpmnService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public NotifyPlagiarismProposalUserHandler(BpmnService bpmnService, IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _bpmnService = bpmnService;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async override Task<IExecutionResult> Process(ExternalTask externalTask)
        {
            try
            {
                var processInstanceResource = _bpmnService.GetProcessInstanceResource(externalTask.ProcessInstanceId);
                var bookName = processInstanceResource.Variables.Get("book_headline").Result.GetValue<string>();
                var writerName = processInstanceResource.Variables.Get("writer_name").Result.GetValue<string>();
                var loggedUsername = processInstanceResource.Variables.Get("loggedUsername").Result.GetValue<string>();

                var writer = _unitOfWork.Users.GetUserByName(writerName);
                var plagiarismProposalWriter = _unitOfWork.Users.GetUserByUsername(loggedUsername);
                var book = _unitOfWork.Books.GetByName(bookName);

                if (writer == null || book == null || plagiarismProposalWriter == null)
                {
                    return new CompleteResult()
                    {
                        Variables = new Dictionary<string, Variable>
                        {
                            ["NotifyPlagiarismProposalUserHandler"] = new Variable("Error something went wrong writer or book or proposal writer was not found", VariableType.String)
                        }
                    };
                }
                await _emailService.SendAsync(plagiarismProposalWriter.Email, "Book was not plagiarism", $"Book that you proposed ${bookName} by ${writerName} was not plagiarism.");
            }
            catch (Exception e)
            {
                return new CompleteResult()
                {
                    Variables = new Dictionary<string, Variable>
                    {
                        ["NotifyPlagiarismProposalUserHandler"] = new Variable(e.Message, VariableType.String)
                    }
                };
            }
            return new CompleteResult() { };
        }
    }
}
