import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute , Router} from '@angular/router';
import { CitiesTranslationsServiceProxy, CitiesTranslationDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditCitiesTranslationModalComponent } from './create-or-edit-citiesTranslation-modal.component';

import { ViewCitiesTranslationModalComponent } from './view-citiesTranslation-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';


@Component({
    templateUrl: './citiesTranslations.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class CitiesTranslationsComponent extends AppComponentBase {
    
    
    @ViewChild('createOrEditCitiesTranslationModal', { static: true }) createOrEditCitiesTranslationModal: CreateOrEditCitiesTranslationModalComponent;
    @ViewChild('viewCitiesTranslationModalComponent', { static: true }) viewCitiesTranslationModal: ViewCitiesTranslationModalComponent;   
    
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    translatedDisplayNameFilter = '';
    languageFilter = '';
        cityDisplayNameFilter = '';






    constructor(
        injector: Injector,
        private _citiesTranslationsServiceProxy: CitiesTranslationsServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getCitiesTranslations(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._citiesTranslationsServiceProxy.getAll(
            this.filterText,
            this.translatedDisplayNameFilter,
            this.languageFilter,
            this.cityDisplayNameFilter,
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

    createCitiesTranslation(): void {
        this.createOrEditCitiesTranslationModal.show();        
    }


    deleteCitiesTranslation(citiesTranslation: CitiesTranslationDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._citiesTranslationsServiceProxy.delete(citiesTranslation.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }
    
    
    
    
}
