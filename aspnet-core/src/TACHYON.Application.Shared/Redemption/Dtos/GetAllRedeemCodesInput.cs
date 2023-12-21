using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Redemption.Dtos
{
    public class GetAllRedeemCodesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string CodeFilter { get; set; }

        public DateTime? MaxExpiryDateFilter { get; set; }
        public DateTime? MinExpiryDateFilter { get; set; }

        public int? IsActiveFilter { get; set; }

        public decimal? MaxValueFilter { get; set; }
        public decimal? MinValueFilter { get; set; }

        public string NoteFilter { get; set; }

        public int? MaxpercentageFilter { get; set; }
        public int? MinpercentageFilter { get; set; }

    }
}