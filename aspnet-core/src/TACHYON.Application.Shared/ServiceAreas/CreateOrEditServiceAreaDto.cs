using Abp.Application.Services.Dto;

namespace TACHYON.ServiceAreas
{
    public class CreateOrEditServiceAreaDto : EntityDto<long?>
    {
        public int CityId { get; set; }
        
    }
}