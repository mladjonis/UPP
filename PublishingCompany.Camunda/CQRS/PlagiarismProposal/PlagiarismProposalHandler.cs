using MediatR;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.Helpers.FormSubmitMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.PlagiarismProposal
{
    public class PlagiarismProposalHandler : IRequestHandler<PlagiarismProposalRequest,PlagiarismProposalResponse>
    {
        private readonly IFormSubmitDtoMapper _formMapper;
        private readonly BpmnService _bpmnService;

        public PlagiarismProposalHandler(IFormSubmitDtoMapper formMapper, BpmnService bpmnService)
        {
            _formMapper = formMapper;
            _bpmnService = bpmnService;
        }

        public async Task<PlagiarismProposalResponse> Handle(PlagiarismProposalRequest request, CancellationToken cancellationToken)
        {
            var plagiarismProposalResponse = new PlagiarismProposalResponse() { ProcessInstanceId = request.ProcessInstanceId };
            try
            {
                var taskFormValues = _formMapper.GetFormValues(request.SubmitFields);
                var task = await _bpmnService.GetTaskById(request.TaskId, request.ProcessInstanceId);
                var taskResource = await _bpmnService.GetUserTaskResource(request.TaskId);
                await taskResource.SubmitForm(taskFormValues);
                plagiarismProposalResponse.ProcessInstanceId = request.ProcessInstanceId;
            }
            catch (Exception e)
            {
                //logger potencijalno
                plagiarismProposalResponse.RegistrationStatus = e.Message;
                plagiarismProposalResponse.ProcessInstanceId = request.ProcessInstanceId;
            }
            return plagiarismProposalResponse;
        }
    }
}
