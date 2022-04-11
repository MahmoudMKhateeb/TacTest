using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.Threading;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;
using TACHYON.Common;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Features;
using TACHYON.Invoices.InoviceNote;
using TACHYON.Invoices.InoviceNote.Dto;
using TACHYON.Invoices.SubmitInvoices;
using TACHYON.MultiTenancy;
using TACHYON.Notifications;

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
            DisableTenancyFilters();
            var invoiceNote = await _invoiceNoteRepository.GetAll()
                .WhereIf(!_AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), x => true)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (invoiceNote == null)
                throw new UserFriendlyException(L("Theinvoicedosenotfound"));

            switch (invoiceNote.Status)
            {
                case NoteStatus.Draft:
                    invoiceNote.Status = NoteStatus.Confirm;
                    await _appNotifier.NewCreditOrDebitNoteAdded(invoiceNote);
                    break;
                case NoteStatus.Confirm:
                    invoiceNote.Status = NoteStatus.WaitingtobePaid;
                    break;
                case NoteStatus.WaitingtobePaid:
                    invoiceNote.Status = NoteStatus.Paid;
                    await _appNotifier.TheCreaditOrDebitNotePaid(invoiceNote);
                    break;
                default:
                    throw new UserFriendlyException(L("YouCanNotChangeInvoiceNoteStatus"));
            }
        }
        public async Task<CreateOrEditInvoiceNoteDto> GetInvoiceNoteForEdit(int id)
        {
            await DisableTenancyFiltersIfTachyonDealer();
            var invoiceNote = await _invoiceNoteRepository.GetAll()
                .Include(x => x.InvoiceItems)
                .ThenInclude(x => x.ShippingRequestTripFK)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (invoiceNote == null)
                throw new UserFriendlyException(L("Theinvoicedosenotfound"));

            return ObjectMapper.Map<CreateOrEditInvoiceNoteDto>(invoiceNote);
        }
        public async Task GenrateFullVoidInvoiceNote(long id)
        {
            DisableTenancyFilters();
            var invoiceTenantId = await _invoiceReposity.GetAll().Where(x => x.Id == id).Select(x => x.TenantId).FirstOrDefaultAsync();
            var tenantEdition = await _tenantRepository.GetAll().Where(x => x.Id == invoiceTenantId).Select(c => c.EditionId).FirstOrDefaultAsync();
            if (tenantEdition == ShipperEditionId)
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
            await DisableTenancyFiltersIfTachyonDealer();
            var invoiceTenantId = await _invoiceReposity.GetAll().Where(x => x.Id == id).Select(x => x.TenantId).FirstOrDefaultAsync();
            var tenantEdition = await _tenantRepository.GetAll().Where(x => x.Id == invoiceTenantId).Select(c => c.EditionId).FirstOrDefaultAsync();
            if (tenantEdition == ShipperEditionId)
            {
                var invoice = await _invoiceReposity.GetAll()
                .Include(x => x.Trips)
                .ThenInclude(x => x.ShippingRequestTripFK)
                .FirstOrDefaultAsync(x => x.Id == id);
                return ObjectMapper.Map<PartialVoidInvoiceDto>(invoice);
            }
            var submitInvoice = await _submitInvoiceReposity.GetAll()
              .Include(x => x.Trips)
              .ThenInclude(x => x.ShippingRequestTripFK)
              .FirstOrDefaultAsync(x => x.Id == id);
            return ObjectMapper.Map<PartialVoidInvoiceDto>(submitInvoice);
        }
        public async Task Canacel(int invoiceId)
        {
            var invoiceNote = await _invoiceNoteRepository.FirstOrDefaultAsync(invoiceId);

            if (invoiceNote == null)
                throw new UserFriendlyException(L("Theinvoicedosenotfound"));

            invoiceNote.Status = NoteStatus.Canceled;
        }
        public async Task AddNote(NoteInputDto input)
        {
            await DisableTenancyFiltersIfTachyonDealer();
            var invoiceNote = await _invoiceNoteRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == input.Id);
            invoiceNote.Note = input.Note;
            invoiceNote.CanBePrinted = input.CanBePrinted;
        }
        public async Task<NoteInputDto> GetNote(int tripId)
        {
            await DisableTenancyFiltersIfTachyonDealer();
            return await _invoiceNoteRepository.GetAll()
                  .Select(y => new NoteInputDto()
                  {
                      Id = y.Id,
                      Note = y.Note,
                      CanBePrinted = y.CanBePrinted
                  }).FirstOrDefaultAsync(x => x.Id == tripId);
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
                .Where(x => x.EditionId == ShipperEditionId || x.EditionId == CarrierEditionId)
                .Select(x => new CompayForDropDownDto { Id = x.Id, DisplayName = x.TenancyName }).AsNoTracking().ToListAsync();
        }
        /// <summary>
        ///  Get All Invoice Number Base On Company DropDown
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<List<InvoiceRefreanceNumberDto>> GetAllInvoiceNumberBaseOnCompanyDropDown(int id)
        {
            DisableTenancyFilters();
            var tenantEdition = await _tenantRepository.FirstOrDefaultAsync(x => x.Id == id);
            if (tenantEdition.EditionId == ShipperEditionId)
            {
                return await _invoiceReposity.GetAll()
               .Where(m => m.TenantId == id)
               .Select(x => new InvoiceRefreanceNumberDto { Id = x.Id, RefreanceNumber = x.InvoiceNumber })
               .AsNoTracking().ToListAsync();
            }
            return await _submitInvoiceReposity.GetAll()
               .Where(m => m.TenantId == id)
               .Select(x => new InvoiceRefreanceNumberDto { Id = x.Id, RefreanceNumber = x.ReferencNumber.Value })
               .AsNoTracking().ToListAsync();
        }
        /// <summary>
        /// Get All Invoice ItemDto
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<List<GetAllInvoiceItemDto>> GetAllInvoiceItemDto(long id)
        {
            DisableTenancyFilters();
            var invoiceTenantId = await _invoiceReposity.GetAll().Where(x => x.Id == id).Select(x => x.TenantId).FirstOrDefaultAsync();
            var tenantEdition = await _tenantRepository.GetAll().Where(x => x.Id == invoiceTenantId).Select(z => z.EditionId).FirstOrDefaultAsync();
            if (tenantEdition == ShipperEditionId)
            {
                return await _invoiveTripRepository.GetAll()
               .Where(x => x.InvoiceId == id)
               .Include(x => x.ShippingRequestTripFK)
               .Select(x => new GetAllInvoiceItemDto() { Id = x.TripId, WaybillNumber = x.ShippingRequestTripFK.WaybillNumber.Value })
               .AsNoTracking()
               .ToListAsync();
            }
            return await _submitInvoiceTrip.GetAll()
            .Where(x => x.SubmitId == id)
            .Include(x => x.ShippingRequestTripFK)
            .Select(x => new GetAllInvoiceItemDto() { Id = x.TripId, WaybillNumber = x.ShippingRequestTripFK.WaybillNumber.Value })
            .AsNoTracking()
            .ToListAsync();
        }
        #endregion

        #region Helper
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
                        TripId = item.Id,
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
        private async Task Update(CreateOrEditInvoiceNoteDto model)
        {
            await DisableTenancyFiltersIfTachyonDealer();
            var inoviceNote = await _invoiceNoteRepository.GetAll().FirstOrDefaultAsync(x => x.Id == model.Id.Value);

            if (inoviceNote == null)
                throw new UserFriendlyException(L("Theinvoicedosenotfound"));

            ObjectMapper.Map(model, inoviceNote);

            var modelItemIds = model.InvoiceItem.Select(x => x.Id).ToList();
            var invoiceItmes = await _invoiceNoteItemReposity.GetAll().Where(x => x.InvoiceNoteId == inoviceNote.Id).ToListAsync();
            var itemsIds = inoviceNote.InvoiceItems.Select(x => x.TripId).ToList();
            var modelItem = modelItemIds.Where(x => !itemsIds.Contains(x));
            if (inoviceNote.Status == NoteStatus.Draft)
            {
                var toAddInovioceItem = new List<InvoiceNoteItem>();
                var toRemoveInovioceItem = new List<InvoiceNoteItem>();
                foreach (var item in modelItem)
                {
                    toAddInovioceItem.Add(new InvoiceNoteItem()
                    {
                        TripId = item,
                        InvoiceNoteId = inoviceNote.Id
                    });
                }
                foreach (var item in invoiceItmes)
                {
                    if (!modelItemIds.Contains(item.TripId))
                        toRemoveInovioceItem.Add(item);
                }
                if (toAddInovioceItem.Any())
                    inoviceNote.InvoiceItems.AddRange(toAddInovioceItem);

                if (toRemoveInovioceItem.Any())
                    inoviceNote.InvoiceItems.RemoveAll(x => toRemoveInovioceItem.Any(v => v.Id == x.Id));

                if (!inoviceNote.IsManual)
                    await ItemValueCalculator(inoviceNote.InvoiceNumber, inoviceNote.InvoiceItems, inoviceNote.Id);
            }
        }
        private async Task ItemValueCalculator(long InvoiceNumber, List<InvoiceNoteItem> items, long invoiceNoteId)
        {
            DisableTenancyFilters();
            var invoiceNote = await _invoiceNoteRepository.FirstOrDefaultAsync(invoiceNoteId);
            var invoiceTenantId = await _invoiceReposity.GetAll().Where(x => x.InvoiceNumber == InvoiceNumber).Select(x => x.TenantId).FirstOrDefaultAsync();
            var tenantEdition = await _tenantRepository.GetAll().Where(x => x.Id == invoiceTenantId).Select(z => z.EditionId).FirstOrDefaultAsync();
            if (tenantEdition == ShipperEditionId)
            {
                await CalculateForShipper(InvoiceNumber, items, invoiceNote);

            }
            else
            {
                await CalculateForCarrier(InvoiceNumber, items, invoiceNote);
            }
        }
        private async Task CalculateForShipper(long InvoiceNumber, List<InvoiceNoteItem> items, InvoiceNote invoiceNote)
        {
            var trips = await _invoiceReposity.GetAll()
                   .Include(x => x.Trips)
                   .ThenInclude(x => x.ShippingRequestTripFK)
                   .ThenInclude(z => z.ShippingRequestTripVases)
                   .Where(x => x.InvoiceNumber == InvoiceNumber)
                   .Select(x => x.Trips).FirstOrDefaultAsync();

            var invoiceItem = trips.Where(x => items.Select(x => x.TripId).Contains(x.TripId));

            invoiceNote.TotalValue = (decimal)invoiceItem.Sum(r => r.ShippingRequestTripFK.TotalAmountWithCommission + r.ShippingRequestTripFK.ShippingRequestTripVases.Sum(v => v.TotalAmountWithCommission));
            invoiceNote.VatAmount = (decimal)invoiceItem.Sum(r => r.ShippingRequestTripFK.VatAmountWithCommission + r.ShippingRequestTripFK.ShippingRequestTripVases.Sum(v => v.VatAmountWithCommission));
            invoiceNote.Price = (decimal)invoiceItem.Sum(r => r.ShippingRequestTripFK.SubTotalAmountWithCommission + r.ShippingRequestTripFK.ShippingRequestTripVases.Sum(v => v.SubTotalAmountWithCommission));
        }
        private async Task CalculateForCarrier(long ReferencNumber, List<InvoiceNoteItem> items, InvoiceNote invoiceNote)
        {
            var trips = await _submitInvoiceReposity.GetAll()
               .Include(x => x.Trips)
               .ThenInclude(x => x.ShippingRequestTripFK)
               .ThenInclude(z => z.ShippingRequestTripVases)
               .Where(x => x.ReferencNumber == ReferencNumber)
               .Select(x => x.Trips).FirstOrDefaultAsync();

            var invoiceItem = trips.Where(x => items.Select(x => x.TripId).Contains(x.TripId));

            invoiceNote.TotalValue = (decimal)invoiceItem.Sum(r => r.ShippingRequestTripFK.TotalAmount + r.ShippingRequestTripFK.ShippingRequestTripVases.Sum(v => v.TotalAmount));
            invoiceNote.VatAmount = (decimal)invoiceItem.Sum(r => r.ShippingRequestTripFK.VatAmount + r.ShippingRequestTripFK.ShippingRequestTripVases.Sum(v => v.VatAmount));
            invoiceNote.Price = (decimal)invoiceItem.Sum(r => r.ShippingRequestTripFK.SubTotalAmount + r.ShippingRequestTripFK.ShippingRequestTripVases.Sum(v => v.SubTotalAmount));
        }
        private async Task FullVoidInvoiceForShipper(long invoiceId)
        {
            var invoice = await _invoiceReposity.GetAll()
                       .Include(x => x.Trips)
                       .Where(x => x.Id == invoiceId)
                       .FirstOrDefaultAsync();

            if (invoice == null)
                throw new UserFriendlyException(L("TheInvoiceNotFound"));

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
        private async Task FullVoidSubmitInvoiceForCarrier(long invoiceId)
        {
            var submitInvoice = await _submitInvoiceReposity.GetAll()
                    .Include(x => x.Trips)
                    .Where(x => x.Id == invoiceId)
                    .FirstOrDefaultAsync();

            if (submitInvoice == null)
                throw new UserFriendlyException(L("TheInvoiceNotFound"));

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
        public List<InvoiceNoteInfoDto> GetInvoiceNoteReportInfo(long id)
        {
            DisableTenancyFilters();
            var invoiceNote = _invoiceNoteRepository
                .GetAll()
                .Include(x => x.Tenant)
                .FirstOrDefault(i => i.Id == id);

            if (invoiceNote == null)
                throw new UserFriendlyException(L("Theinvoicedosenotfound"));

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

            if (invoiceNote.CanBePrinted)
                invoiceNoteDto.Notes = invoiceNote.Note;

            return new List<InvoiceNoteInfoDto>() { invoiceNoteDto };
        }
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
                throw new UserFriendlyException(L("Theinvoicedosenotfound"));

            return invoiceNote;
        }
        public List<InvoiceNoteItemDto> GetInvoiceNoteItemReportInfo(long invoiceNoteId)
        {
            DisableTenancyFilters();
            var invoiceNote = AsyncHelper.RunSync(() => GetInvoiceNoteInfo(invoiceNoteId));
            if (invoiceNote == null) throw new UserFriendlyException(L("Theinvoicedosenotfound"));
            var TotalItem = invoiceNote.InvoiceItems.Count +
                         invoiceNote.InvoiceItems.Sum(v => v.ShippingRequestTripFK.ShippingRequestTripVases.Count);
            int Sequence = 1;
            if (!invoiceNote.IsManual)
            {
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
                    Sequence++;
                });
                return Items;
            }
            return _invoiceNoteRepository.GetAll().Where(x => x.Id == invoiceNoteId).Select(x => new InvoiceNoteItemDto()
            {
                Sequence = $"{Sequence}/{1}",
                Date = x.CreationTime.ToString("dd/MM/yyyy"),
                Price = x.Price,
                TotalAmount = x.TotalValue,
                VatAmount = x.VatAmount,
                WayBillNumber = x.WaybillNumber
            }).ToList();
        }
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
