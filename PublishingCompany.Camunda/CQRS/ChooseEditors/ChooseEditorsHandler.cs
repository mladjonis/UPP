using MediatR;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.Helpers.FormSubmitMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.ChooseEditors
{
    public class ChooseEditorsHandler : IRequestHandler<ChooseEditorsRequest,ChooseEditorsResponse>
    {
        private readonly IFormSubmitDtoMapper _formMapper;
        private readonly BpmnService _bpmnService;

        public ChooseEditorsHandler(IFormSubmitDtoMapper formMapper, BpmnService bpmnService)
        {
            _formMapper = formMapper;
            _bpmnService = bpmnService;
        }

        public async Task<ChooseEditorsResponse> Handle(ChooseEditorsRequest request, CancellationToken cancellationToken)
        {
            var taskFormValues = _formMapper.GetFormValues(request.SubmitFields);
            var chooseEditorsResponse = new ChooseEditorsResponse() { ProcessInstanceId = request.ProcessInstanceId };
            try
            {
                var task = await _bpmnService.GetTaskById(request.TaskId, request.ProcessInstanceId);
                var taskResource = await _bpmnService.GetUserTaskResource(request.TaskId);
                await taskResource.SubmitForm(taskFormValues);
                chooseEditorsResponse.ProcessInstanceId = request.ProcessInstanceId;
                // set sent values to processInstanceVariables
                await _bpmnService.SetProcessVariableByProcessInstanceId("chosenEditors", request.ProcessInstanceId, request.SubmitFields);
            }
            catch (Exception e)
            {
                //logger potencijalno
                chooseEditorsResponse.Status = e.Message;
                chooseEditorsResponse.ProcessInstanceId = request.ProcessInstanceId;
            }

            return chooseEditorsResponse;
        }
    }
}
