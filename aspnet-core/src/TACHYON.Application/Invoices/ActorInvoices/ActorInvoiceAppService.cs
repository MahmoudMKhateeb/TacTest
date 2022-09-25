using Abp.Application.Services;
using Abp.Domain.Repositories;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Features;
using TACHYON.Invoices.ActorInvoices.Dto;
using TACHYON.Invoices.Dto;

namespace TACHYON.Invoices.ActorInvoices
{
    public class ActorInvoiceAppService : TACHYONAppServiceBase ,IApplicationService
    {


        private readonly IRepository<ActorInvoice, long> _actorInvoiceRepository;

        public ActorInvoiceAppService(IRepository<ActorInvoice, long> actorInvoiceRepository)
        {
            _actorInvoiceRepository = actorInvoiceRepository;
        }


        public async Task<LoadResult> GetAll(string filter)

        {
            var query = _actorInvoiceRepository
                .GetAll()
                .ProjectTo<ActorInvoiceListDto>(AutoMapperConfigurationProvider)
                .AsNoTracking();
            if (!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                DisableTenancyFilters();
            }

            return await LoadResultAsync<ActorInvoiceListDto>(query, filter);
        }

    }
}
