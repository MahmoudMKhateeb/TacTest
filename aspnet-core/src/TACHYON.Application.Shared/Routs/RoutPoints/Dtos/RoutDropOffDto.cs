﻿using Abp.Application.Services.Dto;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TACHYON.AddressBook.Dtos;
using TACHYON.Goods.GoodsDetails.Dtos;
using TACHYON.Routs.RoutSteps;

namespace TACHYON.Routs.RoutPoints.Dtos
{
    public class RoutDropOffDto : EntityDto<long>
    {
        public string Facility { get; set; }
        public string Address { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string PackagingType { get; set; }
        public int? Rating { get; set; }
        public string ReceiverFullName { get; set; }
        public string ReceiverPhoneNumber { get; set; }
        public string ReceiverEmailAddress { get; set; }
        public string ReceiverCardIdNumber { get; set; }
        public string Note { get; set; }

        public List<GoodsDetailDto> GoodsDetailListDto { get; set; }


       // public double TotalWeight { get; set; }

       // public string UnitOfMeasure { get; set; } = "Kg";

        //to do receiver attribute
    }
}