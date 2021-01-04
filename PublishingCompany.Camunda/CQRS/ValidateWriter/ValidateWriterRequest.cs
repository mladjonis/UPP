using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.ValidateWriter
{
    public class ValidateWriterRequest : IRequest<ValidateWriterResponse>
    {
        public string ProcessInstanceId { get; set; }
        public string RegistrationVariableName { get; set; }
    }
}
