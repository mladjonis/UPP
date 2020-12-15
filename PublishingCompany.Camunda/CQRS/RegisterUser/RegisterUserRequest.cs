using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PublishingCompany.Camunda.DTO;

namespace PublishingCompany.Camunda.CQRS.RegisterUser
{
    public class RegisterUserRequest : IRequest<RegisterUserResponse>
    {
        public string TaskId { get; set; }
        public string ProcessInstanceId { get; set; }
        public string ProcessDefinitionId { get; set; }
        public List<FormSubmitDto> SubmitFields { get; set; }
    }
}
