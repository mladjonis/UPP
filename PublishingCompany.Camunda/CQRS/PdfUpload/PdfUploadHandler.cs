﻿using MediatR;
using Microsoft.AspNetCore.Identity;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.Domain;
using PublishingCompany.Camunda.Repositories;
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
        private readonly IUnitOfWork _unitOfWork;

        public PdfUploadHandler(BpmnService bpmnService, UserManager<User> userManager, IUnitOfWork unitOfWork)
        {
            _bpmnService = bpmnService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<PdfUploadResponse> Handle(PdfUploadRequest request, CancellationToken cancellationToken)
        {
            PdfUploadResponse response = new PdfUploadResponse();
            try
            {
                var processInstanceResource = _bpmnService.GetProcessInstanceResource(request.ProcessInstanceId);
                var userEmail = processInstanceResource.Variables.Get("userEmail").Result.GetValue<string>();
                var user = _unitOfWork.Users.GetUserByEmail(userEmail);

                var docPath = Path.GetFullPath("docs");
                string userPath = Path.Combine(docPath, $"{user.UserName}");
                if (!Directory.Exists(userPath))
                {
                    Directory.CreateDirectory(userPath);
                }

                foreach (var file in request.FormFiles)
                {
                    using (FileStream stream = new FileStream(Path.Combine(userPath,file.FileName), FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                        user.Files += "https://localhost:44343/docs/" + $"{user.UserName}/" + $"{file.FileName},";
                    }
                }
                _unitOfWork.Users.Update(user);
                _unitOfWork.Complete();

                var task = await _bpmnService.GetFirstTask(request.ProcessInstanceId);
                await _bpmnService.SetProcessVariableByProcessInstanceId("documentCountRequired", request.ProcessInstanceId, true);
                await _bpmnService.ClaimTask(task.Id, user.UserName);
                await _bpmnService.CompleteTask(task.Id);
                response.Status = "Uploaded successfully";
            }
            catch(Exception e)
            {
                response.Status = e.Message;
            }
            return response;
        }
    }
}
