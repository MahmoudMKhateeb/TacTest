import { Component, Injector, ViewEncapsulation, ViewChild, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PackingTypesServiceProxy, PackingTypeDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditPackingTypeModalComponent } from './create-or-edit-packingType-modal.component';

import { ViewPackingTypeModalComponent } from './view-packingType-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { FileDownloadService } from '@shared/utils/file-download.service';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';

@Component({
  templateUrl: './packingTypes.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class PackingTypesComponent extends AppComponentBase implements OnInit {
  @ViewChild('createOrEditPackingTypeModal', { static: true }) createOrEditPackingTypeModal: CreateOrEditPackingTypeModalComponent;
  @ViewChild('viewPackingTypeModalComponent', { static: true }) viewPackingTypeModal: ViewPackingTypeModalComponent;

  advancedFiltersAreShown = false;
  filterText = '';
  dataSource: any = {};

  constructor(
    injector: Injector,
    private _packingTypesServiceProxy: PackingTypesServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.getAll();
  }

  reloadPage(): void {}

  createPackingType(): void {
    this.createOrEditPackingTypeModal.show();
  }

  deletePackingType(packingType: PackingTypeDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._packingTypesServiceProxy.delete(packingType.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }

  getAll() {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        console.log(JSON.stringify(loadOptions));
        return self._packingTypesServiceProxy
          .getAll(JSON.stringify(loadOptions))
          .toPromise()
          .then((response) => {
            return {
              data: response.data,
              totalCount: response.totalCount,
            };
          })
          .catch((error) => {
            console.log(error);
            throw new Error('Data Loading Error');
          });
      },
    });
  }
}
