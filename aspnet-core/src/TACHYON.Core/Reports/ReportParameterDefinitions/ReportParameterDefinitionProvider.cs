using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Cities;
using TACHYON.Dto;
using TACHYON.MultiTenancy;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingTypes;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TrucksTypes;

namespace TACHYON.Reports.ReportParameterDefinitions
{
    public class ReportParameterDefinitionProvider : TACHYONDomainServiceBase, IReportParameterDefinitionProvider
    {
        private readonly Dictionary<ReportType, List<StaticReportParameterDefinition>> _parameterDefinitions;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<TrucksType,long> _truckTypesRepository;
        private readonly IRepository<TransportType> _transportTypeRepository;
        private readonly IRepository<ShippingType> _shippingTypeRepository;
        private readonly IRepository<City> _cityRepository;

        public ReportParameterDefinitionProvider(
            IRepository<Tenant> tenantRepository,
            IRepository<TrucksType,long> truckTypesRepository,
            IRepository<TransportType> transportTypeRepository,
            IRepository<ShippingType> shippingTypeRepository,
            IRepository<City> cityRepository)
         {
             _parameterDefinitions = new Dictionary<ReportType, List<StaticReportParameterDefinition>>();
             RegisterStaticParameterDefinitions();
             _tenantRepository = tenantRepository;
             _truckTypesRepository = truckTypesRepository;
             _transportTypeRepository = transportTypeRepository;
             _shippingTypeRepository = shippingTypeRepository;
             _cityRepository = cityRepository;
         }


