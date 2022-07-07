import { ChangeDetectorRef, Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  PagedResultDtoOfShippingRequestAndTripNotesDto,
  ShippingRequestAndTripNotesServiceProxy,
  TruckUserLookupTableDto,
} from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-notes',
  templateUrl: './notes.component.html',
  styleUrls: ['./notes.component.css'],
  animations: [appModuleAnimation()],
})
export class NotesComponent extends AppComponentBase implements OnInit {
  @ViewChild('AddNewNoteModal', { static: false }) modal: ModalDirective;
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  @Input() shippingRequestId: number;
  @Input() tripId: number;
  @Input() type: string;
  totalCount = 0;
  items: PagedResultDtoOfShippingRequestAndTripNotesDto;
  avatar = AppConsts.appBaseUrl + '/assets/common/images/default-profile-picture.png';

  constructor(
    injector: Injector,
    private changeDetectorRef: ChangeDetectorRef,
    private _shippingRequestAndTripNotesServiceProxy: ShippingRequestAndTripNotesServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.primengTableHelper.defaultRecordsCountPerPage = 5;
  }

  getData(event?: LazyLoadEvent) {
    this.primengTableHelper.records = [];
    this.changeDetectorRef.detectChanges();
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }
    this.primengTableHelper.showLoadingIndicator();

    if (this.type.includes('ShippingRequest')) {
      this._shippingRequestAndTripNotesServiceProxy
        .getShippingRequestNotes(
          this.shippingRequestId,
          undefined,
          undefined,
          this.primengTableHelper.getSkipCount(this.paginator, event),
          this.primengTableHelper.getMaxResultCount(this.paginator, event)
        )
        .subscribe((result) => {
          this.primengTableHelper.totalRecordsCount = result.data.totalCount;
          this.primengTableHelper.records = result.data.items;
          this.primengTableHelper.hideLoadingIndicator();
        });
    } else {
      this._shippingRequestAndTripNotesServiceProxy
        .getTripNotes(
          undefined,
          this.tripId,
          undefined,
          this.primengTableHelper.getSkipCount(this.paginator, event),
          this.primengTableHelper.getMaxResultCount(this.paginator, event)
        )
        .subscribe((result) => {
          this.primengTableHelper.totalRecordsCount = result.data.totalCount;
          this.primengTableHelper.records = result.data.items;
          this.primengTableHelper.hideLoadingIndicator();
        });
    }
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }

  deleteNote(id): void {
    this.message.confirm(this.l('DeleteWarningMessage'), this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._shippingRequestAndTripNotesServiceProxy.delete(id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }
}
