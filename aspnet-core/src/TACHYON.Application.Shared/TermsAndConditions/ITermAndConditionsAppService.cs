using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.TermsAndConditions.Dtos;
using TACHYON.Dto;


namespace TACHYON.TermsAndConditions
{
    public interface ITermAndConditionsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetTermAndConditionForViewDto>> GetAll(GetAllTermAndConditionsInput input);

        Task<GetTermAndConditionForViewDto> GetTermAndConditionForView(int id);

		Task<GetTermAndConditionForEditOutput> GetTermAndConditionForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditTermAndConditionDto input);

		Task Delete(EntityDto input);

		
    }
}