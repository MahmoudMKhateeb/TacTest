import { Component, ViewChild, Injector, Output, EventEmitter, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {
  RoutStepsServiceProxy,
  CreateOrEditRoutStepDto,
  RoutStepCityLookupTableDto,
  RoutStepRouteLookupTableDto,
  CreateOrEditGoodsDetailDto,
  SelectItemDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'createOrEditRoutStepModal',
  templateUrl: './create-or-edit-routStep-modal.component.html',
})
export class CreateOrEditRoutStepModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @Input() routeId: any;

  active = false;
  saving = false;

  routStep: CreateOrEditRoutStepDto = new CreateOrEditRoutStepDto();

  trucksTypeDisplayName = '';
  trailerTypeDisplayName = '';
  goodsDetailName = '';

  cityDisplayName = '';
  cityDisplayName2 = '';
  routeDisplayName = '';

  allCitys: RoutStepCityLookupTableDto[];
  allTrucksTypes: SelectItemDto[];
  allTrailerTypes: SelectItemDto[];
  allGoodsDetails: SelectItemDto[];

  constructor(injector: Injector, private _routStepsServiceProxy: RoutStepsServiceProxy) {
    super(injector);
    // this.routStep.createOrEditGoodsDetailDto = new CreateOrEditGoodsDetailDto();
  }

  show(routStepId?: number): void {
    if (!routStepId) {
      this.routStep = new CreateOrEditRoutStepDto();
      // this.routStep.createOrEditGoodsDetailDto = new CreateOrEditGoodsDetailDto();

      this.routStep.id = routStepId;
      this.cityDisplayName = '';
      this.cityDisplayName2 = '';
      this.trucksTypeDisplayName = '';
      this.trailerTypeDisplayName = '';
      this.goodsDetailName = '';
      this.routeDisplayName = '';
      this.active = true;
      this.modal.show();
    } else {
      this._routStepsServiceProxy.getRoutStepForEdit(routStepId).subscribe((result) => {
        // this.routStep = result.routStep;
        // this.trucksTypeDisplayName = result.trucksTypeDisplayName;
        this.trailerTypeDisplayName = result.trailerTypeDisplayName;
        // this.goodsDetailName = result.goodsDetailName;
        this.routeDisplayName = result.routeDisplayName;

        // this.cityDisplayName = result.cityDisplayName;
        // this.cityDisplayName2 = result.cityDisplayName2;

        this.active = true;
        this.modal.show();
      });
    }
    this._routStepsServiceProxy.getAllCityForTableDropdown().subscribe((result) => {
      this.allCitys = result;
    });
    this._routStepsServiceProxy.getAllCityForTableDropdown().subscribe((result) => {
      this.allCitys = result;
    });
    this._routStepsServiceProxy.getAllTrucksTypeForTableDropdown().subscribe((result) => {
      this.allTrucksTypes = result;
    });
    this._routStepsServiceProxy.getAllTrailerTypeForTableDropdown().subscribe((result) => {
      this.allTrailerTypes = result;
    });
    this._routStepsServiceProxy.getAllGoodsDetailForTableDropdown().subscribe((result) => {
      this.allGoodsDetails = result;
    });
  }

  save(): void {
    this.saving = true;

    this._routStepsServiceProxy
      .createOrEdit(this.routStep)
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
}
