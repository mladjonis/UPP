using AutoMapper;
using Camunda.Api.Client.UserTask;
using MediatR;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.Domain;
using PublishingCompany.Camunda.DTO;
using PublishingCompany.Camunda.Helpers.FormSubmitMapper;
using PublishingCompany.Camunda.Repositories;
using PublishingCompany.Camunda.Repositories.Interfaces;
using PublishingCompany.Camunda.Validators.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.RegisterUser
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserRequest, RegisterUserResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserValidator _userValidator;
        private readonly IMapper _mapper;
        private readonly IFormSubmitDtoMapper _formMapper;
        private readonly BpmnService _bpmnService;

        public RegisterUserHandler(IFormSubmitDtoMapper formMapper, IMapper mapper, IUnitOfWork unitOfWork, IUserValidator userValidator, BpmnService bpmnService)
        {
            this._mapper = mapper;
            this._unitOfWork = unitOfWork;
            this._userValidator = userValidator;
            this._bpmnService = bpmnService;
            this._formMapper = formMapper;
        }

        public async Task<RegisterUserResponse> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var mappedValues = _formMapper.MapDtoToDictionary(request.SubmitFields);

            var task = await _bpmnService.GetTaskById(request.TaskId, request.ProcessInstanceId);
            var procInstanceId = task.ProcessDefinitionId;
            //await _bpmnService.SetProcessVariableByProcessInstanceId(procInstanceId, mappedValues);
            var taskResource = await _bpmnService.GetUserTaskResource(request.TaskId);

            try
            {
                //morace ovako da se postavljaju varijable iz mapiranih vrednosti da se ponovo deserijalizuju...
                await taskResource.SubmitForm(new CompleteTask().SetVariable("username","dsfsdfsdfsdf"));
            }catch(Exception e)
            {
                //logger
            }

            return new RegisterUserResponse();
        }
    }
}
