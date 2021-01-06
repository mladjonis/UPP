using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.CometeeProcessing
{
    public class CometeeProcessingResponse
    {
        public string ProcessInstanceId { get; set; }
        public string Status { get; set; }
    }
}
