using MediatR;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.Helpers.FormSubmitMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.RegisterReader
{
    public class RegisterReaderHandler : IRequestHandler<RegisterReaderRequest, RegisterReaderResponse>
    {
        private readonly IFormSubmitDtoMapper _formMapper;
        private readonly BpmnService _bpmnService;

        public RegisterReaderHandler(IFormSubmitDtoMapper formMapper, BpmnService bpmnService)
        {
            this._bpmnService = bpmnService;
            this._formMapper = formMapper;
        }

        public async Task<RegisterReaderResponse> Handle(RegisterReaderRequest request, CancellationToken cancellationToken)
        {
            var registerUserResponse = new RegisterReaderResponse() { ProcessInstanceId = request.ProcessInstanceId };
            try
            {
                var taskFormValues = _formMapper.GetFormValues(request.SubmitFields);
                var task = await _bpmnService.GetTaskById(request.TaskId, request.ProcessInstanceId);
                var taskResource = await _bpmnService.GetUserTaskResource(request.TaskId);
                await _bpmnService.SetProcessVariableByProcessInstanceId("betaReader", request.ProcessInstanceId, (bool)request.SubmitFields.Where(x=>x.FieldId.Equals("beta_reader")).FirstOrDefault().FieldValue);
                await taskResource.SubmitForm(taskFormValues);
                registerUserResponse.ProcessInstanceId = request.ProcessInstanceId;
                // set sent values to processInstanceVariables
                await _bpmnService.SetProcessVariableByProcessInstanceId("readerRegistrationValues", request.ProcessInstanceId, request.SubmitFields);
            }
            catch (Exception e)
            {
                //logger potencijalno
                registerUserResponse.RegistrationStatus = e.Message;
                registerUserResponse.ProcessInstanceId = request.ProcessInstanceId;
            }
            return registerUserResponse;
        }
    }
}
