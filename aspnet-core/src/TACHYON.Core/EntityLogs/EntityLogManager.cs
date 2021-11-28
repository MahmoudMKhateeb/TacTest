using Abp.Domain.Entities.Auditing;
using Abp.Domain.Repositories;
using Abp.EntityHistory;
using Abp.UI;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Extension;
using TACHYON.Routs.RoutPoints;
using TACHYON.SmartEnums;

namespace TACHYON.EntityLogs
{
    // todo Add EntityLogManager interface

    public class EntityLogManager : TACHYONDomainServiceBase
    {
        private readonly IEntitySnapshotManager _snapshotManager;
        private readonly IEntityChangeSetReasonProvider _reasonProvider;
        private readonly IRepository<EntityLog, Guid> _logRepository;
        private readonly IRepository<EntityChange, long> _lookupEntityChangeRepository;
        private readonly IRepository<EntityChangeSet, long> _lookupChangeSetRepository;

        public EntityLogManager(IEntitySnapshotManager snapshotManager,
            IRepository<EntityLog, Guid> logRepository, IEntityChangeSetReasonProvider reasonProvider,
            IRepository<EntityChange, long> lookupEntityChangeRepository,
            IRepository<EntityChangeSet, long> lookupChangeSetRepository)
        {
            _snapshotManager = snapshotManager;
            _logRepository = logRepository;
            _reasonProvider = reasonProvider;
            _lookupEntityChangeRepository = lookupEntityChangeRepository;
            _lookupChangeSetRepository = lookupChangeSetRepository;
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
                    {tenantId,changeSet.TenantId},
                    {userId,changeSet.UserId},
                    {reason,changeSet.Reason},

                }).FirstOrDefaultAsync();

            EntityLogTransaction logTransaction;
            try
            {
                logTransaction = SmartEnum.FromName<EntityLogTransaction>(additionalLogData[reason].ToString());
            }
            catch (InvalidOperationException e)
            { // We Need To Handle The Exception And Get The Default Value
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
                Data = FormatChangedPropertiesData(entityChange.PropertyChanges, Type.GetType(entityChange.EntityTypeFullName))
            };

            await _logRepository.InsertAsync(log);

            await CurrentUnitOfWork.SaveChangesAsync();

        }

        private static string FormatChangedPropertiesData(IEnumerable<EntityPropertyChange> propertyChanges, Type entityType)
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
                var mNewValue = entityType.AssemblyQualifiedName.GetStringOfPropertyValue(propertyChange.PropertyName, propertyChange.NewValue);
                var mOriginalValue = entityType.AssemblyQualifiedName.GetStringOfPropertyValue(propertyChange.PropertyName, propertyChange.OriginalValue);
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



    }

}