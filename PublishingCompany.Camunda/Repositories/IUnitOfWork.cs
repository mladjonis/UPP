﻿using PublishingCompany.Camunda.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; set; }
        IGenreRepository Genres { get; set; }
        IBetaRepository BetaGenres { get; set; }
        IBookRepository Books { get; set; }
        ICommentRepository Comments { get; set; }
        int Complete();
        Task<int> CompleteAsync();
    }
}
