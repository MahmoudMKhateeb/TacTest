using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Common;
using TACHYON.Invoices.Groups;
using TACHYON.Invoices.Groups.Dto;

namespace TACHYON.AutoMapper.Invoices.SubmitInvoices
{
   public class SubmitInvoiceProfile:Profile
    {
        public SubmitInvoiceProfile()
        {
            CreateMap<GroupPeriodClaimCreateInput,DocumentUpload>();
            CreateMap<IHasDocument, GroupPeriod>().ReverseMap();

        }
    }
}
