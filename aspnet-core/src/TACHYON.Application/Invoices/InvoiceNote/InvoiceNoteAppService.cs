﻿using Abp.Application.Features;
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
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.ShippingRequestTripVases;

namespace TACHYON.Invoices.InvoiceNotes
{
    [AbpAuthorize()]
    public class InvoiceNoteAppService : TACHYONAppServiceBase, IInvoiceNoteAppService
    {
        private readonly IRepository<InvoiceNote, long> _invoiceNoteRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<Invoice, long> _invoiceReposity;
        private readonly IRepository<InvoiceTrip, long> _invoiveTripRepository;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly IRepository<ShippingRequestTripVas, long> _shippingRequestTripVasRepository;
        private readonly IRepository<SubmitInvoiceTrip, long> _submitInvoiceTrip;
        private readonly IRepository<SubmitInvoice, long> _submitInvoiceReposity;
        private readonly IRepository<InvoiceNoteItem, long> _invoiceNoteItemReposity;
        private readonly IRepository<DocumentFile, Guid> _documentFileRepository;
        private readonly IAppNotifier _appNotifier;
        private readonly IAbpSession _AbpSession;
        private readonly UserManager _userManager;

        public InvoiceNoteAppService(IRepository<InvoiceNote, long> invoiceRepository, IRepository<Tenant> tenantRepository, IRepository<Invoice, long> invoiceReposity, IRepository<InvoiceTrip, long> invoiveTripRepository, IRepository<SubmitInvoiceTrip, long> submitInvoiceTrip, IAppNotifier appNotifier, IRepository<SubmitInvoice, long> submitInvoiceReposity, IRepository<InvoiceNoteItem, long> invoiceNoteItemReposity, IRepository<DocumentFile, Guid> documentFileRepository, UserManager userManager, IAbpSession abpSession, IRepository<ShippingRequestTrip> shippingRequestTripRepository, IRepository<ShippingRequestTripVas, long> shippingRequestTripVasRepository)
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
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _shippingRequestTripVasRepository = shippingRequestTripVasRepository;
        }

        #region MainFunctions
        [AbpAuthorize(AppPermissions.Pages_InvoiceNote_View)]
        public async Task<LoadResult> GetAllInoviceNote(LoadOptionsInput input)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            await DisableDraftedFilterIfTachyonDealerOrHost();
            var query = _invoiceNoteRepository.GetAll()
               .AsNoTracking()
               .ProjectTo<GetInvoiceNoteDto>(AutoMapperConfigurationProvider);


