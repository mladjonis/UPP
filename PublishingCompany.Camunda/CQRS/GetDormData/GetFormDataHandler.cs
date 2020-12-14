using AutoMapper;
using MediatR;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.GetDormData
{
    public class GetFormDataHandler : IRequestHandler<GetFormDataRequest, GetFormDataResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly BpmnService _bpmnService;

        public GetFormDataHandler(IUnitOfWork unitOfWork, IMapper mapper, BpmnService bpmnService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _bpmnService = bpmnService;
        }

        public async Task<GetFormDataResponse> Handle(GetFormDataRequest request, CancellationToken cancellationToken)
        {
            var processId = await _bpmnService.StartWriterRegistrationProcess();
            var processDefinition = await _bpmnService.GetProcessDefinitionKey();

            var task = await _bpmnService.GetFirstTask(processId);
            var formData = await _bpmnService.GetFormData(processDefinition.Key, task.Name);
            var registerUserResponse = _mapper.Map<GetFormDataResponse>(formData);

            return registerUserResponse;
        }
    }
}
