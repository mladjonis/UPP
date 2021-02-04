using PublishingCompany.Camunda.DbConfig;
using PublishingCompany.Camunda.Domain;
using PublishingCompany.Camunda.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Repositories.Implementations
{
    public class CommentRepository : Repository<Comment, Guid>, ICommentRepository
    {
        private readonly CamundaContext _context;
        public CommentRepository(CamundaContext context) : base(context)
        {
            this._context = context;
        }
    }
}
