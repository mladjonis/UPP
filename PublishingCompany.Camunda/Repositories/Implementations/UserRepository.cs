using PublishingCompany.Camunda.DbConfig;
using PublishingCompany.Camunda.Domain;
using PublishingCompany.Camunda.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Repositories.Implementations
{
    public class UserRepository : Repository<User, Guid>, IUserRepository
    {
        private readonly CamundaContext _context;
        public UserRepository(CamundaContext context) : base(context)
        {
            this._context = context;
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.Where(x => x.Email.Equals(email)).FirstOrDefault();
        }
    }
}
