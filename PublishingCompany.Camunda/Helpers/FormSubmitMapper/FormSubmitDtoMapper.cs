using PublishingCompany.Camunda.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Helpers.FormSubmitMapper
{
    public class FormSubmitDtoMapper : IFormSubmitDtoMapper
    {
        public string MapDtoToDictionary(List<FormSubmitDto> submitDtos)
        {
            var builder = new StringBuilder();
            foreach (var dto in submitDtos)
            {
                builder.Append($"{dto.FieldId},{dto.FieldValue}").AppendLine();
            }
            return builder.ToString();
        }
    }
}
