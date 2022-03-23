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
using System.Linq.Dynamic.Core;
using System.Text.Json;
using System.Threading.Tasks;
using TACHYON.AddressBook;
using TACHYON.Dto;
using TACHYON.Goods.GoodCategories;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
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
        private readonly IRepository<Facility,long> _facilityRepository;
        private readonly IRepository<GoodCategory> _goodsCategoryRepository;

        public EntityTemplateManager(
            IRepository<EntityTemplate, long> templateRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IRepository<ShippingRequestTrip> tripRepository,
            IRepository<Facility, long> facilityRepository,
            IRepository<GoodCategory> goodsCategoryRepository)
        {
            _templateRepository = templateRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _tripRepository = tripRepository;
            _facilityRepository = facilityRepository;
            _goodsCategoryRepository = goodsCategoryRepository;
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
            
            
            var oldEntityTemplate = await GetById(input.Id.Value);

            var updatedEntityTemplate = ObjectMapper.Map(input, oldEntityTemplate);

            await SetSavedEntity(updatedEntityTemplate);
            
            await _templateRepository.UpdateAsync(updatedEntityTemplate);

            return updatedEntityTemplate.Id.ToString();
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
                SavedEntityType.ShippingRequestTemplate => await GetShippingRequest(template.SavedEntityId),
                SavedEntityType.TripTemplate => await GetTrip(template.SavedEntityId),
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
                .Include(x=> x.ShippingRequestTripVases)
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
                SavedEntityType.ShippingRequestTemplate => JsonConvert.DeserializeObject(entityJson,typeof(CreateOrEditShippingRequestTemplateInputDto)),
                SavedEntityType.TripTemplate => JsonConvert.DeserializeObject(entityJson,typeof(CreateOrEditShippingRequestTripDto)),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            return JsonSerializer.Serialize(handledLoopEntity,
                new JsonSerializerOptions() {PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
        }

        
        /// <summary>
        /// Get All Trip Templates That Can Used To Specific Shipping Request
        /// </summary>
        /// <param name="tripTemplates"></param>
        /// <param name="srId"></param>
        /// <returns></returns>
        public async Task<List<SelectItemDto>> FilterTripTemplatesByParentEntity(List<EntityTemplate> tripTemplates,string srId)
        {
            var shippingRequest = await _shippingRequestRepository.GetAll().AsNoTracking()
                .Where(x => x.Id.ToString().Equals(srId))
                .Select(x => new
                {
                    RoutType = x.RouteTypeId, 
                    x.GoodCategoryId,
                    SourceCityId = x.OriginCityId, 
                    x.DestinationCityId,
                    x.NumberOfDrops
                }).FirstOrDefaultAsync();

            if (shippingRequest == null) throw new UserFriendlyException(L("ThereIsNoShippingRequest"));

            var templatesList = ToTripTemplateDropdownItem(tripTemplates);

            var filteredByRoutTypeItems = shippingRequest.RoutType switch
            {
                ShippingRequestRouteType.SingleDrop => (from item in templatesList
                    where item.Trip?.RoutPoints?.Count(x=> x.PickingType == PickingType.Dropoff) == 1 select item),
                ShippingRequestRouteType.MultipleDrops => (from item in templatesList
                    let pointsCount = item.Trip?.RoutPoints?.Count(x=> x.PickingType == PickingType.Dropoff)
                    where pointsCount > 1 && pointsCount <= shippingRequest.NumberOfDrops
                    select item),
                _ => new List<TripTemplateDropdownItem>()
            };

            var filteredByGoodsCategoryItems = (from item in filteredByRoutTypeItems
                let dropOffPoints = item.Trip.RoutPoints.Where(x => x.PickingType == PickingType.Dropoff)
                where dropOffPoints.All(point => (from goodDetail in point.GoodsDetailListDto
                        from subGoodCategory in _goodsCategoryRepository.GetAll()
                            .Where(g => g.Id == goodDetail.GoodCategoryId).DefaultIfEmpty()
                        where subGoodCategory != null select subGoodCategory)
                        .All(g => g.FatherId == shippingRequest.GoodCategoryId)) select item);

            var matchesOriginAndDestinationItems = (from template in filteredByGoodsCategoryItems
                join originFacility in _facilityRepository.GetAll().AsNoTracking()
                    on template.Trip.OriginFacilityId equals originFacility.Id
                join destinationFacility in _facilityRepository.GetAll().AsNoTracking()
                    on template.Trip.DestinationFacilityId equals destinationFacility.Id
                    where originFacility.CityId == shippingRequest.SourceCityId 
                && destinationFacility.CityId == shippingRequest.DestinationCityId
                select new SelectItemDto() {DisplayName = template.TemplateName, Id = template.Id.ToString()});
            
            return matchesOriginAndDestinationItems.ToList();
        }

        private static List<TripTemplateDropdownItem> ToTripTemplateDropdownItem(List<EntityTemplate> tripTemplates)
        {
            var templatesList = new List<TripTemplateDropdownItem>();
            foreach (var template in tripTemplates)
            {
                var item = new TripTemplateDropdownItem() {TemplateName = template.TemplateName, Id = template.Id};

                try
                {
                    item.Trip = JsonConvert.DeserializeObject<CreateOrEditShippingRequestTripDto>(template.SavedEntity);
                }
                catch
                {
                    continue;
                }

                templatesList.Add(item);
            }

            return templatesList;
        }

        #endregion
    }
}