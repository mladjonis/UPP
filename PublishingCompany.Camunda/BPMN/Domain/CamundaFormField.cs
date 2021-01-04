using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.BPMN.Domain
{
    public class CamundaFormField
    {
        public string FormId { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public string DefaultValue { get; set; }
        public List<FormFieldValidator> Validators { get; set; } = new List<FormFieldValidator>();
        public List<FormFieldValues> Values { get; set; } = new List<FormFieldValues>();
    }
}
