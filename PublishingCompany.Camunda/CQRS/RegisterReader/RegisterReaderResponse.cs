using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.RegisterReader
{
    public class RegisterReaderResponse
    {
        public string RegistrationStatus { get; set; }
        public string ProcessInstanceId { get; set; }
    }
}
