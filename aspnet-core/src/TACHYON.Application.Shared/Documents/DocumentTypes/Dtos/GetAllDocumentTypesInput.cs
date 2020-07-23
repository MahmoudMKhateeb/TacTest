using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Documents.DocumentTypes.Dtos
{
    public class GetAllDocumentTypesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string DisplayNameFilter { get; set; }

        public int IsRequiredFilter { get; set; }

        public DateTime? MaxExpirationDateFilter { get; set; }
        public DateTime? MinExpirationDateFilter { get; set; }

        public int HasExpirationDateFilter { get; set; }

        public string RequiredFromFilter { get; set; }



    }
}