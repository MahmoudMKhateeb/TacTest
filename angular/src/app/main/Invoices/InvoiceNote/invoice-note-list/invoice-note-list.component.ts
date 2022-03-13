import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import {
  CommonLookupServiceProxy,
  InvoiceNoteServiceProxy,
  InvoiceReportServiceServiceProxy,
  ISelectItemDto,
  NoteStatus,
  NoteType,
} from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { DxDataGridComponent } from 'devextreme-angular';
import CustomStore from 'devextreme/data/custom_store';
import { LoadOptions } from 'devextreme/data/load_options';
import { CreateOrEditNoteModalComponent } from './create-or-edit-note-modal/create-or-edit-note-modal.component';
import { InoviceNoteModalComponent } from './inovice-note-modal/inovice-note-modal.component';
import { VoidInvoiceNoteModalComponent } from '@app/main/Invoices/InvoiceNote/invoice-note-list/void-invoice-note-modal/void-invoice-note-modal.component';

@Component({
  selector: 'app-invoice-note-list',
  templateUrl: './invoice-note-list.component.html',
  animations: [appModuleAnimation()],
  providers: [EnumToArrayPipe],
})
export class InvoiceNoteListComponent extends AppComponentBase implements OnInit {
  @ViewChild('dataGrid', { static: true }) dataGrid: DxDataGridComponent;
  @ViewChild('inoviceNoteModalComponent') inoviceNoteModalComponent: InoviceNoteModalComponent;
  @ViewChild('createOrEditNoteModalComponent') createOrEditNoteModalComponent: CreateOrEditNoteModalComponent;
  @ViewChild('voidInvoice') voidInvoice: VoidInvoiceNoteModalComponent;

  Tenants: ISelectItemDto[];
  dataSource: any = {};
  NoteStatus: any;
  NoteType: any;
  NoteStatusesEnum = NoteStatus;

  constructor(
    injector: Injector,
    private _InvoiceNoteServiceProxy: InvoiceNoteServiceProxy,
    private _CommonServices: CommonLookupServiceProxy,
    private _InvoiceReportServiceProxy: InvoiceReportServiceServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private enumToArray: EnumToArrayPipe
  ) {
    super(injector);
    this.NoteStatus = this.enumToArray.transform(NoteStatus);
    this.NoteType = this.enumToArray.transform(NoteType);
  }

  ngOnInit(): void {
    this.getAllInvoiceNotes();
  }

  reloadPage(): void {
    this.refreshDataGrid();
  }

  search(event) {
    this._CommonServices.getAutoCompleteTenants(event.query, '').subscribe((result) => {
      this.Tenants = result;
    });
  }
  canacel(InvoiceNoteId: number): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._InvoiceNoteServiceProxy.canacel(InvoiceNoteId).subscribe(() => {
          this.notify.success(this.l('Successfully'));
          this.refreshDataGrid();
        });
      }
    });
  }

  changeStauts(InvoiceNoteId: number): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._InvoiceNoteServiceProxy.changeInvoiceNoteStatus(InvoiceNoteId).subscribe(() => {
          this.notify.success(this.l('SuccessfullySaved'));
          this.refreshDataGrid();
        });
      }
    });
  }

  downloadReport(id: number) {
    this._InvoiceReportServiceProxy.downloadInvoiceNoteReportPdf(id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }

  StyleStatus(Status: NoteStatus): string {
    switch (Status) {
      case NoteStatus.Draft:
        return 'label label-primary label-inline m-1';
      case NoteStatus.Confirm:
        return 'label label-success label-inline m-1';
      case NoteStatus.Canceled:
        return 'label label-danger label-inline m-1';
      case NoteStatus.WaitingtobePaid:
        return 'label label-warning label-inline m-1';
      case NoteStatus.Paid:
        return 'label label-info label-inline m-1';
      default:
        return 'label label-default label-inline m-1';
    }
  }

  StyleNoteType(noteType: NoteType): string {
    switch (noteType) {
      case NoteType.Credit:
        return 'label label-primary label-inline m-1';
      case NoteType.Debit:
        return 'label label-success label-inline m-1';
      default:
        return 'label label-default label-inline m-1';
    }
  }

  getAllInvoiceNotes() {
    let self = this;
    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        console.log(JSON.stringify(loadOptions));
        return self._InvoiceNoteServiceProxy
          .getAllInoviceNote(JSON.stringify(loadOptions))
          .toPromise()
          .then((response) => {
            return {
              data: response.data,
              totalCount: response.totalCount,
              summary: response.summary,
              groupCount: response.groupCount,
            };
          })
          .catch((error) => {
            console.log(error);
            throw new Error('Data Loading Error');
          });
      },
    });
  }

  refreshDataGrid() {
    this.dataGrid.instance
      .refresh()
      .then(function () {
        // ...
      })
      .catch(function () {
        // ...
      });
  }
}
