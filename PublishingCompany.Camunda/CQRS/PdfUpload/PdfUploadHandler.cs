using MediatR;
using Microsoft.AspNetCore.Identity;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.CQRS.PdfUpload
{
    public class PdfUploadHandler : IRequestHandler<PdfUploadRequest, PdfUploadResponse>
    {
        private readonly BpmnService _bpmnService;
        private readonly UserManager<User> _userManager;

        public PdfUploadHandler(BpmnService bpmnService, UserManager<User> userManager)
        {
            _bpmnService = bpmnService;
            _userManager = userManager;
        }

        public async Task<PdfUploadResponse> Handle(PdfUploadRequest request, CancellationToken cancellationToken)
        {
            PdfUploadResponse response = new PdfUploadResponse();
            try
            {
                var docPath = Path.GetFullPath("docs");
                string userPath = Path.Combine(docPath,$"{request.User.Name}{request.User.Lastname}");
                if (!Directory.Exists(userPath))
                {
                    Directory.CreateDirectory(userPath);
                }


                //srediti putanju da upada u folder a ne vani
                foreach (var file in request.FormFiles)
                {
                    using (FileStream stream = new FileStream(Path.Combine(userPath,file.FileName), FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
            }catch(Exception e)
            {
                response.Status = e.Message;
            }
            response.Status = "Uploaded successfully";
            return response;
        }
    }
}
