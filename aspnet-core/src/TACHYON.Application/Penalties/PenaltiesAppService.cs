using Abp.Application.Features;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Common;
using TACHYON.Configuration;
using TACHYON.Features;
using TACHYON.MultiTenancy;
using TACHYON.Penalties.Dto;

namespace TACHYON.Penalties
{
    public class PenaltiesAppService : TACHYONAppServiceBase, IPenaltiesAppService
    {
        private readonly IRepository<Penalty> _penaltyRepository;
        private readonly IRepository<PenaltyComplaint> _penaltyComplaintRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly PenaltyManager _penaltyManager;
        private readonly ISettingManager _settingManager;
        public PenaltiesAppService(IRepository<Penalty> penaltyRepository, IRepository<Tenant> tenantRepository, IRepository<PenaltyComplaint> penaltyComplaintRepository, PenaltyManager penaltyManager, ISettingManager settingManager)
        {
            _penaltyRepository = penaltyRepository;
            _tenantRepository = tenantRepository;
            _penaltyComplaintRepository = penaltyComplaintRepository;
            _penaltyManager = penaltyManager;
            _settingManager = settingManager;
        }

        #region MainFunctions
        public async Task<LoadResult> GetAll(LoadOptionsInput input)
        {
            var query = _penaltyRepository
                           .GetAll()
                           .Include(x=> x.ShippingRequestTripFK)
                           .ProjectTo<GetAllPenaltiesDto>(AutoMapperConfigurationProvider)
                           .AsNoTracking();
            if (!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                DisableTenancyFilters();
            }
            return await LoadResultAsync<GetAllPenaltiesDto>(query, input.LoadOptions);
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
            var penalty = await _penaltyRepository.FirstOrDefaultAsync(x => x.Id == Id);

            if (penalty == null)
                throw new UserFriendlyException("ThePenaltyDoseNotFounded");

            return ObjectMapper.Map<CreateOrEditPenaltyDto>(penalty);
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
            await _penaltyComplaintRepository.InsertAsync(penaltyComplaint);
        }
        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task CancelPenalty(int id)
        {
            DisableTenancyFilters();
            var penalty = await _penaltyRepository
               .GetAllIncluding(x => x.PenaltyComplaintFK)
               .Where(x => x.Id == id).FirstOrDefaultAsync();

            if (penalty == null)
                throw new UserFriendlyException("ThePenaltyDoseNotFounded");

            if (penalty.Status == PenaltyStatus.Canceled)
                throw new UserFriendlyException("ThePenaltyAlreadyCanceled");

            penalty.Status = PenaltyStatus.Canceled;
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
            var commestion =  _penaltyManager.CalculateValues(model.TotalAmount, model.CommissionType, value, value, value, taxVat);
            var mapper = ObjectMapper.Map(commestion,peanlty);
            await _penaltyRepository.InsertAsync(mapper);
        }
        private async Task Update(CreateOrEditPenaltyDto model)
        {
            DisableTenancyFilters();
            var pnealty = await _penaltyRepository.FirstOrDefaultAsync(x => x.Id == model.Id.Value);
            ObjectMapper.Map(model, pnealty);
        }
        #endregion

    }
}
