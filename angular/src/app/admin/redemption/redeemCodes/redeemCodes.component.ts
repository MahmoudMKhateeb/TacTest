import { Component, Injector, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { RedeemCodeDto, RedeemCodesServiceProxy, TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditRedeemCodeModalComponent } from './create-or-edit-redeemCode-modal.component';

import { ViewRedeemCodeModalComponent } from './view-redeemCode-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as moment from 'moment';
import { LazyLoadEvent } from 'primeng/api';


@Component({
    templateUrl: './redeemCodes.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()],
})
export class RedeemCodesComponent extends AppComponentBase {


    @ViewChild('createOrEditRedeemCodeModal', { static: true }) createOrEditRedeemCodeModal: CreateOrEditRedeemCodeModalComponent;
    @ViewChild('viewRedeemCodeModalComponent', { static: true }) viewRedeemCodeModal: ViewRedeemCodeModalComponent;

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    codeFilter = '';
    maxExpiryDateFilter: moment.Moment;
    minExpiryDateFilter: moment.Moment;
    isActiveFilter = -1;
    maxValueFilter: number;
    maxValueFilterEmpty: number;
    minValueFilter: number;
    minValueFilterEmpty: number;
    noteFilter = '';
    maxpercentageFilter: number;
    maxpercentageFilterEmpty: number;
    minpercentageFilter: number;
    minpercentageFilterEmpty: number;


    constructor(injector: Injector, private _redeemCodesServiceProxy: RedeemCodesServiceProxy, private _notifyService: NotifyService, private _tokenAuth: TokenAuthServiceProxy, private _activatedRoute: ActivatedRoute, private _fileDownloadService: FileDownloadService) {
        super(injector);
    }

    getRedeemCodes(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._redeemCodesServiceProxy.getAll(this.filterText, this.codeFilter, this.maxExpiryDateFilter === undefined ? this.maxExpiryDateFilter : moment(this.maxExpiryDateFilter).endOf('day'), this.minExpiryDateFilter === undefined ? this.minExpiryDateFilter : moment(this.minExpiryDateFilter).startOf('day'), this.isActiveFilter, this.maxValueFilter == null ? this.maxValueFilterEmpty : this.maxValueFilter, this.minValueFilter == null ? this.minValueFilterEmpty : this.minValueFilter, this.noteFilter, this.maxpercentageFilter == null ? this.maxpercentageFilterEmpty : this.maxpercentageFilter, this.minpercentageFilter == null ? this.minpercentageFilterEmpty : this.minpercentageFilter, this.primengTableHelper.getSorting(this.dataTable), this.primengTableHelper.getSkipCount(this.paginator, event), this.primengTableHelper.getMaxResultCount(this.paginator, event)).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createRedeemCode(): void {
        this.createOrEditRedeemCodeModal.show();
    }


    deleteRedeemCode(redeemCode: RedeemCodeDto): void {
        this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
            if (isConfirmed) {
                this._redeemCodesServiceProxy.delete(redeemCode.id)
                    .subscribe(() => {
                        this.reloadPage();
                        this.notify.success(this.l('SuccessfullyDeleted'));
                    });
            }
        });
    }

    exportToExcel(): void {
        this._redeemCodesServiceProxy.getRedeemCodesToExcel(this.filterText, this.codeFilter, this.maxExpiryDateFilter === undefined ? this.maxExpiryDateFilter : moment(this.maxExpiryDateFilter).endOf('day'), this.minExpiryDateFilter === undefined ? this.minExpiryDateFilter : moment(this.minExpiryDateFilter).startOf('day'), this.isActiveFilter, this.maxValueFilter == null ? this.maxValueFilterEmpty : this.maxValueFilter, this.minValueFilter == null ? this.minValueFilterEmpty : this.minValueFilter, this.noteFilter, this.maxpercentageFilter == null ? this.maxpercentageFilterEmpty : this.maxpercentageFilter, this.minpercentageFilter == null ? this.minpercentageFilterEmpty : this.minpercentageFilter)
            .subscribe(result => {
                this._fileDownloadService.downloadTempFile(result);
            });
    }


}
