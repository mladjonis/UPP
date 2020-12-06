using Camunda.Worker;
using Microsoft.Extensions.DependencyInjection;
using PublishingCompany.Camunda.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.BPMN
{
    public static class BpmnCamundaInstaller
    {
        public static IServiceCollection AddCamunda(this IServiceCollection services, string camundaRestApiUri)
        {
            services.AddSingleton(_ => new BpmnService(camundaRestApiUri));
            //services.AddHostedService<BpmnProcessDeployService>();

            //services.AddCamundaWorker(options =>
            //{
            //    options.BaseUri = new Uri(camundaRestApiUri);
            //    options.WorkerCount = 1;
            //})
            //    // handlers for now if needed any add it here
            //.AddHandler<WriterDataValidationHandler>()
            //.AddHandler<RegistrationEmailSendHandler>()
            //.AddHandler<NotifyWriterHandler>()
            //.AddHandler<RegistrationFinishHandler>();

            return services;
        }
    }
}
