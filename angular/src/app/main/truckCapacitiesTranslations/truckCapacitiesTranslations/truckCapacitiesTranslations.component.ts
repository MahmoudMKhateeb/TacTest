import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute , Router} from '@angular/router';
import { TruckCapacitiesTranslationsServiceProxy, TruckCapacitiesTranslationDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditTruckCapacitiesTranslationModalComponent } from './create-or-edit-truckCapacitiesTranslation-modal.component';

import { ViewTruckCapacitiesTranslationModalComponent } from './view-truckCapacitiesTranslation-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';


@Component({
    templateUrl: './truckCapacitiesTranslations.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class TruckCapacitiesTranslationsComponent extends AppComponentBase {
    
    
    @ViewChild('createOrEditTruckCapacitiesTranslationModal', { static: true }) createOrEditTruckCapacitiesTranslationModal: CreateOrEditTruckCapacitiesTranslationModalComponent;
    @ViewChild('viewTruckCapacitiesTranslationModalComponent', { static: true }) viewTruckCapacitiesTranslationModal: ViewTruckCapacitiesTranslationModalComponent;   
    
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    translatedDisplayNameFilter = '';
    languageFilter = '';
        capacityDisplayNameFilter = '';






    constructor(
        injector: Injector,
        private _truckCapacitiesTranslationsServiceProxy: TruckCapacitiesTranslationsServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getTruckCapacitiesTranslations(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._truckCapacitiesTranslationsServiceProxy.getAll(
            this.filterText,
            this.translatedDisplayNameFilter,
            this.languageFilter,
            this.capacityDisplayNameFilter,
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

    createTruckCapacitiesTranslation(): void {
        this.createOrEditTruckCapacitiesTranslationModal.show();        
    }


    deleteTruckCapacitiesTranslation(truckCapacitiesTranslation: TruckCapacitiesTranslationDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._truckCapacitiesTranslationsServiceProxy.delete(truckCapacitiesTranslation.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }
    
    
    
    
}
