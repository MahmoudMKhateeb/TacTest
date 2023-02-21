﻿using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Abp.Threading;
using Microsoft.AspNetCore.Server.IISIntegration;
using System;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;
using TACHYON.Configuration;
using TACHYON.MultiTenancy;

namespace TACHYON
{
    public abstract class TACHYONDomainServiceBase : DomainService
    {
        /* Add your common members for all your domain services. */
        protected TACHYONDomainServiceBase()
        {
            LocalizationSourceName = TACHYONConsts.LocalizationSourceName;
        }

        protected int TachyonEditionId =>
            Convert.ToInt32(SettingManager.GetSettingValue(AppSettings.Editions.TachyonEditionId));

        protected int ShipperEditionId =>
            Convert.ToInt32(SettingManager.GetSettingValue(AppSettings.Editions.ShipperEditionId));

        protected int CarrierEditionId =>
            Convert.ToInt32(SettingManager.GetSettingValue(AppSettings.Editions.CarrierEditionId));

        protected decimal TaxVat =>
            Convert.ToDecimal(SettingManager.GetSettingValue(AppSettings.HostManagement.TaxVat));



        protected virtual void DisableTenancyFilters()
        {
            CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant);
        }
        protected virtual void DisableDraftedFilter()
        {
            CurrentUnitOfWork.DisableFilter("IHasIsDrafted");
        }

    }
}