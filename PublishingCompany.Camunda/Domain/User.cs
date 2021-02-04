using Microsoft.AspNetCore.Identity;
using PublishingCompany.Camunda.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Domain
{
    public class User : IdentityUser<Guid>
    {
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Unknown;
        public string Files { get; set; }
        public List<CometeeComment> CometeeComments { get; set; }
        public List<UserRole> UserRoles { get; set; }
        public List<Genre> Genres { get; set; }
        public List<BetaGenre> BetaGenres { get; set; } = new List<BetaGenre>();
        public long Amount { get; set; }
    }
}
