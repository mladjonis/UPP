using PublishingCompany.Camunda.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.DbConfig
{
    public class CamundaContext : DbContext
    {
        public CamundaContext()
        {}

        public DbSet<User> Users { get; set; }
        public DbSet<Genre> Genres { get; set; }
    }
}
