using Camunda.Api.Client.UserTask;
using PublishingCompany.Camunda.Domain;
using PublishingCompany.Camunda.DTO;
using PublishingCompany.Camunda.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Helpers.FormSubmitMapper
{
    public class FormSubmitDtoMapper : IFormSubmitDtoMapper
    {
        //format string id,value|id,value
        //obratiti paznju na deserijalizaciju zadnjeg clana njega izbaciti jer je space tab da ne bi pucalo
        /*
         * 		var splitted = str.Split('|');
		for(int i=0;i<splitted.Length-1;i++){
			Console.WriteLine(string.IsNullOrWhiteSpace(splitted[i]));
		}
         */
        private readonly IUnitOfWork _unitOfWork;
        public FormSubmitDtoMapper(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public string MapDtoToString(List<FormSubmitDto> submitDtos)
        {
            var builder = new StringBuilder();
            foreach (var dto in submitDtos)
            {
                builder.Append($"{dto.FieldId},{dto.FieldValue}").Append("|");
            }
            return builder.ToString();
        }

        public Dictionary<string,string> DeserializeMappedDtos(string dtoValues)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            var splittedKeyValuePaires = dtoValues.Split('|');
            //-1 zbog zadnjeg | znaka da se izuzme
            for(int i = 0; i < splittedKeyValuePaires.Length - 1; i++)
            {
                //kvp -> string sa id,value deserializovan zbog objecta da bi se znao koji je tip podatka
                var kvp = splittedKeyValuePaires[i].Split(',');
                values.Add(kvp[0], kvp[1]);
            }
            return values;
        }

        public CompleteTask SetFormValues(Dictionary<string, string> dtoKvp)
        {
            var completeTask = new CompleteTask();
            foreach (var dto in dtoKvp)
            {
                completeTask.SetVariable(dto.Key, dto.Value);
            }
            return completeTask;
        }

        public CompleteTask GetFormValues(List<FormSubmitDto> submitDtos)
        {
            var mappedDtos = this.MapDtoToString(submitDtos);
            var deserializedMappedDtos = this.DeserializeMappedDtos(mappedDtos);
            var setFormValues = this.SetFormValues(deserializedMappedDtos);
            return setFormValues;
        }

        public UserDto MapFormDataToUserDto(List<FormSubmitDto> submitDtos)
        {
            var genres = _unitOfWork.Genres.GetAll().ToList();
            var betaGenres = _unitOfWork.BetaGenres.GetAll().ToList();
            UserDto userDto = new UserDto();
            foreach (var data in submitDtos)
            {
                if (data.FieldId.Equals("username"))
                {
                    //Type myType = typeof(FormSubmitDto);
                    //PropertyInfo myPropInfo = myType.GetProperty(data.FieldId);
                    //var propName = myPropInfo.GetValue(data.FieldId);
                    userDto.Username = (string)data.FieldValue;
                }
                if (data.FieldId.Equals("name"))
                {
                    userDto.Name = (string)data.FieldValue;
                }
                 if (data.FieldId.Equals("last_name"))
                {
                    userDto.Lastname = (string)data.FieldValue;
                }
                 if (data.FieldId.Equals("email"))
                {
                    userDto.Email = (string)data.FieldValue;
                }
                 if (data.FieldId.Equals("city"))
                {
                    userDto.City = (string)data.FieldValue;
                }
                 if (data.FieldId.Equals("state"))
                {
                    userDto.State = (string)data.FieldValue;
                }
                 if (data.FieldId.Equals("username"))
                {
                    userDto.State = (string)data.FieldValue;
                }
                 if (data.FieldId.Equals("password"))
                {
                    userDto.Password = (string)data.FieldValue;
                }
                if (data.FieldId.Equals("beta_reader"))
                {
                    userDto.BetaReader = (string)data.FieldValue;
                }
                else if (data.FieldId.Equals("genres_"))
                {
                    var split = data.FieldValue.ToString().Split(',');
                    //// -1 zbog zadnjeg , ipak nce trebati -1 sredjeno na frontu fino sve
                    for (int i = 0; i < split.Length; i++)
                    {
                        //gadno je ali me mrzilo da uradim ljepse.. ovo cu da ubacim u repo da trazi po imenu
                        if (genres.Find(x => x.Name.ToLower().Equals(split[i])) == null)
                        {
                            _unitOfWork.Genres.Add(new Genre() { Name = split[i] });
                            _unitOfWork.Complete();
                        }
                        else
                        {
                            userDto.Genres.Add(genres.Where(g => g.Name.ToLower().Equals(split[i])).FirstOrDefault());
                        }
                    }
                }
                else if (data.FieldId.Equals("beta_reader_genres"))
                {
                    var split = data.FieldValue.ToString().Split(',');
                    //// -1 zbog zadnjeg , ipak nce trebati -1 sredjeno na frontu fino sve
                    for (int i = 0; i < split.Length; i++)
                    {
                        //gadno je ali me mrzilo da uradim ljepse.. ovo cu da ubacim u repo da trazi po imenu
                        if (genres.Find(x => x.Name.ToLower().Equals(split[i])) == null)
                        {
                            _unitOfWork.BetaGenres.Add(new BetaGenre() { Name = split[i] });
                            _unitOfWork.Complete();
                        }
                        else
                        {
                            userDto.BetaReaderGenres.Add(betaGenres.Where(g => g.Name.ToLower().Equals(split[i])).FirstOrDefault());
                        }
                    }
                }
            }
            return userDto;
        }

        public UserEditorDto MapFromDataToUserEditorDto(List<FormSubmitDto> submitDtos)
        {
            var users = _unitOfWork.Users.GetAll();
            UserEditorDto user = new UserEditorDto();
            foreach(var data in submitDtos)
            {
                if (data.FieldId.Equals("plagiarism_editors"))
                {
                    var split = data.FieldValue.ToString().Split(',');
                    for (int i = 0; i < split.Length; i++)
                    {
                        user.Editors.Add(users.Where(g => g.FullName().ToLower().Equals(split[i])).FirstOrDefault());
                    }
                }
            }

            return user;
        }
    }
}
