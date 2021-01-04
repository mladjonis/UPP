using Microsoft.Extensions.DependencyInjection;
using PublishingCompany.Camunda.Helpers.ClientTokenGenerator;
using PublishingCompany.Camunda.Helpers.FormSubmitMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Helpers
{
    public static class HelpersInfrastructure
    {
        public static IServiceCollection AddHelpers(this IServiceCollection services)
        {
            services.AddScoped<IFormSubmitDtoMapper, FormSubmitDtoMapper>();
            services.AddScoped<IGenerateClientJwt, GenerateClientJwt>();
            return services;
        }
    }
}
