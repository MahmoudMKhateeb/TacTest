import { Component, Injector, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {
    RedemptionCodeDto,
    RedemptionCodesServiceProxy,
    TokenAuthServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditRedemptionCodeModalComponent } from './create-or-edit-redemptionCode-modal.component';

import { ViewRedemptionCodeModalComponent } from './view-redemptionCode-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as moment from 'moment';
import { LazyLoadEvent } from 'primeng/api';


@Component({
    templateUrl: './redemptionCodes.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()],
})
export class RedemptionCodesComponent extends AppComponentBase {


    @ViewChild('createOrEditRedemptionCodeModal', { static: true }) createOrEditRedemptionCodeModal: CreateOrEditRedemptionCodeModalComponent;
    @ViewChild('viewRedemptionCodeModalComponent', { static: true }) viewRedemptionCodeModal: ViewRedemptionCodeModalComponent;

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    maxRedemptionDateFilter: moment.Moment;
    minRedemptionDateFilter: moment.Moment;
    maxRedemptionTenantIdFilter: number;
    maxRedemptionTenantIdFilterEmpty: number;
    minRedemptionTenantIdFilter: number;
    minRedemptionTenantIdFilterEmpty: number;
    redeemCodeCodeFilter = '';


    constructor(injector: Injector, private _redemptionCodesServiceProxy: RedemptionCodesServiceProxy, private _notifyService: NotifyService, private _tokenAuth: TokenAuthServiceProxy, private _activatedRoute: ActivatedRoute, private _fileDownloadService: FileDownloadService) {
        super(injector);
    }

    getRedemptionCodes(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._redemptionCodesServiceProxy.getAll(this.filterText, this.maxRedemptionDateFilter === undefined ? this.maxRedemptionDateFilter : moment(this.maxRedemptionDateFilter).endOf('day'), this.minRedemptionDateFilter === undefined ? this.minRedemptionDateFilter : moment(this.minRedemptionDateFilter).startOf('day'), this.maxRedemptionTenantIdFilter == null ? this.maxRedemptionTenantIdFilterEmpty : this.maxRedemptionTenantIdFilter, this.minRedemptionTenantIdFilter == null ? this.minRedemptionTenantIdFilterEmpty : this.minRedemptionTenantIdFilter, this.redeemCodeCodeFilter, this.primengTableHelper.getSorting(this.dataTable), this.primengTableHelper.getSkipCount(this.paginator, event), this.primengTableHelper.getMaxResultCount(this.paginator, event)).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createRedemptionCode(): void {
        this.createOrEditRedemptionCodeModal.show();
    }


    deleteRedemptionCode(redemptionCode: RedemptionCodeDto): void {
        this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
            if (isConfirmed) {
                this._redemptionCodesServiceProxy.delete(redemptionCode.id)
                    .subscribe(() => {
                        this.reloadPage();
                        this.notify.success(this.l('SuccessfullyDeleted'));
                    });
            }
        });
    }

    exportToExcel(): void {
        this._redemptionCodesServiceProxy.getRedemptionCodesToExcel(this.filterText, this.maxRedemptionDateFilter === undefined ? this.maxRedemptionDateFilter : moment(this.maxRedemptionDateFilter).endOf('day'), this.minRedemptionDateFilter === undefined ? this.minRedemptionDateFilter : moment(this.minRedemptionDateFilter).startOf('day'), this.maxRedemptionTenantIdFilter == null ? this.maxRedemptionTenantIdFilterEmpty : this.maxRedemptionTenantIdFilter, this.minRedemptionTenantIdFilter == null ? this.minRedemptionTenantIdFilterEmpty : this.minRedemptionTenantIdFilter, this.redeemCodeCodeFilter)
            .subscribe(result => {
                this._fileDownloadService.downloadTempFile(result);
            });
    }


}
