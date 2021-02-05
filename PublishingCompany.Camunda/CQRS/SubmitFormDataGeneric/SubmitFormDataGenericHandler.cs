using MediatR;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.Helpers.FormSubmitMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.SubmitFormDataGeneric
{
    public class SubmitFormDataGenericHandler : IRequestHandler<SubmitFormDataGenericRequest, SubmitFormDataGenericResponse>
    {
        private readonly IFormSubmitDtoMapper _formMapper;
        private readonly BpmnService _bpmnService;

        public SubmitFormDataGenericHandler(IFormSubmitDtoMapper formMapper, BpmnService bpmnService)
        {
            _formMapper = formMapper;
            _bpmnService = bpmnService;
        }

        public async Task<SubmitFormDataGenericResponse> Handle(SubmitFormDataGenericRequest request, CancellationToken cancellationToken)
        {
            var taskFormValues = _formMapper.GetFormValues(request.SubmitFields);
            var submitFormDataGenericResponse = new SubmitFormDataGenericResponse() { ProcessInstanceId = request.ProcessInstanceId };
            try
            {
                var task = await _bpmnService.GetTaskById(request.TaskId, request.ProcessInstanceId);
                var taskResource = await _bpmnService.GetUserTaskResource(request.TaskId);
                await taskResource.SubmitForm(taskFormValues);
                submitFormDataGenericResponse.ProcessInstanceId = request.ProcessInstanceId;
            }
            catch (Exception e)
            {
                //logger potencijalno
                submitFormDataGenericResponse.Status = e.Message;
                submitFormDataGenericResponse.ProcessInstanceId = request.ProcessInstanceId;
            }
            return submitFormDataGenericResponse;
        }
    }
}
