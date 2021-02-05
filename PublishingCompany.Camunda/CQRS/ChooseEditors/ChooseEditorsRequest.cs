using MediatR;
using PublishingCompany.Camunda.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.ChooseEditors
{
    public class ChooseEditorsRequest : IRequest<ChooseEditorsResponse>
    {
        public string TaskId { get; set; }
        public string ProcessInstanceId { get; set; }
        public string ProcessDefinitionId { get; set; }
        public List<FormSubmitDto> SubmitFields { get; set; }
    }
}
