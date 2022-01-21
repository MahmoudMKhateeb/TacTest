namespace TACHYON.Trucks.PlateTypes.Dtos
{
    public class GetPlateTypeForViewDto
    {
        public PlateTypeDto PlateType { get; set; }

        public virtual string Name { get; set; }

        public string BayanIntegrationId { get; set; }
    }
}