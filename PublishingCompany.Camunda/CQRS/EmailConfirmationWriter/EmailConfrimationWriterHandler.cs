using MediatR;
using PublishingCompany.Camunda.BPMN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.EmailConfirmationWriter
{
    public class EmailConfirmationWriterHandler : IRequestHandler<EmailConfirmationWriterRequest, EmailConfirmationWriterResponse>
    {
        private readonly BpmnService _bpmnService;

        public EmailConfirmationWriterHandler(BpmnService bpmnService)
        {
            _bpmnService = bpmnService;
        }

        public async Task<EmailConfirmationWriterResponse> Handle(EmailConfirmationWriterRequest request, CancellationToken cancellationToken)
        {
            var emailResponse = new EmailConfirmationWriterResponse();
            try
            {
                var processInstanceResource = _bpmnService.GetProcessInstanceResource(request.ProcessInstanceId);
                var task = await _bpmnService.GetFirstTask(request.ProcessInstanceId);

                var userEmail = processInstanceResource.Variables.Get("userEmail").Result.GetValue<string>();
                var camundaUser = await _bpmnService.GetUser(userEmail);
                //klejmuj i komplituj task posle ovoga
                //done he he 
                var claimedTask = await _bpmnService.ClaimTask(task.Id, camundaUser.Id);
                var completedTask = await _bpmnService.CompleteTask(task.Id);
                emailResponse.Status = "";
            }catch(Exception e)
            {
                //logger
                emailResponse.Status = e.Message;
            }
            return emailResponse;
        }
    }
}
