using PublishingCompany.Camunda.DbConfig;
using PublishingCompany.Camunda.Domain;
using PublishingCompany.Camunda.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Repositories.Implementations
{
    public class BookRepository : Repository<Book, Guid>, IBookRepository
    {
        private readonly CamundaContext _context;
        public BookRepository(CamundaContext context) : base(context)
        {
            this._context = context;
        }

        public Book GetByName(string name)
        {
            return _context.Books.Where(x => x.HeadLine.ToLower().Equals(name)).FirstOrDefault();
        }
    }
}
