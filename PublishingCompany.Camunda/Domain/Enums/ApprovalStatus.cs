﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Domain.Enums
{
    public enum ApprovalStatus
    {
        Approved = 0,
        MoreMaterial,
        Rejected,
        Unknown
    }
}
