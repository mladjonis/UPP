﻿using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.BPMN
{
    public class BpmnProcessDeployService : IHostedService
    {
        private readonly BpmnService bpmnService;

        public BpmnProcessDeployService(BpmnService bpmnService)
        {
            this.bpmnService = bpmnService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await bpmnService.DeployProcessDefinition();

            //await bpmnService.CleanupProcessInstances();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
