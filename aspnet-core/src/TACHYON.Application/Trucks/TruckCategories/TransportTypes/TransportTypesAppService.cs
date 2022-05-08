using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Extension;
using TACHYON.Trucks.TruckCategories.TransportTypes.Dtos;
using TACHYON.Trucks.TruckCategories.TransportTypes.TransportTypesTranslations;

namespace TACHYON.Trucks.TruckCategories.TransportTypes
{
    [AbpAuthorize(AppPermissions.Pages_TransportTypes)]
    public class TransportTypesAppService : TACHYONAppServiceBase, ITransportTypesAppService
    {
        private readonly IRepository<TransportType> _transportTypeRepository;


        public TransportTypesAppService(IRepository<TransportType> transportTypeRepository)
        {
            _transportTypeRepository = transportTypeRepository;
        }

        public async Task<PagedResultDto<TransportTypeDto>> GetAll(GetAllTransportTypesInput input)
        {
            var filteredTransportTypes = _transportTypeRepository.GetAll()
                .Include(x => x.Translations)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.DisplayName.Contains(input.Filter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter),
                    e => e.DisplayName == input.DisplayNameFilter)
                .OrderBy(input.Sorting ?? "id asc");


            var transportTypes = await filteredTransportTypes.PageBy(input)
                .Select(x => new TransportTypeDto
                {
                    Id = x.Id,
                    TranslatedDisplayName = x.GetTranslatedDisplayName<TransportType, TransportTypesTranslation>()
                }).ToListAsync();

            return new PagedResultDto<TransportTypeDto>(
                await filteredTransportTypes.CountAsync(),
                transportTypes
            );
        }

        public async Task<GetTransportTypeForViewDto> GetTransportTypeForView(int id)
        {
            var transportType = await _transportTypeRepository
                .GetAllIncluding(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == id);

            var output =
                new GetTransportTypeForViewDto { TransportType = ObjectMapper.Map<TransportTypeDto>(transportType) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_TransportTypes_Edit)]
        public async Task<GetTransportTypeForEditOutput> GetTransportTypeForEdit(EntityDto input)
        {
            var transportType = await _transportTypeRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetTransportTypeForEditOutput
            {
                TransportType = ObjectMapper.Map<CreateOrEditTransportTypeDto>(transportType)
            };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditTransportTypeDto input)
        {
            await CheckNameIsExists(input);
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_TransportTypes_Create)]
        protected virtual async Task Create(CreateOrEditTransportTypeDto input)
        {
            var transportType = ObjectMapper.Map<TransportType>(input);


            await _transportTypeRepository.InsertAsync(transportType);
        }

        [AbpAuthorize(AppPermissions.Pages_TransportTypes_Edit)]
        protected virtual async Task Update(CreateOrEditTransportTypeDto input)
        {
            var transportType = await _transportTypeRepository.FirstOrDefaultAsync((int)input.Id);

            if (transportType.DisplayName.ToLower().Contains(TACHYONConsts.OthersDisplayName)
                && !input.DisplayName.ToLower().Contains(TACHYONConsts.OthersDisplayName))
                throw new UserFriendlyException(L("OtherTransportTypeMustContainOther"));

            ObjectMapper.Map(input, transportType);
        }

        [AbpAuthorize(AppPermissions.Pages_TransportTypes_Delete)]
        public async Task Delete(EntityDto input)
        {
            var transportType = await _transportTypeRepository.SingleAsync(x => x.Id == input.Id);
            if (transportType.DisplayName.ToLower().Contains(TACHYONConsts.OthersDisplayName))
                throw new UserFriendlyException(L("OtherTransportTypeNotRemovable"));

            await _transportTypeRepository.DeleteAsync(transportType);
        }


        #region Heleper

        private async Task CheckNameIsExists(CreateOrEditTransportTypeDto input)
        {
            if (await _transportTypeRepository.GetAll().AnyAsync(x =>
                    x.DisplayName.ToLower() == input.DisplayName.Trim().ToLower() && x.Id != input.Id))
            {
                throw new UserFriendlyException(L("TheNameIsAlreadyExists"));
            }
        }

        #endregion
    }
}