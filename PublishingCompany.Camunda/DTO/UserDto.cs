using PublishingCompany.Camunda.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.DTO
{
    public class UserDto
    {
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string BetaReader { get; set; } = null;
        public List<Genre> Genres { get; set; } = new List<Genre>();
        public List<Genre> BetaReaderGenres { get; set; } = new List<Genre>();
    }
}
