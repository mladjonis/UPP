using AutoMapper;
using Camunda.Worker;
using MediatR;
using Microsoft.AspNetCore.Identity;
using NETCore.MailKit.Core;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.Domain;
using PublishingCompany.Camunda.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Handlers
{
    [HandlerTopics("Topic_RegistrationEmailSendHandler", LockDuration = 10_000)]
    public class RegistrationEmailSendHandler : ExternalTaskHandler
    {
        private readonly BpmnService _bpmnService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;

        public RegistrationEmailSendHandler(IMapper mapper, IMediator mediator, IUnitOfWork unitOfWork, IEmailService emailService, BpmnService bpmnService, UserManager<User> userManager)
        {
            this._mapper = mapper;
            this._mediator = mediator;
            this._unitOfWork = unitOfWork;
            this._bpmnService = bpmnService;
            this._userManager = userManager;
            this._emailService = emailService;
        }

        public async override Task<IExecutionResult> Process(ExternalTask externalTask)
        {
            try
            {
                var processInstanceResource = _bpmnService.GetProcessInstanceResource(externalTask.ProcessInstanceId);
                //izvuci varijablu
                var userEmail = processInstanceResource.Variables.Get("userEmail").Result.GetValue<string>();
                var user = await _userManager.FindByEmailAsync(userEmail);
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await _bpmnService.SetProcessVariableByProcessInstanceId("token", externalTask.ProcessInstanceId, token);
                var confirmationLink = BuildQueryString(user.Id, token);
                await _emailService.SendAsync(userEmail, "Email verification", $"<a href=\"{confirmationLink}\">Verify</a>", true);
            }
            catch(Exception e)
            {
                return new CompleteResult()
                {
                    Variables = new Dictionary<string, Variable>
                    {
                        ["WriterEmailRegistrationError"] = new Variable(e.Message, VariableType.String)
                    }
                };
            }
            return new CompleteResult() { };
        }

        private Uri BuildQueryString(Guid userId, string token)
        {
            var url = new UriBuilder("http://localhost:3000/email-confirmation");

            url.Query = new FormUrlEncodedContent(new Dictionary<string, string>()
                    {
                        {"userId", userId.ToString()},
                        {"token", token},
                    }).ReadAsStringAsync().Result;
            return url.Uri;
        }
    }
}
