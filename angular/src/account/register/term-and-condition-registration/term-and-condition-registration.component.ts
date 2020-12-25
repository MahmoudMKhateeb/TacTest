import { AfterViewInit, Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { GetTermAndConditionForViewDto, ProfileServiceProxy, TenantRegistrationServiceProxy } from '@shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';
import { TenantRegistrationHelperService } from '@account/register/tenant-registration-helper.service';
import { ReCaptchaV3Service } from '@node_modules/ngx-captcha';

@Component({
  selector: 'app-term-and-condition-registration',
  templateUrl: './term-and-condition-registration.component.html',
  styleUrls: ['./term-and-condition-registration.component.css'],
})
export class TermAndConditionRegistrationComponent extends AppComponentBase implements OnInit, AfterViewInit {
  editionId: any;
  item: GetTermAndConditionForViewDto;

  constructor(injector: Injector, private _activatedRoute: ActivatedRoute, private _tenantRegistrationService: TenantRegistrationServiceProxy) {
    super(injector);
  }

  goToHome(): void {
    (window as any).location.href = '/';
  }

  ngOnInit(): void {
    this.editionId = this._activatedRoute.snapshot.queryParams['editionId'];
    this._tenantRegistrationService.getActiveTermAndConditionForViewAndApprove(this.editionId).subscribe((result) => {
      this.item = result;
      console.log(result);
    });
  }

  ngAfterViewInit(): void {
    if (this.editionId) {
      this._tenantRegistrationService.getActiveTermAndConditionForViewAndApprove(this.editionId).subscribe((result) => {
        this.item = result;
        console.log(result);
      });
    }
  }
}
