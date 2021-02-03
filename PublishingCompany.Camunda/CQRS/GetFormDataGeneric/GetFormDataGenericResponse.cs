using PublishingCompany.Camunda.BPMN.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.GetFormDataGeneric
{
    public class GetFormDataGenericResponse
    {
        public string ProcessInstanceId { get; set; }
        public string ProcessDefinitionKey { get; set; }
        public string TaskId { get; set; }
        public string TaskName { get; set; }
        public string FormKey { get; set; }
        public List<CamundaFormField> CamundaFormFields { get; set; } = new List<CamundaFormField>();
    }
}
