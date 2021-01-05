using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.DTO
{
    public class EmailConfirmationDto
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string ProcessInstanceId { get; set; }
    }
}
