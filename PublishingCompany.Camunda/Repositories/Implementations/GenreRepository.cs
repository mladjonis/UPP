using PublishingCompany.Camunda.DbConfig;
using PublishingCompany.Camunda.Domain;
using PublishingCompany.Camunda.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Repositories.Implementations
{
    public class GenreRepository : Repository<Genre, Guid>,IGenreRepository
    {
        public GenreRepository(CamundaContext context):base(context)
        {

        }
    }
}
