using Abp.Application.Services.Dto;
using System;

namespace TACHYON.EmailTemplates.Dtos
{
    public class GetAllEmailTemplatesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string NameFilter { get; set; }

        public string DisplayNameFilter { get; set; }

    }
}