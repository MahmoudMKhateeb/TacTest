﻿using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Vases.Dtos;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public abstract class EditVasStepBaseDto: EntityDto<long>, IHasVasListDto
    {
        public List<CreateOrEditShippingRequestVasListDto> ShippingRequestVasList { get; set; }
        public bool IsDrafted { get; set; }
        public int DraftStep { get; set; }
    }
}