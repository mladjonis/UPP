using PublishingCompany.Camunda.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Domain
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string ProcessInstanceId { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public UserRegistrationStatus UserRegistrationStatus { get; set; }
        public List<Genre> Genres{ get; set; }
    }
}
