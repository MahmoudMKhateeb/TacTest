using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Authorization.Users;
using TACHYON.Common;
using TACHYON.Features;
using TACHYON.Invoices.Balances;
using TACHYON.Invoices.Dto;
using TACHYON.Invoices.Periods;
using TACHYON.Invoices.Transactions;

namespace TACHYON.Invoices
{
    public class InvoiceAppService : TACHYONAppServiceBase, IInvoiceAppService
    {

        private readonly IRepository<Invoice, long> _InvoiceRepository;
        private readonly CommonManager _commonManager;
        private readonly BalanceManager _BalanceManager ;
        private readonly IUnitOfWorkManager _UnitOfWorkManager;
        private readonly UserManager _userManager;
        private readonly InvoiceManager _invoiceManager;
        private readonly TransactionManager _transactionManager;
        public InvoiceAppService(
            IRepository<Invoice, long> InvoiceRepository,
            CommonManager commonManager,
            BalanceManager BalanceManager,
            IUnitOfWorkManager UnitOfWorkManager,
            UserManager userManager,
            InvoiceManager invoiceManager,
            TransactionManager transactionManager)
        {
            _InvoiceRepository = InvoiceRepository;
            _commonManager = commonManager;
            _BalanceManager = BalanceManager;
            _UnitOfWorkManager = UnitOfWorkManager;
            _userManager = userManager;
            _invoiceManager = invoiceManager;
            _transactionManager = transactionManager;
        }


        [AbpAuthorize(AppPermissions.Pages_Invoices)]
        public async Task<PagedResultDto<InvoiceListDto>> GetAll(InvoiceFilterInput input)

        {
            return  await _commonManager.ExecuteMethodIfHostOrTenantUsers(() =>  GetInvoices(input));
        }
        [AbpAuthorize(AppPermissions.Pages_Invoices)]

