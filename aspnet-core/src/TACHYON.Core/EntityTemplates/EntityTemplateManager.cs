using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Validation;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Dto;
using JsonSerializer = System.Text.Json.JsonSerializer;

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


        public virtual async Task<string> Create(CreateOrEditEntityTemplateInputDto input)
        {
            var createdEntityTemplate = ObjectMapper.Map<EntityTemplate>(input); 
            await CheckIfAlreadyExist(createdEntityTemplate);
            await SetSavedEntity(createdEntityTemplate);
            var templateId = await _templateRepository.InsertAndGetIdAsync(createdEntityTemplate);
           return templateId.ToString() ;
        }
        
        public virtual async Task<string> Update(CreateOrEditEntityTemplateInputDto input)
        {
            if (!input.Id.HasValue)
                throw new AbpValidationException(L("IdCanNotBeNullWhenUpdateEntity"));
            
            var updatedEntityTemplate = ObjectMapper.Map<EntityTemplate>(input);
            var oldEntityTemplate = await GetById(input.Id.Value);
            
           return ObjectMapper.Map(updatedEntityTemplate, oldEntityTemplate).Id.ToString();
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
                SavedEntityType.ShippingRequest => await GetShippingRequest(template.SavedEntityId),
                SavedEntityType.Trip => await GetTrip(template.SavedEntityId),
                _ => throw new ArgumentOutOfRangeException()
            };
            if (savedEntity == null)
                throw new EntityNotFoundException(L("EntityWithIdXIsNotFound",template.SavedEntityId));

            template.SavedEntity = SerializeEntityWithFormatting(savedEntity,template.EntityType);
        }

        private async Task<CreateOrEditShippingRequestTripDto> GetTrip(string savedEntityId)
        {
            var trip = await _tripRepository.GetAll().AsNoTracking()
                .Include(x=> x.RoutPoints).ThenInclude(x=> x.GoodsDetails)
                .Include(x=> x.RoutPoints).ThenInclude(x=> x.FacilityFk)
                .FirstOrDefaultAsync(x => x.Id.ToString().Equals(savedEntityId));
           return ObjectMapper.Map<CreateOrEditShippingRequestTripDto>(trip);
        }

        private async Task<CreateOrEditShippingRequestTemplateInputDto> GetShippingRequest(string savedEntityId)
        {
            var shippingRequest = await _shippingRequestRepository.GetAllIncluding(x=> x.ShippingRequestVases)
                .AsNoTracking().FirstOrDefaultAsync(x => x.Id.ToString().Equals(savedEntityId));
            return ObjectMapper.Map<CreateOrEditShippingRequestTemplateInputDto>(shippingRequest);
        }

        private static string SerializeEntityWithFormatting(object entity,SavedEntityType type)
        {
            var entityJson = JsonConvert.SerializeObject(entity, new JsonSerializerSettings()
                {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});

            var handledLoopEntity = type switch
            {
                SavedEntityType.ShippingRequest => JsonConvert.DeserializeObject(entityJson,typeof(CreateOrEditShippingRequestTemplateInputDto)),
                SavedEntityType.Trip => JsonConvert.DeserializeObject(entityJson,typeof(CreateOrEditShippingRequestTripDto)),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            return JsonSerializer.Serialize(handledLoopEntity,
                new JsonSerializerOptions() {PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
        }

        #endregion
    }
}