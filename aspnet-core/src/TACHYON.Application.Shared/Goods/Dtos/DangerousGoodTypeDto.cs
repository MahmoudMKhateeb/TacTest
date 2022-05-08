using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Goods.Dtos
{
    public class DangerousGoodTypeDto : EntityDto
    {
        public string Name { get; set; }

        public int? BayanIntegrationId { get; set; }
    }
}