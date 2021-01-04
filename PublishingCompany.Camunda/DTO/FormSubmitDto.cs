using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.DTO
{
    public class FormSubmitDto
    {
        public FormSubmitDto()
        {

        }
        public string FieldId { get; set; }
        public object FieldValue { get; set; }
    }
}
