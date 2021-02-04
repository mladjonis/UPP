using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.PlagiarismProposal
{
    public class PlagiarismProposalResponse
    {
        public string RegistrationStatus { get; set; }
        public string ProcessInstanceId { get; set; }
    }
}
