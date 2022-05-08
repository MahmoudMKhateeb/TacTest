using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Authorization.Users;
using TACHYON.Common;
using TACHYON.Common.Dto;
using TACHYON.Dto;
using TACHYON.EmailTemplates.Dtos;
using TACHYON.Storage;
using TACHYON.Trucks.TrucksTypes.TrucksTypesTranslations.Dtos;

namespace TACHYON.EmailTemplates
{
    [AbpAuthorize(AppPermissions.Pages_EmailTemplates)]
    public class EmailTemplatesAppService : TACHYONAppServiceBase, IEmailTemplatesAppService
    {
        private readonly IRepository<EmailTemplate> _emailTemplateRepository;
        private readonly IRepository<EmailTemplateTranslation> _emailTemplatesTranslationRepository;
        private readonly IUserEmailer _emailer;

        public EmailTemplatesAppService(
            IRepository<EmailTemplate> emailTemplateRepository,
            IRepository<EmailTemplateTranslation> emailTemplatesTranslationRepository,
            IUserEmailer emailer)
        {
            _emailTemplateRepository = emailTemplateRepository;
            _emailTemplatesTranslationRepository = emailTemplatesTranslationRepository;
            _emailer = emailer;
        }

        public async Task<PagedResultDto<GetEmailTemplateForViewDto>> GetAll(GetAllEmailTemplatesInput input)
        {

            var filteredEmailTemplates = _emailTemplateRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.DisplayName.Contains(input.Filter) || e.Content.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter), e => e.DisplayName == input.DisplayNameFilter);

            var pagedAndFilteredEmailTemplates = filteredEmailTemplates
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var emailTemplates = from o in pagedAndFilteredEmailTemplates
                                 select new
                                 {

                                     o.Name,
                                     o.DisplayName,
                                     Id = o.Id
                                 };

            var totalCount = await filteredEmailTemplates.CountAsync();

            var dbList = await emailTemplates.ToListAsync();
            var results = new List<GetEmailTemplateForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetEmailTemplateForViewDto()
                {
                    EmailTemplate = new EmailTemplateDto
                    {

                        Name = o.Name,
                        DisplayName = o.DisplayName,
                        Id = o.Id,
                    }
                };

                results.Add(res);
            }

            return new PagedResultDto<GetEmailTemplateForViewDto>(
                totalCount,
                results
            );

        }

        public async Task<LoadResult> DxGetAll(LoadOptionsInput input)
        {
            var emailTemplates = _emailTemplateRepository.GetAll()
                .AsNoTracking()
                .ProjectTo<EmailTemplateDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(emailTemplates, input.LoadOptions);
        }

        public async Task<LoadResult> GetAllTranslations(GetAllTranslationInput<long> input)
        {
            var translations = _emailTemplatesTranslationRepository
                .GetAll().Where(x => x.CoreId == input.CoreId)
                .AsNoTracking().ProjectTo<EmailTemplateTranslationDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(translations, input.LoadOptions);
        }


        public async Task CreateOrEditTranslation(CreateOrEditEmailTemplateTranslationDto input)
        {
            //? Check if Core of Translation Is Exist Or Not
            #region CoreValidation

            var isTemplateExist = await _emailTemplateRepository.GetAll().AnyAsync(x => x.Id == input.CoreId);
            if (!isTemplateExist)
                throw new UserFriendlyException(L("CoreNotFound"));

            #endregion

            if (!input.Id.HasValue)
                await CreateTranslation(input);
            else
                await UpdateTranslation(input);
        }


        protected virtual async Task CreateTranslation(CreateOrEditEmailTemplateTranslationDto input)
        {

            var translation = ObjectMapper.Map<EmailTemplateTranslation>(input);

            await _emailTemplatesTranslationRepository.InsertAsync(translation);
        }

        protected virtual async Task UpdateTranslation(CreateOrEditEmailTemplateTranslationDto input)
        {
            var translation = await _emailTemplatesTranslationRepository
                .SingleAsync(x => x.Id == input.Id.Value);

            ObjectMapper.Map(input, translation);
        }

        public async Task DeleteTranslation(EntityDto input)
        {
            await _emailTemplatesTranslationRepository.DeleteAsync(input.Id);
        }



        public async Task<GetEmailTemplateTranslationForEditOutput> GetEmailTemplateTranslationForCreateOrEdit(int? id, int coreId)
        {
            GetEmailTemplateTranslationForEditOutput output = new GetEmailTemplateTranslationForEditOutput();
            output.EmailTemplate = new CreateOrEditEmailTemplateTranslationDto();

            if (id.HasValue)
            {
                var emailTemplateTranslation = await _emailTemplatesTranslationRepository.FirstOrDefaultAsync(id.Value);

                output.EmailTemplate = ObjectMapper.Map<CreateOrEditEmailTemplateTranslationDto>(emailTemplateTranslation);
            }




            var core = await _emailTemplateRepository.FirstOrDefaultAsync(coreId);

            if (output.EmailTemplate.TranslatedContent.IsNullOrEmpty())
            {
                output.EmailTemplate.TranslatedContent = core.Content;
            }
            return output;
        }


        public async Task<GetEmailTemplateForViewDto> GetEmailTemplateForView(int id)
        {
            var emailTemplate = await _emailTemplateRepository.GetAsync(id);

            var output = new GetEmailTemplateForViewDto { EmailTemplate = ObjectMapper.Map<EmailTemplateDto>(emailTemplate) };

            return output;
        }

        public async Task<GetEmailTemplateLayoutForView> GetEmailTemplateLayout()
        {
            string content = await _emailTemplateRepository.GetAll()
                .Where(x => x.EmailTemplateType == EmailTemplateTypesEnum.EmailTemplateLayout)
                .Select(x => x.Content)
                .FirstOrDefaultAsync();
            var adminUser = await UserManager.GetAdminHostAsync();
            return new GetEmailTemplateLayoutForView() { Content = content, DefaultTestEmail = adminUser.EmailAddress };
        }

        public async Task SendTestEmailTemplate(TestEmailTemplateInputDto input)
        => await _emailer.SendTestTemplateEmail(input);

        [AbpAuthorize(AppPermissions.Pages_EmailTemplates_Edit)]
        public async Task<GetEmailTemplateForEditOutput> GetEmailTemplateForEdit(EntityDto input)
        {
            var emailTemplate = await _emailTemplateRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetEmailTemplateForEditOutput { EmailTemplate = ObjectMapper.Map<CreateOrEditEmailTemplateDto>(emailTemplate) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditEmailTemplateDto input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_EmailTemplates_Create)]
        protected virtual async Task Create(CreateOrEditEmailTemplateDto input)
        {
            var emailTemplate = ObjectMapper.Map<EmailTemplate>(input);

            await _emailTemplateRepository.InsertAsync(emailTemplate);

        }

        [AbpAuthorize(AppPermissions.Pages_EmailTemplates_Edit)]
        protected virtual async Task Update(CreateOrEditEmailTemplateDto input)
        {
            var emailTemplate = await _emailTemplateRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, emailTemplate);

        }

        [AbpAuthorize(AppPermissions.Pages_EmailTemplates_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _emailTemplateRepository.DeleteAsync(input.Id);
        }

    }


}