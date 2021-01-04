using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.EmailConfirmationWriter
{
    public class EmailConfirmationWriterRequest : IRequest<EmailConfirmationWriterResponse>
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string ProcessInstanceId { get; set; }
    }
}
