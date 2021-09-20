import { Injectable, OnInit } from '@angular/core';
import { WidgetViewDefinition, WidgetFilterViewDefinition } from './definitions';
import { DashboardCustomizationConst } from './DashboardCustomizationConsts';
import { WidgetGeneralStatsComponent } from './widgets/widget-general-stats/widget-general-stats.component';
import { WidgetDailySalesComponent } from './widgets/widget-daily-sales/widget-daily-sales.component';
import { WidgetProfitShareComponent } from './widgets/widget-profit-share/widget-profit-share.component';
import { WidgetMemberActivityComponent } from './widgets/widget-member-activity/widget-member-activity.component';
import { WidgetRegionalStatsComponent } from './widgets/widget-regional-stats/widget-regional-stats.component';
import { WidgetSalesSummaryComponent } from './widgets/widget-sales-summary/widget-sales-summary.component';
import { WidgetIncomeStatisticsComponent } from './widgets/widget-income-statistics/widget-income-statistics.component';
import { WidgetRecentTenantsComponent } from './widgets/widget-recent-tenants/widget-recent-tenants.component';
import { WidgetEditionStatisticsComponent } from './widgets/widget-edition-statistics/widget-edition-statistics.component';
import { WidgetSubscriptionExpiringTenantsComponent } from './widgets/widget-subscription-expiring-tenants/widget-subscription-expiring-tenants.component';
import { WidgetHostTopStatsComponent } from './widgets/widget-host-top-stats/widget-host-top-stats.component';
import { FilterDateRangePickerComponent } from './filters/filter-date-range-picker/filter-date-range-picker.component';
import { WidgetTopStatsComponent } from './widgets/widget-top-stats/widget-top-stats.component';
import { CompletedTripsWidgetComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/completed-trips-widget/completed-trips-widget.component';
import { CompletedTripVsPodComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/completed-trip-vs-pod/completed-trip-vs-pod.component';
import { AcceptedVsRejecedRequestsComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/accepted-vs-rejeced-requests/accepted-vs-rejeced-requests.component';
import { InvoicesVsPaidInvoicesComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/invoices-vs-paid-invoices/invoices-vs-paid-invoices.component';
import { MostWorkedWithCarriersComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/most-worked-with-carriers/most-worked-with-carriers.component';
import { RequestsInMarketPlaceComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/requests-in-market-place/requests-in-market-place.component';
import { NextInvoiceFrequancyDateComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/next-invoice-frequancy-date/next-invoice-frequancy-date.component';
import { InvoiceDueDateComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/invoice-due-date/invoice-due-date.component';
import { ShipperDueDateInDaysComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/shipper-due-date-in-days/shipper-due-date-in-days.component';
import { MostUsedOriginComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/most-used-origin/most-used-origin.component';
import { MostUsedDestinationsComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/most-used-destinations/most-used-destinations.component';
import { TrackingMapComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/tracking-map/tracking-map.component';
import { CarrierInvoicesVsPaidInvoicesComponent } from '@app/shared/common/customizable-dashboard/widgets/carrier/invoices-vs-paid-invoices/invoices-vs-paid-invoices.component';
import { AcceptedVsRejectedPricingComponent } from '@app/shared/common/customizable-dashboard/widgets/carrier/accepted-vs-rejected-pricing/accepted-vs-rejected-pricing.component';

@Injectable({
  providedIn: 'root',
})
export class DashboardViewConfigurationService {
  public WidgetViewDefinitions: WidgetViewDefinition[] = [];
  public widgetFilterDefinitions: WidgetFilterViewDefinition[] = [];

  constructor() {
    this.initializeConfiguration();
  }

  private initializeConfiguration() {
    let filterDateRangePicker = new WidgetFilterViewDefinition(
      DashboardCustomizationConst.filters.filterDateRangePicker,
      FilterDateRangePickerComponent
    );
    //add your filters here
    this.widgetFilterDefinitions.push(filterDateRangePicker);

    let generalStats = new WidgetViewDefinition(DashboardCustomizationConst.widgets.tenant.generalStats, WidgetGeneralStatsComponent, 6, 4);

    let dailySales = new WidgetViewDefinition(DashboardCustomizationConst.widgets.tenant.dailySales, WidgetDailySalesComponent);

    let profitShare = new WidgetViewDefinition(DashboardCustomizationConst.widgets.tenant.profitShare, WidgetProfitShareComponent);

    let memberActivity = new WidgetViewDefinition(DashboardCustomizationConst.widgets.tenant.memberActivity, WidgetMemberActivityComponent);

    let regionalStats = new WidgetViewDefinition(DashboardCustomizationConst.widgets.tenant.regionalStats, WidgetRegionalStatsComponent);

    let salesSummary = new WidgetViewDefinition(DashboardCustomizationConst.widgets.tenant.salesSummary, WidgetSalesSummaryComponent);

    let topStats = new WidgetViewDefinition(DashboardCustomizationConst.widgets.tenant.topStats, WidgetTopStatsComponent);
    //add your tenant side widgets here

    let incomeStatistics = new WidgetViewDefinition(DashboardCustomizationConst.widgets.host.incomeStatistics, WidgetIncomeStatisticsComponent);

    let editionStatistics = new WidgetViewDefinition(DashboardCustomizationConst.widgets.host.editionStatistics, WidgetEditionStatisticsComponent);

    let recentTenants = new WidgetViewDefinition(DashboardCustomizationConst.widgets.host.recentTenants, WidgetRecentTenantsComponent);

    let subscriptionExpiringTenants = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.subscriptionExpiringTenants,
      WidgetSubscriptionExpiringTenantsComponent
    );

    let hostTopStats = new WidgetViewDefinition(DashboardCustomizationConst.widgets.host.topStats, WidgetHostTopStatsComponent);
    //add your host side widgets here

    //Shipper
    //1
    let completedTrips = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperNumberOfCompletedTripsWidget,
      CompletedTripsWidgetComponent
    );
    //2
    let acceptedVsRejectedRequestsWidget = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperAcceptedVsRejectedRequestsWidget,
      AcceptedVsRejecedRequestsComponent
    );

    //3
    let shipperCompletedTripsVsPodWidget = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperCompletedTripsVsPodWidget,
      CompletedTripVsPodComponent
    );
    //4
    let shipperInvoicesVsPaidInvoices = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperInvoicesVsPaidInvoicesWidget,
      InvoicesVsPaidInvoicesComponent
    );

    //5
    let mostWorkedWithCarriers = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperMostWorkedWithCarriersWidget,
      MostWorkedWithCarriersComponent
    );

    //5
    let shippingRequestsInMarketPlace = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperRequestsInMarketplaceWidget,
      RequestsInMarketPlaceComponent
    );
    //6
    let nextInvoiceFrequancyDate = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperNextInvoiceFrequancyEndDateWidget,
      NextInvoiceFrequancyDateComponent,
      2,
      4
    );
    //7
    let invoiceDueDate = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperInvoiceDueDateInDaysWidget,
      InvoiceDueDateComponent,
      2,
      4
    );
    //8
    let dueDateInDays = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperDocumentDueDateInDaysWidget,
      ShipperDueDateInDaysComponent,
      2,
      4
    );

    //9
    let mostUsedOrigins = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperMostUsedOriginsWidget,
      MostUsedOriginComponent,
      4,
      6
    );

    //9
    let mostUsedDest = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperMostUsedDestinationsWidget,
      MostUsedDestinationsComponent,
      4,
      6
    );

    let trackingMapOfShipper = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperTrackingMapWidget,
      TrackingMapComponent,
      8,
      8
    );

    //carrier Widgets
    let carrierInvoicesVsPaid = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Carrier.CarrierInvoicesVsPaidInvoicesWidget,
      CarrierInvoicesVsPaidInvoicesComponent,
      4,
      4
    );

    let carrierAcceptedVsRejected = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Carrier.CarrierAcceptedVsRejectedPricingWidget,
      AcceptedVsRejectedPricingComponent,
      4,
      4
    );

    //shipperPush
    this.WidgetViewDefinitions.push(completedTrips);
    this.WidgetViewDefinitions.push(acceptedVsRejectedRequestsWidget);
    this.WidgetViewDefinitions.push(shipperCompletedTripsVsPodWidget);
    this.WidgetViewDefinitions.push(shipperInvoicesVsPaidInvoices);
    this.WidgetViewDefinitions.push(mostWorkedWithCarriers);
    this.WidgetViewDefinitions.push(shippingRequestsInMarketPlace);
    this.WidgetViewDefinitions.push(nextInvoiceFrequancyDate);
    this.WidgetViewDefinitions.push(invoiceDueDate);
    this.WidgetViewDefinitions.push(dueDateInDays);
    this.WidgetViewDefinitions.push(mostUsedOrigins);
    this.WidgetViewDefinitions.push(mostUsedDest);
    this.WidgetViewDefinitions.push(trackingMapOfShipper);

    //Carrier
    this.WidgetViewDefinitions.push(carrierInvoicesVsPaid);
    this.WidgetViewDefinitions.push(carrierAcceptedVsRejected);

    this.WidgetViewDefinitions.push(generalStats);
    this.WidgetViewDefinitions.push(dailySales);
    this.WidgetViewDefinitions.push(profitShare);
    this.WidgetViewDefinitions.push(memberActivity);
    this.WidgetViewDefinitions.push(regionalStats);
    this.WidgetViewDefinitions.push(salesSummary);
    this.WidgetViewDefinitions.push(topStats);
    this.WidgetViewDefinitions.push(incomeStatistics);
    this.WidgetViewDefinitions.push(editionStatistics);
    this.WidgetViewDefinitions.push(recentTenants);
    this.WidgetViewDefinitions.push(subscriptionExpiringTenants);
    this.WidgetViewDefinitions.push(hostTopStats);
  }
}
