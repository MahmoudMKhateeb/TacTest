import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { GroupPeriodInfoDto, GroupPeriodServiceProxy, SubmitInvoiceStatus, SubmitInvoiceItemDto } from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';

@Component({
  animations: [appModuleAnimation()],
  templateUrl: './group-detail.component.html',
})
export class GroupDetailComponent extends AppComponentBase {
  Data: GroupPeriodInfoDto;
  Items: SubmitInvoiceItemDto[];
  TotalItems: number;
  constructor(
    injector: Injector,
    private route: ActivatedRoute,
    private router: Router,
    private _CurrentService: GroupPeriodServiceProxy,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
    this.Data = this.route.snapshot.data.groupinfo;
    this.Items = this.Data.items;
    this.TotalItems = this.Items.length;
  }

  delete(): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._CurrentService.delete(this.Data.id).subscribe(() => {
          this.notify.success(this.l('SuccessfullyDeleted'));
          this.router.navigate([`/app/main/invoices/groupperiods`]);
        });
      }
    });
  }
  Claim(): void {
    this.Data.status = SubmitInvoiceStatus.Claim;
    this.Data.documentName = 'foundfile';
  }
  Rejected(): void {
    this.Data.status = SubmitInvoiceStatus.Rejected;
  }

  Accepted(): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._CurrentService.accepted(this.Data.id).subscribe(() => {
          this.notify.success(this.l('Successfully'));
          this.Data.status = SubmitInvoiceStatus.Accepted;
        });
      }
    });
  }

  downloadDocument(): void {
    this._CurrentService.getFileDto(this.Data.id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
}
