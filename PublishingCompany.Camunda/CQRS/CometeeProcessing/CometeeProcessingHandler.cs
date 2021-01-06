using MediatR;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.Helpers.FormSubmitMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.CometeeProcessing
{
    public class CometeeProcessingHandler : IRequestHandler<CometeeProcessingRequest, CometeeProcessingResponse>
    {
        private readonly IFormSubmitDtoMapper _formMapper;
        private readonly BpmnService _bpmnService;

        public CometeeProcessingHandler(IFormSubmitDtoMapper formMapper, BpmnService bpmnService)
        {
            _formMapper = formMapper;
            _bpmnService = bpmnService;
        }

        public async Task<CometeeProcessingResponse> Handle(CometeeProcessingRequest request, CancellationToken cancellationToken)
        {
            var taskFormValues = _formMapper.GetFormValues(request.SubmitFields);
            var cometeeResponse = new CometeeProcessingResponse() { ProcessInstanceId = request.ProcessInstanceId };
            try
            {
                var task = await _bpmnService.GetTaskById(request.TaskId, request.ProcessInstanceId);
                var taskResource = await _bpmnService.GetUserTaskResource(request.TaskId);
                await taskResource.SubmitForm(taskFormValues);
                cometeeResponse.ProcessInstanceId = request.ProcessInstanceId;
            }
            catch (Exception e)
            {
                //logger potencijalno
                cometeeResponse.Status = e.Message;
                cometeeResponse.ProcessInstanceId = request.ProcessInstanceId;
            }
            return cometeeResponse;
        }
    }
}
