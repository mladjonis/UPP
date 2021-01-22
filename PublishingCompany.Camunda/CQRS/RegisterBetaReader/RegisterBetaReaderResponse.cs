using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.RegisterBetaReader
{
    public class RegisterBetaReaderResponse
    {
        public string RegistrationStatus { get; set; }
        public string ProcessInstanceId { get; set; }
    }
}
