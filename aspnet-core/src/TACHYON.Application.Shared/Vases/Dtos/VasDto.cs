using System;
using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace TACHYON.Vases.Dtos
{
    public class VasDto : EntityDto
    {
        public string Key { get; set; }

        public bool HasAmount { get; set; }

        public bool HasCount { get; set; }
    }
}