using MediatR;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.Helpers.FormSubmitMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.Payment
{
    public class PaymentHandler : IRequestHandler<PaymentRequest, PaymentResponse>
    {
        private readonly IFormSubmitDtoMapper _formMapper;
        private readonly BpmnService _bpmnService;

        public async Task<PaymentResponse> Handle(PaymentRequest request, CancellationToken cancellationToken)
        {
            var paymentResponse = new PaymentResponse() { ProcessInstanceId = request.ProcessInstanceId };
            try
            {
                var taskFormValues = _formMapper.GetFormValues(request.SubmitFields);
                var task = await _bpmnService.GetTaskById(request.TaskId, request.ProcessInstanceId);
                var taskResource = await _bpmnService.GetUserTaskResource(request.TaskId);
                await taskResource.SubmitForm(taskFormValues);
                paymentResponse.ProcessInstanceId = request.ProcessInstanceId;
            }
            catch (Exception e)
            {
                //logger potencijalno
                paymentResponse.PaymentStatus = e.Message;
                paymentResponse.ProcessInstanceId = request.ProcessInstanceId;
            }
            return paymentResponse;
        }
    }
}
