﻿using AutoMapper;
using Camunda.Worker;
using Microsoft.AspNetCore.Identity;
using NETCore.MailKit.Core;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.Domain;
using PublishingCompany.Camunda.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Handlers
{
    [HandlerTopics("Topic_UserApprovalHandler", LockDuration = 10_000)]
    public class UserApprovalHandler : ExternalTaskHandler
    {
        private readonly BpmnService _bpmnService;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public UserApprovalHandler(IMapper mapper, IUnitOfWork unitOfWork, IEmailService emailService, BpmnService bpmnService, UserManager<User> userManager)
        {
            this._mapper = mapper;
            this._unitOfWork = unitOfWork;
            this._bpmnService = bpmnService;
            this._emailService = emailService;
            this._userManager = userManager;
        }

        public async override Task<IExecutionResult> Process(ExternalTask externalTask)
        {
            try
            {
                var processInstanceResource = _bpmnService.GetProcessInstanceResource(externalTask.ProcessInstanceId);
                //izvuci varijablu
                var userEmail = processInstanceResource.Variables.Get("userEmail").Result.GetValue<string>();
                var user = await _userManager.FindByEmailAsync(userEmail);
                user.ApprovalStatus = Domain.Enums.ApprovalStatus.Approved;
                _unitOfWork.Users.Update(user);
                _unitOfWork.Complete();
            }
            catch (Exception e)
            {
                return new CompleteResult()
                {
                    Variables = new Dictionary<string, Variable>
                    {
                        ["UserApprovalError"] = new Variable(e.Message, VariableType.String)
                    }
                };
            }
            return new CompleteResult() { };
        }
    }
}
