using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.EntityTemplates
{
    public class EntityTemplateManager : TACHYONDomainServiceBase
    {
        private readonly IRepository<EntityTemplate, long> _templateRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<ShippingRequestTrip> _tripRepository;

        public EntityTemplateManager(
            IRepository<EntityTemplate, long> templateRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IRepository<ShippingRequestTrip> tripRepository)
        {
            _templateRepository = templateRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _tripRepository = tripRepository;
        }


        public virtual async Task Create(CreateOrEditEntityTemplateInputDto input)
        {
            var createdEntityTemplate = ObjectMapper.Map<EntityTemplate>(input); 
            await CheckIfAlreadyExist(createdEntityTemplate);
            await SetSavedEntity(createdEntityTemplate);
            
            await _templateRepository.InsertAsync(createdEntityTemplate);
        }
        
        public virtual async Task Update(CreateOrEditEntityTemplateInputDto input)
        {
            if (!input.Id.HasValue)
                throw new AbpValidationException(L("IdCanNotBeNullWhenUpdateEntity"));
            
            var updatedEntityTemplate = ObjectMapper.Map<EntityTemplate>(input);
            var oldEntityTemplate = await GetById(input.Id.Value);
            
            ObjectMapper.Map(updatedEntityTemplate, oldEntityTemplate);
        }

        private async Task<EntityTemplate> GetById(long templateId)
        {
            var template = await _templateRepository.FirstOrDefaultAsync(templateId);

            if (template == null)
                throw new EntityNotFoundException(L("TemplateWithIdXIsNotFound", templateId));
            return template;
        }

        #region Helpers

        private async Task CheckIfAlreadyExist(EntityTemplate template)
        {
            if (template.SavedEntityId.IsNullOrEmpty()) return;

            var isExist = await _templateRepository.GetAll()
                .Where(x => x.EntityType == template.EntityType)
                .AnyAsync(x => x.SavedEntityId.Equals(template.SavedEntityId));

            if (isExist)
                throw new AbpValidationException(L("ThisEntityAlreadySavedBefore"));

        }

        private async Task SetSavedEntity(EntityTemplate template)
        {
            if (template.SavedEntityId.IsNullOrEmpty() || !template.SavedEntity.IsNullOrEmpty()) return;

            CurrentUnitOfWork.DisableFilter(nameof(IHasIsDrafted));
            
            object savedEntity = template.EntityType switch
            {
                SavedEntityType.ShippingRequest => await _shippingRequestRepository.GetAll().AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id.ToString().Equals(template.SavedEntityId)),
                SavedEntityType.Trip => await _tripRepository.GetAll().AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id.ToString().Equals(template.SavedEntityId)),
                _ => throw new ArgumentOutOfRangeException()
            };
            if (savedEntity == null)
                throw new EntityNotFoundException(L("EntityWithIdXIsNotFound",template.SavedEntityId));

            template.SavedEntity = JsonSerializer.Serialize(savedEntity,
                new JsonSerializerOptions() {PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
        }

        #endregion
    }
}