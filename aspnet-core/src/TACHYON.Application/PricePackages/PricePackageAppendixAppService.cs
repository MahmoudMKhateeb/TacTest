using Abp.Authorization;
using Abp.BackgroundJobs;
using Abp.Domain.Repositories;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Common;
using TACHYON.PricePackages.Dto.PricePackageAppendices;
using TACHYON.PricePackages.PricePackageAppendices;
using TACHYON.PricePackages.PricePackageAppendices.Jobs;

namespace TACHYON.PricePackages
{
    [AbpAuthorize(AppPermissions.Pages_PricePackageAppendix)]
    public class PricePackageAppendixAppService : TACHYONAppServiceBase, IPricePackageAppendixAppService
    {
        private readonly IRepository<PricePackageAppendix> _appendixRepository;
        private readonly IBackgroundJobManager _jobManager;

        public PricePackageAppendixAppService(
            IRepository<PricePackageAppendix> appendixRepository,
            IBackgroundJobManager jobManager)
        {
            _appendixRepository = appendixRepository;
            _jobManager = jobManager;
        }

        public async Task<LoadResult> GetAll(LoadOptionsInput input)
        {
            var appendices = _appendixRepository.GetAll()
                .AsNoTracking().ProjectTo<AppendixListDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(appendices, input.LoadOptions);
        }

        public async Task<AppendixForViewDto> GetForView(int id)
        {
            var appendix = await _appendixRepository.GetAllIncluding(x => x.Proposal)
                .AsNoTracking().SingleAsync(x => x.Id == id);

            return ObjectMapper.Map<AppendixForViewDto>(appendix);
        }

        [AbpAuthorize(AppPermissions.Pages_PricePackageAppendix_Update)]
        public async Task<CreateOrEditAppendixDto> GetForEdit(int id)
        {
            var appendix = await _appendixRepository.GetAllIncluding(x=> x.Proposal)
                .AsNoTracking().SingleAsync(x => x.Id == id);

            return ObjectMapper.Map<CreateOrEditAppendixDto>(appendix);
        }

        public async Task CreateOrEdit(CreateOrEditAppendixDto input)
        {
            if (input.Id.HasValue)
            {
                await Update(input);
                return;
            }

            await Create(input);
        }

        [AbpAuthorize(AppPermissions.Pages_PricePackageAppendix_Create)]
        protected virtual async Task Create(CreateOrEditAppendixDto input)
        {
            var createdAppendix = ObjectMapper.Map<PricePackageAppendix>(input);

            var appendixId = await _appendixRepository.InsertAndGetIdAsync(createdAppendix);

           // Todo fix this asap
            // await _jobManager.EnqueueAsync<GenerateAppendixFileJob, GenerateAppendixFileJobArgument>(
            //     new GenerateAppendixFileJobArgument
            //     {
            //         AppendixId = appendixId, FileReceiverEmailAddress = input.EmailAddress
            //     });
        }

        [AbpAuthorize(AppPermissions.Pages_PricePackageAppendix_Update)]
        protected virtual async Task Update(CreateOrEditAppendixDto input)
        {
            if (!input.Id.HasValue) return;
            
            var updatedAppendix = await _appendixRepository.FirstOrDefaultAsync(input.Id.Value);

            if (updatedAppendix.Status == AppendixStatus.Confirmed)
                throw new UserFriendlyException(L("YouCanNotUpdateConfirmedAppendix"));

            ObjectMapper.Map(updatedAppendix, input);
        }

        [AbpAuthorize(AppPermissions.Pages_PricePackageAppendix_Accept)]
        public async Task Accept(int appendixId)
        {
            var status = await _appendixRepository.GetAll().Select(x => x.Status).FirstOrDefaultAsync();

            if (status == default) throw new UserFriendlyException(L("NotFound"));

            if (status == AppendixStatus.Confirmed) throw new UserFriendlyException(L("AlreadyConfirmed"));

            _appendixRepository.Update(appendixId, x => x.Status = AppendixStatus.Confirmed);
        }
        
        [AbpAuthorize(AppPermissions.Pages_PricePackageAppendix_Reject)]
        public async Task Reject(int appendixId)
        {
            var status = await _appendixRepository.GetAll().Select(x => x.Status).FirstOrDefaultAsync();

            switch (status)
            {
                case default(AppendixStatus):
                    throw new UserFriendlyException(L("NotFound"));
                case AppendixStatus.Confirmed:
                    throw new UserFriendlyException(L("YouCanNotChangeStatus"));
                case AppendixStatus.Rejected:
                    throw new UserFriendlyException(L("AlreadyRejected"));
                default:
                    _appendixRepository.Update(appendixId, x => x.Status = AppendixStatus.Rejected);
                    break;
            }
        }

        [AbpAuthorize(AppPermissions.Pages_PricePackageAppendix_Delete)]
        public async Task Delete(int id)
        {
            var appendix = await _appendixRepository.FirstOrDefaultAsync(x => x.Id == id);

            if (appendix == null) throw new UserFriendlyException(L("NotFound"));

            if (appendix.Status == AppendixStatus.Confirmed)
                throw new UserFriendlyException(L("YouCanNotDeleteConfirmedAppendix"));
            
            await _appendixRepository.DeleteAsync(appendix);
        }
    }
}