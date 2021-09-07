using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Features;
using TACHYON.MultiTenancy;
using TACHYON.Shipping.DirectRequests.Dto;
using TACHYON.Shipping.ShippingRequests.Dtos;

namespace TACHYON.Shipping.DirectRequests
{
    public class ShippingRequestDirectRequestManager : TACHYONDomainServiceBase
    {
        private readonly IFeatureChecker _featureChecker;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<TenantCarrier, long> _tenantCarrierRepository;


        public ShippingRequestDirectRequestManager(IFeatureChecker featureChecker, IRepository<Tenant> tenantRepository, IRepository<TenantCarrier, long> tenantCarrierRepository)
        {
            _featureChecker = featureChecker;
            _tenantRepository = tenantRepository;
            _tenantCarrierRepository = tenantCarrierRepository;
        }

        /// <summary>
        /// Get list of carrier for dropdown can send direct request
        /// </summary>
        /// <returns></returns>
        
        public async Task<List<CarriersForDropDownDto>> GetCarriersForDropDownByPermissionAsync()
        {
            List<CarriersForDropDownDto> list;
            if (await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                list =await _tenantRepository
                                .GetAll()
                                .AsNoTracking()
                                .Where(t => t.IsActive && t.Edition.DisplayName == TACHYONConsts.CarrierEdtionName)
                                .Select(r => new CarriersForDropDownDto
                                {
                                    Id = r.Id,
                                    DisplayName = r.TenancyName
                                }).ToListAsync();
            }
            else if (await _featureChecker.IsEnabledAsync(AppFeatures.Shipper) && await _featureChecker.IsEnabledAsync(AppFeatures.SendDirectRequest))
            {
                list =await _tenantCarrierRepository
                                .GetAll()
                                .AsNoTracking()
                                .Where(t => t.CarrierShipper.IsActive)
                                .Select(r => new CarriersForDropDownDto
                                {
                                    Id = r.CarrierTenantId,
                                    DisplayName = r.CarrierShipper.TenancyName
                                }).ToListAsync();
            }
            else
            {
                return null;
            }
            return list;
        }

    }
}
