using Camunda.Api.Client.UserTask;
using PublishingCompany.Camunda.Domain;
using PublishingCompany.Camunda.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Helpers.FormSubmitMapper
{
    public interface IFormSubmitDtoMapper
    {
        string MapDtoToString(List<FormSubmitDto> submitDtos);
        Dictionary<string, string> DeserializeMappedDtos(string dtoValues);
        CompleteTask SetFormValues(Dictionary<string, string> dtoKvp);
        CompleteTask GetFormValues(List<FormSubmitDto> submitDtos);
        UserDto MapFormDataToUserDto(List<FormSubmitDto> submitDtos);
    }
}
