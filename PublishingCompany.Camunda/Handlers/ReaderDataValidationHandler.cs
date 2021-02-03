using AutoMapper;
using Camunda.Api.Client.User;
using Camunda.Worker;
using MediatR;
using Microsoft.AspNetCore.Identity;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.Domain;
using PublishingCompany.Camunda.DTO;
using PublishingCompany.Camunda.Helpers.FormSubmitMapper;
using PublishingCompany.Camunda.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Handlers
{
    [HandlerTopics("Topic_ReaderDataValidationHandler", LockDuration = 10_000)]
    public class ReaderDataValidationHandler : ExternalTaskHandler
    {
        private readonly BpmnService _bpmnService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFormSubmitDtoMapper _dtoMapper;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public ReaderDataValidationHandler(IMapper mapper, IMediator mediator, IFormSubmitDtoMapper dtoMapper, IUnitOfWork unitOfWork, BpmnService bpmnService, UserManager<User> userManager)
        {
            this._mapper = mapper;
            this._mediator = mediator;
            this._unitOfWork = unitOfWork;
            this._bpmnService = bpmnService;
            this._dtoMapper = dtoMapper;
            this._userManager = userManager;

        }
        public async override Task<IExecutionResult> Process(ExternalTask externalTask)
        {
            if (externalTask.Retries == null || externalTask.Retries == 0)
            {
                externalTask.Retries = 3;
            }

            externalTask.Retries -= 1;
            //
            try
            {
                //deserializacija vrednosti koje su potrebne za serijalizaciju na workeru
                var processInstanceResource = _bpmnService.GetProcessInstanceResource(externalTask.ProcessInstanceId);
                var readerRegistrationValues = await processInstanceResource.Variables.Get("readerRegistrationValues");
                var readerBetaValues = await processInstanceResource.Variables.Get("readerRegistrationValuesGenres");
                var jArrayValue = (Newtonsoft.Json.Linq.JArray)readerRegistrationValues.Value;
                var submittedData = jArrayValue.ToObject<List<FormSubmitDto>>();
                //mapiraj submitovane podatke na usera
                var userDto = _dtoMapper.MapFormDataToUserDto(submittedData);
                var user = _mapper.Map<User>(userDto);
                var jArrayValue2 = (Newtonsoft.Json.Linq.JArray)readerRegistrationValues.Value;
                var submittedGenres = jArrayValue.ToObject<List<FormSubmitDto>>();
                var betaGenres = _dtoMapper.MapFormDataToUserDto(submittedGenres);

                var userProfile = _mapper.Map<UserProfileInfo>(user);

                //izvrsi validaciju i ovde mozda mada prilikom submitovanja forme se vec vrsi validacija i puca ako nije dobro nesto tako da je mozda nepotrebno

                //proveri da li korisnik sa tim emailom postoji u bazi vec
                var userExist = _unitOfWork.Users.GetUserByEmail(userDto.Email);
                var userNameExists = _unitOfWork.Users.Find(x => x.UserName.Equals(userDto.Username)).ToList().FirstOrDefault();
                if (userExist != null && userNameExists != null)
                {
                    //    //postavi procesnu varijablu validacija na false jer valdiacija nije prosla - vec je postavljena u bpmnService klasi
                    return new CompleteResult()
                    {
                        Variables = new Dictionary<string, Variable>
                        {
                            ["ReaderValidationError"] = new Variable("User already exists", VariableType.String)
                        }
                    };
                }
                //ako ne postoji dodaj korisnika u bazu i u camundinu bazu da bi kasnije mogao da claimuje usertaskove koji se claimuju a ne kao forma automatski
                var registrationResult = await _userManager.CreateAsync(user, userDto.Password);
                if (!registrationResult.Succeeded)
                {
                    return new CompleteResult()
                    {
                        Variables = new Dictionary<string, Variable>
                        {
                            ["ReaderValidationError"] = new Variable(registrationResult.Errors, VariableType.Object)
                        }
                    };
                }

                if (bool.Parse(userDto.BetaReader)) 
                {
                    var userRole = await _userManager.AddToRoleAsync(user, "BetaReader");
                    if (!userRole.Succeeded)
                    {
                        return new CompleteResult()
                        {
                            Variables = new Dictionary<string, Variable>
                            {
                                ["WriterValidationError"] = new Variable(userRole.Errors, VariableType.Object)
                            }
                        };
                    }
                    user.BetaGenres = betaGenres.BetaReaderGenres;
                    _unitOfWork.Users.Update(user);
                    _unitOfWork.Complete();
                }
                else
                {
                    var userRole = await _userManager.AddToRoleAsync(user, "Reader");
                    if (!userRole.Succeeded)
                    {
                        return new CompleteResult()
                        {
                            Variables = new Dictionary<string, Variable>
                            {
                                ["WriterValidationError"] = new Variable(userRole.Errors, VariableType.Object)
                            }
                        };
                    }
                }

                await _bpmnService.CreateUser(userProfile, userDto.Password);
                await _bpmnService.SetProcessVariableByProcessInstanceId("validation", externalTask.ProcessInstanceId, true);
                await _bpmnService.SetProcessVariableByProcessInstanceId("userEmail", externalTask.ProcessInstanceId, user.Email);

            }
            catch (Exception e)
            {
                return new CompleteResult()
                {
                    Variables = new Dictionary<string, Variable>
                    {
                        ["WriterValidationError"] = new Variable(e.Message, VariableType.String)
                    }
                };
            }

            return new CompleteResult()
            {
                Variables = new Dictionary<string, Variable>
                {
                    ["WriterValidationError"] = new Variable("", VariableType.String)
                }
            };
        }
    }
}
