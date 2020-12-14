using AutoMapper;
using PublishingCompany.Camunda.CQRS.GetDormData;
using PublishingCompany.Camunda.CQRS.RegisterUser;
using PublishingCompany.Camunda.Domain;
using PublishingCompany.Camunda.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            //add mapings
            CreateMap<FormFieldsDto, GetFormDataResponse>();
        }
    }
}
