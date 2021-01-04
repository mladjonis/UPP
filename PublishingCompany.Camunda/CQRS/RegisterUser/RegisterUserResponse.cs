using PublishingCompany.Camunda.BPMN.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.RegisterUser
{
    public class RegisterUserResponse
    {
        public string RegistrationStatus { get; set; }
        public string ProcessInstanceId { get; set; }
    }
}
