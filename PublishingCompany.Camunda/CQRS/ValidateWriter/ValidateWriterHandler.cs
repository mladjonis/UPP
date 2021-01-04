using MediatR;
using PublishingCompany.Camunda.BPMN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.ValidateWriter
{
    public class ValidateWriterHandler : IRequestHandler<ValidateWriterRequest, ValidateWriterResponse>
    {
        private readonly BpmnService _bpmnService;

        public ValidateWriterHandler(BpmnService bpmnService)
        {
            _bpmnService = bpmnService;
        }

        public async Task<ValidateWriterResponse> Handle(ValidateWriterRequest request, CancellationToken cancellationToken)
        {
            ValidateWriterResponse writerResponse = new ValidateWriterResponse();
            var processInstance = _bpmnService.GetProcessInstanceResource(request.ProcessInstanceId);

            var registrationValidationVariable = await processInstance.Variables.Get(request.RegistrationVariableName);
            var registrationValidationVariableValue = registrationValidationVariable.GetValue<bool>();
            writerResponse.RegistrationStatus = registrationValidationVariableValue.ToString();

            return writerResponse;
        }
    }
}
