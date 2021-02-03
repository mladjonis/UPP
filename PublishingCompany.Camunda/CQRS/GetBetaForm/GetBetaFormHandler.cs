using AutoMapper;
using MediatR;
using PublishingCompany.Camunda.BPMN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.GetBetaForm
{
    public class GetBetaFormHandler : IRequestHandler<GetBetaFormRequest, GetBetaFormResponse>
    {
        private readonly IMapper _mapper;
        private readonly BpmnService _bpmnService;

        public GetBetaFormHandler(IMapper mapper, BpmnService bpmnService)
        {
            _mapper = mapper;
            _bpmnService = bpmnService;
        }

        public async Task<GetBetaFormResponse> Handle(GetBetaFormRequest request, CancellationToken cancellationToken)
        {
            var processDefinition = await _bpmnService.GetProcessDefinitionKey();

            //ovako se koristi kada ide po redu sa taskovima, a nama treba da uvek kada se udje na frontu da uvek izbaci odgovarajucu formu valjda?
            //var task = await _bpmnService.GetFirstTask(request.ProcessInstanceId);
            var formData = await _bpmnService.GetFormData(processDefinition.Key, request.TaskNameOrId);
            var registerUserResponse = _mapper.Map<GetBetaFormResponse>(formData);

            return registerUserResponse;
        }
    }
}
