import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { PricePackageProposalServiceProxy, ProposalForViewDto } from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { FileDownloadService } from '@shared/utils/file-download.service';

@Component({
  selector: 'app-view-price-package-proposal',
  templateUrl: './view-price-package-proposal.component.html',
  styleUrls: ['./view-price-package-proposal.component.css'],
})
export class ViewPricePackageProposalComponent extends AppComponentBase {
  proposal: ProposalForViewDto;
  @ViewChild('ViewProposalModal') viewProposalModal: ModalDirective;
  isModalActive = false;

  constructor(
    private injector: Injector,
    private _proposalServiceProxy: PricePackageProposalServiceProxy,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  show(proposalId: number) {
    this.isModalActive = true;
    this.proposal = new ProposalForViewDto();
    this._proposalServiceProxy.getForView(proposalId).subscribe((result) => {
      this.proposal = result;
    });
    this.viewProposalModal.show();
  }

  close(): void {
    this.proposal = new ProposalForViewDto();
    this.isModalActive = false;
    this.viewProposalModal.hide();
  }

  downloadProposalFile(proposalFileId: string) {
    this._fileDownloadService.downloadFileByBinary(proposalFileId, this.proposal.proposalName, 'application/pdf');
  }
}
