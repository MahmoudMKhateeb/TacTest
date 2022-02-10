using Abp.Dependency;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Repositories;
using Abp.EntityHistory;
using Abp.Events.Bus;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.EntityLogs.Dto;
using TACHYON.Extension;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestUpdates;
using TACHYON.SmartEnums;

namespace TACHYON.EntityLogs
{
    // todo Add EntityLogManager interface

    public class EntityLogManager : TACHYONDomainServiceBase
    {
        private readonly IRepository<EntityLog, Guid> _logRepository;
        private readonly IRepository<EntityChangeSet, long> _lookupChangeSetRepository;
        private readonly IEventBus _eventBus;
        private readonly IConfigurationProvider _autoMapperConfigurationProvider;

        public EntityLogManager(
            IRepository<EntityLog, Guid> logRepository,
            IRepository<EntityChangeSet, long> lookupChangeSetRepository,
            IEventBus eventBus)
        {
            _logRepository = logRepository;
            _lookupChangeSetRepository = lookupChangeSetRepository;
            _eventBus = eventBus;
            var mapper = IocManager.Instance.Resolve<IMapper>();
            _autoMapperConfigurationProvider = mapper.ConfigurationProvider;
        }
        // In Domain Service We Will Get Data As EntityLog Model
        // And in Application Service We Will Convert EntityLog Model to Dto it is Readable to Front-End  

        // to do Before User GetEntityLog You Must Set Transaction Value For Your EntityLogTransaction // Done
        // todo Publish Entity Log When Created // InProgress


        public async Task CreateEntityLog(EntityChange entityChange)
        {
            const string userId = "UserId", tenantId = "TenantId", reason = "Reason"; // Just Used As Flag

            DisableTenancyFilters();
            var additionalLogData = await (from changeSet in _lookupChangeSetRepository.GetAll()
                where changeSet.Id == entityChange.EntityChangeSetId
                select new Dictionary<string, object>()
                {
                    { tenantId, changeSet.TenantId }, { userId, changeSet.UserId }, { reason, changeSet.Reason },
                }).FirstOrDefaultAsync();

            EntityLogTransaction logTransaction;
            try
            {
                logTransaction = SmartEnum.FromName<EntityLogTransaction>(additionalLogData[reason].ToString());
            }
            catch (InvalidOperationException e)
            {
                // We Need To Handle The Exception And Get The Default Value
                logTransaction = EntityLogTransaction.DefaultLogTransaction;
            }

            var log = new EntityLog()
            {
                Core = entityChange.EntityTypeFullName,
                CoreId = entityChange.EntityId,
                CreationTime = DateTime.Now,
                ChangeTime = entityChange.ChangeTime,
                IsDeleted = false,
                CreatorUserId = (long?)additionalLogData[userId],
                TenantId = (int?)additionalLogData[tenantId],
                LogTransaction = logTransaction,
                Data = FormatChangedPropertiesData(entityChange.PropertyChanges,
                    Type.GetType(entityChange.EntityTypeFullName))
            };

           var logId = await _logRepository.InsertAndGetIdAsync(log);

           if (log.Core.Equals(typeof(ShippingRequest).ToString()))
           {
               var eventData = new UpdatedShippingRequestEventData()
               {
                   EntityLogId = logId, ShippingRequestId = long.Parse(log.CoreId)
               }; 
               await _eventBus.TriggerAsync(eventData);
           }

            await CurrentUnitOfWork.SaveChangesAsync();
        }

        private static string FormatChangedPropertiesData(IEnumerable<EntityPropertyChange> propertyChanges,
            Type entityType)
        {
            var data = new Dictionary<string, Dictionary<string, string>>();

            foreach (EntityPropertyChange propertyChange in propertyChanges)
            {
                var subData = new Dictionary<string, string>();

                var propertyInfo = entityType.GetProperty(propertyChange.PropertyName);
                if (propertyInfo == null || !propertyInfo.PropertyType.IsEnum)
                {
                    subData.Add(nameof(propertyChange.NewValue), propertyChange.NewValue);
                    subData.Add(nameof(propertyChange.OriginalValue), propertyChange.OriginalValue);
                    data.Add(propertyChange.PropertyName, subData);
                    continue;
                }

                var mNewValue =
                    entityType.AssemblyQualifiedName.GetStringOfPropertyValue(propertyChange.PropertyName,
                        propertyChange.NewValue);
                var mOriginalValue =
                    entityType.AssemblyQualifiedName.GetStringOfPropertyValue(propertyChange.PropertyName,
                        propertyChange.OriginalValue);
                subData.Add(nameof(propertyChange.NewValue), mNewValue);
                subData.Add(nameof(propertyChange.OriginalValue), mOriginalValue);
                data.Add(propertyChange.PropertyName, subData);
            }

            return JsonConvert.SerializeObject(data);
        }

        // Don't Use it In Foreach loop Plz..
        public IQueryable<EntityLog> GetAllEntityLogs<TEntity, TKey>(string coreType, string coreId)
            where TEntity : FullAuditedEntity<TKey>
            => _logRepository.GetAll().AsNoTracking()
                .Where(x => x.Core.Equals(coreType)
                            && x.CoreId.Equals(coreId))
                .OrderByDescending(x => x.CreationTime);
        public async Task<EntityLogListDto> GetEntityLogById(Guid logId)
            => await _logRepository.GetAll().AsNoTracking()
                .ProjectTo<EntityLogListDto>(_autoMapperConfigurationProvider)
                .FirstOrDefaultAsync();
    }
}