using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.EmailTemplates.Dtos;
using TACHYON.Dto;

namespace TACHYON.EmailTemplates
{
    public interface IEmailTemplatesAppService : IApplicationService
    {
        Task<PagedResultDto<GetEmailTemplateForViewDto>> GetAll(GetAllEmailTemplatesInput input);

        Task<GetEmailTemplateForViewDto> GetEmailTemplateForView(int id);

        Task<GetEmailTemplateForEditOutput> GetEmailTemplateForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditEmailTemplateDto input);

        Task Delete(EntityDto input);

    }
}