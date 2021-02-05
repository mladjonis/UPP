using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.SubmitFormDataGeneric
{
    public class SubmitFormDataGenericResponse
    {
        public string ProcessInstanceId { get; set; }
        public string Status { get; set; }
    }
}
