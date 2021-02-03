using PublishingCompany.Camunda.DbConfig;
using PublishingCompany.Camunda.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CamundaContext _context;

        public IUserRepository Users { get; set; }

        public IGenreRepository Genres { get; set; }
        public IBetaRepository BetaGenres { get; set; }

        public UnitOfWork(CamundaContext context, IUserRepository userRepository, IGenreRepository genreRepository, IBetaRepository betaRepository)
        {
            _context = context;
            this.Users = userRepository;
            this.Genres = genreRepository;
            this.BetaGenres = betaRepository;
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
    }
}
