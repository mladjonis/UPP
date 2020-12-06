using Camunda.Api.Client;
using Camunda.Api.Client.Deployment;
using Camunda.Api.Client.ExternalTask;
using Camunda.Api.Client.ProcessDefinition;
using Camunda.Api.Client.ProcessInstance;
using Camunda.Api.Client.UserTask;
using PublishingCompany.Camunda.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.BPMN
{
    public class BpmnService
    {
        private readonly CamundaClient camunda;

        public BpmnService(string camundaRestApiUri)
        {
            this.camunda = CamundaClient.Create(camundaRestApiUri);
        }

        public async Task GetAllExternalTasks()
        {
            var list = await camunda.ExternalTasks.Query().List();
            return;
        }

        public async Task GetAllProcesses()
        {
            var list = await camunda.ProcessDefinitions.Query().List();
            return;
            //return Task.Run(() => camunda.ProcessDefinitions.Query().List()).Result;
        }

        public async Task DeployProcessDefinition()
        {
            var bpmnResourceStream = this.GetType()
                .Assembly
                .GetManifestResourceStream("PublishingCompany.Camunda.BPMN.MODEL_NAME.bpmn");

            try
            {
                await camunda.Deployments.Create(
                    "DEPLOYMENT_NAME",
                    true,
                    true,
                    null,
                    null,
                    new ResourceDataContent(bpmnResourceStream, "MODEL_NAME.bpmn"));
            }
            catch (Exception e)
            {
                throw new ApplicationException("Failed to deploy process definition", e);
            }
        }

        public async Task CleanupProcessInstances()
        {
            var instances = await camunda.ProcessInstances
                .Query(new ProcessInstanceQuery
                {
                    ProcessDefinitionKey = "PROCESS_DEFINITION_KEY" //u nasem slucaju Process_Writer_Registration
                })
                .List();

            if (instances.Count > 0)
            {
                await camunda.ProcessInstances.Delete(new DeleteProcessInstances
                {
                    ProcessInstanceIds = instances.Select(i => i.Id).ToList()
                });
            }
        }

        public async Task<string> StartProcessFor(User user)
        {
            var processParams = new StartProcessInstance()
                .SetVariable("userId", VariableValue.FromObject(user.Id.ToString()));

            //ako bude trebao businessKey
            //processParams.BusinessKey = user.Id.ToString();

            var processStartResult = await
                camunda.ProcessDefinitions.ByKey("Process_Writer_Registration").StartProcessInstance(processParams);

            return processStartResult.Id;
        }

        public async Task<UserTaskInfo> ClaimTask(string taskId, string user)
        {
            await camunda.UserTasks[taskId].Claim(user);
            var task = await camunda.UserTasks[taskId].Get();
            return task;
        }

        public async Task<UserTaskInfo> CompleteTask(string taskId, User userFromFront)
        {
            var task = await camunda.UserTasks[taskId].Get();
            var completeTask = new CompleteTask(); //dovrsiti
            await camunda.UserTasks[taskId].Complete(completeTask);
            return task;
        }
    }
}
