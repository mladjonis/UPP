using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.CQRS.EmailConfirmationWriter;
using PublishingCompany.Camunda.CQRS.GetDormData;
using PublishingCompany.Camunda.CQRS.RegisterUser;
using PublishingCompany.Camunda.Domain;
using PublishingCompany.Camunda.DTO;
using PublishingCompany.Camunda.Helpers.ClientTokenGenerator;
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

        public RegistrationController(
            IMediator mediator, UserManager<User> userManager, 
            SignInManager<User> signInManager, IConfiguration config, 
            BpmnService bpmnService, IGenerateClientJwt clientJwt, IMapper mapper)
        {
            _mediator = mediator;
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _bpmnService = bpmnService;
            _clientJwt = clientJwt;
            _mapper = mapper;
        }

        //treba ubaciti u cqrs ako se hoce ispostovati do kraja sve ali samo zbog jedne linije..
        [HttpGet("StartWriterProcess")]
        public async Task<ActionResult<string>> StartProcessAsync()
        {
            var cometee = await _userManager.GetUsersInRoleAsync("Cometee");
            return Ok(await _bpmnService.StartWriterRegistrationProcess(cometee));
        }

        [HttpGet("GetFormData")]
        public async Task<ActionResult> Get([FromQuery]GetFormDataRequest request)
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

        //treba ubaciti u cqrs isto da kada klikne na ovo fino ga napravi ali za sada neka ovde ima jako malo koda
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

        [HttpGet("Logout")]
        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }


        //[Authorize(Roles ="Writer")]
        [HttpGet("dummy")]
        public ActionResult Dummy()
        {
            var userDto = new UserDto() { Name = "ijasdoiajsdoi" };
            var user = _mapper.Map<User>(userDto);
            return Ok("RADI autorizacija i autentifikacija");
        }
    }
}
