using Abp.Application.Services.Dto;
using System;

namespace TACHYON.DriverLicenseTypes.Dtos
{
    public class GetAllDriverLicenseTypesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }



    }
}