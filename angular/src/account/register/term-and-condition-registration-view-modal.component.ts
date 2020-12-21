import { Component, ViewChild, Injector, Output, EventEmitter, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  GetTermAndConditionForViewDto,
  TenantRegistrationServiceProxy,
  TermAndConditionDto,
  TermAndConditionsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';

@Component({
  selector: 'termAndConditionRegisterationViewModal',
  templateUrl: './term-and-condition-registration-view-modal.component.html',
})
export class TermAndConditionRegistrationViewModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('content', { static: false }) content: ElementRef;

  active = false;
  saving = false;

  item: GetTermAndConditionForViewDto;

  constructor(injector: Injector, private _tenantRegistrationService: TenantRegistrationServiceProxy) {
    super(injector);
    this.item = new GetTermAndConditionForViewDto();
    this.item.termAndCondition = new TermAndConditionDto();
  }

  show(editionId): void {
    this._tenantRegistrationService.getActiveTermAndConditionForViewAndApprove(editionId).subscribe((result) => {
      this.item = result;
      this.active = true;
      this.modal.show();
    });
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }

  downloadAsPDF() {
    var data = document.getElementById('content');
    // the screen shot component contains a bug, it needs to scroll to the top to get a full screen shot
    window.scroll(0, 0);
    ////

    // scale gives the image its quality
    html2canvas(data, {
      scale: 2,
    }).then((canvas) => {
      // Few necessary setting options
      var imgWidth = 208;
      var pageHeight = 295;
      var imgHeight = (canvas.height * imgWidth) / canvas.width;
      var heightLeft = imgHeight;

      const contentDataURL = canvas.toDataURL();
      let pdf = new jsPDF('p', 'mm', 'a4'); // A4 size page of PDF
      var position = 0;
      pdf.addImage(contentDataURL, 'PNG', 0, position, imgWidth, imgHeight);
      pdf.save('TachyonPlatform-TermsAndConditions.pdf'); // Generated PDF
      pdf.close();
    });
  }
}
