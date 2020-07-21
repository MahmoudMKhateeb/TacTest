import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute , Router} from '@angular/router';
import { GoodsDetailsServiceProxy, GoodsDetailDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditGoodsDetailModalComponent } from './create-or-edit-goodsDetail-modal.component';

import { ViewGoodsDetailModalComponent } from './view-goodsDetail-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './goodsDetails.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class GoodsDetailsComponent extends AppComponentBase {
    
    
    @ViewChild('createOrEditGoodsDetailModal', { static: true }) createOrEditGoodsDetailModal: CreateOrEditGoodsDetailModalComponent;
    @ViewChild('viewGoodsDetailModalComponent', { static: true }) viewGoodsDetailModal: ViewGoodsDetailModalComponent;   
    
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    nameFilter = '';
    descriptionFilter = '';
    quantityFilter = '';
    weightFilter = '';
    dimentionsFilter = '';
    isDangerousGoodFilter = -1;
    dangerousGoodsCodeFilter = '';
        goodCategoryDisplayNameFilter = '';




    constructor(
        injector: Injector,
        private _goodsDetailsServiceProxy: GoodsDetailsServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getGoodsDetails(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._goodsDetailsServiceProxy.getAll(
            this.filterText,
            this.nameFilter,
            this.descriptionFilter,
            this.quantityFilter,
            this.weightFilter,
            this.dimentionsFilter,
            this.isDangerousGoodFilter,
            this.dangerousGoodsCodeFilter,
            this.goodCategoryDisplayNameFilter,
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

    createGoodsDetail(): void {
        this.createOrEditGoodsDetailModal.show();        
    }


    deleteGoodsDetail(goodsDetail: GoodsDetailDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._goodsDetailsServiceProxy.delete(goodsDetail.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._goodsDetailsServiceProxy.getGoodsDetailsToExcel(
        this.filterText,
            this.nameFilter,
            this.descriptionFilter,
            this.quantityFilter,
            this.weightFilter,
            this.dimentionsFilter,
            this.isDangerousGoodFilter,
            this.dangerousGoodsCodeFilter,
            this.goodCategoryDisplayNameFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
}
