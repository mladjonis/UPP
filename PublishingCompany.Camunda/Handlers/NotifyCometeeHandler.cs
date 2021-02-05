using AutoMapper;
using Camunda.Worker;
using NETCore.MailKit.Core;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.Domain;
using PublishingCompany.Camunda.DTO;
using PublishingCompany.Camunda.Helpers.FormSubmitMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Handlers
{
    [HandlerTopics("Topic_NotifyCometeeHandler", LockDuration = 10_000)]
    public class NotifyCometeeHandler : ExternalTaskHandler
    {
        private readonly BpmnService _bpmnService;
        private readonly IFormSubmitDtoMapper _dtoMapper;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public NotifyCometeeHandler(BpmnService bpmnService, IFormSubmitDtoMapper dtoMapper, IMapper mapper, IEmailService emailService)
        {
            _bpmnService = bpmnService;
            _dtoMapper = dtoMapper;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async override Task<IExecutionResult> Process(ExternalTask externalTask)
        {
            try
            {
                var processInstanceResource = _bpmnService.GetProcessInstanceResource(externalTask.ProcessInstanceId);
                var cometeeValues = await processInstanceResource.Variables.Get("cometees");
                var cometees = cometeeValues.GetValue<List<User>>();
                var link = "http://localhost:3000/decision-plagiarism";
                foreach (var cometee in cometees)
                {
                    _emailService.Send(cometee.Email, "Izabrani ste za clanove komesije za odluku za knjige koju je potrebno proveriti da li je plagijarizam", $"Vise informacija na linku <a href=\"{link}\">Go</a>", true);
                }
            }
            catch (Exception e)
            {
                return new CompleteResult()
                {
                    Variables = new Dictionary<string, Variable>
                    {
                        ["NotifyEditorsError"] = new Variable(e.Message, VariableType.String)
                    }
                };
            }

            return new CompleteResult()
            {
                Variables = new Dictionary<string, Variable>
                {
                    ["NotifyEditorsError"] = new Variable("", VariableType.String)
                }
            };
        }
    }
}
