import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  EditionSelectDto,
  EditionWithFeaturesDto,
  EditionsSelectOutput,
  FlatFeatureSelectDto,
  TenantRegistrationServiceProxy,
  EditionPaymentType,
  SubscriptionStartType,
} from '@shared/service-proxies/service-proxies';
import * as _ from 'lodash';
import { EditionHelperService } from '@account/payment/edition-helper.service';

@Component({
  templateUrl: './select-edition.component.html',
  styleUrls: ['./select-edition.component.less'],
  encapsulation: ViewEncapsulation.None,
  animations: [accountModuleAnimation()],
})
export class SelectEditionComponent extends AppComponentBase implements OnInit {
  editionsSelectOutput: EditionsSelectOutput = new EditionsSelectOutput();
  isUserLoggedIn = false;
  isSetted = false;
  editionPaymentType: typeof EditionPaymentType = EditionPaymentType;
  subscriptionStartType: typeof SubscriptionStartType = SubscriptionStartType;
  /*you can change your edition icons order within editionIcons variable */

  // editionIcons: string[] = [
  //   'flaticon-truck',
  //   'flaticon-bus-stop',
  //   'flaticon-gift',
  // ];

  constructor(
    injector: Injector,
    private _tenantRegistrationService: TenantRegistrationServiceProxy,
    private _editionHelperService: EditionHelperService,
    private _router: Router
  ) {
    super(injector);
  }

  ngOnInit() {
    this.isUserLoggedIn = abp.session.userId > 0;

    this._tenantRegistrationService.getEditionsForSelect().subscribe((result) => {
      this.editionsSelectOutput = result;

      if (!this.editionsSelectOutput.editionsWithFeatures || this.editionsSelectOutput.editionsWithFeatures.length <= 0) {
        this._router.navigate(['/account/register-tenant']);
      }
    });
  }

  isFree(edition: EditionSelectDto): boolean {
    return this._editionHelperService.isEditionFree(edition);
  }

  isTrueFalseFeature(feature: FlatFeatureSelectDto): boolean {
    return feature.inputType.name === 'CHECKBOX';
  }

  featureEnabledForEdition(feature: FlatFeatureSelectDto, edition: EditionWithFeaturesDto): boolean {
    const featureValues = _.filter(edition.featureValues, { name: feature.name });
    if (!featureValues || featureValues.length <= 0) {
      return false;
    }

    const featureValue = featureValues[0];
    return featureValue.value.toLowerCase() === 'true';
  }

  getFeatureValueForEdition(feature: FlatFeatureSelectDto, edition: EditionWithFeaturesDto): string {
    const featureValues = _.filter(edition.featureValues, { name: feature.name });
    if (!featureValues || featureValues.length <= 0) {
      return '';
    }

    const featureValue = featureValues[0];
    return featureValue.value;
  }

  upgrade(upgradeEdition: EditionSelectDto, editionPaymentType: EditionPaymentType): void {
    this._router.navigate(['/account/upgrade'], { queryParams: { upgradeEditionId: upgradeEdition.id, editionPaymentType: editionPaymentType } });
  }
}
