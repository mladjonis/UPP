using MediatR;
using PublishingCompany.Camunda.BPMN;
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
        private readonly IUserRepository _userRepository;
        private readonly IUserValidator _userValidator;
        private readonly BpmnService _bpmnService;

        public RegisterUserHandler(IUserRepository userRepository, IUserValidator userValidator, BpmnService bpmnService)
        {
            this._userRepository = userRepository;
            this._userValidator = userValidator;
            this._bpmnService = bpmnService;
        }

        public Task<RegisterUserResponse> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var checkUserExistance = _userValidator.UserExists(request.Email);
            RegisterUserResponse registerUserResponse = new RegisterUserResponse();
            return Task.FromResult(registerUserResponse);
        }
    }
}