        public async Task<InvoiceInfoDto> GetById(EntityDto input)
        {
            var Invoice= await _commonManager.ExecuteMethodIfHostOrTenantUsers(() => GetInvoiceInfo(input.Id));

            var InvoiceDto = ObjectMapper.Map<InvoiceInfoDto>(Invoice);
            if (InvoiceDto != null)
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    var user = await _userManager.Users.SingleAsync(u => u.TenantId == Invoice.TenantId && u.UserName== AbpUserBase.AdminUserName);

                    InvoiceDto.Email = user.EmailAddress;
                    InvoiceDto.Phone = user.PhoneNumber;
                }

            }
            return InvoiceDto;

        }

        private  async Task<Invoice> GetInvoiceInfo(int InvoiceId)
        {
         return  await _InvoiceRepository
                            .GetAll()
                            .Include(i => i.InvoicePeriod)
                            .Include(i => i.Tenant)
                            .Include(i => i.ShippingRequests)
                             .ThenInclude(r => r.ShippingRequests)
                              .ThenInclude(r=>r.TrucksTypeFk)
                            .SingleAsync(i => i.Id == InvoiceId);
        }
        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_Delete)]

        public async Task Delete(EntityDto Input)
        {
            var Invoice = await GetInvoice(Input.Id);
            if (Invoice != null)
            {
                if (Invoice.IsPaid)
                {
                    if (!_invoiceManager.IsCarrier(Invoice.TenantId))
                    {
                        await _BalanceManager.AddBalanceToShipper(Invoice.TenantId, Invoice.AmountWithTaxVat);
                        if ((InvoicePeriodType)Invoice.PeriodId !=InvoicePeriodType.PayInAdvance)
                        {
                            await _BalanceManager.AddCreditBalanceToShipper(Invoice.TenantId, -Invoice.AmountWithTaxVat);

                        }
                        await _transactionManager.Delete(Invoice.Id, ChannelType.Invoices);

                    }
                    else
                        await _BalanceManager.AddBalanceToCarrier(Invoice.TenantId, +Invoice.AmountWithTaxVat);
                }

                await _InvoiceRepository.DeleteAsync(Input.Id);
                await _invoiceManager.RemoveInvoiceFromRequest(Input.Id);

            }

        }

        public async Task<bool> MakePaid(long invoiceId)
        {
            var Invoice = await GetInvoice(invoiceId);
            if (Invoice != null && !Invoice.IsPaid)
            {
                return await _commonManager.ExecuteMethodIfHostOrTenantUsers(async () =>
                {
                    if (!_invoiceManager.IsCarrier(Invoice.TenantId))
                    {

                        if (await _BalanceManager.CheckShipperCanPaidFromBalance(Invoice.TenantId, Invoice.AmountWithTaxVat))
                        {
                            await _BalanceManager.AddBalanceToShipper(Invoice.TenantId, -Invoice.AmountWithTaxVat);

                            if ((InvoicePeriodType)Invoice.PeriodId != InvoicePeriodType.PayInAdvance)
                            {
                                await _BalanceManager.AddCreditBalanceToShipper(Invoice.TenantId, Invoice.AmountWithTaxVat);
                            }
                        }
                        else
                        return false;
                    }
                    else
                    {
                        await _BalanceManager.AddBalanceToCarrier(Invoice.TenantId, +Invoice.AmountWithTaxVat);
                    }
                    await _transactionManager.Create(new Transaction
                    {
                        Amount = Invoice.AmountWithTaxVat,
                        ChannelId = (byte)ChannelType.Invoices,
                        TenantId = Invoice.TenantId,
                        SourceId = Invoice.Id,
                    });
                    Invoice.IsPaid = true;
                    return true;
                }, AppFeatures.ShipperCanMakInvoicePaid);
            }
            return false;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_MakeUnPaid)]
        public async Task MakeUnPaid(long invoiceId)
        {
            var Invoice = await GetInvoice(invoiceId);
            if (Invoice != null && Invoice.IsPaid)
            {

                if (!_invoiceManager.IsCarrier(Invoice.TenantId))
                {
                    await _BalanceManager.AddBalanceToShipper(Invoice.TenantId, Invoice.AmountWithTaxVat);
                    if ((InvoicePeriodType)Invoice.PeriodId != InvoicePeriodType.PayInAdvance)
                    {
                        await _BalanceManager.AddCreditBalanceToShipper(Invoice.TenantId, -Invoice.AmountWithTaxVat);
                    }
                }
                else
                {
                    await _BalanceManager.AddBalanceToCarrier(Invoice.TenantId, -Invoice.AmountWithTaxVat);
                }

                await _transactionManager.Delete(Invoice.Id, ChannelType.Invoices);
                Invoice.IsPaid = false;
            }
        }


        #region Helper 
        private async Task<PagedResultDto<InvoiceListDto>> GetInvoices(InvoiceFilterInput input)
        {
            var query = _InvoiceRepository
                .GetAll()
                .Include(i=>i.InvoicePeriod)
                .Include(i => i.Tenant)
                 .ThenInclude(t=>t.Edition)
                .WhereIf(input.IsAccountReceivable.HasValue, e => e.IsAccountReceivable == input.IsAccountReceivable)
                .WhereIf(input.IsPaid.HasValue, e => e.IsPaid == input.IsPaid)
                .WhereIf(input.TenantId.HasValue, i => i.TenantId == input.TenantId)
                .WhereIf(input.PeriodId.HasValue, i => i.PeriodId == input.PeriodId)
                .WhereIf(input.FromDate.HasValue && input.ToDate.HasValue, i => i.DueDate >= input.FromDate && i.CreationTime < input.ToDate)
                .AsNoTracking();
            var pagedInvoices = query
              .OrderBy(input.Sorting ?? "IsPaid asc")
              .PageBy(input);

            var totalCount = await pagedInvoices.CountAsync();

            return new PagedResultDto<InvoiceListDto>(
                totalCount,
                ObjectMapper.Map<List<InvoiceListDto>>(pagedInvoices)
            );
        }

        private async Task<Invoice> GetInvoice(long  invoiceId)
        {
          return await _InvoiceRepository.SingleAsync(i => i.Id == invoiceId);
        }

      




        #endregion
    }
}
