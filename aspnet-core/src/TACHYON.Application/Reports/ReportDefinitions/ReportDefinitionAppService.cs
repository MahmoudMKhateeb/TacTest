using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.MultiTenancy;

namespace TACHYON.Reports.ReportDefinitions
{
    public class ReportDefinitionAppService : TACHYONAppServiceBase
    {
        private readonly IRepository<ReportDefinition> _reportDefinitionRepository;
        private readonly IRepository<Tenant> _tenantRepository;

        public ReportDefinitionAppService(IRepository<ReportDefinition> reportDefinitionRepository, IRepository<Tenant> tenantRepository)
        {
            _reportDefinitionRepository = reportDefinitionRepository;
            _tenantRepository = tenantRepository;
        }

        public async Task CreateOrEdit(CreateOrEditReportDefinitionDto input)
        {
            if (input.Id.HasValue)
            {
                await Update(input);
                return;
            }

            await Create(input);
        }

        protected virtual async Task Create(CreateOrEditReportDefinitionDto input)
        {

        }

        protected virtual async Task Update(CreateOrEditReportDefinitionDto input)
        {
            
        }

        public async Task<List<SelectItemDto>> GetCompanies(params int?[] tenantIds)
        {
           return await _tenantRepository.GetAll().Where(x => tenantIds.Contains(x.EditionId)).Select(x =>
                new SelectItemDto { Id = x.Id.ToString(), DisplayName = x.Name }).ToListAsync();
        }

    }
}
