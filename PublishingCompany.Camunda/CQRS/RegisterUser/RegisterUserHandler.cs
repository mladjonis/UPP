using AutoMapper;
using Camunda.Api.Client.UserTask;
using MediatR;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.Domain;
using PublishingCompany.Camunda.DTO;
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
        private readonly BpmnService _bpmnService;


        public RegisterUserHandler(IMapper mapper, IUnitOfWork unitOfWork, IUserValidator userValidator, BpmnService bpmnService)
        {
            this._mapper = mapper;
            this._unitOfWork = unitOfWork;
            this._userValidator = userValidator;
            this._bpmnService = bpmnService;
        }

        public async Task<RegisterUserResponse> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var processId = await _bpmnService.StartWriterRegistrationProcess();

            var task = await _bpmnService.GetFirstTask(processId);
            var formData = await _bpmnService.GetFormData(processId, task.Id);
            var registerUserResponse = _mapper.Map<RegisterUserResponse>(formData);
            //var ss = await _bpmnService.ClaimTask(task.Id, "demo");
            //var aa = _bpmnService.GetRealUserTask(task.Id);
            //await aa.SubmitForm(new CompleteTask());

            //_unitOfWork.Users.Add(_mapper.Map<User>(request));

            return registerUserResponse;
        }
    }
}
