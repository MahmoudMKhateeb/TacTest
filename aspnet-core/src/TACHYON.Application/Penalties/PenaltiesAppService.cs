using Abp.Application.Features;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Common;
using TACHYON.Configuration;
using TACHYON.Extension;
using TACHYON.Features;
using TACHYON.MultiTenancy;
using TACHYON.Notifications;
using TACHYON.Penalties.Dto;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Penalties
{
    public class PenaltiesAppService : TACHYONAppServiceBase, IPenaltiesAppService
    {
        private readonly IRepository<Penalty> _penaltyRepository;
        private readonly IRepository<PenaltyComplaint> _penaltyComplaintRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly PenaltyManager _penaltyManager;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly IAppNotifier _appNotifier;
        private readonly ISettingManager _settingManager;
        public PenaltiesAppService(IRepository<Penalty> penaltyRepository, IRepository<Tenant> tenantRepository, IRepository<PenaltyComplaint> penaltyComplaintRepository, PenaltyManager penaltyManager, IRepository<ShippingRequestTrip> shippingRequestTripRepository, IAppNotifier appNotifier, ISettingManager settingManager)
        {
            _penaltyRepository = penaltyRepository;
            _tenantRepository = tenantRepository;
            _penaltyComplaintRepository = penaltyComplaintRepository;
            _penaltyManager = penaltyManager;
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _appNotifier = appNotifier;
            _settingManager = settingManager;
        }

        #region MainFunctions
        public async Task<LoadResult> GetAll(LoadOptionsInput input)
        {
            var isTms = await IsEnabledAsync(AppFeatures.TachyonDealer);

            if (!AbpSession.TenantId.HasValue || isTms)
            {
                DisableTenancyFilters();
            }

            var query = _penaltyRepository
                           .GetAll()
                           .WhereIf(!(await IsTachyonDealer()) && AbpSession.TenantId.HasValue,
                           x => x.Status == PenaltyStatus.Paid || x.Status== PenaltyStatus.Confirmed || 
                           (x.Status==PenaltyStatus.Draft && x.Type==PenaltyType.NotLogged) ||
                           (x.Status == PenaltyStatus.Canceled && x.Type == PenaltyType.NotLogged))
                           .Include(x => x.ShippingRequestTripFK)
                           .ProjectTo<GetAllPenaltiesDto>(AutoMapperConfigurationProvider)
                           .AsNoTracking();

            return await LoadResultAsync(query, input.LoadOptions);

        }
        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task CreateOrEdit(CreateOrEditPenaltyDto input)
        {
            if (input.TenantId == input.DestinationTenantId)
                throw new UserFriendlyException(L("DestinationCompanyShouldNotBeSourceCompany"));

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
        public async Task<CreateOrEditPenaltyDto> GetPenaltyForEditDto(long Id)
        {
            DisableTenancyFilters();

            var penalty = await _penaltyRepository
                .GetAll()
                .Include(x=>x.PenaltyItems)
                .ThenInclude(x=>x.ShippingRequestTripFK)
                .FirstOrDefaultAsync(x => x.Id == Id && x.Status==PenaltyStatus.Draft);

            if (penalty == null)
                throw new UserFriendlyException("ThePenaltyDoseNotFounded");

            var list= ObjectMapper.Map<CreateOrEditPenaltyDto>(penalty);

            return list;
        }

        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task ConfirmPenalty(long id)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            var penalty = await _penaltyRepository.FirstOrDefaultAsync(x => x.Id == id && x.Status==PenaltyStatus.Draft);
            if(penalty== null)
            {
                throw new UserFriendlyException(L("PenaltyNotFound"));
            }
            penalty.Status = PenaltyStatus.Confirmed;
        }

        public async Task<PenaltyComplaintDto> GetPenaltyComplaintForView(int id)
        {
            var penaltyComplaint = await _penaltyComplaintRepository.FirstOrDefaultAsync(x => x.PenaltyId == id);

            if (penaltyComplaint == null)
                throw new UserFriendlyException("ThePenaltyComplaintDoseNotFounded");

            return ObjectMapper.Map<PenaltyComplaintDto>(penaltyComplaint);
        }

        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task AcceptComplaint(int id)
        {
            DisableTenancyFilters();
            var penalty = await _penaltyRepository
                .GetAllIncluding(x=> x.PenaltyComplaintFK)
                .Where(x => x.PenaltyComplaintFK.Id == id).FirstOrDefaultAsync();

            if (penalty == null)
                throw new UserFriendlyException("ThePenaltyDoseNotFounded");

            penalty.PenaltyComplaintFK.Status = ComplaintStatus.Accepted;
        }

        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task RejectComplaint(RejectComplaintDto input)
        {
            DisableTenancyFilters();
            var penaltyComplaint = await _penaltyComplaintRepository.GetAllIncluding(x=> x.PenaltyFK)
                .Where(x => x.PenaltyId == input.Id).FirstOrDefaultAsync();
            penaltyComplaint.RejectReason = input.RejectReason;
            penaltyComplaint.Status  = ComplaintStatus.Rejected;
        }
        public async Task RegisterComplaint(RegisterPenaltyComplaintDto input)
        {
            var penalty = await _penaltyRepository.FirstOrDefaultAsync(x => x.Id == input.PenaltyId &&
            x.Status == PenaltyStatus.Draft);
            if (penalty ==null)
                throw new UserFriendlyException(L("PenaltyConnotFound"));

            var penaltyComplaint = ObjectMapper.Map<PenaltyComplaint>(input);
            var complaintId = await _penaltyComplaintRepository.InsertAndGetIdAsync(penaltyComplaint);
            _penaltyRepository.Update(input.PenaltyId,x=> x.PenaltyComplaintId = complaintId);
            
            if (AbpSession.TenantId.HasValue)
                await _appNotifier.NotifyHostAndTmsWhenPenaltyComplaintAdded(AbpSession.TenantId.Value, input.PenaltyId);
        }

        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task CancelPenalty(int id)
        {
            DisableTenancyFilters();

            var penalty = await _penaltyRepository
               .GetAllIncluding(x => x.PenaltyComplaintFK)
               .Where(x => x.Id == id && x.Status==PenaltyStatus.Draft && x.Status != PenaltyStatus.Canceled)
               .FirstOrDefaultAsync();

            if (penalty == null)
                throw new UserFriendlyException("ThePenaltyDoesNotFounded");


            penalty.Status = PenaltyStatus.Canceled;
        }

        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task<List<PenaltyItemDto>> GetAllWaybillsByCompany(int companyTenantId, int? destinationCompanyTenantId)
        {
            DisableTenancyFilters();
            var trips= await _shippingRequestTripRepository.GetAll()
                .Where(x => x.ShippingRequestFk.TenantId == companyTenantId)
                .WhereIf(destinationCompanyTenantId != null, x => x.ShippingRequestFk.CarrierTenantId == destinationCompanyTenantId)
                .ToListAsync();
            return trips.Select(x => new PenaltyItemDto
            {
                ShippingRequestTripId = x.Id,
                WaybillNumber = x.WaybillNumber.Value.ToString()
            }).ToList();
            //return ObjectMapper.Map<List<PenaltyItemDto>>(trips);
        }


        public decimal GetTaxVat()
        {
            return _settingManager.GetSettingValue<decimal>(AppSettings.HostManagement.TaxVat);
        }
        #endregion

        #region Lookups
        public async Task<List<GetAllCompanyForDropDownDto>> GetAllCompanyForDropDown()
        {
            return await _tenantRepository.GetAll()
                .Where(x => x.EditionId == ShipperEditionId || x.EditionId == CarrierEditionId)
                .Select(x => new GetAllCompanyForDropDownDto { Id = x.Id, DisplayName = x.TenancyName }).ToListAsync();
        }
        #endregion

        #region Helper
        private async Task Create(CreateOrEditPenaltyDto model)
        {
            var peanlty = ObjectMapper.Map<Penalty>(model);
            //peanlty.CommissionType = PriceOffers.PriceOfferCommissionType.CommissionValue;
            var value = model.CommissionPercentageOrAddValue;
            var taxVat = _settingManager.GetSettingValue<decimal>(AppSettings.HostManagement.TaxVat);


            //calculate for penalty
            var commestion =  _penaltyManager.CalculateValues(model.CommissionType, value, value, value, taxVat, model.PenaltyItems);
            var penalty = ObjectMapper.Map(commestion,peanlty);
            penalty.TaxVat = taxVat;
            await _penaltyRepository.InsertAsync(penalty);
        }
        private async Task Update(CreateOrEditPenaltyDto model)
        {
            DisableTenancyFilters();
            var penalty = await _penaltyRepository.FirstOrDefaultAsync(x => x.Id == model.Id.Value && x.Status==PenaltyStatus.Draft);
            var output = ObjectMapper.Map(model, penalty);
            var value = output.CommissionPercentageOrAddValue;
            var taxVat = _settingManager.GetSettingValue<decimal>(AppSettings.HostManagement.TaxVat);
            var commestion = _penaltyManager.CalculateValues( output.CommissionType, value, value, value, taxVat, model.PenaltyItems);
             ObjectMapper.Map(commestion, output);
           // ObjectMapper.Map(output, penalty);
        }
        #endregion

    }
}
