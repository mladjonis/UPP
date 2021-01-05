using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.CometeeProcessing
{
    public class CometeeProcessingHandler : IRequestHandler<CometeeProcessingRequest, CometeeProcessingResponse>
    {
        public async Task<CometeeProcessingResponse> Handle(CometeeProcessingRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
