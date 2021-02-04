﻿using Camunda.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Handlers
{
    [HandlerTopics("Topic_ChooseNewEditorHandler", LockDuration = 10_000)]
    public class ChooseNewEditor : ExternalTaskHandler
    {
        public async override Task<IExecutionResult> Process(ExternalTask externalTask)
        {
            throw new NotImplementedException();
        }
    }
}
