import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { GroupPeriodInfoDto, GroupPeriodServiceProxy } from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';

@Component({
  animations: [appModuleAnimation()],
  templateUrl: './group-detail.component.html',
})
export class GroupDetailComponent extends AppComponentBase {
  Data: GroupPeriodInfoDto;
  constructor(
    injector: Injector,
    private route: ActivatedRoute,
    private router: Router,
    private _CurrentService: GroupPeriodServiceProxy,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
    this.Data = this.route.snapshot.data.groupinfo;
  }

  delete(): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._CurrentService.delete(this.Data.id).subscribe(() => {
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }
  Demand(): void {
    this.Data.isDemand = true;
    this.Data.demandFileName = 'foundfile';
  }
  UnDemand(): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._CurrentService.unDemand(this.Data.id).subscribe(() => {
          this.notify.success(this.l('Successfully'));
          this.Data.isDemand = false;
          this.Data.demandFileName = '';
        });
      }
    });
  }
  Claim(): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._CurrentService.claim(this.Data.id).subscribe(() => {
          this.notify.success(this.l('Successfully'));
          this.Data.isClaim = true;
        });
      }
    });
  }
  UnClaim(): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._CurrentService.unClaim(this.Data.id).subscribe(() => {
          this.notify.success(this.l('Successfully'));
          this.Data.isClaim = false;
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
