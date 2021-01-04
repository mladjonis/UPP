using PublishingCompany.Camunda.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Helpers.ClientTokenGenerator
{
    public interface IGenerateClientJwt
    {
        Task<string> GenerateJwtToken(User user);
    }
}
