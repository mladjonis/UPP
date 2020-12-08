using MediatR;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.RegisterUser
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserRequest, Unit>
    {
        private readonly IUserRepository userRepository;
        private readonly BpmnService bpmnService;

        public RegisterUserHandler(IUserRepository userRepository, BpmnService bpmnService)
        {
            this.userRepository = userRepository;
            this.bpmnService = bpmnService;
        }

        public Task<Unit> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
