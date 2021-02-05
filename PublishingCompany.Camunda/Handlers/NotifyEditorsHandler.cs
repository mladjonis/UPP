using AutoMapper;
using Camunda.Worker;
using NETCore.MailKit.Core;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.DTO;
using PublishingCompany.Camunda.Helpers.FormSubmitMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Handlers
{
    [HandlerTopics("Topic_NotifyEditorsHandler", LockDuration = 10_000)]
    public class NotifyEditorsHandler : ExternalTaskHandler
    {
        private readonly BpmnService _bpmnService;
        private readonly IFormSubmitDtoMapper _dtoMapper;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public NotifyEditorsHandler(BpmnService bpmnService, IFormSubmitDtoMapper dtoMapper, IMapper mapper, IEmailService emailService)
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
                var registrationValues = await processInstanceResource.Variables.Get("chosenEditors");
                var jArrayValue = (Newtonsoft.Json.Linq.JArray)registrationValues.Value;
                var submittedData = jArrayValue.ToObject<List<FormSubmitDto>>();
                var editorDto = _dtoMapper.MapFromDataToUserEditorDto(submittedData);
                await foreach(var editor in editorDto.Editors)
                {
                    await _emailService.SendAsync(editor.Email, "Izabrani ste za editora knjige koju je potrebno proveriti da li je plagijarizam", "PORUKA", true);
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
