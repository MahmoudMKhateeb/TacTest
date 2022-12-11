using Abp.Application.Services.Dto;

namespace TACHYON.PriceOffers
{
    public class PricePackageAppendixItem: EntityDto
    {
        public bool IsTmsPricePackage { get; set; }
    }
}