using Abp.Application.Features;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.Threading;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;
using TACHYON.Common;
using TACHYON.Configuration;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Features;
using TACHYON.Invoices.Dto;
using TACHYON.Invoices.InoviceNote;
using TACHYON.Invoices.InoviceNote.Dto;
using TACHYON.Invoices.SubmitInvoices;
using TACHYON.MultiTenancy;
using TACHYON.Notifications;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Invoices.InvoiceNotes
{
    [AbpAuthorize()]
    public class InvoiceNoteAppService : TACHYONAppServiceBase, IInvoiceNoteAppService
    {
        private readonly IRepository<InvoiceNote, long> _invoiceNoteRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<Invoice, long> _invoiceReposity;
        private readonly IRepository<InvoiceTrip, long> _invoiveTripRepository;
        private readonly IRepository<SubmitInvoiceTrip, long> _submitInvoiceTrip;
        private readonly IRepository<SubmitInvoice, long> _submitInvoiceReposity;
        private readonly IRepository<InvoiceNoteItem, long> _invoiceNoteItemReposity;
        private readonly IRepository<DocumentFile, Guid> _documentFileRepository;
        private readonly IAppNotifier _appNotifier;
        private readonly IAbpSession _AbpSession;
        private readonly UserManager _userManager;

        public InvoiceNoteAppService(IRepository<InvoiceNote, long> invoiceRepository, IRepository<Tenant> tenantRepository, IRepository<Invoice, long> invoiceReposity, IRepository<InvoiceTrip, long> invoiveTripRepository, IRepository<SubmitInvoiceTrip, long> submitInvoiceTrip, IAppNotifier appNotifier, IRepository<SubmitInvoice, long> submitInvoiceReposity, IRepository<InvoiceNoteItem, long> invoiceNoteItemReposity, IRepository<DocumentFile, Guid> documentFileRepository, UserManager userManager, IAbpSession abpSession)
        {
            _invoiceNoteRepository = invoiceRepository;
            _tenantRepository = tenantRepository;
            _invoiceReposity = invoiceReposity;
            _invoiveTripRepository = invoiveTripRepository;
            _submitInvoiceTrip = submitInvoiceTrip;
            _appNotifier = appNotifier;
            _submitInvoiceReposity = submitInvoiceReposity;
            _invoiceNoteItemReposity = invoiceNoteItemReposity;
            _documentFileRepository = documentFileRepository;
            _userManager = userManager;
            _AbpSession = abpSession;
        }

        #region MainFunctions
        public async Task<LoadResult> GetAllInoviceNote(LoadOptionsInput input)
        {
            DisableTenancyFilters();
            var query = _invoiceNoteRepository.GetAll()
               .AsNoTracking()
               .ProjectTo<GetInvoiceNoteDto>(AutoMapperConfigurationProvider);

            if (!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                DisableTenancyFilters();
            }
            return await LoadResultAsync<GetInvoiceNoteDto>(query, input.LoadOptions);
        }
        public async Task CreateOrEdit(CreateOrEditInvoiceNoteDto input)
        {
            if (!input.Id.HasValue)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }
        public async Task ChangeInvoiceNoteStatus(long id)
        {
            var invoiceNote = await _invoiceNoteRepository.GetAll()
                .WhereIf(!_AbpSession.TenantId.HasValue, x => true)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (invoiceNote == null)
                throw new UserFriendlyException(L("theinvoicedosenotfound"));

            switch (invoiceNote.Status)
            {
                case NoteStatus.Draft:
                    await Confirm(id);
                    break;
                case NoteStatus.Confirm:
                    invoiceNote.Status = NoteStatus.WaitingtobePaid;
                    break;
                case NoteStatus.WaitingtobePaid:
                    invoiceNote.Status = NoteStatus.Paid;
                    break;
                case NoteStatus.Paid:
                    invoiceNote.Status = NoteStatus.Canceled;
                    break;
                default:
                    throw new UserFriendlyException(L("YouCanNotChangeInvoiceNoteStatus"));
            }
        }
        public async Task<GetInvoiceNoteForEditDto> GetInvoiceNoteForEdit(int id)
        {
            var invoiceNote = await _invoiceNoteRepository.GetAll()
                .Include(x=>x.InvoiceItems)
                .ThenInclude(x => x.ShippingRequestTripFK)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (invoiceNote == null)
                throw new UserFriendlyException(L("TheInvoiceNotFound"));

            return  ObjectMapper.Map<GetInvoiceNoteForEditDto>(invoiceNote);
        }
        public async Task GenrateFullVoidInvoiceNote(long id)
        {
            DisableTenancyFilters();
            var shipperEditionId = await SettingManager.GetSettingValueAsync(AppSettings.Editions.ShipperEditionId);
            var invoiceTenantId = await _invoiceReposity.GetAll().Where(x => x.Id == id).Select(x => x.TenantId).FirstOrDefaultAsync();
            var tenantEdition = await _tenantRepository.GetAll().Where(x => x.Id == invoiceTenantId).Select(c => c.EditionId).FirstOrDefaultAsync();
            if (tenantEdition == int.Parse(shipperEditionId))
            {
                await FullVoidInvoiceForShipper(id);
            }
            else
            {
                await FullVoidSubmitInvoiceForCarrier(id);
            }
        }
        public async Task<PartialVoidInvoiceDto> GetInvoiceForPartialVoid(long id)
        {
            var invoice = await _invoiceReposity.GetAll()
                .Include(x => x.Trips)
                .ThenInclude(x => x.ShippingRequestTripFK)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (invoice == null)
                throw new UserFriendlyException(L("TheInvoiceNotFound"));

            return ObjectMapper.Map<PartialVoidInvoiceDto>(invoice);
        }
        #endregion

        #region LookUps
        /// <summary>
        /// Get All Compay Drop Down
        /// </summary>
        /// <returns></returns>
        public async Task<List<CompayForDropDownDto>> GetAllCompanyForDropDown()
        {
            return await _tenantRepository.GetAll()
                .Where(x => x.EditionId == 2 || x.EditionId == 3)
                .Select(x => new CompayForDropDownDto { Id = x.Id, DisplayName = x.TenancyName }).ToListAsync();
        }
        /// <summary>
        ///  Get All Invoice Number Base On Company DropDown
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<List<InvoiceRefreanceNumberDto>> GetAllInvoiceNumberBaseOnCompanyDropDown(int id)
        {
            DisableTenancyFilters();
            var shipperEditionId = await SettingManager.GetSettingValueAsync(AppSettings.Editions.ShipperEditionId);
            var tenantEdition = await _tenantRepository.GetAll().Where(x => x.Id == id).Select(c => c.EditionId).FirstOrDefaultAsync();
            if (tenantEdition == int.Parse(shipperEditionId))
            {
                return await _invoiceReposity.GetAll()
               .Where(m => m.TenantId == id)
               .Select(x => new InvoiceRefreanceNumberDto { Id = x.Id, RefreanceNumber = x.InvoiceNumber }).ToListAsync();
            }
            return await _submitInvoiceReposity.GetAll()
               .Where(m => m.TenantId == id)
               .Select(x => new InvoiceRefreanceNumberDto { Id = x.Id, RefreanceNumber = x.ReferencNumber.Value }).ToListAsync();
        }
        /// <summary>
        /// Get All Invoice ItemDto
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<List<GetAllInvoiceItemDto>> GetAllInvoiceItemDto(long id)
        {
            DisableTenancyFilters();
            var shipperEditionId = await SettingManager.GetSettingValueAsync(AppSettings.Editions.ShipperEditionId);
            var invoiceTenantId = await _invoiceReposity.GetAll().Where(x => x.Id == id).Select(x => x.TenantId).FirstOrDefaultAsync();
            var tenantEdition = await _tenantRepository.GetAll().Where(x => x.Id == invoiceTenantId).Select(c => c.EditionId).FirstOrDefaultAsync();
            if (tenantEdition == int.Parse(shipperEditionId))
            {
                return await _invoiveTripRepository.GetAll()
               .Where(x => x.InvoiceId == id)
               .Include(x => x.ShippingRequestTripFK)
               .Select(x => new GetAllInvoiceItemDto() { Id = x.TripId, WaybillNumber = x.ShippingRequestTripFK.WaybillNumber.Value })
               .ToListAsync();
            }
            return await _submitInvoiceTrip.GetAll()
            .Where(x => x.SubmitId == id)
            .Include(x => x.ShippingRequestTripFK)
            .Select(x => new GetAllInvoiceItemDto() { Id = x.TripId, WaybillNumber = x.ShippingRequestTripFK.WaybillNumber.Value })
            .ToListAsync();
        }
        #endregion

        #region Helper
        /// <summary>
        /// Create new InvoiceNote 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task Create(CreateOrEditInvoiceNoteDto model)
        {
            var items = new List<InvoiceNoteItem>();
            var mapper = ObjectMapper.Map<InvoiceNote>(model);
            if (model.InvoiceItem.Any())
            {
                foreach (var item in model.InvoiceItem)
                {
                    items.Add(new InvoiceNoteItem()
                    {
                        TripId = item,
                        InvoiceNoteId = mapper.Id
                    });
                }
                mapper.InvoiceItems.AddRange(items);
            }

            var invoiceNoteId = await _invoiceNoteRepository.InsertAndGetIdAsync(mapper);

            mapper.ReferanceNumber = GenerateInvoiceNoteReferanceNumber(invoiceNoteId, mapper.NoteType);

            if (model.InvoiceItem.Any())
                await ItemValueCalculator(model.InvoiceNumber, items, invoiceNoteId);
        }
        /// <summary>
        /// Update new InvoiceNote 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        private async Task Update(CreateOrEditInvoiceNoteDto model)
        {
            var inoviceNote = await _invoiceNoteRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == model.Id.Value);
            if (inoviceNote == null) throw new UserFriendlyException("TheNoteDoseNotFound");
            ObjectMapper.Map(model, inoviceNote);
            var invoices = await _invoiceNoteItemReposity.GetAll().Where(x => x.InvoiceNoteId == inoviceNote.Id).ToListAsync();
            var itemsIds = inoviceNote.InvoiceItems.Select(x => x.TripId).ToList();
            if (inoviceNote.Status == NoteStatus.Draft)
            {
                var toAddInovioceItem = new List<InvoiceNoteItem>();
                var toRemoveInovioceItem = new List<InvoiceNoteItem>();
                foreach (var item in model.InvoiceItem)
                {
                    if (!itemsIds.Contains(item))
                    {
                        toAddInovioceItem.Add(new InvoiceNoteItem()
                        {
                            TripId = item,
                            InvoiceNoteId = inoviceNote.Id
                        });
                    }

                }
                foreach (var item in invoices)
                {
                    if (!model.InvoiceItem.Contains(item.TripId))
                    {
                        toRemoveInovioceItem.Add(item);
                    }
                }
                if (toAddInovioceItem.Any())
                {
                    inoviceNote.InvoiceItems.AddRange(toAddInovioceItem);
                }
                if (toRemoveInovioceItem.Any())
                {
                    inoviceNote.InvoiceItems.RemoveAll(x => toRemoveInovioceItem.Any(v => v.Id == x.Id));
                }
                await ItemValueCalculator(inoviceNote.InvoiceNumber, inoviceNote.InvoiceItems, inoviceNote.Id);
            }
        }
        /// <summary>
        ///  Confirm Action after noted has drafted
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        private async Task Confirm(long invoiceId)
        {
            var invoiceNote = await _invoiceNoteRepository.GetAll()
                .Where(x => x.Status != NoteStatus.Canceled)
                .FirstOrDefaultAsync(x => x.Id == invoiceId);
            if (invoiceNote == null)
                throw new UserFriendlyException("TheNoteDoseNotFounded");
            if (invoiceNote.Status == NoteStatus.Draft)
            {
                invoiceNote.Status = NoteStatus.Confirm;
                await _appNotifier.NewCreditOrDebitNoteAdded(invoiceNote);
            }
        }
        /// <summary>
        ///  Calautae Item Base on Given item
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        private async Task ItemValueCalculator(long InvoiceNumber, List<InvoiceNoteItem> items, long invoiceNoteId)
        {
            DisableTenancyFilters();
            var invoiceNote = await _invoiceNoteRepository.FirstOrDefaultAsync(invoiceNoteId);
            var shipperEditionId = await SettingManager.GetSettingValueAsync(AppSettings.Editions.ShipperEditionId);
            var invoiceTenantId = await _invoiceReposity.GetAll().Where(x => x.InvoiceNumber == InvoiceNumber).Select(x => x.TenantId).FirstOrDefaultAsync();
            var tenantEdition = await _tenantRepository.GetAll().Where(x => x.Id == invoiceTenantId).Select(c => c.EditionId).FirstOrDefaultAsync();
            if (tenantEdition == int.Parse(shipperEditionId))
            {
                await CalculateForShipper(InvoiceNumber, items, invoiceNote);

            }
            else
            {
                await CalculateForCarrier(InvoiceNumber, items, invoiceNote);
            }
        }
        /// <summary>
        /// Calculate For Shipper
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="items"></param>
        /// <param name="invoiceNote"></param>
        /// <returns></returns>
        private async Task CalculateForShipper(long InvoiceNumber, List<InvoiceNoteItem> items, InvoiceNote invoiceNote)
        {
            var shippingRequstTrips = await _invoiceReposity.GetAll()
                   .Include(x => x.Trips)
                   .ThenInclude(x => x.ShippingRequestTripFK)
                   .ThenInclude(z => z.ShippingRequestTripVases)
                   .Where(x => x.InvoiceNumber == InvoiceNumber).Select(x => x.Trips).FirstOrDefaultAsync();
            var ivoiceItem = new List<InvoiceTrip>();
            foreach (var item in items)
            {
                var trip = shippingRequstTrips.FirstOrDefault(x => x.TripId == item.TripId);
                ivoiceItem.Add(trip);
            }
            invoiceNote.TotalValue = (decimal)ivoiceItem.Sum(r => r.ShippingRequestTripFK.TotalAmountWithCommission + r.ShippingRequestTripFK.ShippingRequestTripVases.Sum(v => v.TotalAmountWithCommission));
            invoiceNote.VatAmount = (decimal)ivoiceItem.Sum(r => r.ShippingRequestTripFK.VatAmountWithCommission + r.ShippingRequestTripFK.ShippingRequestTripVases.Sum(v => v.VatAmountWithCommission));
            invoiceNote.Price = (decimal)ivoiceItem.Sum(r => r.ShippingRequestTripFK.SubTotalAmountWithCommission + r.ShippingRequestTripFK.ShippingRequestTripVases.Sum(v => v.SubTotalAmountWithCommission));
        }
        /// <summary>
        /// Calculate For Carrier
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="items"></param>
        /// <param name="invoiceNote"></param>
        /// <returns></returns>
        private async Task CalculateForCarrier(long ReferencNumber, List<InvoiceNoteItem> items, InvoiceNote invoiceNote)
        {
            var shippingRequstTripss = await _submitInvoiceReposity.GetAll()
               .Include(x => x.Trips)
               .ThenInclude(x => x.ShippingRequestTripFK)
               .ThenInclude(z => z.ShippingRequestTripVases)
               .Where(x => x.ReferencNumber == ReferencNumber).Select(x => x.Trips).FirstOrDefaultAsync();
            var ivoiceItems = new List<SubmitInvoiceTrip>();
            foreach (var item in items)
            {
                var trip = shippingRequstTripss.FirstOrDefault(x => x.TripId == item.TripId);
                ivoiceItems.Add(trip);
            }
            invoiceNote.TotalValue = (decimal)ivoiceItems.Sum(r => r.ShippingRequestTripFK.TotalAmount + r.ShippingRequestTripFK.ShippingRequestTripVases.Sum(v => v.TotalAmount));
            invoiceNote.VatAmount = (decimal)ivoiceItems.Sum(r => r.ShippingRequestTripFK.VatAmount + r.ShippingRequestTripFK.ShippingRequestTripVases.Sum(v => v.VatAmount));
            invoiceNote.Price = (decimal)ivoiceItems.Sum(r => r.ShippingRequestTripFK.SubTotalAmount + r.ShippingRequestTripFK.ShippingRequestTripVases.Sum(v => v.SubTotalAmount));
        }
        /// <summary>
        /// Full Void Invoice For Shipper
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        private async Task FullVoidInvoiceForShipper(long invoiceId)
        {
            var invoice = await _invoiceReposity.GetAll()
                       .Include(x => x.Trips).Where(x => x.Id == invoiceId).FirstOrDefaultAsync();
            if (invoice == null)
                throw new UserFriendlyException("TherIsNoInvoiceFounded");
            var invoiceNote = new InvoiceNote()
            {
                NoteType = NoteType.Credit,
                Status = NoteStatus.Draft,
                Price = invoice.SubTotalAmount,
                TenantId = invoice.TenantId,
                TotalValue = invoice.TotalAmount,
                VatAmount = invoice.VatAmount,
                InvoiceNumber = invoice.InvoiceNumber,
                VoidType = VoidType.FullVoid,
                InvoiceItems = invoice.Trips.Select(x => new InvoiceNoteItem()
                {
                    TripId = x.TripId
                }).ToList()
            };
            var invoiceNoteId = await _invoiceNoteRepository.InsertAndGetIdAsync(invoiceNote);
            invoiceNote.ReferanceNumber = GenerateInvoiceNoteReferanceNumber(invoiceNoteId, NoteType.Credit);
        }
        /// <summary>
        /// Full Void SubmitInvoice For Carrier
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        private async Task FullVoidSubmitInvoiceForCarrier(long invoiceId)
        {
            var submitInvoice = await _submitInvoiceReposity.GetAll()
                    .Include(x => x.Trips).Where(x => x.Id == invoiceId).FirstOrDefaultAsync();

            if (submitInvoice == null)
                throw new UserFriendlyException("TherIsNoSubmitInvoiceFounded");

            var invoiceNote = new InvoiceNote()
            {
                NoteType = NoteType.Credit,
                Status = NoteStatus.Draft,
                Price = submitInvoice.SubTotalAmount,
                TenantId = submitInvoice.TenantId,
                TotalValue = submitInvoice.TotalAmount,
                VatAmount = submitInvoice.VatAmount,
                InvoiceNumber = submitInvoice.ReferencNumber.Value,
                VoidType = VoidType.FullVoid,
                InvoiceItems = submitInvoice.Trips.Select(x => new InvoiceNoteItem()
                {
                    TripId = x.TripId
                }).ToList()
            };
            var invoiceNoteId = await _invoiceNoteRepository.InsertAndGetIdAsync(invoiceNote);
            invoiceNote.ReferanceNumber = GenerateInvoiceNoteReferanceNumber(invoiceNoteId, NoteType.Credit);
        }
        /// <summary>
        /// Get InvoiceNote Report Info
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public List<InvoiceNoteInfoDto> GetInvoiceNoteReportInfo(long id)
        {
            DisableTenancyFilters();
            var invoiceNote = _invoiceNoteRepository
                .GetAll()
                .Include(x => x.Tenant)
                .FirstOrDefault(i => i.Id == id);

            if (invoiceNote == null)
                throw new UserFriendlyException(L("TheInvoiceNotFound"));

            var invoiceNoteDto = ObjectMapper.Map<InvoiceNoteInfoDto>(invoiceNote);

            var admin = AsyncHelper.RunSync(() => _userManager.GetAdminByTenantIdAsync(invoiceNote.TenantId));
            var invoice = AsyncHelper.RunSync(() => _invoiceReposity.FirstOrDefaultAsync(x => x.InvoiceNumber == invoiceNote.InvoiceNumber));

            invoiceNoteDto.Email = admin.EmailAddress;
            invoiceNoteDto.Attn = admin.FullName;

            if (invoice != null)
                invoiceNoteDto.ReInvoiceDate = invoice.CreationTime.ToString("dd/mm/yyyy mm:hh");

            var document = AsyncHelper.RunSync(() => _documentFileRepository
            .FirstOrDefaultAsync(x => x.TenantId == invoiceNote.TenantId && x.DocumentTypeId == 14));

            if (document != null)
                invoiceNoteDto.CR = document.Number;

            var documentVat = AsyncHelper.RunSync(() => _documentFileRepository
            .FirstOrDefaultAsync(x => x.TenantId == invoiceNote.TenantId && x.DocumentTypeId == 15));

            if (document != null)
                invoiceNoteDto.TenantVatNumber = documentVat.Number;

            return new List<InvoiceNoteInfoDto>() { invoiceNoteDto };
        }
        /// <summary>
        ///  Get InvoiceNote Info
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        private async Task<InvoiceNote> GetInvoiceNoteInfo(long invoiceNoteId)
        {
            var invoiceNote = await _invoiceNoteRepository.GetAll()
                 .Include(x => x.InvoiceItems)
                 .ThenInclude(z => z.ShippingRequestTripFK)
                 .ThenInclude(v => v.ShippingRequestTripVases)
                 .ThenInclude(t => t.ShippingRequestVasFk)
                 .ThenInclude(m => m.VasFk)
                 .FirstOrDefaultAsync(x => x.Id == invoiceNoteId);
            if (invoiceNote == null)
                throw new UserFriendlyException("TheInvoiceNoteDoseNotFound");
            return invoiceNote;
        }
        /// <summary>
        /// Get Invoice Shipping Requests Report Info
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public List<InvoiceNoteItemDto> GetInvoiceNoteItemReportInfo(long invoiceNoteId)
        {
            DisableTenancyFilters();
            var invoiceNote = AsyncHelper.RunSync(() => GetInvoiceNoteInfo(invoiceNoteId));
            if (invoiceNote == null) throw new UserFriendlyException(L("TheInvoiceNotFound"));
            var TotalItem = invoiceNote.InvoiceItems.Count +
                         invoiceNote.InvoiceItems.Sum(v => v.ShippingRequestTripFK.ShippingRequestTripVases.Count);
            int Sequence = 1;
            List<InvoiceNoteItemDto> Items = new List<InvoiceNoteItemDto>();
            invoiceNote.InvoiceItems.ToList().ForEach(trip =>
            {
                Items.Add(new InvoiceNoteItemDto
                {
                    Sequence = $"{Sequence}/{TotalItem}",
                    Price =
                            AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier)
                                ? trip.ShippingRequestTripFK.SubTotalAmount.Value
                                : trip.ShippingRequestTripFK.SubTotalAmountWithCommission.Value,
                    VatAmount =
                            AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier)
                                ? trip.ShippingRequestTripFK.VatAmount.Value
                                : trip.ShippingRequestTripFK.VatAmountWithCommission.Value,
                    TotalAmount =
                            AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier)
                                ? trip.ShippingRequestTripFK.TotalAmount.Value
                                : trip.ShippingRequestTripFK.TotalAmountWithCommission.Value,
                    WayBillNumber = trip.ShippingRequestTripFK.WaybillNumber.ToString(),
                    Date =
                        trip.ShippingRequestTripFK.EndTripDate.HasValue
                            ? trip.ShippingRequestTripFK.EndTripDate.Value.ToString("dd/MM/yyyy")
                            : trip.InvoiceNoteFK.CreationTime.ToString("dd/MM/yyyy")
                });
                trip.ShippingRequestTripFK.WaybillNumber.ToString();
                Sequence++;
            });
            return Items;
        }
        /// <summary>
        /// Generate InvoiceNote Referance Number
        /// </summary>
        /// <param name="id"></param>
        /// <param name="noteType"></param>
        /// <returns></returns>
        private string GenerateInvoiceNoteReferanceNumber(long id, NoteType noteType)
        {
            string noteFormat = noteType == NoteType.Debit ? "TDN" : "TCN";
            var referanceId = id + 10000;
            var referanceNumber = "{0}-{1}";
            return string.Format(referanceNumber, noteFormat, referanceId);
        }
        #endregion
    }
}
