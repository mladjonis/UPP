using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Domain
{
    public class Book
    {
        public Guid Id { get; set; }
        public string HeadLine { get; set; }
        public List<User> Writers { get; set; } = new List<User>();
        public Genre Genre { get; set; }
        public string ISBN { get; set; }
        public string Keywords { get; set; }
        public int YearIssued { get; set; }
        public string Issuer { get; set; }
        public string PlaceIssued { get; set; }
        public int NumberOfPages { get; set; }
        public string Synopsis { get; set; }
        public bool IsPlagiarism { get; set; }
        public bool IsPublished { get; set; }
        public string BookNameInSystem { get; set; }
        public List<Comment> BookComments { get; set; } = new List<Comment>();
    }
}
