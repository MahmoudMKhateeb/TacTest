import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {
  OffersServiceProxy,
  CreateOrEditOfferDto,
  OfferTrucksTypeLookupTableDto,
  OfferTrailerTypeLookupTableDto,
  OfferGoodCategoryLookupTableDto,
  OfferRouteLookupTableDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'createOrEditOfferModal',
  templateUrl: './create-or-edit-offer-modal.component.html',
})
export class CreateOrEditOfferModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  offer: CreateOrEditOfferDto = new CreateOrEditOfferDto();

  trucksTypeDisplayName = '';
  trailerTypeDisplayName = '';
  goodCategoryDisplayName = '';
  routeDisplayName = '';

  allTrucksTypes: OfferTrucksTypeLookupTableDto[];
  allTrailerTypes: OfferTrailerTypeLookupTableDto[];
  allGoodCategorys: OfferGoodCategoryLookupTableDto[];
  allRoutes: OfferRouteLookupTableDto[];

  constructor(injector: Injector, private _offersServiceProxy: OffersServiceProxy) {
    super(injector);
  }

  show(offerId?: number): void {
    if (!offerId) {
      //create
      this.offer = new CreateOrEditOfferDto();
      this.offer.id = offerId;
      this.trucksTypeDisplayName = '';
      this.trailerTypeDisplayName = '';
      this.goodCategoryDisplayName = '';
      this.routeDisplayName = '';

      this.active = true;
      this.modal.show();
    } else {
      this._offersServiceProxy.getOfferForEdit(offerId).subscribe((result) => {
        this.offer = result.offer;

        this.trucksTypeDisplayName = result.trucksTypeDisplayName;
        this.trailerTypeDisplayName = result.trailerTypeDisplayName;
        this.goodCategoryDisplayName = result.goodCategoryDisplayName;
        this.routeDisplayName = result.routeDisplayName;

        this.active = true;
        this.modal.show();
      });
    }
    this._offersServiceProxy.getAllTrucksTypeForTableDropdown().subscribe((result) => {
      this.allTrucksTypes = result;
    });
    this._offersServiceProxy.getAllTrailerTypeForTableDropdown().subscribe((result) => {
      this.allTrailerTypes = result;
    });
    this._offersServiceProxy.getAllGoodCategoryForTableDropdown().subscribe((result) => {
      this.allGoodCategorys = result;
    });
    this._offersServiceProxy.getAllRouteForTableDropdown().subscribe((result) => {
      this.allRoutes = result;
    });
  }

  save(): void {
    this.saving = true;

    this._offersServiceProxy
      .createOrEdit(this.offer)
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
