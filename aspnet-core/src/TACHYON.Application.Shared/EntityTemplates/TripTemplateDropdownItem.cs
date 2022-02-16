using Abp.Application.Services.Dto;
using TACHYON.Shipping.Trips.Dto;

namespace TACHYON.EntityTemplates
{
    public class TripTemplateDropdownItem : EntityDto<long>
    {
        public CreateOrEditShippingRequestTripDto Trip { get; set; }

        public string TemplateName { get; set; }
    }
}