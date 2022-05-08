using Abp.Application.Services.Dto;

namespace TACHYON.Vases.Dtos
{
    public class AvailableVasDto : EntityDto
    {
        public string VasName { get; set; }

        public int VasId { get; set; }

        public double Price { get; set; }

        public int? MaxCount { get; set; }

        public int? MaxAmount { get; set; }
    }
}