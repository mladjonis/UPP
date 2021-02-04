using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Domain
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string BookComment { get; set; }
        public Book Book { get; set; }
        public User User { get; set; }
    }
}
