using PublishingCompany.Camunda.DbConfig;
using PublishingCompany.Camunda.Domain;
using PublishingCompany.Camunda.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Repositories.Implementations
{
    public class BetaRepository : Repository<BetaGenre, Guid>, IBetaRepository
    {
        public BetaRepository(CamundaContext context) : base(context)
        {

        }
    }
}
