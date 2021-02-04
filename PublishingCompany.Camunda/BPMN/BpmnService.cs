using Camunda.Api.Client;
using Camunda.Api.Client.Deployment;
using Camunda.Api.Client.ExternalTask;
using Camunda.Api.Client.Group;
using Camunda.Api.Client.ProcessDefinition;
using Camunda.Api.Client.ProcessInstance;
using Camunda.Api.Client.User;
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
        public string processInstanceId = string.Empty;

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
        //private Regex beginningRegex = new Regex(@"<bpmn:userTask.+>$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
        //private Regex endingRegex = new Regex(@"</bpmn:userTask>", RegexOptions.Compiled);
        //private Regex xmlAttributesRegex = new Regex("\\s[a-zA-Z0-9_:]+=(\"[\\w\\s]+\")", RegexOptions.Compiled);
        //private List<string> taskListFromXML = new List<string>();

        //private Regex beginningRegex = new Regex(@"<bpmn:userTask.+>$[\s\W\w.]+</bpmn:userTask>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
        private Regex beginningRegex = new Regex("<bpmn:userTask.*?</bpmn:userTask>", RegexOptions.Singleline);
        private Regex formFieldManipulation = new Regex(@"<camunda:formField.+>$[\s\S]*?</camunda:formField>", RegexOptions.Multiline);
        private Regex extensionInputElements = new Regex(@"<camunda:inputParameter.+>[0-9]+</camunda:inputParameter>", RegexOptions.Multiline);
        private Regex getNumber = new Regex(@"\d+");
        private Regex formFieldWithoutValidation = new Regex(@"<camunda:formField.+/>$", RegexOptions.Multiline);
        private Regex formFieldInfo = new Regex(@"<camunda:formField.+>$", RegexOptions.Multiline);
        private Regex formValidation = new Regex(@"<camunda:validation>[\s\S]*?</camunda:validation>", RegexOptions.Multiline);
        private Regex formValue = new Regex(@"<camunda:value.+/>$", RegexOptions.Multiline);
        private Regex stringManipulation = new Regex("\\s[a-zA-Z0-9_:]+=(\"[\\w\\s,]+\")", RegexOptions.Compiled);
        private Regex userTask = new Regex("<bpmn:userTask.+>$", RegexOptions.Multiline);
        #endregion


        /* za dobijanje input parametra u user task formi
<bpmn:userTask id="Task_Upload_PDFs" name="Upload PDFs" camunda:assignee="${username}">
        <bpmn:extensionElements>
          <camunda:inputOutput>
            <camunda:inputParameter name="docCount">2</camunda:inputParameter>
          </camunda:inputOutput>
        </bpmn:extensionElements>
        <bpmn:incoming>Flow_0a1olag</bpmn:incoming>
        <bpmn:outgoing>Flow_03g0sp7</bpmn:outgoing>
      </bpmn:userTask>
         */
        public async Task<FormFieldsDto> GetFormData(string processDefinitionKey, string taskKeyDefinitionOrTaskName)
        {
            var stringXml = await camunda.ProcessDefinitions.ByKey(processDefinitionKey).GetXml();
            var mmm = beginningRegex.Matches(stringXml.Bpmn20Xml);
            var taskList = new List<string>();
            foreach (Match m in mmm)
            {
                taskList.Add(m.Value);
            }

            var task = taskList.Find(z=>z.Contains($"id=\"{taskKeyDefinitionOrTaskName}\"")|| z.Contains($"name=\"{taskKeyDefinitionOrTaskName}\""));
            if (string.IsNullOrEmpty(task))
            {
                return null;
            }
            //ili krace
            //var taskKrace = taskList.Find(x=>x.Contains(taskKeyDefinitionOrTaskName));
            var userTaskInfo = userTask.Matches(task);
            var formFields = formFieldManipulation.Matches(task);
            var formFieldsWithoutValdiations = formFieldWithoutValidation.Matches(task);
            //ima bug neki
            var inputParams = extensionInputElements.Matches(task);
            var processInstance = await GetProcessInstancesInfo(this.processInstanceId);
            var form = new FormFieldsDto(){ProcessDefinitionKey= processDefinitionKey, ProcessDefinitionId = processDefinitionId, ProcessInstanceId = this.processInstanceId};
            //var form = new FormFieldsDto() { ProcessDefinitionKey = processDefinitionKey, ProcessDefinitionId = processDefinitionId, ProcessInstanceId = "f3eea732-52a4-11eb-9f77-e4f89c5bfdff" };
            var taskInfo = stringManipulation.Matches(userTaskInfo[0].Value);
            foreach (Match ut in taskInfo)
            {
                if (ut.Value.Contains("id="))
                {
                    form.TaskId = ut.Groups[1].Value.Replace("\"", "");
                }
                else if (ut.Value.Contains("name="))
                {
                    form.TaskName = ut.Groups[1].Value.Replace("\"", "");
                }
                else if (ut.Value.Contains("formKey="))
                {
                    form.FormKey = ut.Groups[1].Value.Replace("\"", "");
                }
            }
            //za potrebe ako forma ima neka ogranicenja recimo za brojeve dokumenata u ovom slucaju vrati samo taj broj jer za kasnije se mogu drugacije uraditi
            if (inputParams.Count > 0)
            {
                //var camundaFF = new CamundaFormField();
                //camundaFF.DefaultValue = getNumber.Match(inputParams[0].Value).Value;
                //form.CamundaFormFields.Add(camundaFF);
                form.FormKey = getNumber.Match(inputParams[0].Value).Value;
                return form;
            }

            //var formInfo = stringManipulation.Matches(userTaskInfo[0].Value);
            //foreach(Match fi in formInfo)
            //{
            //    if (fi.Value.Contains("id"))
            //    {
            //        form.TaskId = fi.Groups[1].Value.Replace("\"","");
            //    } else if (fi.Value.Contains("name"))
            //    {
            //        form.TaskName = fi.Groups[1].Value.Replace("\"", "");
            //    } else if (fi.Value.Contains("formKey"))
            //    {
            //        form.FormKey = fi.Groups[1].Value.Replace("\"", "");
            //    }
            //}
            for (int i = 0; i < formFields.Count; i++)
            {
                var camundaFrom1 = new CamundaFormField();
                var ffInfo = formFieldInfo.Matches(formFields[i].Value);
                var ffDetails = stringManipulation.Matches(ffInfo[0].Value);
                foreach (Match fd in ffDetails)
                {
                    if (fd.Value.Contains("id="))
                    {
                        camundaFrom1.FormId = fd.Groups[1].Value.Replace("\"", "");
                    }
                    else if (fd.Value.Contains("label="))
                    {
                        camundaFrom1.Label = fd.Groups[1].Value.Replace("\"", "");
                    }
                    else if (fd.Value.Contains("type="))
                    {
                        camundaFrom1.Type = fd.Groups[1].Value.Replace("\"", "");
                    }
                    else if (fd.Value.Contains("defaultValue="))
                    {
                        var t = GetUserTaskResource(taskKeyDefinitionOrTaskName).Result;
                        if (t != null)
                        {
                            camundaFrom1.DefaultValue = t.GetFormVariables().Result.Where(x => x.Key.Equals("ga")).FirstOrDefault().Value.Value.ToString();
                        }
                        else
                        {
                            var t2 = GetUserTaskResourceById(taskKeyDefinitionOrTaskName).Result;
                            var gg = t2.GetFormVariables().Result;
                            camundaFrom1.DefaultValue = t2.GetFormVariables().Result.Where(x => x.Key.Equals("ga")).FirstOrDefault().Value.Value.ToString();
                        }
                    }
                }
                var values = formValue.Matches(formFields[i].Value);
                if (values.Count > 0 && camundaFrom1.Type.Equals("enum"))
                {
                    for (int p = 0; p < values.Count; p++)
                    {
                        var valuesDetails = stringManipulation.Matches(values[p].Value);
                        FormFieldValues formVal = new FormFieldValues();
                        for (int k = 0; k < valuesDetails.Count; k++)
                        {
                            if (valuesDetails[k].Value.Contains("id="))
                            {
                                formVal.Id = valuesDetails[k].Groups[1].Value.Replace("\"", "");
                                continue;
                            }
                            else if (valuesDetails[k].Value.Contains("label="))
                            {
                                formVal.Label = valuesDetails[k].Groups[1].Value.Replace("\"", "");
                                continue;
                            }
                            else if (valuesDetails[k].Value.Contains("name="))
                            {
                                formVal.Name = valuesDetails[k].Groups[1].Value.Replace("\"", "");
                                continue;
                            }
                        }
                        camundaFrom1.Values.Add(formVal);
                    }
                }
                //if (camundaFrom1.Type.Equals("enum"))
                //{
                //    var processInstanceResource = GetProcessInstanceResource(this.processInstanceId);
                //    var registrationValues = await processInstanceResource.Variables.Get("genres");
                //    var jArrayValue = (Newtonsoft.Json.Linq.JArray)registrationValues.Value;
                //    var submittedData = jArrayValue.ToObject<List<Genre>>();
                //    foreach (var genre in submittedData)
                //    {
                //        FormFieldValues formVal = new FormFieldValues();
                //        formVal.Id = genre.Name;
                //        formVal.Name = genre.Name;
                //        formVal.Label = "Genres";
                //        camundaFrom1.Values.Add(formVal);
                //    }
                    
                //}
                var val = formValidation.Matches(formFields[i].Value);
                if (val.Count > 0)
                {
                    var valDetails = stringManipulation.Matches(val[0].Value);
                    for (int j = 0; j < valDetails.Count; j++)
                    {
                        FormFieldValidator ffv = new FormFieldValidator();
                        if (valDetails[j].Value.Contains("config")) continue;
                        if (valDetails[j].Value.Contains("required") || valDetails[j].Value.Contains("readonly"))
                        {
                            ffv.ValidatorName = valDetails[j].Groups[1].Value.Replace("\"", "");
                            camundaFrom1.Validators.Add(ffv);
                            continue;
                        }
                        else if (valDetails[j].Value.Contains("minlength") || valDetails[j].Value.Contains("maxlength") || valDetails[j].Value.Contains("min") || valDetails[j].Value.Contains("max"))
                        {
                            ffv.ValidatorName = valDetails[j].Groups[1].Value.Replace("\"", "");
                            ffv.ValidatorConfig = valDetails[j + 1].Groups[1].Value.Replace("\"", "");
                            camundaFrom1.Validators.Add(ffv);
                            continue;
                        }
                    }
                }
            form.CamundaFormFields.Add(camundaFrom1);
            }

            //bez valdiacije isti process
            for (int i = 0; i < formFieldsWithoutValdiations.Count; i++)
            {
                var camundaFrom1 = new CamundaFormField();
                var ffInfo = formFieldWithoutValidation.Matches(formFieldsWithoutValdiations[i].Value);
                var ffDetails = stringManipulation.Matches(ffInfo[0].Value);
                foreach (Match fd in ffDetails)
                {
                    if (fd.Value.Contains("id="))
                    {
                        camundaFrom1.FormId = fd.Groups[1].Value.Replace("\"", "");
                    }
                    else if (fd.Value.Contains("label="))
                    {
                        camundaFrom1.Label = fd.Groups[1].Value.Replace("\"", "");
                    }
                    else if (fd.Value.Contains("type="))
                    {
                        camundaFrom1.Type = fd.Groups[1].Value.Replace("\"", "");
                    }
                    else if (fd.Value.Contains("defaultValue="))
                    {
                        var t = GetUserTaskResource(taskKeyDefinitionOrTaskName).Result;
                        if (t != null)
                        {
                            camundaFrom1.DefaultValue = t.GetFormVariables().Result.Where(x => x.Key.Equals("ga")).FirstOrDefault().Value.Value.ToString();
                        }
                        else
                        {
                            var t2 = GetUserTaskResourceById(taskKeyDefinitionOrTaskName).Result;
                            var gg = t2.GetFormVariables().Result;
                            camundaFrom1.DefaultValue = t2.GetFormVariables().Result.Where(x => x.Key.Equals("ga")).FirstOrDefault().Value.Value.ToString();
                        }
                    }
                }
                var values = formValue.Matches(formFieldsWithoutValdiations[i].Value);
                if (values.Count > 0 && camundaFrom1.Type.Equals("enum"))
                {
                    for (int p = 0; p < values.Count; p++)
                    {
                        var valuesDetails = stringManipulation.Matches(values[p].Value);
                        FormFieldValues formVal = new FormFieldValues();
                        for (int k = 0; k < valuesDetails.Count; k++)
                        {
                            if (valuesDetails[k].Value.Contains("id"))
                            {
                                formVal.Id = valuesDetails[k].Groups[1].Value.Replace("\"", "");
                                continue;
                            }
                            else if (valuesDetails[k].Value.Contains("label"))
                            {
                                formVal.Label = valuesDetails[k].Groups[1].Value.Replace("\"", "");
                                continue;
                            }
                            else if (valuesDetails[k].Value.Contains("name"))
                            {
                                formVal.Name = valuesDetails[k].Groups[1].Value.Replace("\"", "");
                                continue;
                            }
                        }
                        camundaFrom1.Values.Add(formVal);
                    }
                }
                //if (camundaFrom1.Type.Equals("enum"))
                //{
                //    var processInstanceResource = GetProcessInstanceResource(this.processInstanceId);
                //    var registrationValues = await processInstanceResource.Variables.Get("genres");
                //    var jArrayValue = (Newtonsoft.Json.Linq.JArray)registrationValues.Value;
                //    var submittedData = jArrayValue.ToObject<List<Genre>>();
                //    foreach (var genre in submittedData)
                //    {
                //        FormFieldValues formVal = new FormFieldValues();
                //        formVal.Id = genre.Id.ToString();
                //        formVal.Name = genre.Name;
                //        formVal.Label = "Genres";
                //        camundaFrom1.Values.Add(formVal);
                //    }

                //}

                var val = formValidation.Matches(formFieldsWithoutValdiations[i].Value);
                if (val.Count > 0)
                {
                    var valDetails = stringManipulation.Matches(val[0].Value);
                    for (int j = 0; j < valDetails.Count; j++)
                    {
                        FormFieldValidator ffv = new FormFieldValidator();
                        if (valDetails[j].Value.Contains("config")) continue;
                        if (valDetails[j].Value.Contains("required") || valDetails[j].Value.Contains("readonly"))
                        {
                            ffv.ValidatorName = valDetails[j].Groups[1].Value.Replace("\"", "");
                            camundaFrom1.Validators.Add(ffv);
                            continue;
                        }
                        else if (valDetails[j].Value.Contains("minlength") || valDetails[j].Value.Contains("maxlength") || valDetails[j].Value.Contains("min") || valDetails[j].Value.Contains("max"))
                        {
                            ffv.ValidatorName = valDetails[j].Groups[1].Value.Replace("\"", "");
                            ffv.ValidatorConfig = valDetails[j + 1].Groups[1].Value.Replace("\"", "");
                            camundaFrom1.Validators.Add(ffv);
                            continue;
                        }
                    }
                }
                form.CamundaFormFields.Add(camundaFrom1);
            }
            return form;
        }


        //stara verzija
        //#region Populate regexIndexes and taskListFromXML methods
        //private List<int> PopulateRegexIndexesWithStartingIndexes(string xmlString)
        //{
        //    List<int> regexIndexes = new List<int>();
        //    MatchCollection matchesBegining = beginningRegex.Matches(xmlString);
        //    foreach (Match match in matchesBegining)
        //    {
        //        regexIndexes.Add(match.Index);
        //    }
        //    return regexIndexes;
        //}
        //private List<int> PopulateRegexIndexesWithEndingIndexes(string xmlString)
        //{
        //    List<int> regexIndexes = new List<int>();
        //    MatchCollection matchesEnding = endingRegex.Matches(xmlString);
        //    foreach (Match match in matchesEnding)
        //    {
        //        regexIndexes.Add(match.Index);
        //    }
        //    return regexIndexes;
        //}
        ///// <summary>
        ///// Broj prolaza kroz listu se deli sa 2 jer ce lista uvek da sadrzi paran broj clanova tj
        ///// svaki userTask ima svoj pocetni i zavrsni tag.
        ///// parametri regexIndexes[i] i regexIndexes[i+2] -regexIndexes[i] predstavjaju indeks pocetka i zavrsetka
        ///// userTaska u xmlString formatu modela
        ///// </summary>
        ///// <param name="xmlString"></param>
        //private void PopulateTaskListFromXML(string xmlString)
        //{
        //    var regexIndexes = PopulateRegexIndexesWithStartingIndexes(xmlString);
        //    regexIndexes.AddRange(PopulateRegexIndexesWithEndingIndexes(xmlString));
        //    taskListFromXML.Clear();
        //    //ako je broj taskova veci od 2 u procesu sto ce biti 99% prepolovi duzinu liste na 2 bice uvek paran broj
        //    var length = regexIndexes.Count >= 2 ? regexIndexes.Count / 2 : regexIndexes.Count;
        //    for (int i = 0; i < length; i++) // /2
        //    {
        //        //taskListFromXML.Add(xmlString.Substring(regexIndexes[i], regexIndexes[i + 2] - regexIndexes[i]));
        //        taskListFromXML.Add(xmlString.Substring(regexIndexes[i], regexIndexes[i + 1] - regexIndexes[i])); //samo za sada za probu ako ima samo 1 task

        //    }
        //}
        //#endregion

        ////ovu metodu bi trebalo izmestiti u posebnu klasu u principu...ali za sada posto koristi ovaj servis neka je ovde
        //public async Task<FormFieldsDto> GetFormData(string processDefinitionKey, string taskKeyDefinitionOrTaskName)
        //{
        //    var processDefinition = await GetProcessDefinitionDiagramAsync(processDefinitionKey);
        //    PopulateTaskListFromXML(processDefinition.Bpmn20Xml);
        //    var processInstance = await GetProcessInstancesInfo(this.processInstanceId);
        //    FormFieldsDto formFields = new FormFieldsDto() { ProcessDefinitionKey= processDefinitionKey, ProcessDefinitionId = processDefinition.Id, ProcessInstanceId = processInstance.FirstOrDefault().Id };
        //    foreach (var task in taskListFromXML)
        //    {
        //        if (task.Contains($"id=\"{taskKeyDefinitionOrTaskName}\"") || task.Contains($"name=\"{taskKeyDefinitionOrTaskName}\""))
        //        {
        //            var r = xmlAttributesRegex.Matches(task);
        //            var camundaFormField = new CamundaFormField();
        //            for (int i = 0; i < r.Count; i++)
        //            {
        //                if (r[i].Value.Contains("config="))
        //                    continue;
        //                if (r[i].Value.Contains("required") || r[i].Value.Contains("readonly"))
        //                {
        //                    FormFieldValidator formFieldsValidators = new FormFieldValidator() { ValidatorName = r[i].Groups[1].Value.Replace("\"", ""), ValidatorConfig = "none" };
        //                    camundaFormField.Validators.Add(formFieldsValidators);
        //                    continue;
        //                }
        //                else if (r[i].Value.Contains("minlength") || r[i].Value.Contains("maxlength") || r[i].Value.Contains("min") || r[i].Value.Contains("max"))
        //                {
        //                    FormFieldValidator formFieldsValidators = new FormFieldValidator() { ValidatorName = r[i].Groups[1].Value.Replace("\"", ""), ValidatorConfig = r[i + 1].Groups[1].Value.Replace("\"", "") };
        //                    camundaFormField.Validators.Add(formFieldsValidators);
        //                    continue;
        //                }
        //                if (r[i].Value.Contains("id=") && string.IsNullOrEmpty(formFields.TaskId))
        //                {
        //                    formFields.TaskId = r[i].Groups[1].Value.Replace("\"", "");
        //                    continue;
        //                }
        //                else if (r[i].Value.Contains("name="))
        //                {
        //                    formFields.TaskName = r[i].Groups[1].Value.Replace("\"", "");
        //                    continue;
        //                }
        //                else if (r[i].Value.Contains("formKey="))
        //                {
        //                    formFields.FormKey = r[i].Groups[1].Value.Replace("\"", "");
        //                    continue;
        //                }
        //                else if (r[i].Value.Contains("id="))
        //                {
        //                    camundaFormField.FormId = r[i].Groups[1].Value.Replace("\"", "");
        //                    continue;
        //                }
        //                else if (r[i].Value.Contains("type="))
        //                {
        //                    camundaFormField.Type = r[i].Groups[1].Value.Replace("\"", "");
        //                    continue;
        //                }
        //                else if (r[i].Value.Contains("label="))
        //                {
        //                    camundaFormField.Label = r[i].Groups[1].Value.Replace("\"", "");
        //                    continue;
        //                }
        //            }
        //            formFields.CamundaFormFields.Add(camundaFormField);
        //        }
        //    }
        //    return formFields;
        //}

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

        public async Task<List<ProcessInstanceInfo>> GetProcessInstancesInfo(string processInstanceId)
        {
            return await camunda.ProcessInstances.Query(new ProcessInstanceQuery() { ProcessInstanceIds = new List<string>() { processInstanceId } }).List();
        }

        public ProcessInstanceResource GetProcessInstanceResource(string processInstanceId)
        {
            return camunda.ProcessInstances[processInstanceId];
        }

        //public async Task SetProcessVariableByProcessInstanceId(string processInstanceId, CompleteTask values)
        //{
        //    var processInstanceVariables = camunda.ProcessInstances[processInstanceId].Variables;
        //    await processInstanceVariables.Set("registrationValues", VariableValue.FromObject(values));
        //}

        public async Task SetProcessVariableByProcessInstanceId(string variableName, string processInstanceId, List<FormSubmitDto> values)
        {
            var processInstanceVariables = camunda.ProcessInstances[processInstanceId].Variables;
            await processInstanceVariables.Set(variableName, VariableValue.FromObject(values));
        }
        public async Task SetProcessVariableByProcessInstanceId(string variableName, string processInstanceId, bool validation)
        {
            var processInstanceVariables = camunda.ProcessInstances[processInstanceId].Variables;
            await processInstanceVariables.Set(variableName, VariableValue.FromObject(validation));
        }
        public async Task SetProcessVariableByProcessInstanceId(string variableName, string processInstanceId, object value)
        {
            var processInstanceVariables = camunda.ProcessInstances[processInstanceId].Variables;
            await processInstanceVariables.Set(variableName, VariableValue.FromObject(value));
        }
        public async Task SetProcessVariableByProcessInstanceId(string variableName, string processInstanceId, string value)
        {
            var processInstanceVariables = camunda.ProcessInstances[processInstanceId].Variables;
            await processInstanceVariables.Set(variableName, VariableValue.FromObject(value));
        }
        public async Task SetProcessVariableByProcessInstanceId(string variableName, string processInstanceId, long value)
        {
            var processInstanceVariables = camunda.ProcessInstances[processInstanceId].Variables;
            await processInstanceVariables.Set(variableName, VariableValue.FromObject(value));
        }

        public ProcessDefinitionResource GetProc()
        {
            return camunda.ProcessDefinitions[processDefinitionId];
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
            if (task.Count == 0)
                return null;
            return camunda.UserTasks[task.FirstOrDefault().Id];
        }

        public async Task<TaskResource> GetUserTaskResourceById(string taskId)
        {
            var task = await camunda.UserTasks.Query(new TaskQuery() { Name = taskId, ProcessInstanceId = this.processInstanceId }).List();
            if (task.Count == 0)
                return null;
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

        public async Task CreateUser(UserProfileInfo userInfo, string password)
        {
            await camunda.Users.Create(userInfo, password);
        }

        public async Task<UserProfileInfo> GetUser(string email)
        {
            var user = await camunda.Users.Query(new UserQuery() { Email = email }).List();
            return user.FirstOrDefault();
        }

        public async Task CleanupProcessInstances(string processDefinitionKey)
        {
            var instances = await camunda.ProcessInstances
                .Query(new ProcessInstanceQuery
                {
                    ProcessDefinitionKey = processDefinitionKey
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
        public async Task<string> StartWriterRegistrationProcess(IList<User> cometee, IEnumerable<Genre> genres)
        {
            var processInstance = new StartProcessInstance().SetVariable("validation", false)
                .SetVariable("cometees", cometee)
                .SetVariable("genres", genres)
                .SetVariable("documentCountRequired", false);
            //ako bude trebao businessKey
            //processParams.BusinessKey = user.Id.ToString();
            processDefinitionId = "Process_Writer_Registration3";
            var processStartResult = await
                camunda.ProcessDefinitions.ByKey(processDefinitionId).StartProcessInstance(processInstance);
            return processStartResult.Id;
        }

        public async Task<string> StartReaderRegistrationProcess(IEnumerable<Genre> genres,IEnumerable<BetaGenre> betaGenres)
        {
            var processInstance = new StartProcessInstance()
                .SetVariable("validation", false)
                .SetVariable("genres", genres)
                .SetVariable("beta_g", betaGenres);
            //ako bude trebao businessKey
            //processParams.BusinessKey = user.Id.ToString();
            processDefinitionId = "Process_Reader_Registration4";

            var processStartResult = await
                camunda.ProcessDefinitions.ByKey(processDefinitionId).StartProcessInstance(processInstance);
            return processStartResult.Id;
        }

        public async Task<string> StartPlagiarismProcess(IEnumerable<User> cometees, IEnumerable<User> editors, string username)
        {
            var processInstance = new StartProcessInstance()
                .SetVariable("cometees", cometees)
                .SetVariable("editors", editors)
                .SetVariable("loggedUsername", username);
            //ako bude trebao businessKey
            //processParams.BusinessKey = user.Id.ToString();
            processDefinitionId = "Process_Plagiarism";

            var processStartResult = await
                camunda.ProcessDefinitions.ByKey(processDefinitionId).StartProcessInstance(processInstance);
            return processStartResult.Id;
        }

        public async Task<string> StartBookPublishingProcess(IEnumerable<Genre> genres, IEnumerable<BetaGenre> betaGenres, string username)
        {
            var processInstance = new StartProcessInstance()
                .SetVariable("validation", false)
                .SetVariable("genres", genres)
                .SetVariable("beta_g", betaGenres);
            //ako bude trebao businessKey
            //processParams.BusinessKey = user.Id.ToString();
            processDefinitionId = "Process_Book";

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
