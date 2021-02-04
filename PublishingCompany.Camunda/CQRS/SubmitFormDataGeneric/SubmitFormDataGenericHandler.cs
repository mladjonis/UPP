﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.SubmitFormDataGeneric
{
    public class SubmitFormDataGenericHandler : IRequestHandler<SubmitFormDataGenericRequest, SubmitFormDataGenericResponse>
    {
        public Task<SubmitFormDataGenericResponse> Handle(SubmitFormDataGenericRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
