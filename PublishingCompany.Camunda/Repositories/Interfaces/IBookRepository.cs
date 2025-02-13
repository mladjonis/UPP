﻿using PublishingCompany.Camunda.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Repositories.Interfaces
{
    public interface IBookRepository : IRepository<Book, Guid>
    {
        Book GetByName(string name);
    }
}
