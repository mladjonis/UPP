using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PublishingCompany.Camunda.Domain;
using PublishingCompany.Camunda.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GenresController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public ActionResult<Genre> Get(Guid id)
        {
            return Ok(_unitOfWork.Genres.Get(id));
        }

        [HttpGet("GetAll")]
        public ActionResult<IEnumerable<Genre>> GetAll()
        {
            return Ok(_unitOfWork.Genres.GetAll());
        }
    }
}
