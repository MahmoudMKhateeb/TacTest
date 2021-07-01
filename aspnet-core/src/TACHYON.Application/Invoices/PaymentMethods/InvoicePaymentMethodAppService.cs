using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Common.Dto;
using TACHYON.Invoices.PaymentMethods.Dto;

namespace TACHYON.Invoices.PaymentMethods
{
    [AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_PaymentMethods)]
    public class InvoicePaymentMethodAppService :TACHYONAppServiceBase, IInvoicePaymentMethodAppService
    {
        private readonly IRepository<InvoicePaymentMethod> _invoicePaymentMethodRepository;
        private readonly IHostApplicationLifetime _appLifetime;

        public InvoicePaymentMethodAppService(IRepository<InvoicePaymentMethod> invoicePaymentMethodRepository, IHostApplicationLifetime appLifetime)
        {
            _invoicePaymentMethodRepository = invoicePaymentMethodRepository;
            _appLifetime = appLifetime;
        }


        public ListResultDto<InvoicePaymentMethodListDto> GetAll(FilterInput Input)
        {
            var query = _invoicePaymentMethodRepository
                .GetAll()
                .WhereIf(!string.IsNullOrEmpty(Input.Filter), x => x.DisplayName.ToLower().Contains(Input.Filter.Trim().ToLower()))
                .OrderBy(Input.Sorting ?? "id");

           return new ListResultDto<InvoicePaymentMethodListDto>(ObjectMapper.Map<List<InvoicePaymentMethodListDto>>(query));
        }
        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_PaymentMethod_Edit)]

        public async Task<CreateOrEditInvoicePaymentMethod> CreateOrEdit(CreateOrEditInvoicePaymentMethod input)
        {
            if (_invoicePaymentMethodRepository.GetAll().Any(x=>x.Id !=input.Id && x.DisplayName.ToLower()== input.DisplayName.Trim().ToLower()))
            {
               throw new UserFriendlyException(L("TheNameIsRepeated"));

            }
            if (input.Id == 0)
            {
                await Task.Run(() => { _appLifetime.StopApplication(); });
                return await Create(input);

            }
            else
               return await Edit(input);

        }

        private async Task<CreateOrEditInvoicePaymentMethod> Create(CreateOrEditInvoicePaymentMethod input)
        {
            input.Id = await _invoicePaymentMethodRepository.InsertAndGetIdAsync(ObjectMapper.Map<InvoicePaymentMethod>(input));
           return input;

        }
        private async Task<CreateOrEditInvoicePaymentMethod> Edit(CreateOrEditInvoicePaymentMethod input)
        {
            var payment = await _invoicePaymentMethodRepository.FirstOrDefaultAsync(x => x.Id == input.Id);
            ObjectMapper.Map(input, payment);
            return input;
        }

        public async Task Delete(EntityDto input)
        {
            await  _invoicePaymentMethodRepository.DeleteAsync(input.Id);
        }


    }
}
