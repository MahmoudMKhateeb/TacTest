using Abp.Application.Features;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Runtime.Validation;
using Abp.UI.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using TACHYON.Configuration;
using TACHYON.Invoices.PaymentMethods;
using TACHYON.Invoices.Periods;
using TACHYON.MultiTenancy;
using TACHYON.PriceOffers;

namespace TACHYON.Features
{
    public class AppFeatureProvider : FeatureProvider
    {
        private IRepository<InvoicePeriod> _PeriodRepository;
        private IRepository<Tenant> _tenantRepository;
        private readonly IRepository<InvoicePaymentMethod> _invoicePaymentMethodRepository;
        public ISettingManager SettingManager { get; set; }


        public AppFeatureProvider(IRepository<InvoicePeriod> periodRepository, IRepository<InvoicePaymentMethod> invoicePaymentMethodRepository, IRepository<Tenant> tenantRepository)
        {
            _PeriodRepository = periodRepository;
            _invoicePaymentMethodRepository = invoicePaymentMethodRepository;
            _tenantRepository = tenantRepository;
        }

        [UnitOfWork]
        public override void SetFeatures(IFeatureDefinitionContext context)
        {

            context.Create(
                AppFeatures.MaxUserCount,
                "0", //0 = unlimited
                L("MaximumUserCount"),
                L("MaximumUserCount_Description"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue))
            )[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
            {
                ValueTextNormalizer = value => value == "0" ? L("Unlimited") : new FixedLocalizableString(value),
                IsVisibleOnPricingTable = true
            };

            #region ######## Example Features - You can delete them #########

            //context.Create("TestTenantScopeFeature", "false", L("TestTenantScopeFeature"), scope: FeatureScopes.Tenant);
            //context.Create("TestEditionScopeFeature", "false", L("TestEditionScopeFeature"), scope: FeatureScopes.Edition);

            //context.Create(
            //    AppFeatures.TestCheckFeature,
            //    defaultValue: "false",
            //    displayName: L("TestCheckFeature"),
            //    inputType: new CheckboxInputType()
            //)[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
            //{
            //    IsVisibleOnPricingTable = true,
            //    TextHtmlColor = value => value == "true" ? "#5cb85c" : "#d9534f"
            //};

            //context.Create(
            //    AppFeatures.TestCheckFeature2,
            //    defaultValue: "true",
            //    displayName: L("TestCheckFeature2"),
            //    inputType: new CheckboxInputType()
            //)[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
            //{
            //    IsVisibleOnPricingTable = true,
            //    TextHtmlColor = value => value == "true" ? "#5cb85c" : "#d9534f"
            //};

            #endregion
            //---Y
            #region ######## Tachyon features #########

            //var shipperFeature1 = context.Create(
            //    AppFeatures.Shipper,
            //    "false",
            //    L("ShipperFeature"), // todo add localization here
            //    inputType: new CheckboxInputType()
            //)[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
            //{
            //    IsVisibleOnPricingTable = true,
            //    TextHtmlColor = value => value == "true" ? "#c300ff" : "#d9534f"
            //};


            var shipperFeature = context.Create(
                AppFeatures.Shipper,
                "false",
                L("ShipperFeature"),
                inputType: new CheckboxInputType()
            );

            var carrierFeature = context.Create(
                        AppFeatures.Carrier,
                        "false",
                        L("CarrierFeature"), // todo add localization here
                        inputType: new CheckboxInputType()
                    );

            var periods = _PeriodRepository.GetAll().Where(p => p.Enabled == true).ToList();

            shipperFeature.CreateChildFeature(
                AppFeatures.ShipperCreditLimit,
                "false",
                L(AppFeatures.ShipperCreditLimit),
                inputType: new SingleLineStringInputType());
            if (periods != null && periods.Count > 0)
            {

                LocalizableComboboxItem[] ShipperPeriods = periods.Where(x => x.Enabled).Select(i => new LocalizableComboboxItem(i.Id.ToString(), L(i.DisplayName))).ToArray();
                shipperFeature.CreateChildFeature(
                          AppFeatures.ShipperPeriods,
                          defaultValue: "false",
                          displayName: L(AppFeatures.ShipperPeriods),
                          inputType: new ComboboxInputType(
                              new StaticLocalizableComboboxItemSource(ShipperPeriods)
                          )
                        );

                var Carrierperiods = periods.Where(x => x.Enabled && !x.ShipperOnlyUsed).Select(i => new LocalizableComboboxItem(i.Id.ToString(), L(i.DisplayName))).ToArray();
                if (Carrierperiods != null && Carrierperiods.Length > 0)
                {
                    carrierFeature.CreateChildFeature(
                                  AppFeatures.CarrierPeriods,
                                  defaultValue: "false",
                                  displayName: L(AppFeatures.CarrierPeriods),
                                  inputType: new ComboboxInputType(
                                      new StaticLocalizableComboboxItemSource(Carrierperiods)
                                  )
                                );
                }

            }
            var invoicePaymentMethodsCrarrier = _invoicePaymentMethodRepository.GetAll()
               .Select(x => new LocalizableComboboxItem(x.Id.ToString(), L(x.DisplayName))).ToArray();
            if (invoicePaymentMethodsCrarrier.Length > 0)
            {
                carrierFeature.CreateChildFeature(
                    AppFeatures.InvoicePaymentMethodCrarrier,
                    "false",
                    L("InvoicePaymentMethod"),
                    inputType: new ComboboxInputType(new StaticLocalizableComboboxItemSource
                        (invoicePaymentMethodsCrarrier)));
            }




            var broker = context.Create(
                AppFeatures.Broker,
                "false",
                L("BrokerFeature"), // todo add localization here
                inputType: new CheckboxInputType()
            )[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
            {
                IsVisibleOnPricingTable = true,
                TextHtmlColor = value => value == "true" ? "#c300ff" : "#d9534f"
            };

            var receiver = context.Create(
                AppFeatures.Receiver,
                "false",
                L("ReceiverFeature"), // todo add localization here
                inputType: new CheckboxInputType()
            )[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
            {
                IsVisibleOnPricingTable = true,
                TextHtmlColor = value => value == "true" ? "#c300ff" : "#d9534f"
            };

            var tachyonDealer = context.Create(
                AppFeatures.TachyonDealer,
                "false",
                L("TachyonDealerFeature"), // todo add localization here
                inputType: new CheckboxInputType()
            );



            var shippingRequest = context.Create(
                AppFeatures.ShippingRequest,
                "false",
                L("shippingRequest"), // todo add localization here
                inputType: new CheckboxInputType()
            )[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
            {
                IsVisibleOnPricingTable = true,
                TextHtmlColor = value => value == "true" ? "#c300ff" : "#d9534f"
            };


            //<see href="https://tachyonhub.atlassian.net/browse/TAC-1336">here</see>
            var marketPlace = context.Create(
                AppFeatures.MarketPlace,
                "true",
                L("MarketPlace"), // todo add localization here
                inputType: new CheckboxInputType()
            );

            var OffersMarketPlace = context.Create(
                AppFeatures.OffersMarketPlace,
                "false",
                L("OffersMarketPlace"), // todo add localization here
                inputType: new CheckboxInputType()
            )[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata { IsVisibleOnPricingTable = true, TextHtmlColor = value => value == "true" ? "#c300ff" : "#d9534f" };

            var SendDirectRequest = context.Create(
                AppFeatures.SendDirectRequest,
                "false",
                L("SendDirectRequest"), // todo add localization here
                inputType: new CheckboxInputType()
            );

            var sendTachyonDealShippingRequest = context.Create(
            AppFeatures.SendTachyonDealShippingRequest,
            "false",
            L("SendTachyonDealShippingRequest"), // todo add localization here
            inputType: new CheckboxInputType()
            )[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
            {
                IsVisibleOnPricingTable = false,
                TextHtmlColor = value => value == "true" ? "#c300ff" : "#d9534f"
            };

            ///*Invoices*/


            #endregion
            //---Y

            #region Commission

            List<LocalizableComboboxItem> CommissionTypes = new List<LocalizableComboboxItem>();
            foreach (byte i in Enum.GetValues(typeof(PriceOfferCommissionType)))
            {
                if (i != 1 && i != 2) continue;
                CommissionTypes.Add(new LocalizableComboboxItem(i.ToString(), L(Enum.GetName(typeof(PriceOfferCommissionType), i))));
            }

            var invoicePaymentMethods = _invoicePaymentMethodRepository.GetAll()
                .Select(x => new LocalizableComboboxItem(x.Id.ToString(), L(x.DisplayName))).ToArray();
            if (invoicePaymentMethods.Length > 0)
            {
                shipperFeature.CreateChildFeature(
                    AppFeatures.InvoicePaymentMethod,
                    "false",
                    L("InvoicePaymentMethod"),
                    inputType: new ComboboxInputType(new StaticLocalizableComboboxItemSource
                        (invoicePaymentMethods)));
            }


            shipperFeature.CreateChildFeature(
                AppFeatures.TripCommissionType,
                "false",
                L("TripCommissionType"),
                inputType: new ComboboxInputType(new StaticLocalizableComboboxItemSource(CommissionTypes.ToArray())));

            shipperFeature.CreateChildFeature(
                AppFeatures.TripCommissionPercentage,
                "false",
                L("TripCommissionPercentage"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));

            shipperFeature.CreateChildFeature(
                AppFeatures.TripCommissionValue,
                "false",
                L("TripCommissionValue"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));

            shipperFeature.CreateChildFeature(
                AppFeatures.TripMinValueCommission,
                "false",
                L("TripMinValueCommission"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));

            shipperFeature.CreateChildFeature(
                AppFeatures.VasCommissionType,
                "false",
                L("VasCommissionType"),
                inputType: new ComboboxInputType(new StaticLocalizableComboboxItemSource(CommissionTypes.ToArray())));

            shipperFeature.CreateChildFeature(
                AppFeatures.VasCommissionPercentage,
                "false",
                L("VasCommissionPercentage"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));

            shipperFeature.CreateChildFeature(
                AppFeatures.VasCommissionValue,
                "false",
                L("VasCommissionValue"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));

            shipperFeature.CreateChildFeature(
                AppFeatures.VasMinValueCommission,
                "false",
                L("VasMinValueCommission"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));


            shipperFeature.CreateChildFeature(
                AppFeatures.TachyonDealerTripCommissionType,
                "false",
                L("TachyonDealerTripCommissionType"),
                inputType: new ComboboxInputType(
                                      new StaticLocalizableComboboxItemSource(CommissionTypes.ToArray())
                                  )
                                );

            shipperFeature.CreateChildFeature(
                AppFeatures.TachyonDealerTripCommissionPercentage,
                "false",
                L("TachyonDealerTripCommissionPercentage"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));

            shipperFeature.CreateChildFeature(
                AppFeatures.TachyonDealerTripCommissionValue,
                "false",
                L("TachyonDealerTripCommissionValue"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));

            shipperFeature.CreateChildFeature(
                AppFeatures.TachyonDealerTripMinValueCommission,
                "false",
                L("TachyonDealerTripMinValueCommission"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));

            shipperFeature.CreateChildFeature(
                AppFeatures.TachyonDealerVasCommissionType,
                "false",
                L("TachyonDealerVasCommissionType"),
                inputType: new ComboboxInputType(new StaticLocalizableComboboxItemSource(CommissionTypes.ToArray())));

            shipperFeature.CreateChildFeature(
                AppFeatures.TachyonDealerVasCommissionPercentage,
                "false",
                L("TachyonDealerVasCommissionPercentage"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));

            shipperFeature.CreateChildFeature(
                AppFeatures.TachyonDealerVasCommissionValue,
                "false",
                L("TachyonDealerVasCommissionValue"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));

            shipperFeature.CreateChildFeature(
                AppFeatures.TachyonDealerVasMinValueCommission,
                "false",
                L("TachyonDealerVasMinValueCommission"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));





            //        #region Removed

            shipperFeature.CreateChildFeature(
                AppFeatures.BiddingCommissionPercentage,
                "false",
                L(AppFeatures.BiddingCommissionPercentage),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));

            shipperFeature.CreateChildFeature(
                AppFeatures.BiddingCommissionValue,
                "false",
                L(AppFeatures.BiddingCommissionValue),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));

            shipperFeature.CreateChildFeature(AppFeatures.BiddingMinValueCommission,
                "false",
                L(AppFeatures.BiddingMinValueCommission),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));



            shipperFeature.CreateChildFeature(
                AppFeatures.TachyonDealerCommissionType,
                "false",
                L("TachyonDealerCommissionType"),
                inputType: new ComboboxInputType(new StaticLocalizableComboboxItemSource(CommissionTypes.ToArray())));

            shipperFeature.CreateChildFeature(
    AppFeatures.TachyonDealerCommissionPercentage,
    "false",
    L("TachyonDealerCommissionPercentage"),
    inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));

            shipperFeature.CreateChildFeature(
                AppFeatures.TachyonDealerCommissionValue,
                "false",
                L("TachyonDealerCommissionValue"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));

            shipperFeature.CreateChildFeature(
                AppFeatures.TachyonDealerMinValueCommission,
                "false",
                L("TachyonDealerMinValueCommission"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));


            // direct request commission 
            shipperFeature.CreateChildFeature(
                AppFeatures.DirectRequestCommissionType,
                "false",
                L("DirectRequestCommissionType"),
                inputType: new ComboboxInputType(new StaticLocalizableComboboxItemSource(CommissionTypes.ToArray())));

            shipperFeature.CreateChildFeature(
                AppFeatures.DirectRequestCommissionPercentage,
                "false",
                L("DirectRequestCommissionPercentage"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, 100)));

            shipperFeature.CreateChildFeature(
                AppFeatures.DirectRequestCommissionValue,
                "false",
                L("DirectRequestCommissionValue"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));



            shipperFeature.CreateChildFeature(
                AppFeatures.DirectRequestCommissionMinValue,
                "false",
                L("DirectDirectRequestCommissionMinValue"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));



            shipperFeature.CreateChildFeature(
                AppFeatures.DirectRequestVasCommissionType,
                "false",
                L("DirectRequestVASCommissionType"),
                inputType: new ComboboxInputType(new StaticLocalizableComboboxItemSource(CommissionTypes.ToArray())));



            shipperFeature.CreateChildFeature(
                AppFeatures.DirectRequestVasCommissionPercentage,
                "false",
                L("DirectRequestVASCommissionPercentage"),
               inputType: new SingleLineStringInputType(new NumericValueValidator(0, 100)));



            shipperFeature.CreateChildFeature(
                AppFeatures.DirectRequestVasCommissionValue,
                "false",
                L("DirectRequestVASCommissionValue"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));


            shipperFeature.CreateChildFeature(
                AppFeatures.DirectRequestVasCommissionMinValue,
                "false",
                L("DirectRequestVASCommissionMinValue"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));



            #endregion

            var BayanIntegration = context.Create(
                AppFeatures.BayanIntegration,
                "true",
                L("BayanIntegration"),
                inputType: new CheckboxInputType()
            );

            var chatFeature = context.Create(
                AppFeatures.ChatFeature,
                "false",
                L("ChatFeature"),
                inputType: new CheckboxInputType()
            );

            chatFeature.CreateChildFeature(
                AppFeatures.TenantToTenantChatFeature,
                "false",
                L("TenantToTenantChatFeature"),
                inputType: new CheckboxInputType()
            );

            chatFeature.CreateChildFeature(
                AppFeatures.TenantToHostChatFeature,
                "false",
                L("TenantToHostChatFeature"),
                inputType: new CheckboxInputType()
            );


            var carrierEditionId = Convert.ToInt32(SettingManager.GetSettingValue(AppSettings.Editions.CarrierEditionId));
            ILocalizableComboboxItem[] tenants = _tenantRepository.GetAll()
                .Where(x=> x.EditionId == carrierEditionId)
                .Select(i => new LocalizableComboboxItem(i.Id.ToString(), L(i.TenancyName +" - "+i.AccountNumber))).ToArray();

          var saasFeature =   shipperFeature.CreateChildFeature(
                AppFeatures.Saas,
                defaultValue: "false",
                displayName: L(AppFeatures.Saas),
                inputType: new CheckboxInputType()
            );
                saasFeature.CreateChildFeature(
                    AppFeatures.SaasRelatedCarrier,
                    defaultValue: "false",
                    displayName: L(AppFeatures.SaasRelatedCarrier),
                    inputType: new ComboboxInputType(
                        new StaticLocalizableComboboxItemSource(tenants)
                    )
                );
            

        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, TACHYONConsts.LocalizationSourceName);
        }
    }
}