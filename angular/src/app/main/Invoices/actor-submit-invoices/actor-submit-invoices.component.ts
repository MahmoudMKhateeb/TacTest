import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { ActorSubmitInvoiceServiceProxy, SubmitInvoiceStatus } from '@shared/service-proxies/service-proxies';
import { DxDataGridComponent } from 'devextreme-angular';
import CustomStore from 'devextreme/data/custom_store';
import { LoadOptions } from 'devextreme/data/load_options';

@Component({
  selector: 'app-actor-submit-invoices',
  templateUrl: './actor-submit-invoices.component.html',
  styleUrls: ['./actor-submit-invoices.component.css'],
})
export class ActorSubmitInvoicesComponent extends AppComponentBase implements OnInit {
  dataSource: any = {};
  SubmitStatus: any;
  @ViewChild('dataGrid', { static: true }) dataGrid: DxDataGridComponent;
  constructor(injector: Injector, private _ActorSubmitInvoiceServiceProxy: ActorSubmitInvoiceServiceProxy, private enumToArray: EnumToArrayPipe) {
    super(injector);
    this.SubmitStatus = this.enumToArray.transform(SubmitInvoiceStatus);
  }

  ngOnInit(): void {
    this.getAllInvoices();
  }

  reloadPage(): void {
    this.refreshDataGrid();
  }

  getAllInvoices() {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        console.log(JSON.stringify(loadOptions));
        return self._ActorSubmitInvoiceServiceProxy
          .getAll(JSON.stringify(loadOptions))
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
      .catch(function (error) {
        // ...
      });
  }

  MakePaid(invoice: any): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._ActorSubmitInvoiceServiceProxy.makeActorSubmitInvoicePaid(invoice.id).subscribe(() => {
          this.notify.success(this.l('Successfully'));
          this.reloadPage();
        });
      }
    });
  }

  MakeUnPaid(invoice: any): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._ActorSubmitInvoiceServiceProxy.makeActorSubmitInvoiceUnPaid(invoice.id).subscribe(() => {
          this.notify.success(this.l('Successfully'));
          this.reloadPage();
        });
      }
    });
  }
}
