import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import {
  CreateOrEditPenaltyDto,
  GetAllCompanyForDropDownDto,
  GetAllWaybillsDto,
  PenaltiesServiceProxy,
  PriceOfferCommissionType,
} from '@shared/service-proxies/service-proxies';
import { isNotNullOrUndefined } from 'codelyzer/util/isNotNullOrUndefined';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'createOrEditPenaltyModal',
  templateUrl: './create-or-edit-penalty-modal.component.html',
  styleUrls: ['./create-or-edit-penalty-modal.component.css'],
})
export class CreateOrEditPenaltyModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  allCompanies: GetAllCompanyForDropDownDto[];
  active = false;
  saving: boolean;
  form: CreateOrEditPenaltyDto;
  priceOfferCommissionType: any;
  CompanyPrice: number;
  CompanyVatAmount: number;
  TotalCompanyPrice: number;
  DestinationCompanyPrice: number;
  DestinationCompanyVatAmount: number;
  TotalDestinationCompanyPrice: number;
  CommissionAmount: number;
  Allwaybills: GetAllWaybillsDto[] = [];

  constructor(inject: Injector, private _PenaltiesServiceProxy: PenaltiesServiceProxy, private enumToArray: EnumToArrayPipe) {
    super(inject);
  }
  ngOnInit(): void {
    this.loadDropDowns();
    this.priceOfferCommissionType = this.enumToArray.transform(PriceOfferCommissionType);
  }
  loadDropDowns() {
    this.getAllCompaniesForDropDown();
  }
  getAllCompaniesForDropDown() {
    this._PenaltiesServiceProxy.getAllCompanyForDropDown().subscribe((res) => {
      this.allCompanies = res;
    });
  }
  save() {
    this.saving = true;
    this._PenaltiesServiceProxy
      .createOrEdit(this.form)
      .pipe(
        finalize(() => {
          this.saving = false;
          this.notify.success('SavedSuccessfully');
        })
      )
      .subscribe(() => {
        this.close();
        this.modalSave.emit();
      });
  }
  show(id?: number) {
    this.active = true;
    this.form = new CreateOrEditPenaltyDto();
    if (isNotNullOrUndefined(id)) {
      //edit
      this._PenaltiesServiceProxy.getPenaltyForEditDto(id).subscribe((res) => {
        this.form = res;
        this.Calculator();
      });
    }
    this.modal.show();
    console.log(this.form);
  }
  close() {
    this.active = false;
    this.modal.hide();
    this.CompanyPrice = undefined;
    this.CompanyVatAmount = undefined;
    this.TotalCompanyPrice = undefined;
    this.DestinationCompanyPrice = undefined;
    this.DestinationCompanyVatAmount = undefined;
    this.TotalDestinationCompanyPrice = undefined;
    this.CommissionAmount = undefined;
  }

  Calculator() {
    if (this.form.commissionType == PriceOfferCommissionType.CommissionPercentage) {
      this.CommissionAmount = (this.form.itmePrice * this.form.commissionPercentageOrAddValue) / 100;
    } else if (this.form.commissionType == PriceOfferCommissionType.CommissionValue) {
      this.CommissionAmount = this.form.itmePrice + this.form.commissionPercentageOrAddValue;
    } else if (this.form.commissionType == PriceOfferCommissionType.CommissionMinimumValue) {
      this.CommissionAmount = this.form.itmePrice + this.form.commissionPercentageOrAddValue;
    }

    this.CompanyPrice = this.form.itmePrice + this.CommissionAmount;
    this.CompanyVatAmount = (this.CompanyPrice * 15) / 100;
    this.TotalCompanyPrice = this.CompanyPrice + this.CompanyVatAmount;

    if (this.form.destinationTenantId) {
      this.DestinationCompanyPrice = this.form.itmePrice;
      this.DestinationCompanyVatAmount = (this.DestinationCompanyPrice * 15) / 100;
      this.TotalDestinationCompanyPrice = this.DestinationCompanyPrice + this.DestinationCompanyVatAmount;
    }

    this.GetWaybillsByCompany();
  }

  GetWaybillsByCompany() {
    this._PenaltiesServiceProxy.getAllWaybillsByCompany(this.form.tenantId, this.form.destinationTenantId).subscribe((res) => {
      this.Allwaybills = res;
    });
  }
}
