import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { Table } from '@node_modules/primeng/table';
import { Paginator } from '@node_modules/primeng/paginator';
import * as moment from '@node_modules/moment';
import {
  EntityTemplateListDto,
  EntityTemplateServiceProxy,
  SavedEntityType,
  TerminologyVersion,
  TokenAuthServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@node_modules/abp-ng2-module';
import { ActivatedRoute, Router } from '@angular/router';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { LazyLoadEvent } from '@node_modules/primeng/api';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditFacilityModalComponent } from '@app/main/addressBook/facilities/create-or-edit-facility-modal.component';
import { ViewFacilityModalComponent } from '@app/main/addressBook/facilities/view-facility-modal.component';
import Swal from 'sweetalert2';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

@Component({
  selector: 'request-templates',
  templateUrl: './request-templates.component.html',
  styleUrls: ['./request-templates.component.css'],
  providers: [EnumToArrayPipe],
})
export class RequestTemplatesComponent extends AppComponentBase implements OnInit {
  @ViewChild('createOrEditFacilityModal', { static: true }) createOrEditFacilityModal: CreateOrEditFacilityModalComponent;
  @ViewChild('viewFacilityModalComponent', { static: true }) viewFacilityModal: ViewFacilityModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  nameFilter = '';
  templateTypeFilter: any;

  fromDate: moment.Moment | null | undefined;
  toDate: moment.Moment | null | undefined;
  creationDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  creationDateRangeActive: boolean = false;
  entityType: SavedEntityType = SavedEntityType.ShippingRequestTemplate;
  availableEntityTypes = this.enumToArray.transform(SavedEntityType);

  private entityTypesEnum = SavedEntityType;

  constructor(
    injector: Injector,
    private _templateService: EntityTemplateServiceProxy,
    private _router: Router,
    private enumToArray: EnumToArrayPipe
  ) {
    super(injector);
  }

  getTemplates(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();
    console.log('templateTypeFilter', this.templateTypeFilter, this.availableEntityTypes);
    this._templateService
      .getAll(
        this.templateTypeFilter,
        this.nameFilter,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getMaxResultCount(this.paginator, event),
        this.primengTableHelper.getSkipCount(this.paginator, event)
      )
      .subscribe((result) => {
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.hideLoadingIndicator();
      });
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }

  createFacility(): void {
    this.createOrEditFacilityModal.show();
  }

  ngOnInit(): void {}

  /**
   * Deletes a Template
   * @param id
   */
  deleteTemplate(id: number) {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._templateService.delete(id).subscribe(() => {
          this.notify.success('SuccessfullyDeleted');
          this.getTemplates();
        });
      }
    });
  }

  /**
   * uses Entity Template
   * @param record
   */
  useTemplate(record: EntityTemplateListDto) {
    console.log(record);
    if (record.entityType === this.entityTypesEnum.ShippingRequestTemplate) {
      console.log('reached here');
      //handle Redirect To Wizard
      // this._router.navigate(['/app/main/shippingRequests/shippingRequestWizard', { templateId: record.id }]);
      this._router.navigate(['/app/main/shippingRequests/shippingRequestWizard'], {
        queryParams: {
          templateId: record.id,
        },
      });
    } else if (record.entityType === this.entityTypesEnum.TripTemplate) {
      Swal.fire({
        icon: 'info',
        title: 'Oops...',
        text: this.l('TripTemplateUsageMsg'),
      });
    }
  }

  // /**
  //  * edit a Saved Template
  //  * @param record
  //  */
  // editTemplate(record: EntityTemplateListDto) {
  //   console.log(record);
  // }
}
