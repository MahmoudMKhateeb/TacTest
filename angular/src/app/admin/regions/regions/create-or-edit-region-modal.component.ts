import { Component, ViewChild, Injector, Output, EventEmitter, OnInit, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { RegionsServiceProxy, CreateOrEditRegionDto, RegionCountyLookupTableDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'createOrEditRegionModal',
  templateUrl: './create-or-edit-region-modal.component.html',
})
export class CreateOrEditRegionModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  region: CreateOrEditRegionDto = new CreateOrEditRegionDto();

  countyDisplayName = '';

  allCountys: RegionCountyLookupTableDto[];

  constructor(injector: Injector, private _regionsServiceProxy: RegionsServiceProxy) {
    super(injector);
  }

  show(regionId?: number): void {
    if (!regionId) {
      this.region = new CreateOrEditRegionDto();
      this.region.id = regionId;
      this.countyDisplayName = '';

      this.active = true;
      this.modal.show();
    } else {
      this._regionsServiceProxy.getRegionForEdit(regionId).subscribe((result) => {
        this.region = result.region;

        this.countyDisplayName = result.countyDisplayName;

        this.active = true;
        this.modal.show();
      });
    }
    this._regionsServiceProxy.getAllCountyForTableDropdown().subscribe((result) => {
      this.allCountys = result;
    });
  }

  save(): void {
    this.saving = true;

    this._regionsServiceProxy
      .createOrEdit(this.region)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(null);
      });
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }

  ngOnInit(): void {}
}
