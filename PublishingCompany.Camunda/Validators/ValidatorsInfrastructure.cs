using Microsoft.Extensions.DependencyInjection;
using PublishingCompany.Camunda.Validators.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Validators
{
    public static class ValidatorsInfrastructure
    {
        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddTransient<IUserValidator, UserValidator>();
            return services;
        }
    }
}
