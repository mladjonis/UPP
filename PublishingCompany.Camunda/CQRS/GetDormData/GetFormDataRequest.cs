﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.GetDormData
{
    public class GetFormDataRequest: IRequest<GetFormDataResponse>
    {
    }
}