            return await LoadResultAsync<GetInvoiceNoteDto>(query, input.LoadOptions);
        }
        public async Task CreateOrEdit(CreateOrEditInvoiceNoteDto input)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer);

            //Check duplication in waybills
            if(input.InvoiceItems.Where(x=>x.WaybillNumber!=null).GroupBy(x => x.WaybillNumber)
                .Any(group => group.Count() > 1))
            {
                throw new UserFriendlyException(L("WaybillsShouldnotBeDuplicated"));
            }
                    

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
            DisableDraftedFilter();
            var invoiceNote = await _invoiceNoteRepository.GetAll()
                .WhereIf(!_AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), x => true)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (invoiceNote == null)
                throw new UserFriendlyException(L("Theinvoicedoesnotfound"));

            switch (invoiceNote.Status)
            {
                case NoteStatus.Draft:
                    invoiceNote.Status = NoteStatus.Confirm;
                    invoiceNote.IsDrafted = false;
                    invoiceNote.ReferanceNumber = GenerateInvoiceNoteReferanceNumber(invoiceNote);
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
            DisableDraftedFilter();
            var invoiceNote = await _invoiceNoteRepository.GetAll()
                .Where(x=>x.Status==NoteStatus.Draft)
                .Include(x => x.InvoiceItems)
                .ThenInclude(t => t.ShippingRequestTripFK)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (invoiceNote == null)
                throw new UserFriendlyException(L("Theinvoicedoesnotfound"));

            var list= ObjectMapper.Map<CreateOrEditInvoiceNoteDto>(invoiceNote);
            list.InvoiceItems.ForEach(x => x.Checked = true);
            return list;
        }
      
        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task Canacel(int invoiceId)
        {
            DisableTenancyFilters();
            DisableDraftedFilter();
            var invoiceNote = await _invoiceNoteRepository.FirstOrDefaultAsync(x=>x.Id==invoiceId && x.Status == NoteStatus.Draft);

            if (invoiceNote == null)
                throw new UserFriendlyException(L("Theinvoicedoesnotfound"));

            invoiceNote.Status = NoteStatus.Canceled;
        }
        #endregion

        #region Notes
        [AbpAuthorize(AppPermissions.Pages_InvoiceNote_Create)]
        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task AddNote(NoteInputDto input)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            await DisableDraftedFilterIfTachyonDealerOrHost();
            var invoiceNote = await _invoiceNoteRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == input.Id);
            invoiceNote.Note = input.Note;
            invoiceNote.CanBePrinted = input.CanBePrinted;
        }
        public async Task<NoteInputDto> GetNote(int tripId)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            await DisableDraftedFilterIfTachyonDealerOrHost();
            return await _invoiceNoteRepository.GetAll()
                  .Select(y => new NoteInputDto()
                  {
                      Id = y.Id,
                      Note = y.Note,
                      CanBePrinted = y.CanBePrinted
                  }).FirstOrDefaultAsync(x => x.Id == tripId);
        }
        #endregion

        #region Void
        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task GeneratePartialInvoiceNote(CreateOrEditInvoiceNoteDto input)
        {
            var invoiceNote = ObjectMapper.Map<InvoiceNote>(input);

            invoiceNote.VoidType = VoidType.PartialVoid;
            input.IsDrafted = true;
            var invoiceNoteId = await _invoiceNoteRepository.InsertAndGetIdAsync(invoiceNote);


            //invoiceNote.ReferanceNumber = GenerateInvoiceNoteReferanceNumber(invoiceNoteId, invoiceNote.NoteType);
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

        public async Task GenrateFullVoidInvoiceNote(long id)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            var invoiceTenant = await _invoiceReposity.FirstOrDefaultAsync(x => x.Id == id);
            if (invoiceTenant != null)
                await FullVoidInvoiceForShipper(id);
            else
                await FullVoidSubmitInvoiceForCarrier(id);
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
                .Select(x => new CompayForDropDownDto { Id = x.Id, DisplayName = x.TenancyName,isShipper=x.EditionId == ShipperEditionId }).AsNoTracking().ToListAsync();
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
      
                var invoices =  await _invoiceReposity.GetAll()
               .Where(m => m.TenantId == id)
               .Select(x => new InvoiceRefreanceNumberDto { Id = x.Id, RefreanceNumber = x.InvoiceNumber})
               .AsNoTracking().ToListAsync();
            
            var submitInvoices =  await _submitInvoiceReposity.GetAll()
               .Where(m => m.TenantId == id)
               .Select(x => new InvoiceRefreanceNumberDto { Id = x.Id, RefreanceNumber = x.ReferencNumber.Value })
               .AsNoTracking().ToListAsync();

            return invoices.Concat<InvoiceRefreanceNumberDto>(submitInvoices).ToList();
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
            if (invoiceTenant != null )
            {
                return await _invoiveTripRepository.GetAll()
               .Where(x => x.InvoiceId == id)
               .Include(x => x.ShippingRequestTripFK)
               .Select(x => new GetAllInvoiceItemDto() {TripId = x.TripId,
                   WaybillNumber = x.ShippingRequestTripFK.WaybillNumber.Value,
               Price=x.ShippingRequestTripFK.SubTotalAmountWithCommission.Value,
               VatAmount=x.ShippingRequestTripFK.VatAmountWithCommission.Value,
               TotalAmount=x.ShippingRequestTripFK.TotalAmountWithCommission.Value,
               TaxVat=x.ShippingRequestTripFK.TaxVat.Value
               })
               .AsNoTracking()
               .ToListAsync();
            }
            return await _submitInvoiceTrip.GetAll()
            .Where(x => x.SubmitId == id)
            .Include(x => x.ShippingRequestTripFK)
            .Select(x => new GetAllInvoiceItemDto() {TripId = x.TripId, 
                WaybillNumber = x.ShippingRequestTripFK.WaybillNumber.Value,
                Price = x.ShippingRequestTripFK.SubTotalAmount.Value,
                VatAmount = x.ShippingRequestTripFK.VatAmount.Value,
                TotalAmount = x.ShippingRequestTripFK.TotalAmount.Value,
                TaxVat=x.ShippingRequestTripFK.TaxVat.Value
            })
            .AsNoTracking()
            .ToListAsync();
        }

        public async Task<WaybillsVasesPricesOutput> GetWaybillOrVasPrices(int? tripId, long? tripVasId, bool isShipperTenant)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            if (tripId != null)
            {
                return await CalculateTripPrices(tripId, isShipperTenant);
            }
            else
            {
                return await CalculateTripVasPrices(tripVasId, isShipperTenant);
            }

        }

        private async Task<WaybillsVasesPricesOutput> CalculateTripVasPrices(long? tripVasId, bool isShipperTenant)
        {
            var tripVas = await _shippingRequestTripVasRepository.FirstOrDefaultAsync(tripVasId.Value);
            if (isShipperTenant)
            {
                return new WaybillsVasesPricesOutput
                {
                    price = tripVas.TotalAmountWithCommission,
                    TotalAmount = tripVas.TotalAmount,
                    VatAmount = tripVas.VatAmount
                };
            }
            else
            {
                return new WaybillsVasesPricesOutput
                {
                    price = tripVas.TotalAmount,
                    TotalAmount = tripVas.TotalAmount,
                    VatAmount = tripVas.VatAmount
                };
            }
        }

        private async Task<WaybillsVasesPricesOutput> CalculateTripPrices(int? tripId, bool isShipperTenant)
        {
            var trip = await _shippingRequestTripRepository.FirstOrDefaultAsync(tripId.Value);

            if (isShipperTenant)
            {
                return new WaybillsVasesPricesOutput
                {
                    price = trip.TotalAmountWithCommission,
                    TotalAmount = trip.TotalAmountWithCommission,
                    VatAmount = trip.VatAmountWithCommission
                };
            }
            else
            {
                return new WaybillsVasesPricesOutput
                {
                    price = trip.TotalAmount,
                    TotalAmount = trip.TotalAmount,
                    VatAmount = trip.VatAmount
                };
            }
        }
        #endregion

        #region Helper
        [AbpAuthorize(AppPermissions.Pages_InvoiceNote_Create)]
        [RequiresFeature(AppFeatures.TachyonDealer)]
        private async Task Create(CreateOrEditInvoiceNoteDto input)
        {
            if (input.InvoiceItems.Any())
            {
                foreach (var noteItem in input.InvoiceItems)
                {
                    noteItem.TotalAmount = noteItem.Price + noteItem.VatAmount;
                }
            }
            else
            {
                throw new UserFriendlyException(L("YouMustEnterInvoiceItems"));
            }

            var invoiceNote = ObjectMapper.Map<InvoiceNote>(input);
            invoiceNote.IsDrafted = true;
            var invoiceNoteId = await _invoiceNoteRepository.InsertAndGetIdAsync(invoiceNote);

            //invoiceNote.ReferanceNumber = GenerateInvoiceNoteReferanceNumber(invoiceNoteId, invoiceNote.NoteType);
            if (input.InvoiceItems.Any())
                 ItemValueCalculator(invoiceNote);
        }
        [AbpAuthorize(AppPermissions.Pages_InvoiceNote_Edit)]
        [RequiresFeature(AppFeatures.TachyonDealer)]
        private async Task Update(CreateOrEditInvoiceNoteDto model)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            DisableDraftedFilter();
            var inoviceNote = await _invoiceNoteRepository
                .GetAllIncluding(x => x.InvoiceItems)
                .FirstOrDefaultAsync(x => x.Id == model.Id.Value &&
                x.Status==NoteStatus.Draft);

            if (inoviceNote == null)
                throw new UserFriendlyException(L("Theinvoicedoesnotfound"));

            await RemoveDeletedWaybills(model, inoviceNote);
            ObjectMapper.Map(model, inoviceNote);
            ItemValueCalculator(inoviceNote);
            
        }

        private async Task RemoveDeletedWaybills(CreateOrEditInvoiceNoteDto model, InvoiceNote inoviceNote)
        {
            foreach (var item in inoviceNote.InvoiceItems.ToList())
            {
                if (!model.InvoiceItems.Any(x => x.Id == item.Id))
                {
                    await _invoiceNoteItemReposity.DeleteAsync(item);
                    inoviceNote.InvoiceItems.Remove(item);
                }
            }
        }

        private void ItemValueCalculator(InvoiceNote invoiceNote)
        {
            invoiceNote.Price = invoiceNote.InvoiceItems.Count()>0 ? invoiceNote.InvoiceItems.Sum(r => r.Price):invoiceNote.Price;
            invoiceNote.VatAmount = invoiceNote.InvoiceItems.Count() > 0 ? invoiceNote.InvoiceItems.Sum(r => r.VatAmount) :invoiceNote.VatAmount;
            invoiceNote.TotalValue = invoiceNote.Price+ invoiceNote.VatAmount;
        }


        private async Task FullVoidInvoiceForShipper(long invoiceId)
        {
            var invoice = await _invoiceReposity.GetAll()
                       .Include(x => x.Trips)
                       .ThenInclude(x=>x.ShippingRequestTripFK)
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
                    TripId = x.TripId,
                    Price = x.ShippingRequestTripFK.TotalAmountWithCommission.Value,
                    TotalAmount = x.ShippingRequestTripFK.TotalAmountWithCommission.Value,
                    VatAmount = x.ShippingRequestTripFK.VatAmountWithCommission.Value
                }).ToList()
            };
            var invoiceNoteId = await _invoiceNoteRepository.InsertAndGetIdAsync(invoiceNote);
            //invoiceNote.ReferanceNumber = GenerateInvoiceNoteReferanceNumber(invoiceNoteId, NoteType.Credit);
            invoiceNote.IsDrafted = true;
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
                    TripId = x.TripId,
                    Price = x.ShippingRequestTripFK.TotalAmountWithCommission.Value,
                    TotalAmount = x.ShippingRequestTripFK.TotalAmountWithCommission.Value,
                    VatAmount = x.ShippingRequestTripFK.VatAmountWithCommission.Value
                }).ToList()
            };
            var invoiceNoteId = await _invoiceNoteRepository.InsertAndGetIdAsync(invoiceNote);
            //invoiceNote.ReferanceNumber = GenerateInvoiceNoteReferanceNumber(invoiceNoteId, NoteType.Credit);
            invoiceNote.IsDrafted = true;
        }
        #endregion

        #region report
        public List<InvoiceNoteInfoDto> GetInvoiceNoteReportInfo(long id)
        {
            DisableTenancyFilters();
            DisableDraftedFilter();
            var invoiceNote = _invoiceNoteRepository
                .GetAll()
                .Include(x => x.Tenant)
                .FirstOrDefault(i => i.Id == id);

            if (invoiceNote == null)
                throw new UserFriendlyException(L("Theinvoicedoesnotfound"));

            var invoiceNoteDto = ObjectMapper.Map<InvoiceNoteInfoDto>(invoiceNote);

            var admin = AsyncHelper.RunSync(() => _userManager.GetAdminByTenantIdAsync(invoiceNote.TenantId));
            var invoice = AsyncHelper.RunSync(() => _invoiceReposity.FirstOrDefaultAsync(x => x.InvoiceNumber == invoiceNote.InvoiceNumber));
            var submitInvoice = AsyncHelper.RunSync(() => _submitInvoiceReposity.FirstOrDefaultAsync(x => x.ReferencNumber == invoiceNote.InvoiceNumber));

            invoiceNoteDto.Email = admin.EmailAddress;
            invoiceNoteDto.Attn = admin.FullName;

            if (invoice != null)
                invoiceNoteDto.ReInvoiceDate = invoice.CreationTime.ToString("dd/mm/yyyy mm:hh");
            else if(submitInvoice!=null)
            {
                invoiceNoteDto.ReInvoiceDate = submitInvoice.CreationTime.ToString("dd/mm/yyyy mm:hh");
            }
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
       
        public List<InvoiceNoteItemDto> GetInvoiceNoteItemReportInfo(long invoiceNoteId)
        {
            DisableTenancyFilters();
            DisableDraftedFilter();
            var invoiceNote = AsyncHelper.RunSync(() => GetInvoiceNoteInfo(invoiceNoteId));
            if (invoiceNote == null) throw new UserFriendlyException(L("Theinvoicedoesnotfound"));
            var TotalItem = invoiceNote.InvoiceItems.Count;
            int Sequence = 1;
            if (!invoiceNote.IsManual)
            {
                List<InvoiceNoteItemDto> Items = new List<InvoiceNoteItemDto>();
                invoiceNote.InvoiceItems.ToList().ForEach(invoiceItem =>
                {
                    Items.Add(new InvoiceNoteItemDto
                    {
                        Sequence = $"{Sequence}/{TotalItem}",
                        Price =
                                //AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier)
                                //    ? invoiceItem.ShippingRequestTripFK.SubTotalAmount.Value
                                //    : invoiceItem.ShippingRequestTripFK.SubTotalAmountWithCommission.Value,
                                invoiceItem.Price,
                        VatAmount =
                                //AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier)
                                //    ? trip.ShippingRequestTripFK.VatAmount.Value
                                //    : trip.ShippingRequestTripFK.VatAmountWithCommission.Value,
                                invoiceItem.VatAmount,
                        TotalAmount =
                                //AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier)
                                //    ? trip.ShippingRequestTripFK.TotalAmount.Value
                                //    : trip.ShippingRequestTripFK.TotalAmountWithCommission.Value,
                                invoiceItem.TotalAmount,
                        WayBillNumber = invoiceItem.ShippingRequestTripFK?.WaybillNumber.ToString(),
                        Date =
                            invoiceItem.ShippingRequestTripFK!=null && invoiceItem.ShippingRequestTripFK.EndTripDate.HasValue
                                ? invoiceItem.ShippingRequestTripFK?.ActualDeliveryDate.Value.ToString("dd/MM/yyyy")
                                : invoiceItem.InvoiceNoteFK.CreationTime.ToString("dd/MM/yyyy")
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
        private string GenerateInvoiceNoteReferanceNumber(InvoiceNote invoiceNote)
        {
            string noteFormat = invoiceNote.NoteType == NoteType.Debit ? "TDN" : "TCN";
            var lastItemReference=_invoiceNoteRepository.GetAll()
                .Where(x => x.ReferanceNumber.Contains(noteFormat))
                .OrderByDescending(y => y.ReferanceNumber)
                .Select(x=>x.ReferanceNumber)
                .FirstOrDefault();
            long referanceId= 10001;
            if (lastItemReference !=null)
            {
                referanceId = Convert.ToInt64(lastItemReference.Split(noteFormat+"-")[1]) + 1;
            }
            var referanceNumber = "{0}-{1}";
            return string.Format(referanceNumber, noteFormat, referanceId);
        }
        #endregion
    }
}