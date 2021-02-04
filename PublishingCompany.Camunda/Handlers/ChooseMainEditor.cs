using Camunda.Worker;
using Microsoft.AspNetCore.Identity;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.Domain;
using PublishingCompany.Camunda.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Handlers
{
    [HandlerTopics("Topic_ChooseMainEditorHandler", LockDuration = 10_000)]
    public class ChooseMainEditor : ExternalTaskHandler
    {
        private readonly BpmnService _bpmnService;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public ChooseMainEditor(BpmnService bpmnService, UserManager<User> userManager, IUnitOfWork unitOfWork)
        {
            _bpmnService = bpmnService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async override Task<IExecutionResult> Process(ExternalTask externalTask)
        {
            try
            {
                var processInstanceResource = _bpmnService.GetProcessInstanceResource(externalTask.ProcessInstanceId);
                var editors = processInstanceResource.Variables.Get("editors").Result.GetValue<List<User>>();
                Random random = new Random();
                int editorIndex = random.Next(0, editors.Count);
                await _bpmnService.SetProcessVariableByProcessInstanceId("main_editor_username", externalTask.ProcessInstanceId, editors[editorIndex].UserName);
            }
            catch (Exception e)
            {
                return new CompleteResult()
                {
                    Variables = new Dictionary<string, Variable>
                    {
                        ["ChooseMainEditorHandler"] = new Variable(e.Message, VariableType.String)
                    }
                };
            }
            return new CompleteResult() { };
        }
    }
}
