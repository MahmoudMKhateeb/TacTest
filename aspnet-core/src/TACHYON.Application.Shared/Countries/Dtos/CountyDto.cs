﻿
using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Countries.Dtos
{
    public class CountyDto : EntityDto
    {
		public string DisplayName { get; set; }

		public string Code { get; set; }



    }
}