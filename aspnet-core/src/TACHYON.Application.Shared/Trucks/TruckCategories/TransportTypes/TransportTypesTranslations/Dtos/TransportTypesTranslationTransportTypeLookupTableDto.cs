using Abp.Application.Services.Dto;

namespace TACHYON.Trucks.TruckCategories.TransportTypes.TransportTypesTranslations.Dtos
{
    public class TransportTypesTranslationTransportTypeLookupTableDto
    {
        public int Id { get; set; }

        public string TranslatedDisplayName { get; set; }
    }
}