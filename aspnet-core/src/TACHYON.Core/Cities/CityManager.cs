using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Polygons;

namespace TACHYON.Cities
{
    public class CityManager : TACHYONDomainServiceBase
    {

        private readonly IRepository<City> _cityRepository;

        public CityManager(IRepository<City> cityRepository)
        {
            _cityRepository = cityRepository;
        }


        public async Task<string> ImportAllPolygonsIntoCities(List<CityPolygon> polygonsList)
        {
            var failedImportedCityPolygon = "";
            CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant);
            foreach (CityPolygon polygon in polygonsList)
            {
                try
                {
                    var polygonJson = JsonConvert.SerializeObject(polygon);
                    var cityId = int.Parse(polygon.Properties.CityId);
                    
                    _cityRepository.Update(cityId, x => x.Polygon = polygonJson);
                }
                catch
                {
                    failedImportedCityPolygon+= polygon.Properties.CityId+",";
                }
            }

            return failedImportedCityPolygon;
        }
        
        
    }
}