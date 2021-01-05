using MediatR;
using Microsoft.AspNetCore.Http;
using PublishingCompany.Camunda.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.PdfUpload
{
    public class PdfUploadRequest : IRequest<PdfUploadResponse>
    {
        public List<IFormFile> FormFiles { get; set; }
        public User User { get; set; }
        public string ProcessInstanceId { get; set; }
    }
}
