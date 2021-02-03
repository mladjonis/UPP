using Camunda.Worker;
using PublishingCompany.Camunda.BPMN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Handlers
{
    [HandlerTopics("Topic_DeleteProcesHandler", LockDuration = 10_000)]
    public class DeleteProcesHandler : ExternalTaskHandler
    {
        private readonly BpmnService _bpmnService;

        public DeleteProcesHandler(BpmnService bpmnService)
        {
            _bpmnService = bpmnService;
        }

        public async override Task<IExecutionResult> Process(ExternalTask externalTask)
        {
            try
            {
                await _bpmnService.CleanupProcessInstances(externalTask.ProcessDefinitionKey);
            }catch(Exception e)
            {
                return new CompleteResult() 
                { 
                    Variables = new Dictionary<string, Variable>() {
                        ["DeleteProcesHandler"] = new Variable(e.Message, VariableType.String)
                    } 
                };
            }
            return new CompleteResult(){ };
        }
    }
}
