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
                if (isTms)
                    DisableDraftedFilter();
            }
            var options = JsonConvert.DeserializeObject<DataSourceLoadOptionsBase>(input.LoadOptions);

            var query = await _penaltyRepository
                           .GetAll()
                           .Include(x=> x.ShippingRequestTripFK)
                           .ProjectTo<GetAllPenaltiesDto>(AutoMapperConfigurationProvider)
                           .Skip(options.Skip).Take(options.Take)
                           .AsNoTracking().ToListAsync();

            return LoadResult(query, input.LoadOptions);
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
            if(await IsTachyonDealer())
            {
                DisableDraftedFilter();
            }
            var penalty = await _penaltyRepository.FirstOrDefaultAsync(x => x.Id == Id && x.Status==PenaltyStatus.Draft);

            if (penalty == null)
                throw new UserFriendlyException("ThePenaltyDoseNotFounded");

            return ObjectMapper.Map<CreateOrEditPenaltyDto>(penalty);
        }

        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task ConfirmPenalty(long id)
        {
            DisableDraftedFilter();
            await DisableTenancyFilterIfTachyonDealerOrHost();
            var penalty = await _penaltyRepository.FirstOrDefaultAsync(x => x.Id == id && x.Status==PenaltyStatus.Draft);
            if(penalty== null)
            {
                throw new UserFriendlyException(L("PenaltyNotFound"));
            }
            penalty.Status = PenaltyStatus.Confirmed;
            penalty.IsDrafted = false;
        }

        public async Task<PenaltyComplaintDto> GetPenaltyComplaintForView(int id)
        {
            if(await IsTachyonDealer())
            {
                DisableDraftedFilter();
            }
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

            penalty.Status = PenaltyStatus.Canceled;
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
            if (!await _penaltyRepository.GetAll().AnyAsync(x => x.Id == input.PenaltyId))
                throw new UserFriendlyException(L(""));

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
            DisableDraftedFilter();

            var penalty = await _penaltyRepository
               .GetAllIncluding(x => x.PenaltyComplaintFK)
               .Where(x => x.Id == id && x.IsDrafted==true && x.Status != PenaltyStatus.Canceled)
               .FirstOrDefaultAsync();

            if (penalty == null)
                throw new UserFriendlyException("ThePenaltyDoesNotFounded");


            penalty.Status = PenaltyStatus.Canceled;
        }

        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task<List<GetAllWaybillsDto>> GetAllWaybillsByCompany(int companyTenantId, int? destinationCompanyTenantId)
        {
            DisableTenancyFilters();
            var trips= await _shippingRequestTripRepository.GetAll()
                .Where(x => x.ShippingRequestFk.TenantId == companyTenantId)
                .WhereIf(destinationCompanyTenantId != null, x => x.ShippingRequestFk.CarrierTenantId == destinationCompanyTenantId)
                .ToListAsync();
            return ObjectMapper.Map<List<GetAllWaybillsDto>>(trips);
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
            peanlty.CommissionType = PriceOffers.PriceOfferCommissionType.CommissionValue;
            var value = model.CommissionPercentageOrAddValue;
            var taxVat = _settingManager.GetSettingValue<decimal>(AppSettings.HostManagement.TaxVat);
            var commestion =  _penaltyManager.CalculateValues(model.ItmePrice, model.CommissionType, value, value, value, taxVat);
            var penalty = ObjectMapper.Map(commestion,peanlty);
            penalty.IsDrafted = true;
            penalty.TaxVat = taxVat;
            await _penaltyRepository.InsertAsync(penalty);
        }
        private async Task Update(CreateOrEditPenaltyDto model)
        {
            DisableTenancyFilters();
            DisableDraftedFilter();

            var penalty = await _penaltyRepository.FirstOrDefaultAsync(x => x.Id == model.Id.Value && x.IsDrafted == true);
            var output = ObjectMapper.Map(model, penalty);
            var value = output.CommissionPercentageOrAddValue;
            var taxVat = _settingManager.GetSettingValue<decimal>(AppSettings.HostManagement.TaxVat);
            var commestion = _penaltyManager.CalculateValues(output.ItmePrice, output.CommissionType, value, value, value, taxVat);
             ObjectMapper.Map(commestion, output);
           // ObjectMapper.Map(output, penalty);
        }
        #endregion

    }
}
