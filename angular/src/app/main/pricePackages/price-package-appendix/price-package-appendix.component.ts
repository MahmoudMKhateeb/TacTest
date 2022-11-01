import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AppendixStatus, PricePackageAppendixServiceProxy } from '@shared/service-proxies/service-proxies';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

@Component({
  selector: 'app-price-package-appendix',
  templateUrl: './price-package-appendix.component.html',
  styleUrls: ['./price-package-appendix.component.css'],
})
export class PricePackageAppendixComponent extends AppComponentBase implements OnInit {
  dataSource: any;
  appendixStatus = AppendixStatus;
  statusList = this.enumToArray.transform(this.appendixStatus);

  constructor(private injector: Injector, private _appendicesServiceProxy: PricePackageAppendixServiceProxy, private enumToArray: EnumToArrayPipe) {
    super(injector);
  }

  ngOnInit(): void {
    this.getAllPricePackageAppendices();
  }

  Accept(id) {
    this._appendicesServiceProxy.accept(id).subscribe(() => {
      this.notify.success(this.l('SuccessfullyConfirmed'));
      this.getAllPricePackageAppendices();
    });
  }

  Reject(id) {
      this._appendicesServiceProxy.reject(id).subscribe(() => {
          this.notify.info(this.l('SuccessfullyRejected'));
          this.getAllPricePackageAppendices();
      });
  }

  getStatusDisplayName(status) {
    let displayName = this.statusList?.find((x) => x.key == status)?.value;
    return this.l(displayName);
  }

  getAllPricePackageAppendices() {
    let self = this;
    return (this.dataSource = new CustomStore({
      loadMode: 'raw',
      key: 'id',
      load(loadOptions: LoadOptions) {
        return self._appendicesServiceProxy
          .getAll(JSON.stringify(loadOptions))
          .toPromise()
          .then((response) => {
            return response.data;
          });
      },
    }));
  }
}
