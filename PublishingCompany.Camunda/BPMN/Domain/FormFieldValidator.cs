using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.BPMN.Domain
{
    public class FormFieldValidator
    {
        public FormFieldValidator()
        {

        }
        public FormFieldValidator(string validatorName, string validatorConfig)
        {
            ValidatorName = validatorName;
            ValidatorConfig = validatorConfig;
        }

        public string ValidatorName { get; set; }
        public string ValidatorConfig { get; set; }
    }
}
