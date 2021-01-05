using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.CometeeProcessing
{
    public class CometeeProcessingRequest : IRequest<CometeeProcessingResponse>
    {
        public string Comment { get; set; }
        public object Decision { get; set; }
    }
}
