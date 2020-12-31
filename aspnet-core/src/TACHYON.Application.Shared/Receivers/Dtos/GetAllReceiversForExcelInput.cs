using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Receivers.Dtos
{
    public class GetAllReceiversForExcelInput
    {
        public string Filter { get; set; }

        public string FullNameFilter { get; set; }

        public string EmailFilter { get; set; }

        public string PhoneNumberFilter { get; set; }

        public string FacilityNameFilter { get; set; }

    }
}