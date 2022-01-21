using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Routs.Dtos
{
    public class GetAllRoutesForExcelInput
    {
        public string Filter { get; set; }

        public string DisplayNameFilter { get; set; }


        public string RoutTypeDisplayNameFilter { get; set; }
    }
}