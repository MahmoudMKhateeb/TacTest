import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TrackingComponent } from '@app/main/shippingRequests/shippingRequests/tracking/tracking.component';
import { AppConsts } from '@shared/AppConsts';

const routes: Routes = [
  {
    path: 'shipmentTracking',
    component: TrackingComponent,
    data: { shipmentType: AppConsts.Tracking_NormalShipment },
  },
  {
    path: 'directShipmentTracking',
    component: TrackingComponent,
    data: {
      shipmentType: AppConsts.Tracking_DirectShipment,
      permission: 'Pages.Shipment.Tracking.DirectShipmentTracking',
    },
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class TrackingRoutingModule {}
