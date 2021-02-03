using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.GetFormDataGeneric
{
    public class GetFormDataGenericRequest : IRequest<GetFormDataGenericResponse>
    {
        public string ProcessInstanceId { get; set; }
        public string TaskNameOrId { get; set; }
    }
}
