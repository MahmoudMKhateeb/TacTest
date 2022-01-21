using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TACHYON.Common;
using TACHYON.CustomValidation;
using TACHYON.Routs.RoutPoints.Dtos;

namespace TACHYON.Shipping.Trips.Accidents.Dto
{
    public class CreateOrEditShippingRequestTripAccidentCommentDto : EntityDto
    {
        public long AccidentId { get; set; }
        public string Comment { get; set; }
    }
}