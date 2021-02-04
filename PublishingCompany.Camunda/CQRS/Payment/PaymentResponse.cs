using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.Payment
{
    public class PaymentResponse
    {
        public string PaymentStatus { get; set; }
        public string ProcessInstanceId { get; set; }
    }
}
