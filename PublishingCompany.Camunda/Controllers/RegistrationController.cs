using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.CQRS.EmailConfirmationWriter;
using PublishingCompany.Camunda.CQRS.GetBetaForm;
using PublishingCompany.Camunda.CQRS.GetDormData;
using PublishingCompany.Camunda.CQRS.GetFormDataGeneric;
using PublishingCompany.Camunda.CQRS.RegisterBetaReader;
using PublishingCompany.Camunda.CQRS.RegisterReader;
using PublishingCompany.Camunda.CQRS.RegisterUser;
using PublishingCompany.Camunda.CQRS.SubmitFormDataGeneric;
using PublishingCompany.Camunda.Domain;
using PublishingCompany.Camunda.DTO;
using PublishingCompany.Camunda.Helpers.ClientTokenGenerator;
using PublishingCompany.Camunda.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly BpmnService _bpmnService;
        private readonly IGenerateClientJwt _clientJwt;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public RegistrationController(
            IMediator mediator, UserManager<User> userManager, 
            SignInManager<User> signInManager, IConfiguration config, 
            BpmnService bpmnService, IGenerateClientJwt clientJwt, 
            IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _bpmnService = bpmnService;
            _clientJwt = clientJwt;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        //treba ubaciti u cqrs ako se hoce ispostovati do kraja sve ali samo zbog jedne linije..
        [HttpGet("StartWriterProcess")]
        public async Task<ActionResult<string>> StartWriterProcess()
        {
            var cometee = await _userManager.GetUsersInRoleAsync("Cometee");
            var genres = _unitOfWork.Genres.GetAll();
            return Ok(await _bpmnService.StartWriterRegistrationProcess(cometee,genres));
        }

        [HttpGet("StartReaderProcess")]
        public async Task<ActionResult<string>> StartReaderProccess()
        {
            var genres = _unitOfWork.Genres.GetAll();
            var beta = _unitOfWork.BetaGenres.GetAll();
            return Ok(await _bpmnService.StartReaderRegistrationProcess(genres,beta));
        }

        [HttpGet("StartPlagiarismProcess")]
        public async Task<ActionResult<string>> StartPlagiarismProcess()
        {
            var cometees = await _userManager.GetUsersInRoleAsync("Cometee");
            var editors = await _userManager.GetUsersInRoleAsync("Editor");
            var username = await GetCurrentUserAsync();
            return Ok(await _bpmnService.StartPlagiarismProcess(cometees,editors,username.UserName));
        }

        //[HttpGet("StartBookProcess")]
        //public async Task<ActionResult<string>> StartBookProcess()
        //{
        //    return Ok(await _bpmnService.StartBookProcess());
        //}

        [HttpGet("GetFormData")]
        public async Task<ActionResult> Get([FromQuery]GetFormDataRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet("GetGenresFormData")]
        public async Task<ActionResult> GetBetaGenresForm([FromQuery]GetBetaFormRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet("GetGenericFormData")]
        public async Task<ActionResult> GetGenericForm([FromQuery] GetFormDataGenericRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("SubmitGenericForm")]
        public async Task<ActionResult> SubmitGenericForm(SubmitFormDataGenericRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("RegisterUser")]
        public async Task<ActionResult> RegisterUser(RegisterUserRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("RegisterReader")]
        public async Task<ActionResult> RegisterReadrUser(RegisterReaderRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("RegisterBetaReader")]
        public async Task<ActionResult> RegisterBetaReader(RegisterBetaReaderRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet("EmailConfirmation")]
        public async Task<ActionResult> EmailConfirmationAsync([FromQuery] EmailConfirmationDto dto)
        {
            var response = await _mediator.Send(new EmailConfirmationWriterRequest() {ProcessInstanceId = dto.ProcessInstanceId, UserId = dto.UserId, Token = dto.Token });
            return Ok(response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto userForLoginDto)
        {
            var user = await _userManager.FindByEmailAsync(userForLoginDto.Email);

            var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);

            if (result.Succeeded)
            {
                var userWithProperties = await _userManager.Users
                    .Include(u => u.UserRoles).ThenInclude(r => r.Role)
                    .FirstOrDefaultAsync(u => u.Email.Equals(userForLoginDto.Email));
                return Ok(new
                {
                    token = _clientJwt.GenerateJwtToken(userWithProperties).Result,
                    user = userWithProperties
                });
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost("Logout")]
        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        private async Task<User> GetCurrentUserAsync() => await _userManager.GetUserAsync(HttpContext.User);


        //[Authorize(Roles ="Writer")]
        [HttpGet("dummy")]
        public ActionResult Dummy()
        {
            var task =  _bpmnService.GetUserTaskResource("RegistrationTask").Result;
            var gg = task.GetRenderedForm().Result;
            var ga = task.GetForm().Result;
            var gf = task.GetFormVariables().Result;
            return Ok("RADI autorizacija i autentifikacija");
        }
    }
}
