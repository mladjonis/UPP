﻿using MediatR;
using PublishingCompany.Camunda.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.CometeeProcessing
{
    public class CometeeProcessingRequest : IRequest<CometeeProcessingResponse>
    {
        public string TaskId { get; set; }
        public string ProcessInstanceId { get; set; }
        public string ProcessDefinitionId { get; set; }
        public List<FormSubmitDto> SubmitFields { get; set; }
    }
}