         // Register the Parameters Definitions of your Report Type
         // by using Register(ReportType.YourReportType, new StaticReportParameterDefinition{Name = "ParameterName", Type = typeof(ParameterType)});
         private void RegisterStaticParameterDefinitions()
         {
             #region Shared Parameters

             var shipperNameParameterDefinition = new StaticReportParameterDefinition
                 
             {
                 Name = ReportParameterNames.ShipperName,
                 Type = typeof(int?),
                 ListDataCallback = () => GetCompanies(ShipperEditionId),
                 ExpressionCallback = (args) =>
                     x =>  (x.ShippingRequestId.HasValue ? x.ShippingRequestFk.TenantId : x.ShipperTenantId) == int.Parse(args.ParameterValue)
             };
             var carrierNameParameterDefinition = new StaticReportParameterDefinition
             {
                 Name = ReportParameterNames.CarrierName,
                 Type = typeof(int?),
                 ListDataCallback = () => GetCompanies(CarrierEditionId),
                 ExpressionCallback = (args) => x => (x.ShippingRequestId.HasValue ? x.ShippingRequestFk.CarrierTenantId : x.CarrierTenantId) ==
                                                               int.Parse(args.ParameterValue) 
             };
             var truckTypeParameterDefinition = new StaticReportParameterDefinition
             {
                 Name = ReportParameterNames.TruckType, Type = typeof(long?), ListDataCallback = GetTruckTypes,
                 ExpressionCallback = (args) => x=> (x.ShippingRequestId.HasValue
                     ? x.ShippingRequestFk.TrucksTypeId
                     : x.AssignedTruckFk.TrucksTypeId) == long.Parse(args.ParameterValue)
             };
             var transportParameterDefinition = new StaticReportParameterDefinition
             {
                 Name = ReportParameterNames.TransportType,
                 Type = typeof(int?),
                 ListDataCallback = GetTransportTypes,
                 ExpressionCallback = (args) => x=> (x.ShippingRequestId.HasValue
                     ? x.ShippingRequestFk.TrucksTypeId
                     : x.AssignedTruckFk.TrucksTypeId) == long.Parse(args.ParameterValue)
             };
             var shippingTypeParameterDefinition = new StaticReportParameterDefinition
             {
                 Name = ReportParameterNames.ShippingType, Type = typeof(int?), ListDataCallback = GetShippingTypes,
                 ExpressionCallback = (args) => x=> (x.ShippingRequestId.HasValue ? x.ShippingRequestFk.ShippingTypeId : x.ShippingTypeId) ==
                                                              (ShippingTypeEnum)byte.Parse(args.ParameterValue)
             };
             var requestTypeParameterDefinition = new StaticReportParameterDefinition
             {
                 Name = ReportParameterNames.RequestType,
                 Type = typeof(ShippingRequestType?),
                 ListDataCallback = GetEnumAsList<ShippingRequestType>,
                 ExpressionCallback = (args) => x=> x.ShippingRequestId.HasValue && x.ShippingRequestFk.RequestType ==
                     (ShippingRequestType)byte.Parse(args.ParameterValue)
             };
             var routeTypeParameterDefinition = new StaticReportParameterDefinition
             {
                 Name = ReportParameterNames.RouteType,
                 Type = typeof(ShippingRequestRouteType?),
                 ListDataCallback = GetEnumAsList<ShippingRequestRouteType>,
                 ExpressionCallback = (args) => x=> (x.ShippingRequestId.HasValue ? x.ShippingRequestFk.RouteTypeId : x.RouteType) ==
                                                              (ShippingRequestRouteType)byte.Parse(args.ParameterValue)
             };
             var originParameterDefinition = new StaticReportParameterDefinition
             {
                 Name = ReportParameterNames.Origin, Type = typeof(int?), ListDataCallback = GetCities,
                 ExpressionCallback = (args) => x=> (x.ShippingRequestId.HasValue ? x.ShippingRequestFk.OriginCityId : x.OriginCityId) ==
                                                              int.Parse(args.ParameterValue)
             };
             var destinationParameterDefinition = new StaticReportParameterDefinition
             {
                 Name = ReportParameterNames.Destination, Type = typeof(int?), ListDataCallback = GetCities,
                 ExpressionCallback = (args) => x=> (x.ShippingRequestId.HasValue
                         ? x.ShippingRequestFk.ShippingRequestDestinationCities
                         : x.ShippingRequestDestinationCities)
                     .Any(c => c.CityId == int.Parse(args.ParameterValue))
             };
             var isPodSubmittedParameterDefinition = new StaticReportParameterDefinition
             {
                 Name = ReportParameterNames.IsPodSubmitted, Type = typeof(bool?),
                 ExpressionCallback = (args) => x=> x.RoutPoints != null && x.RoutPoints.Any() && x.RoutPoints
                     .Where(p => p.PickingType == PickingType.Dropoff).All(p =>
                         p.IsPodUploaded == bool.Parse(args.ParameterValue))
             };
             
             var invoiceIssuedParameterDefinition = new StaticReportParameterDefinition
             {
                 Name = ReportParameterNames.InvoiceIssued, Type = typeof(bool?),
                 ExpressionCallback = (args) => x=> bool.Parse(args.ParameterValue) && ((args.IsCarrier && x.IsCarrierHaveInvoice) || (args.IsShipper && x.IsShipperHaveInvoice))
             };
             var deliveryDateParameterDefinition = new StaticReportParameterDefinition
             {
                 Name = ReportParameterNames.DeliveryDate, Type = typeof(DateTime?),
                 ExpressionCallback = (args) => x=> x.EndWorking.HasValue && DateTime.Parse(args.ParameterValue).Date == x.EndWorking.Value.Date
             };
             
             var invoiceStatusParameterDefinition = new StaticReportParameterDefinition
             {
                 Name = ReportParameterNames.InvoiceStatus,
                 Type = typeof(ReportInvoiceStatus?),
                 ListDataCallback = GetEnumAsList<ReportInvoiceStatus>,
                 ExpressionCallback = (args) => x=> (ReportInvoiceStatus.InvoiceIssued == (ReportInvoiceStatus)int.Parse(args.ParameterValue) && ((args.IsCarrier && x.IsCarrierHaveInvoice) || (args.IsShipper && x.IsShipperHaveInvoice)))
                 || (ReportInvoiceStatus.InvoiceNotIssued == (ReportInvoiceStatus)int.Parse(args.ParameterValue) && ((args.IsCarrier && !x.IsCarrierHaveInvoice) || (args.IsShipper && !x.IsShipperHaveInvoice)))
             };             
             var podStatusParameterDefinition = new StaticReportParameterDefinition
             {
                 Name = ReportParameterNames.PodStatus,
                 Type = typeof(ReportPodStatus?),
                 ListDataCallback = GetEnumAsList<ReportPodStatus>,
                 ExpressionCallback = (args) => x=> (ReportPodStatus.PodSubmitted == (ReportPodStatus)int.Parse(args.ParameterValue) && x.RoutPoints.Where(p=> p.PickingType == PickingType.Dropoff).All(p=> p.IsPodUploaded))
                 || (ReportPodStatus.PodNotSubmitted == (ReportPodStatus)int.Parse(args.ParameterValue) && x.RoutPoints.Where(p=> p.PickingType == PickingType.Dropoff).Any(p=> !p.IsPodUploaded))
             };

             #endregion
             
             #region Trip Details Report Parameters Definitions

             Register(ReportType.TripDetailsReport, shipperNameParameterDefinition);
             Register(ReportType.TripDetailsReport, carrierNameParameterDefinition);
             Register(ReportType.TripDetailsReport, truckTypeParameterDefinition);
             Register(ReportType.TripDetailsReport, transportParameterDefinition);
             Register(ReportType.TripDetailsReport, shippingTypeParameterDefinition);
             Register(ReportType.TripDetailsReport, requestTypeParameterDefinition);
             Register(ReportType.TripDetailsReport, routeTypeParameterDefinition);
             Register(ReportType.TripDetailsReport, originParameterDefinition);
             Register(ReportType.TripDetailsReport, destinationParameterDefinition);             
             Register(ReportType.TripDetailsReport, isPodSubmittedParameterDefinition );
             Register(ReportType.TripDetailsReport, invoiceIssuedParameterDefinition );
             Register(ReportType.TripDetailsReport, deliveryDateParameterDefinition);
             
             #endregion

             #region POD Performance Report Parameters Definitions

             Register(ReportType.PodPerformanceReport, originParameterDefinition);
             Register(ReportType.PodPerformanceReport, destinationParameterDefinition);
             Register(ReportType.PodPerformanceReport, transportParameterDefinition);
             Register(ReportType.PodPerformanceReport, carrierNameParameterDefinition);
             Register(ReportType.PodPerformanceReport, shipperNameParameterDefinition);
             Register(ReportType.PodPerformanceReport, shippingTypeParameterDefinition);
             Register(ReportType.PodPerformanceReport, invoiceStatusParameterDefinition);
             Register(ReportType.PodPerformanceReport, truckTypeParameterDefinition);
             Register(ReportType.PodPerformanceReport, podStatusParameterDefinition);

             #endregion

             #region Financial Report Parameter Definitions

             Register(ReportType.FinancialReport,shipperNameParameterDefinition);
             Register(ReportType.FinancialReport,carrierNameParameterDefinition);
             Register(ReportType.FinancialReport,truckTypeParameterDefinition);
             Register(ReportType.FinancialReport,transportParameterDefinition);

             #endregion
         }
         
