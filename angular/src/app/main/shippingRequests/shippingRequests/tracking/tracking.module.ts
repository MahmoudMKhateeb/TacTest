import { NgModule } from '@angular/core';
import { NewTrackingConponent } from '@app/main/shippingRequests/shippingRequests/tracking/new-tracking/new-tracking-conponent';
import { TrackingComponent } from '@app/main/shippingRequests/shippingRequests/tracking/tracking.component';
import { TrackinSearchModelComponent } from '@app/main/shippingRequests/shippingRequests/tracking/tacking-search-model.component';
import { TrackingConfirmModalComponent } from '@app/main/shippingRequests/shippingRequests/tracking/tacking-confirm-code-model.component';
import { TrackingPODModalComponent } from '@app/main/shippingRequests/shippingRequests/tracking/tacking-pod-model.component';
import { AngularFireModule } from '@angular/fire/compat';
import { environment } from '../../../../../environments/environment';
import { AngularFirestoreModule } from '@angular/fire/compat/firestore';
import { AgmCoreModule } from '@node_modules/@agm/core';
import { AgmDirectionModule } from '@node_modules/agm-direction';
import { FormsModule } from '@angular/forms';
import { NgxSkeletonLoaderModule } from '@node_modules/ngx-skeleton-loader';
import { UtilsModule } from '@shared/utils/utils.module';
import { CommonModule } from '@angular/common';
import { RatingModule } from '@node_modules/primeng/rating';
import { StepsModule } from '@node_modules/primeng/steps';
import { AppCommonModule } from '@app/shared/common/app-common.module';
import { CreateOrEditTripAccidentModalComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/accident/create-or-edit-trip-accident-modal.component';
import { ViewTripAccidentModelComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/accident/view-trip-accident-modal.component';
import { ModalModule } from '@node_modules/ngx-bootstrap/modal';
import { FileUploadModule } from '@node_modules/primeng/fileupload';
import { BsDatepickerModule } from '@node_modules/ngx-bootstrap/datepicker';
import { CollapseModule } from '@node_modules/ngx-bootstrap/collapse';
import { ViewDetailsAccidentModalComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/accident/view-details-accident-modal.component';
import { AddAccidentCommentModalComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/accident/accident-comment/add-accident-comment-modal.component';
import { TableModule } from '@node_modules/primeng/table';
import { TrackingRoutingModule } from '@app/main/shippingRequests/shippingRequests/tracking/tracking-routing.module';
import { NgbDropdownModule } from '@node_modules/@ng-bootstrap/ng-bootstrap';
import { TimelineModule } from '@node_modules/primeng/timeline';
import { BsDropdownModule } from '@node_modules/ngx-bootstrap/dropdown';
import { PaginatorModule } from '@node_modules/primeng/paginator';
import { CascadeSelectModule } from '@node_modules/primeng/cascadeselect';
import { TripAccidentResolveModalComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/accident/resolves/trip-accident-resolve-modal.component';
import { TrackingForceDeliverTripComponent } from '@app/main/shippingRequests/shippingRequests/tracking/tracking-force-deliver-trip/tracking-force-deliver-trip.component';
import { DxDataGridModule } from '@node_modules/devextreme-angular/ui/data-grid';
import { DxCheckBoxModule } from '@node_modules/devextreme-angular/ui/check-box';
import { DxSelectBoxModule } from '@node_modules/devextreme-angular/ui/select-box';
import { DxTextBoxModule } from '@node_modules/devextreme-angular/ui/text-box';
import { DxNumberBoxModule } from '@node_modules/devextreme-angular/ui/number-box';
import { DxDateBoxModule } from '@node_modules/devextreme-angular/ui/date-box';
import { DxPopoverModule } from '@node_modules/devextreme-angular/ui/popover';
import { CustomMarkerComponent } from '@app/main/shippingRequests/shippingRequests/tracking/new-tracking/custom-marker/custom-marker.component';
import { CustomTimelineComponent } from '@app/main/shippingRequests/shippingRequests/tracking/new-tracking/custom-timeline/custom-timeline.component';
import { UploadAdditionalDocumentsComponent } from '@app/main/shippingRequests/shippingRequests/tracking/new-tracking/upload-additional-documents/upload-additional-documents.component';
import { TrackingTableViewComponent } from '@app/main/shippingRequests/shippingRequests/tracking/tracking-table-view/tracking-table-view.component';
import { DxSelectBoxModule } from '@node_modules/devextreme-angular/ui/select-box';
import { ViewForceDeliverTripsFromExcelModalComponent } from '@app/main/shippingRequests/shippingRequests/tracking/tracking-force-deliver-trip/view-force-deliver-trips-from-excel-modal/view-force-deliver-trips-from-excel-modal.component';

@NgModule({
  declarations: [
    NewTrackingConponent,
    TrackingComponent,
    TrackinSearchModelComponent,
    TrackingConfirmModalComponent,
    TrackingPODModalComponent,
    CreateOrEditTripAccidentModalComponent,
    ViewTripAccidentModelComponent,
    ViewDetailsAccidentModalComponent,
    AddAccidentCommentModalComponent,
    TripAccidentResolveModalComponent,
    TrackingForceDeliverTripComponent,
    ViewForceDeliverTripsFromExcelModalComponent,
    CustomMarkerComponent,
    CustomTimelineComponent,
    UploadAdditionalDocumentsComponent,
    TrackingTableViewComponent,
  ],
  imports: [
    AngularFireModule.initializeApp(environment.firebase),
    AngularFirestoreModule,
    AgmCoreModule,
    AgmDirectionModule,
    FormsModule,
    NgxSkeletonLoaderModule,
    UtilsModule,
    CommonModule,
    RatingModule,
    StepsModule,
    AppCommonModule,
    ModalModule,
    FileUploadModule,
    BsDatepickerModule,
    CollapseModule,
    TableModule,
    TrackingRoutingModule,
    NgbDropdownModule,
    TimelineModule,
    BsDropdownModule,
    PaginatorModule,
    CascadeSelectModule,
    DxDataGridModule,
    DxCheckBoxModule,
    DxSelectBoxModule,
    DxPopoverModule,
    DxTextBoxModule,
    DxSelectBoxModule,
    DxNumberBoxModule,
    DxDateBoxModule,
  ],
  exports: [
    CreateOrEditTripAccidentModalComponent,
    ViewTripAccidentModelComponent,
    ViewDetailsAccidentModalComponent,
    AddAccidentCommentModalComponent,
  ],
})
export class TrackingModule {}
