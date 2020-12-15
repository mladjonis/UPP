using PublishingCompany.Camunda.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Helpers.FormSubmitMapper
{
    public interface IFormSubmitDtoMapper
    {
        string MapDtoToDictionary(List<FormSubmitDto> submitDtos);
    }
}
