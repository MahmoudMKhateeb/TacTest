﻿using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Vases.Dtos
{
    public class VasDto : EntityDto
    {
        public string Name { get; set; }

        public bool HasAmount { get; set; }

        public bool HasCount { get; set; }
        public  DateTime CreationTime { get; set; }


    }
}