using Camunda.Api.Client;
using Camunda.Api.Client.Deployment;
using Camunda.Api.Client.ExternalTask;
using Camunda.Api.Client.ProcessDefinition;
using Camunda.Api.Client.ProcessInstance;
using Camunda.Api.Client.UserTask;
using PublishingCompany.Camunda.Domain;
using PublishingCompany.Camunda.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.BPMN
{
    public class BpmnService
    {
        private readonly CamundaClient camunda;
        private readonly IUnitOfWork _unitOfWork;

        public BpmnService(string camundaRestApiUri)
        {
            this.camunda = CamundaClient.Create(camundaRestApiUri);
            //this._unitOfWork = unitOfWork;
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

        public async Task<List<ProcessInstanceInfo>> GetProcessInstance(string processInstanceId)
        {
            return await camunda.ProcessInstances.Query(new ProcessInstanceQuery() { ProcessDefinitionId = processInstanceId }).List();
        }

        public async Task<UserTaskInfo> GetFirstTask(string processInstanceId)
        {
            var tasks = await camunda.UserTasks.Query(new TaskQuery() { ProcessInstanceId = processInstanceId }).List();
            return tasks.FirstOrDefault();
        }

        public async Task<Dictionary<string,VariableValue>> GetTaskFormData(string taskId)
        {
            var formData = await camunda.UserTasks[taskId].GetFormVariables();
            return formData;
        }

        public 

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

        public async Task<string> StartWriterRegistrationProcess()
        {
            var processInstance = new StartProcessInstance().SetVariable("validation", false);
            //proba
            //var form = new FormInfo().Key;
            //var startForm = new SubmitStartForm().SetVariable("username", "asa");
            //var asa = await camunda.UserTasks.Query(new TaskQuery()).List();


            //ako bude trebao businessKey
            //processParams.BusinessKey = user.Id.ToString();


            //namestiti kada se zavrse modeli da vrati clanove komisije koji ce ici u model
            //var comitee = _unitOfWork.Users.GetAll().ToList();
            //stara podesavanja vratiti kada se istestira 
            //var processStartResult = await
            //    camunda.ProcessDefinitions.ByKey("Process_Writer_Registration").StartProcessInstance(processInstance);
            var processStartResult = await
                camunda.ProcessDefinitions.ByKey("Process_Probe_11").StartProcessInstance(processInstance);
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
