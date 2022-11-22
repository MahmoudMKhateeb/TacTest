using TACHYON.Actors;

using System;
using Abp.Application.Services.Dto;
using TACHYON.Documents.DocumentFiles.Dtos;

namespace TACHYON.Actors.Dtos
{
    public class ActorDto : EntityDto
    {
        public string CompanyName { get; set; }

        public ActorTypesEnum ActorType { get; set; }

        public string MoiNumber { get; set; }

        public string Address { get; set; }

        public string MobileNumber { get; set; }

        public string Email { get; set; }

        public int InvoiceDueDays { get; set; }
        public bool IsActive { get; set; }
        public DocumentFileDto DocumentFile { get; set; }


    }
}