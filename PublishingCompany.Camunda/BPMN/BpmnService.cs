using Camunda.Api.Client;
using Camunda.Api.Client.Deployment;
using Camunda.Api.Client.ExternalTask;
using Camunda.Api.Client.ProcessDefinition;
using Camunda.Api.Client.ProcessInstance;
using Camunda.Api.Client.UserTask;
using Camunda.Api.Client.VariableInstance;
using PublishingCompany.Camunda.BPMN.Domain;
using PublishingCompany.Camunda.Domain;
using PublishingCompany.Camunda.DTO;
using PublishingCompany.Camunda.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.BPMN
{
    public class BpmnService
    {
        private readonly CamundaClient camunda;
        private string processDefinitionId = string.Empty;
        private string processInstanceId = string.Empty;

        public BpmnService(string camundaRestApiUri)
        {
            this.camunda = CamundaClient.Create(camundaRestApiUri);
        }

        #region Regex and XML configuration
        private static readonly string beginningRegexString = @"<bpmn:userTask.+>$";
        private static readonly string endingRegexString = @"</bpmn:userTask>";
        /// <summary>
        /// format za atribute je sledeci \sIMEATRIBUTA=VREDNOST_POD_NAVODNICIMA
        /// sa regexom se izvlace imena atributa i vrednosti
        /// </summary>
        private static readonly string xmlAttributesRegexString = "\\s[a-zA-Z0-9_:]+=(\"[\\w\\s] +\")";
        private Regex beginningRegex = new Regex(@"<bpmn:userTask.+>$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
        private Regex endingRegex = new Regex(@"</bpmn:userTask>", RegexOptions.Compiled);
        private Regex xmlAttributesRegex = new Regex("\\s[a-zA-Z0-9_:]+=(\"[\\w\\s]+\")", RegexOptions.Compiled);
        private List<string> taskListFromXML = new List<string>();
        #endregion

        #region Populate regexIndexes and taskListFromXML methods
        private List<int> PopulateRegexIndexesWithStartingIndexes(string xmlString)
        {
            List<int> regexIndexes = new List<int>();
            MatchCollection matchesBegining = beginningRegex.Matches(xmlString);
            foreach (Match match in matchesBegining)
            {
                regexIndexes.Add(match.Index);
            }
            return regexIndexes;
        }
        private List<int> PopulateRegexIndexesWithEndingIndexes(string xmlString)
        {
            List<int> regexIndexes = new List<int>();
            MatchCollection matchesEnding = endingRegex.Matches(xmlString);
            foreach (Match match in matchesEnding)
            {
                regexIndexes.Add(match.Index);
            }
            return regexIndexes;
        }
        /// <summary>
        /// Broj prolaza kroz listu se deli sa 2 jer ce lista uvek da sadrzi paran broj clanova tj
        /// svaki userTask ima svoj pocetni i zavrsni tag.
        /// parametri regexIndexes[i] i regexIndexes[i+2] -regexIndexes[i] predstavjaju indeks pocetka i zavrsetka
        /// userTaska u xmlString formatu modela
        /// </summary>
        /// <param name="xmlString"></param>
        private void PopulateTaskListFromXML(string xmlString)
        {
            var regexIndexes = PopulateRegexIndexesWithStartingIndexes(xmlString);
            regexIndexes.AddRange(PopulateRegexIndexesWithEndingIndexes(xmlString));
            taskListFromXML.Clear();
            for (int i = 0; i < regexIndexes.Count / 2; i++)
            {
                taskListFromXML.Add(xmlString.Substring(regexIndexes[i], regexIndexes[i + 2] - regexIndexes[i]));
            }
        }
        #endregion

        //ovu metodu bi trebalo izmestiti u posebnu klasu u principu...ali za sada posto koristi ovaj servis neka je ovde
        public async Task<FormFieldsDto> GetFormData(string processDefinitionKey, string taskKeyDefinitionOrTaskName)
        {
            var processDefinition = await GetProcessDefinitionDiagramAsync(processDefinitionKey);
            PopulateTaskListFromXML(processDefinition.Bpmn20Xml);
            var processInstance = await GetProcessInstance(this.processInstanceId);
            FormFieldsDto formFields = new FormFieldsDto() { ProcessDefinitionKey= processDefinitionKey, ProcessDefinitionId = processDefinition.Id, ProcessInstanceId = processInstance.FirstOrDefault().Id };
            foreach (var task in taskListFromXML)
            {
                if (task.Contains($"id=\"{taskKeyDefinitionOrTaskName}\"") || task.Contains($"name=\"{taskKeyDefinitionOrTaskName}\""))
                {
                    var r = xmlAttributesRegex.Matches(task);
                    var camundaFormField = new CamundaFormField();
                    for (int i = 0; i < r.Count; i++)
                    {
                        if (r[i].Value.Contains("config="))
                            continue;
                        if (r[i].Value.Contains("required") || r[i].Value.Contains("readonly"))
                        {
                            FormFieldValidator formFieldsValidators = new FormFieldValidator() { ValidatorName = r[i].Groups[1].Value.Replace("\"", ""), ValidatorConfig = "none" };
                            camundaFormField.Validators.Add(formFieldsValidators);
                            continue;
                        }
                        else if (r[i].Value.Contains("minlength") || r[i].Value.Contains("maxlength") || r[i].Value.Contains("min") || r[i].Value.Contains("max"))
                        {
                            FormFieldValidator formFieldsValidators = new FormFieldValidator() { ValidatorName = r[i].Groups[1].Value.Replace("\"", ""), ValidatorConfig = r[i + 1].Groups[1].Value.Replace("\"", "") };
                            camundaFormField.Validators.Add(formFieldsValidators);
                            continue;
                        }
                        if (r[i].Value.Contains("id=") && string.IsNullOrEmpty(formFields.TaskId))
                        {
                            formFields.TaskId = r[i].Groups[1].Value.Replace("\"", "");
                            continue;
                        }
                        else if (r[i].Value.Contains("name="))
                        {
                            formFields.TaskName = r[i].Groups[1].Value.Replace("\"", "");
                            continue;
                        }
                        else if (r[i].Value.Contains("formKey="))
                        {
                            formFields.FormKey = r[i].Groups[1].Value.Replace("\"", "");
                            continue;
                        }
                        else if (r[i].Value.Contains("id="))
                        {
                            camundaFormField.FormId = r[i].Groups[1].Value.Replace("\"", "");
                            continue;
                        }
                        else if (r[i].Value.Contains("type="))
                        {
                            camundaFormField.Type = r[i].Groups[1].Value.Replace("\"", "");
                            continue;
                        }
                        else if (r[i].Value.Contains("label="))
                        {
                            camundaFormField.Label = r[i].Groups[1].Value.Replace("\"", "");
                            continue;
                        }
                    }
                    formFields.CamundaFormFields.Add(camundaFormField);
                }
            }
            return formFields;
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
            return await camunda.ProcessInstances.Query(new ProcessInstanceQuery() { ProcessInstanceIds = new List<string>() { processInstanceId } }).List();
        }

        public async Task SetProcessVariableByProcessInstanceId(string processInstanceId, string values)
        {
            var processInstanceVariables = camunda.ProcessInstances[processInstanceId].Variables;
            await processInstanceVariables.Set("registrationValues", VariableValue.FromObject(values));
        }

        public async Task<ProcessDefinitionInfo> GetProcessDefinitionKey()
        {
            var processes=  await camunda.ProcessDefinitions.Query(new ProcessDefinitionQuery() { Key = this.processDefinitionId }).List();
            return processes.FirstOrDefault();
        }

        public async Task<UserTaskInfo> GetFirstTask(string processInstanceId)
        {
            this.processInstanceId = processInstanceId;
            var tasks = await camunda.UserTasks.Query(new TaskQuery() { ProcessInstanceId = processInstanceId }).List();
            return tasks.FirstOrDefault();
        }

        public async Task<UserTaskInfo> GetTaskById(string taskKey, string processInstanceId)
        {
            var tasks = await camunda.UserTasks.Query(new TaskQuery() { TaskDefinitionKey = taskKey, ProcessInstanceId = processInstanceId }).List();
            return tasks.FirstOrDefault();
        }

        public async Task<TaskResource> GetUserTaskResource(string taskKey)
        {
            var task = await camunda.UserTasks.Query(new TaskQuery() { TaskDefinitionKey = taskKey, ProcessInstanceId = this.processInstanceId }).List();
            return camunda.UserTasks[task.FirstOrDefault().Id];
        }

        /// <summary>
        /// vraca samo tip inputa i ime
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<Dictionary<string,VariableValue>> GetTaskFormData(string taskId)
        {
            //ili namestiti u metodi koje hocemo variable jer iz nekog razloga uzima i procesne varijable
            //ili nakon izvlacenja isfiltrirati
            var formData = await camunda.UserTasks[taskId].GetFormVariables();
            return formData;
        }

        public async Task<ProcessDefinitionDiagram> GetProcessDefinitionDiagramAsync(string processDefinitionKey)
        {
            return await camunda.ProcessDefinitions.ByKey(processDefinitionKey).GetXml();
        }

        public async Task<string> GetProcessDefinitionXMLASync(string processDefinitionKey)
        {
            var processDefinitionDiagram = await camunda.ProcessDefinitions.ByKey(processDefinitionKey).GetXml();
            return processDefinitionDiagram.Bpmn20Xml;
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns> Process definition Id</returns>
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
            processDefinitionId = "Process_Probe_36";
            var processStartResult = await
                camunda.ProcessDefinitions.ByKey(processDefinitionId).StartProcessInstance(processInstance);
            return processStartResult.Id;
        }

        public async Task<UserTaskInfo> ClaimTask(string taskId, string user)
        {
            await camunda.UserTasks[taskId].Claim(user);
            var task = await camunda.UserTasks[taskId].Get();
            return task;
        }

        public async Task<UserTaskInfo> CompleteTask(string taskId)
        {
            var task = await camunda.UserTasks[taskId].Get();
            var completeTask = new CompleteTask(); //dovrsiti ako treba
            await camunda.UserTasks[taskId].Complete(completeTask);
            return task;
        }
         
    }
}
