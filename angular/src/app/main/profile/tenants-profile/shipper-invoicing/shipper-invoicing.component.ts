import { Component, Injector, Input, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { InvoicingInformationDto, ProfileServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'profile-shipper-invoicing',
  templateUrl: './shipper-invoicing.component.html',
  styleUrls: ['./shipper-invoicing.component.css'],
})
export class ShipperInvoicingComponent extends AppComponentBase implements OnInit {
  @Input() giverUserId: number;
  InvoicingInfo: InvoicingInformationDto = new InvoicingInformationDto();
  loading = true;

  constructor(injector: Injector, private _profileServiceProxy: ProfileServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this._profileServiceProxy.getInvoicingInformation(this.giverUserId).subscribe(
      (result) => {
        this.InvoicingInfo = result;
        this.loading = false;
      },
      (error) => {
        // this._router.navigate(['/app/main/dashboard']);
      }
    );
  }
}
