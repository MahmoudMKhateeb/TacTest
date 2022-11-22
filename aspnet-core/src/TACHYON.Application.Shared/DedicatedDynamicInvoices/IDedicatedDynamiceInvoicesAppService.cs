using Abp.Application.Services;
using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.DedicatedDynamicInvoices.Dtos;

namespace TACHYON.DedicatedDynamicInvoices
{
    public interface IDedicatedDynamiceInvoicesAppService : IApplicationService
    {
        Task<LoadResult> GetAll(string filter);
        Task CreateOrEdit(CreateOrEditDedicatedInvoiceDto input);
        Task<CreateOrEditDedicatedInvoiceDto> GetDedicatedInvoiceForEdit(long id);
        Task Delete(long id);
        Task GenerateDedicatedInvoice(long id);

    }
}
