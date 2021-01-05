using AutoMapper;
using Camunda.Api.Client.User;
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
            CreateMap<UserDto, User>()
                .ForMember(destination => destination.UserName, src => src.MapFrom(s => s.Username));
            CreateMap<User, UserProfileInfo>()
                .ForMember(destination => destination.Id, src => src.MapFrom(s => s.UserName))
                .ForMember(destination => destination.FirstName, src => src.MapFrom(s => s.Name))
                .ForMember(destination => destination.LastName, src => src.MapFrom(s => s.Lastname))
                .ForMember(destination => destination.Email, src => src.MapFrom(s => s.Email));

        }
    }
}
