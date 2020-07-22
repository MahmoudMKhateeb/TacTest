using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Trailers.TrailerStatuses.Dtos
{
    public class GetAllTrailerStatusesForExcelInput
    {
        public string Filter { get; set; }

        public string DisplayNameFilter { get; set; }



    }
}