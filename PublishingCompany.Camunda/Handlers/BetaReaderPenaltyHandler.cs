﻿using Camunda.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Handlers
{
    [HandlerTopics("Topic_BetaReaderPenaltyHandler", LockDuration = 10_000)]
    public class BetaReaderPenaltyHandler : ExternalTaskHandler
    {
        public override Task<IExecutionResult> Process(ExternalTask externalTask)
        {
            throw new NotImplementedException();
        }
    }
}
