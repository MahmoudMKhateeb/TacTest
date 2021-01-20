using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Nationalities.Dtos
{
    public class GetAllNationalitiesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string NameFilter { get; set; }

    }
}