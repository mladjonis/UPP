using AutoMapper;
using Camunda.Api.Client.UserTask;
using MediatR;
using Newtonsoft.Json;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.Domain;
using PublishingCompany.Camunda.DTO;
using PublishingCompany.Camunda.Helpers.FormSubmitMapper;
using PublishingCompany.Camunda.Repositories;
using PublishingCompany.Camunda.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.RegisterUser
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserRequest, RegisterUserResponse>
    {
        private readonly IFormSubmitDtoMapper _formMapper;
        private readonly BpmnService _bpmnService;

        public RegisterUserHandler(IFormSubmitDtoMapper formMapper, BpmnService bpmnService)
        {
            this._bpmnService = bpmnService;
            this._formMapper = formMapper;
        }

        public async Task<RegisterUserResponse> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var taskFormValues = _formMapper.GetFormValues(request.SubmitFields);
            var registerUserResponse = new RegisterUserResponse() { ProcessInstanceId = request.ProcessInstanceId };
            try
            {
                var task = await _bpmnService.GetTaskById(request.TaskId, request.ProcessInstanceId);
                var taskResource = await _bpmnService.GetUserTaskResource(request.TaskId);
                //morace ovako da se postavljaju varijable iz mapiranih vrednosti da se ponovo deserijalizuju...
                //await taskResource.SubmitForm(new CompleteTask().SetVariable("username","dsfsdfsdfsdf"));
                //DONE ali neka opisa
                //await taskResource.Claim("username");
                await taskResource.SubmitForm(taskFormValues);
                //ne moze ovde mora da ide nakon validacije, ali kako to??
                //registerUserResponse.RegistrationStatus = "Registration status pending, confirm email to finish";
                registerUserResponse.ProcessInstanceId = request.ProcessInstanceId;
                // set sent values to processInstanceVariables
                await _bpmnService.SetProcessVariableByProcessInstanceId("registrationValues", request.ProcessInstanceId, request.SubmitFields);
            }
            catch (Exception e)
            {
                //logger potencijalno
                registerUserResponse.RegistrationStatus = e.Message;
                registerUserResponse.ProcessInstanceId = request.ProcessInstanceId;
            }

            return registerUserResponse;
        }
    }
}
