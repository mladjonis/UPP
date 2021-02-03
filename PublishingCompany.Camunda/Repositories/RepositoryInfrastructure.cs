using Microsoft.Extensions.DependencyInjection;
using PublishingCompany.Camunda.DbConfig;
using PublishingCompany.Camunda.Repositories.Implementations;
using PublishingCompany.Camunda.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Repositories
{
    public static class RepositoryInfrastructure
    {
        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IGenreRepository, GenreRepository>();
            services.AddTransient<IBetaRepository, BetaRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