         public IEnumerable<StaticReportParameterDefinition> GetParameterDefinitions(ReportType type)
         {
             return _parameterDefinitions.TryGetValue(type, out List<StaticReportParameterDefinition> definitions)
                 ? definitions
                 : Enumerable.Empty<StaticReportParameterDefinition>();
         }

         public bool IsParameterDefined(string parameterName, ReportType reportType)
         {
             return GetParameterDefinitions(reportType).Any(parameter => parameter.Name == parameterName);
         }         
         public bool AnyParameterNotDefined(ReportType reportType, params string[] parameterNames)
         {
             var parameters = GetParameterDefinitions(reportType);
             return !parameterNames.All(parameterName => parameters.Any(x => x.Name == parameterName));
         }

         public StaticReportParameterDefinition GetParameterDefinition(ReportType type, string parameterName)
         {
             return _parameterDefinitions[type].FirstOrDefault(x => x.Name == parameterName);
         }

         #region Helpers

         private void Register(ReportType type, StaticReportParameterDefinition parameterDefinition)
         {
             if (!_parameterDefinitions.ContainsKey(type))
             {
                 _parameterDefinitions[type] = new List<StaticReportParameterDefinition>();
             }
             
             _parameterDefinitions[type].Add(parameterDefinition);
         }

         #endregion

         #region Drop-downs

         private async Task<List<SelectItemDto>> GetCompanies(int editionId)
         {
             return await _tenantRepository.GetAll()
                 .Where(x => x.Edition.Id == editionId)
                 .Select(x => new SelectItemDto { Id = x.Id.ToString(), DisplayName = x.TenancyName }).ToListAsync();
         }
         
         private async Task<List<SelectItemDto>> GetTruckTypes()
         {
             var truckTypesList = await ( from truckType in _truckTypesRepository.GetAll().AsNoTracking()
                 let truckTypeTrans = truckType.Translations.FirstOrDefault(t=> t.Language.Contains(CultureInfo.CurrentUICulture.Name))
                 select new SelectItemDto()
                 {
                     Id = truckType.Id.ToString(),
                     DisplayName = truckTypeTrans != null ? truckTypeTrans.DisplayName : truckType.Key
                 }).ToListAsync();

             return truckTypesList;
         }

         private async Task<List<SelectItemDto>> GetTransportTypes()
         {
             return await _transportTypeRepository.GetAllIncluding(x => x.Translations).AsNoTracking()
                 .Select(p => new SelectItemDto
                 {
                     DisplayName = p.Translations.Any(x => x.Language.Contains(CultureInfo.CurrentUICulture.Name))
                         ? p.Translations.FirstOrDefault(x => x.Language.Contains(CultureInfo.CurrentUICulture.Name))
                             .DisplayName
                         : p.DisplayName,
                     Id = p.Id.ToString()
                 }).ToListAsync();
         }

         private async Task<List<SelectItemDto>> GetShippingTypes()
         {
             
             return await _shippingTypeRepository.GetAll()
                 .Select(x => new SelectItemDto()
                 {
                     Id = x.Id.ToString(),
                     DisplayName = x.Translations.Any(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name))
                         ? x.Translations.FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)).DisplayName
                         : x.DisplayName
                 }).ToListAsync();
         }

         private async Task<List<SelectItemDto>> GetCities()
         {
             return await _cityRepository.GetAll()
                 .Select(city => new SelectItemDto
                 {
                     Id = city.Id.ToString(),
                     DisplayName = city.Translations.Any(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name))
                         ? city.Translations.FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)).TranslatedDisplayName
                         : city.DisplayName
                 }).ToListAsync();
         }
         private Task<List<SelectItemDto>> GetEnumAsList<TEnum>()
             where TEnum : Enum
         {
             var baseType = Enum.GetUnderlyingType(typeof(TEnum));
             var list = Enum.GetValues(typeof(TEnum)).Cast<TEnum>()
                 .Select(x => new SelectItemDto
                 {
                     DisplayName = LocalizationSource.GetString(x.ToString()),
                     Id = Convert.ChangeType(x, baseType).ToString()
                 }).ToList();

             return Task.FromResult(list);
         }
         #endregion
    }
}