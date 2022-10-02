using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using TACHYON.Common;
using TACHYON.CustomValidation;
using TACHYON.Documents.DocumentFiles.Dtos;

namespace TACHYON.Actors.Dtos
{
    public class GetActorForEditOutput
    {
        public CreateOrEditActorDto Actor { get; set; }

    }
}