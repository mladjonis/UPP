using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Validators.User
{
    public interface IUserValidator
    {
        bool UserExists(string email);
    }
}
