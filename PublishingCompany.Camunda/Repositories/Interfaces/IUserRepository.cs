﻿using PublishingCompany.Camunda.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User, Guid>
    {
        User GetUserByEmail(string email);
        User GetUserByName(string name);
        User GetUserByUsername(string username);
    }
}
