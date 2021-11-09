using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.EntityHistory;
using Abp.Linq.Extensions;
using Abp.Runtime.Validation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;
using TACHYON.EntityLogs.Dto;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.EntityLogs
{
    // todo add permission here
    [AbpAuthorize()]
    public class EntityLogAppService : TACHYONAppServiceBase
    {
        private readonly EntityLogManager _logManager;
        private readonly IRepository<User, long> _userRepository;

        public EntityLogAppService(EntityLogManager logManager, IRepository<User, long> userRepository)
        {
            _logManager = logManager;
            _userRepository = userRepository;
        }


        public virtual async Task<PagedResultDto<EntityLogListDto>> GetAllEntityLogs(GetAllEntityLogInput input)
            => await GetPagedAndFilteredEntityLogs(input);

        private async Task<PagedResultDto<EntityLogListDto>> GetPagedAndFilteredEntityLogs(GetAllEntityLogInput input)
        {
            DisableTenancyFilters(); // todo remove it asap
            //  Important ==> This is not The final workaround 
            IQueryable<EntityLog> logs;
            switch (input.EntityType)
            {
                case EntityLogType.RoutPoint:
                    logs = _logManager.GetAllEntityLogs<RoutPoint, long>(typeof(RoutPoint).ToString(), input.EntityId);
                    break;
                case EntityLogType.ShippingRequestTrip:
                    logs = _logManager.GetAllEntityLogs<ShippingRequestTrip, int>(typeof(ShippingRequestTrip).ToString(), input.EntityId);
                    break;
                case EntityLogType.ShippingRequest:
                    logs = _logManager.GetAllEntityLogs<RoutPoint, long>(typeof(RoutPoint).ToString(), input.EntityId);
                    break;
                case EntityLogType.ShippingRequestPriceOffer:
                    logs = _logManager.GetAllEntityLogs<RoutPoint, long>(typeof(RoutPoint).ToString(), input.EntityId);
                    break;
                default:
                    throw new AbpValidationException("Don't Play With Me...Go To Hell"); // Test Msg todo add localization here
            }

            var entityLogs = await logs.PageBy(input).ToListAsync();

            return new PagedResultDto<EntityLogListDto>()
            {
                Items = await ToEntityLogListDto<RoutPoint>(entityLogs),
                TotalCount = await logs.CountAsync()
            };
        }

        #region Helpers

        // Here I passed The Entity logs as List of EntityLog Not As IEnumerable
        // For More Details See https://www.jetbrains.com/help/resharper/PossibleMultipleEnumeration.html

        private async Task<List<EntityLogListDto>> ToEntityLogListDto<TEntity>(List<EntityLog> entityLogs)
        {
            List<EntityLogListDto> logDtos = new List<EntityLogListDto>();

            // Don't Set those list null garbage cleaner will remove it 
            var users = entityLogs.Select(x => x.CreatorUserId);

            var userNames = await _userRepository.GetAll()
                .Where(x => users.Any(i => i == x.Id)) // No Need To Null Check
                .ToDictionaryAsync(x => x.Id, y => y.UserName);


            foreach (EntityLog log in entityLogs)
            {
                var dto = new EntityLogListDto()
                {
                    Id = log.Id,
                    Transaction = L(log.LogTransaction.Transaction),
                    ModificationTime = log.CreationTime,
                    ModifierUserId = log.CreatorUserId,
                    ChangesData = log.Data,
                    ModifierTenantId = log.TenantId
                };

                if (log.CreatorUserId != null)
                    // Can't Use This Code `await GetUserData(log.CreatorUserId.Value, x => x.UserName)`
                    // Here We Are in Foreach loop ... So I Get All Data in One Request Before loop Begin (Line 80-87)
                    dto.ModifierUserName = userNames[log.CreatorUserId.Value];


                logDtos.Add(dto);
            }

            return logDtos;
        }



        private async Task<TSource> GetUserData<TSource>(long userId, Expression<Func<User, TSource>> selector)
        {
            DisableTenancyFilters();
            return await _userRepository.GetAll().Where(x => x.Id == userId)
                .Select(selector).FirstOrDefaultAsync();
        }

        #endregion
    }
}