using Abp.Application.Features;
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
using TACHYON.Authorization;
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
        [AbpAuthorize(AppPermissions.Pages_InvoiceNote_View)]
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
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer);

            if (!input.Id.HasValue)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }
        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task ChangeInvoiceNoteStatus(long id)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            var invoiceNote = await _invoiceNoteRepository.GetAll()
                .WhereIf(!_AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), x => true)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (invoiceNote == null)
                throw new UserFriendlyException(L("Theinvoicedoesnotfound"));

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
        [AbpAuthorize(AppPermissions.Pages_InvoiceNote_Edit)]
        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task<CreateOrEditInvoiceNoteDto> GetInvoiceNoteForEdit(int id)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            var invoiceNote = await _invoiceNoteRepository.GetAll()
                .Include(x => x.InvoiceItems)
                .ThenInclude(t => t.ShippingRequestTripFK)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (invoiceNote == null)
                throw new UserFriendlyException(L("Theinvoicedoesnotfound"));

            return ObjectMapper.Map<CreateOrEditInvoiceNoteDto>(invoiceNote);
        }
        public async Task GenrateFullVoidInvoiceNote(long id)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            var invoiceTenant = await _invoiceReposity.FirstOrDefaultAsync(x => x.Id == id);
            if (invoiceTenant != null)
                await FullVoidInvoiceForShipper(id);
            else
                await FullVoidSubmitInvoiceForCarrier(id);
        }
        public async Task<PartialVoidInvoiceDto> GetInvoiceForPartialVoid(long id)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            var invoiceTenant = await _invoiceReposity.FirstOrDefaultAsync(x => x.Id == id);
            if (invoiceTenant != null)
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

        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task Canacel(int invoiceId)
        {
            DisableTenancyFilters();
            var invoiceNote = await _invoiceNoteRepository.FirstOrDefaultAsync(x=>x.Id==invoiceId && x.Status == NoteStatus.Draft);

            if (invoiceNote == null)
                throw new UserFriendlyException(L("Theinvoicedoesnotfound"));

            invoiceNote.Status = NoteStatus.Canceled;
        }
        [AbpAuthorize(AppPermissions.Pages_InvoiceNote_Create)]
        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task AddNote(NoteInputDto input)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            var invoiceNote = await _invoiceNoteRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == input.Id);
            invoiceNote.Note = input.Note;
            invoiceNote.CanBePrinted = input.CanBePrinted;
        }
        public async Task<NoteInputDto> GetNote(int tripId)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            return await _invoiceNoteRepository.GetAll()
                  .Select(y => new NoteInputDto()
                  {
                      Id = y.Id,
                      Note = y.Note,
                      CanBePrinted = y.CanBePrinted
                  }).FirstOrDefaultAsync(x => x.Id == tripId);
        }
        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task GeneratePartialInvoiceNote(CreateOrEditInvoiceNoteDto input)
        {
            var invoiceNote = ObjectMapper.Map<InvoiceNote>(input);

            invoiceNote.VoidType = VoidType.PartialVoid;

            var invoiceNoteId = await _invoiceNoteRepository.InsertAndGetIdAsync(invoiceNote);

            invoiceNote.ReferanceNumber = GenerateInvoiceNoteReferanceNumber(invoiceNoteId, invoiceNote.NoteType);
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
               .Select(x => new InvoiceRefreanceNumberDto { Id = x.Id, RefreanceNumber = x.InvoiceNumber})
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
        public async Task<List<GetAllInvoiceItemDto>> GetAllInvoicmItemDto(long id)
        {
            DisableTenancyFilters();
            var invoiceTenant = await _invoiceReposity.FirstOrDefaultAsync(x => x.Id == id);
            if (invoiceTenant != null)
            {
                return await _invoiveTripRepository.GetAll()
               .Where(x => x.InvoiceId == id)
               .Include(x => x.ShippingRequestTripFK)
               .Select(x => new GetAllInvoiceItemDto() {TripId = x.TripId, WaybillNumber = x.ShippingRequestTripFK.WaybillNumber.Value })
               .AsNoTracking()
               .ToListAsync();
            }
            return await _submitInvoiceTrip.GetAll()
            .Where(x => x.SubmitId == id)
            .Include(x => x.ShippingRequestTripFK)
            .Select(x => new GetAllInvoiceItemDto() {TripId = x.TripId, WaybillNumber = x.ShippingRequestTripFK.WaybillNumber.Value })
            .AsNoTracking()
            .ToListAsync();
        }
        #endregion

        #region Helper
        [AbpAuthorize(AppPermissions.Pages_InvoiceNote_Create)]
        [RequiresFeature(AppFeatures.TachyonDealer)]
        private async Task Create(CreateOrEditInvoiceNoteDto input)
        {
            var invoiceNote = ObjectMapper.Map<InvoiceNote>(input);
            
            var invoiceNoteId = await _invoiceNoteRepository.InsertAndGetIdAsync(invoiceNote);

            invoiceNote.ReferanceNumber = GenerateInvoiceNoteReferanceNumber(invoiceNoteId, invoiceNote.NoteType);
            if (input.InvoiceItems.Any())
                await ItemValueCalculator(input.InvoiceNumber, invoiceNote.InvoiceItems, invoiceNote);
        }
        [AbpAuthorize(AppPermissions.Pages_InvoiceNote_Edit)]
        [RequiresFeature(AppFeatures.TachyonDealer)]
        private async Task Update(CreateOrEditInvoiceNoteDto model)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            var inoviceNote = await _invoiceNoteRepository.GetAllIncluding(x=>x.InvoiceItems).FirstOrDefaultAsync(x => x.Id == model.Id.Value);

            if (inoviceNote == null)
                throw new UserFriendlyException(L("Theinvoicedoesnotfound"));

            ObjectMapper.Map(model, inoviceNote);

            if (!inoviceNote.IsManual)
                await ItemValueCalculator(inoviceNote.InvoiceNumber, inoviceNote.InvoiceItems, inoviceNote);
        }
        private async Task ItemValueCalculator(long InvoiceNumber, List<InvoiceNoteItem> items, InvoiceNote invoiceNote)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            var invoiceTenant = await _invoiceReposity.FirstOrDefaultAsync(x => x.InvoiceNumber == InvoiceNumber);
            if (invoiceTenant != null)
                await CalculateForShipper(InvoiceNumber, items, invoiceNote);
            else
                await CalculateForCarrier(InvoiceNumber, items, invoiceNote);
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

            var invoiceItem = trips.Where(y => items.Select(x => x.TripId).Contains(y.TripId));

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
                throw new UserFriendlyException(L("Theinvoicedoesnotfound"));

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
                throw new UserFriendlyException(L("Theinvoicedoesnotfound"));

            return invoiceNote;
        }
        public List<InvoiceNoteItemDto> GetInvoiceNoteItemReportInfo(long invoiceNoteId)
        {
            DisableTenancyFilters();
            var invoiceNote = AsyncHelper.RunSync(() => GetInvoiceNoteInfo(invoiceNoteId));
            if (invoiceNote == null) throw new UserFriendlyException(L("Theinvoicedoesnotfound"));
            var TotalItem = invoiceNote.InvoiceItems.Count;
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
