using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Runtime.Validation;
using Abp.UI.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using TACHYON.Invoices.PaymentMethods;
using TACHYON.Invoices.Periods;
using TACHYON.PriceOffers;

namespace TACHYON.Features
{
    public class AppFeatureProvider : FeatureProvider
    {
        private IRepository<InvoicePeriod> _PeriodRepository;
        private readonly IRepository<InvoicePaymentMethod> _invoicePaymentMethodRepository;

        public AppFeatureProvider(IRepository<InvoicePeriod> periodRepository, IRepository<InvoicePaymentMethod> invoicePaymentMethodRepository)
        {
            _PeriodRepository = periodRepository;
            _invoicePaymentMethodRepository = invoicePaymentMethodRepository;
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

                LocalizableComboboxItem[] Shipperitems = new LocalizableComboboxItem[periods.Count];

                for (var i = 0; i < periods.Count; i++)
                {
                    Shipperitems[i] = new LocalizableComboboxItem(periods[i].Id.ToString(), L(periods[i].DisplayName));

                }


                shipperFeature.CreateChildFeature(
                          AppFeatures.ShipperPeriods,
                          defaultValue: "false",
                          displayName: L(AppFeatures.ShipperPeriods),
                          inputType: new ComboboxInputType(
                              new StaticLocalizableComboboxItemSource(Shipperitems)
                          )
                        );

                var Carrierperiods = periods.Where(p => p.ShipperOnlyUsed == false).ToList();
                if (Carrierperiods != null && Carrierperiods.Count > 0)
                {
                    LocalizableComboboxItem[] Carrieritems = new LocalizableComboboxItem[Carrierperiods.Count];

                    for (var i = 0; i < Carrierperiods.Count; i++)
                    {
                        Carrieritems[i] = new LocalizableComboboxItem(Carrierperiods[i].Id.ToString(), L(Carrierperiods[i].DisplayName));

                    }
                    carrierFeature.CreateChildFeature(
                                  AppFeatures.CarrierPeriods,
                                  defaultValue: "false",
                                  displayName: L(AppFeatures.CarrierPeriods),
                                  inputType: new ComboboxInputType(
                                      new StaticLocalizableComboboxItemSource(Carrieritems)
                                  )
                                );
                }

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
                if (i !=1 && i!=2) continue;
                CommissionTypes.Add(new LocalizableComboboxItem(i.ToString(), L(Enum.GetName(typeof(PriceOfferCommissionType), i))));
            }


            shipperFeature.CreateChildFeature(
                AppFeatures.InvoicePaymentMethod,
                "false",
                L("InvoicePaymentMethod"),
                inputType: new ComboboxInputType(new StaticLocalizableComboboxItemSource
                (_invoicePaymentMethodRepository.GetAll()
                .Select(x=> new LocalizableComboboxItem(x.Id.ToString(), L(x.DisplayName))).ToArray())));


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
            //        #endregion
            #endregion

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


        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, TACHYONConsts.LocalizationSourceName);
        }
    }
}