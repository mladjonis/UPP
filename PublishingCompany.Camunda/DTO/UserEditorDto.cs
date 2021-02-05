using PublishingCompany.Camunda.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.DTO
{
    public class UserEditorDto
    {
        public List<User> Editors { get; set; } = new List<User>();
    }
}
