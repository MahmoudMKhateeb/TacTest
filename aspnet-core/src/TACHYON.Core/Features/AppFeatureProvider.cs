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


        public AppFeatureProvider(IRepository<InvoicePeriod> periodRepository,
            IRepository<InvoicePaymentMethod> invoicePaymentMethodRepository,
            IRepository<Tenant> tenantRepository)
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

            var payFeature = context.Create(
                AppFeatures.Pay,
                "false",
                L("PayFeature"),
                inputType: new CheckboxInputType()
            );
            var receiptFeature = context.Create(
                        AppFeatures.Receipt,
                        "false",
                        L("ReceiptFeature"),
                        inputType: new CheckboxInputType()
                    );





           

            var broker = context.Create(
                AppFeatures.Broker,
                "false",
                L("BrokerFeature"), // todo add localization here
                inputType: new CheckboxInputType()
            )[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
            {
                IsVisibleOnPricingTable = true, TextHtmlColor = value => value == "true" ? "#c300ff" : "#d9534f"
            };

            var receiver = context.Create(
                AppFeatures.Receiver,
                "false",
                L("ReceiverFeature"), // todo add localization here
                inputType: new CheckboxInputType()
            )[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
            {
                IsVisibleOnPricingTable = true, TextHtmlColor = value => value == "true" ? "#c300ff" : "#d9534f"
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
                IsVisibleOnPricingTable = true, TextHtmlColor = value => value == "true" ? "#c300ff" : "#d9534f"
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
            )[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
            {
                IsVisibleOnPricingTable = true,
                TextHtmlColor = value => value == "true" ? "#c300ff" : "#d9534f"
            };

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
                IsVisibleOnPricingTable = false, TextHtmlColor = value => value == "true" ? "#c300ff" : "#d9534f"
            };


            ///*Invoices*/

            #endregion

            //---Y

            #region Commission










            //        #region Removed

            #region Pay




            var invoicePaymentMethods = _invoicePaymentMethodRepository.GetAll()
                .Select(x => new LocalizableComboboxItem(x.Id.ToString(), L(x.DisplayName))).ToArray();
            if (invoicePaymentMethods.Length > 0)
            {
                payFeature.CreateChildFeature(
                    AppFeatures.InvoicePaymentMethod,
                    "false",
                    L("InvoicePaymentMethod"),
                    inputType: new ComboboxInputType(new StaticLocalizableComboboxItemSource
                        (invoicePaymentMethods)));
            }





            var periods = _PeriodRepository.GetAll().Where(p => p.Enabled).ToList();

            if (periods.Count > 0)
            {
                LocalizableComboboxItem[] ShipperPeriods = periods
                    .Select(i => new LocalizableComboboxItem(i.Id.ToString(), L(i.DisplayName)))
                    .ToArray();

                payFeature.CreateChildFeature(
                    AppFeatures.ShipperPeriods,
                    defaultValue: "false",
                    displayName: L(AppFeatures.ShipperPeriods),
                    inputType: new ComboboxInputType(
                        new StaticLocalizableComboboxItemSource(ShipperPeriods)
                    )
                );


            }


            // crdit limmit 


            payFeature.CreateChildFeature(
                AppFeatures.ShipperCreditLimit,
                "false",
                L(AppFeatures.ShipperCreditLimit),
                inputType: new SingleLineStringInputType());



            //bidding
            var biddingFeature = payFeature.CreateChildFeature(
                   AppFeatures.Bidding,
                   "false",
                   L(AppFeatures.Bidding),
                   inputType: new CheckboxInputType()
                   );


            List<LocalizableComboboxItem> CommissionTypes = new List<LocalizableComboboxItem>();
            foreach (byte i in Enum.GetValues(typeof(PriceOfferCommissionType)))
            {
                if (i != 1 && i != 2) continue;
                CommissionTypes.Add(new LocalizableComboboxItem(i.ToString(),
                    L(Enum.GetName(typeof(PriceOfferCommissionType), i))));
            }

            biddingFeature.CreateChildFeature(
               AppFeatures.TripCommissionType,
               "false",
               L("TripCommissionType"),
               inputType: new ComboboxInputType(new StaticLocalizableComboboxItemSource(CommissionTypes.ToArray())));

            biddingFeature.CreateChildFeature(
                AppFeatures.TripCommissionPercentage,
                "false",
                L("TripCommissionPercentage"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));

            biddingFeature.CreateChildFeature(
                AppFeatures.TripCommissionValue,
                "false",
                L("TripCommissionValue"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));

            biddingFeature.CreateChildFeature(
                AppFeatures.TripMinValueCommission,
                "false",
                L("TripMinValueCommission"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));

            biddingFeature.CreateChildFeature(
                AppFeatures.VasCommissionType,
                "false",
                L("VasCommissionType"),
                inputType: new ComboboxInputType(new StaticLocalizableComboboxItemSource(CommissionTypes.ToArray())));

            biddingFeature.CreateChildFeature(
                AppFeatures.VasCommissionPercentage,
                "false",
                L("VasCommissionPercentage"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));

            biddingFeature.CreateChildFeature(
                AppFeatures.VasCommissionValue,
                "false",
                L("VasCommissionValue"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));

            biddingFeature.CreateChildFeature(
                AppFeatures.VasMinValueCommission,
                "false",
                L("VasMinValueCommission"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));

            // direct request commission 

            var createDirectRequestFeature = payFeature.CreateChildFeature(
                   AppFeatures.CreateDirectRequest,
                   "false",
                   L(AppFeatures.CreateDirectRequest),
                   inputType: new CheckboxInputType()
                   );


            createDirectRequestFeature.CreateChildFeature(
                AppFeatures.DirectRequestCommissionType,
                "false",
                L("DirectRequestCommissionType"),
                inputType: new ComboboxInputType(new StaticLocalizableComboboxItemSource(CommissionTypes.ToArray())));

            createDirectRequestFeature.CreateChildFeature(
                AppFeatures.DirectRequestCommissionPercentage,
                "false",
                L("DirectRequestCommissionPercentage"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, 100)));

            createDirectRequestFeature.CreateChildFeature(
                AppFeatures.DirectRequestCommissionValue,
                "false",
                L("DirectRequestCommissionValue"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));


            createDirectRequestFeature.CreateChildFeature(
                AppFeatures.DirectRequestCommissionMinValue,
                "false",
                L("DirectDirectRequestCommissionMinValue"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));


            createDirectRequestFeature.CreateChildFeature(
                AppFeatures.DirectRequestVasCommissionType,
                "false",
                L("DirectRequestVASCommissionType"),
                inputType: new ComboboxInputType(new StaticLocalizableComboboxItemSource(CommissionTypes.ToArray())));


            createDirectRequestFeature.CreateChildFeature(
                AppFeatures.DirectRequestVasCommissionPercentage,
                "false",
                L("DirectRequestVASCommissionPercentage"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, 100)));


            createDirectRequestFeature.CreateChildFeature(
                AppFeatures.DirectRequestVasCommissionValue,
                "false",
                L("DirectRequestVASCommissionValue"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));


            createDirectRequestFeature.CreateChildFeature(
                AppFeatures.DirectRequestVasCommissionMinValue,
                "false",
                L("DirectRequestVASCommissionMinValue"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));


            // TMS request 


            var createTmsRequestFeature = payFeature.CreateChildFeature(
                AppFeatures.CreateTmsRequest,
                "false",
                L(AppFeatures.CreateTmsRequest),
                inputType: new CheckboxInputType()
                );

            createTmsRequestFeature.CreateChildFeature(
               AppFeatures.TachyonDealerCommissionType,
               "false",
               L("TachyonDealerCommissionType"),
               inputType: new ComboboxInputType(new StaticLocalizableComboboxItemSource(CommissionTypes.ToArray())));

            createTmsRequestFeature.CreateChildFeature(
                AppFeatures.TachyonDealerCommissionPercentage,
                "false",
                L("TachyonDealerCommissionPercentage"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));

            createTmsRequestFeature.CreateChildFeature(
                AppFeatures.TachyonDealerCommissionValue,
                "false",
                L("TachyonDealerCommissionValue"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));

            createTmsRequestFeature.CreateChildFeature(
                AppFeatures.TachyonDealerMinValueCommission,
                "false",
                L("TachyonDealerMinValueCommission"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));


            createTmsRequestFeature.CreateChildFeature(
               AppFeatures.TachyonDealerTripCommissionType,
               "false",
               L("TachyonDealerTripCommissionType"),
               inputType: new ComboboxInputType(
                   new StaticLocalizableComboboxItemSource(CommissionTypes.ToArray())
               )
           );

            createTmsRequestFeature.CreateChildFeature(
                AppFeatures.TachyonDealerTripCommissionPercentage,
                "false",
                L("TachyonDealerTripCommissionPercentage"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));

            createTmsRequestFeature.CreateChildFeature(
                AppFeatures.TachyonDealerTripCommissionValue,
                "false",
                L("TachyonDealerTripCommissionValue"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));

            createTmsRequestFeature.CreateChildFeature(
                AppFeatures.TachyonDealerTripMinValueCommission,
                "false",
                L("TachyonDealerTripMinValueCommission"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));

            createTmsRequestFeature.CreateChildFeature(
                AppFeatures.TachyonDealerVasCommissionType,
                "false",
                L("TachyonDealerVasCommissionType"),
                inputType: new ComboboxInputType(new StaticLocalizableComboboxItemSource(CommissionTypes.ToArray())));

            createTmsRequestFeature.CreateChildFeature(
                AppFeatures.TachyonDealerVasCommissionPercentage,
                "false",
                L("TachyonDealerVasCommissionPercentage"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));

            createTmsRequestFeature.CreateChildFeature(
                AppFeatures.TachyonDealerVasCommissionValue,
                "false",
                L("TachyonDealerVasCommissionValue"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));

            createTmsRequestFeature.CreateChildFeature(
                AppFeatures.TachyonDealerVasMinValueCommission,
                "false",
                L("TachyonDealerVasMinValueCommission"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue)));


            // carrier as Saas

            var carrierAsASaas = payFeature.CreateChildFeature(
                                 AppFeatures.CarrierAsASaas,
                                 "false",
                                 L(AppFeatures.CarrierAsASaas),
                                 inputType: new CheckboxInputType()
                                 );


            carrierAsASaas.CreateChildFeature(
                AppFeatures.CarrierAsSaasCommissionValue,
                "false",
                L(AppFeatures.CarrierAsSaasCommissionValue),
               inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue))
                );


            #endregion


            #region Receipt

            if (periods.Count > 0)
            {
                LocalizableComboboxItem[] ShipperPeriods = periods
                    .Select(i => new LocalizableComboboxItem(i.Id.ToString(), L(i.DisplayName)))
                    .ToArray();


                var Carrierperiods = periods.Where(x => x.Enabled && !x.ShipperOnlyUsed)
                    .Select(i => new LocalizableComboboxItem(i.Id.ToString(), L(i.DisplayName))).ToArray();
                if (Carrierperiods != null && Carrierperiods.Length > 0)
                {
                    receiptFeature.CreateChildFeature(
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
                receiptFeature.CreateChildFeature(
                    AppFeatures.InvoicePaymentMethodCrarrier,
                    "false",
                    L("InvoicePaymentMethod"),
                    inputType: new ComboboxInputType(new StaticLocalizableComboboxItemSource
                        (invoicePaymentMethodsCrarrier)));
            }


            #endregion




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


            var carrierEditionId =
                Convert.ToInt32(SettingManager.GetSettingValue(AppSettings.Editions.CarrierEditionId));
            ILocalizableComboboxItem[] tenants = _tenantRepository.GetAll()
                .Where(x => x.EditionId == carrierEditionId)
                .Select(i => new LocalizableComboboxItem(i.Id.ToString(), L(i.TenancyName + " - " + i.AccountNumber))).ToArray();

            var saasFeature = shipperFeature.CreateChildFeature(
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