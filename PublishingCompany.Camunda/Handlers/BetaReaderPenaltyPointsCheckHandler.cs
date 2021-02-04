using Camunda.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Handlers
{
    [HandlerTopics("Topic_BetaReaderPenaltyPointsCheckHandler", LockDuration = 10_000)]
    public class BetaReaderPenaltyPointsCheckHandler : ExternalTaskHandler
    {
        public override Task<IExecutionResult> Process(ExternalTask externalTask)
        {
            throw new NotImplementedException();
        }
    }
}
