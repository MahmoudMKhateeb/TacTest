import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute , Router} from '@angular/router';
import { OffersServiceProxy, OfferDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditOfferModalComponent } from './create-or-edit-offer-modal.component';

import { ViewOfferModalComponent } from './view-offer-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './offers.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class OffersComponent extends AppComponentBase {
    
    
    @ViewChild('createOrEditOfferModal', { static: true }) createOrEditOfferModal: CreateOrEditOfferModalComponent;
    @ViewChild('viewOfferModalComponent', { static: true }) viewOfferModal: ViewOfferModalComponent;   
    
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    displayNameFilter = '';
    descriptionFilter = '';
    maxPriceFilter : number;
		maxPriceFilterEmpty : number;
		minPriceFilter : number;
		minPriceFilterEmpty : number;
        trucksTypeDisplayNameFilter = '';
        trailerTypeDisplayNameFilter = '';
        goodCategoryDisplayNameFilter = '';
        routeDisplayNameFilter = '';




    constructor(
        injector: Injector,
        private _offersServiceProxy: OffersServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getOffers(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._offersServiceProxy.getAll(
            this.filterText,
            this.displayNameFilter,
            this.descriptionFilter,
            this.maxPriceFilter == null ? this.maxPriceFilterEmpty: this.maxPriceFilter,
            this.minPriceFilter == null ? this.minPriceFilterEmpty: this.minPriceFilter,
            this.trucksTypeDisplayNameFilter,
            this.trailerTypeDisplayNameFilter,
            this.goodCategoryDisplayNameFilter,
            this.routeDisplayNameFilter,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createOffer(): void {
        this.createOrEditOfferModal.show();        
    }


    deleteOffer(offer: OfferDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._offersServiceProxy.delete(offer.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._offersServiceProxy.getOffersToExcel(
        this.filterText,
            this.displayNameFilter,
            this.descriptionFilter,
            this.maxPriceFilter == null ? this.maxPriceFilterEmpty: this.maxPriceFilter,
            this.minPriceFilter == null ? this.minPriceFilterEmpty: this.minPriceFilter,
            this.trucksTypeDisplayNameFilter,
            this.trailerTypeDisplayNameFilter,
            this.goodCategoryDisplayNameFilter,
            this.routeDisplayNameFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
}
