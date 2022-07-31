import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { NgModel } from '@angular/forms';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import {
  CreateOrEditPenaltyDto,
  GetAllCompanyForDropDownDto,
  PenaltiesServiceProxy,
  PenaltyItemDto,
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
  CommissionAmount: number = 0;
  Allwaybills: PenaltyItemDto[] = [];
  FilteredWaybills: PenaltyItemDto[] = [];
  SelectedWaybills: PenaltyItemDto[] = [];
  TaxVat: number;
  waybillsLoading = false;
  newAttribute: any = {};

  constructor(inject: Injector, private _PenaltiesServiceProxy: PenaltiesServiceProxy, private enumToArray: EnumToArrayPipe) {
    super(inject);
  }
  ngOnInit(): void {
    this.loadDropDowns();
    this.priceOfferCommissionType = this.enumToArray.transform(PriceOfferCommissionType);
    this._PenaltiesServiceProxy
      .getTaxVat()
      .pipe(
        finalize(() => {
          this.CalculatePrices();
        })
      )
      .subscribe((res) => {
        this.TaxVat = res / 100;
      });
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
    this.form.penaltyItems = this.SelectedWaybills.map(
      (item) =>
        new PenaltyItemDto({
          id: item.id,
          itemPrice: item.itemPrice,
          itemTotalAmountPostVat: item.itemTotalAmountPostVat,
          shippingRequestTripId: item.shippingRequestTripId,
          vatAmount: item.vatAmount,
          waybillNumber: item.waybillNumber,
        })
    );
    this.saving = true;
    console.log(this.form.penaltyItems);
    this._PenaltiesServiceProxy
      .createOrEdit(this.form)
      .pipe(
        finalize(() => {
          this.saving = false;
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
        this.SelectedWaybills = res.penaltyItems;
        this.CalculatePrices();
        this.GetWaybillsByCompany();
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
    this.form = undefined;
    this.SelectedWaybills = [];
    this.Allwaybills = [];
    this.FilteredWaybills = [];
    this.newAttribute = {};
  }

  // Calculator() {
  //   // this._PenaltiesServiceProxy
  //   //   .getTaxVat()
  //   //   .pipe(
  //   //     finalize(() => {
  //   //       this.CalculatePrices();
  //   //     })
  //   //   )
  //   //   .subscribe((res) => {
  //   //     this.TaxVat = res / 100;
  //   //   });
  //   this.CalculatePrices();
  // }

  // calculateItemPrice(itemPrice:number, index:number){
  //   console.log(this.TaxVat);
  //   var vatAmount=itemPrice*this.TaxVat;
  //   this.Allwaybills[index].vatAmount=vatAmount;
  //   this.Allwaybills[index].itemTotalAmountPostVat=itemPrice+vatAmount;
  //   this.CalculatePrices();
  // }

  // Calculator(){
  //   if(this.TaxVat==null){
  //     this._PenaltiesServiceProxy
  //     .getTaxVat().pipe(finalize(()=> {
  //       this.CalculatePrices();
  //     }))
  //     .subscribe((res) => {
  //       this.TaxVat = res / 100;
  //     });
  // }
  //   }

  CalculatePrices() {
    // this.SelectedWaybills = this.Allwaybills.filter((item) => {
    //   return item.checked; });

    this.form.itmePrice = this.SelectedWaybills.reduce((prev, next) => prev + next.itemPrice, 0);

    //calculate commission if there is destination company
    if (this.form.destinationTenantId && this.form.commissionType == PriceOfferCommissionType.CommissionPercentage) {
      this.CommissionAmount = (this.form.itmePrice * this.form.commissionPercentageOrAddValue) / 100;
    } else if (
      this.form.commissionType == PriceOfferCommissionType.CommissionValue ||
      this.form.commissionType == PriceOfferCommissionType.CommissionMinimumValue
    ) {
      this.CommissionAmount = this.form.commissionPercentageOrAddValue;
    }

    //calculate prices
    if (!this.form.destinationTenantId) {
      this.CompanyPrice = this.form.itmePrice;
      this.CompanyVatAmount = this.CompanyPrice * this.TaxVat;
      this.TotalCompanyPrice = this.CompanyPrice + this.CompanyVatAmount;
    } else {
      this.CompanyPrice = this.form.itmePrice + this.CommissionAmount;
      this.CompanyVatAmount = this.CompanyPrice * this.TaxVat;
      this.TotalCompanyPrice = this.CompanyPrice + this.CompanyVatAmount;

      this.DestinationCompanyPrice = this.form.itmePrice;
      this.DestinationCompanyVatAmount = this.DestinationCompanyPrice * this.TaxVat;
      this.TotalDestinationCompanyPrice = this.DestinationCompanyPrice + this.DestinationCompanyVatAmount;
    }

    // this.GetWaybillsByCompany();
  }

  GetWaybillsByCompany() {
    this.waybillsLoading = true;
    this._PenaltiesServiceProxy.getAllWaybillsByCompany(this.form.tenantId, this.form.destinationTenantId).subscribe((res) => {
      this.Allwaybills = res;
      this.waybillsLoading = false;
    });
    console.log(this.Allwaybills);
    this.CalculatePrices();
  }

  addFieldValue() {
    //this.newAttribute.checked = true;
    if (this.newAttribute.penaltyItemDto != null) {
      this.newAttribute.shippingRequestTripId = this.newAttribute.penaltyItemDto.shippingRequestTripId;
      this.newAttribute.waybillNumber = this.newAttribute.penaltyItemDto.waybillNumber;
    }
    this.SelectedWaybills.push(this.newAttribute);
    console.log('selecteWaybills', this.SelectedWaybills);
    this.newAttribute = {};
    this.CalculatePrices();
  }

  deleteFieldValue(index) {
    this.SelectedWaybills.splice(index, 1);
    this.CalculatePrices();
  }

  calculateNewPrice(newPrice: number): void {
    var newVatAmount: number;
    newVatAmount = newPrice * this.TaxVat;
    this.newAttribute.vatAmount = newVatAmount;
    var newTotalAmount = newPrice + newVatAmount;
    this.newAttribute.itemTotalAmountPostVat = newTotalAmount;
  }

  filterWaybills(event) {
    this.FilteredWaybills = this.Allwaybills.filter(
      (item) =>
        item.waybillNumber.toString().includes(event.query.toLowerCase()) || item.id?.toString().toLowerCase().includes(event.query.toLowerCase())
    );
  }

  isInvalidAutoComplete(waybillnumberAutoComplete: NgModel) {
    if (!waybillnumberAutoComplete.value) {
      return false;
    }
    return (
      this.FilteredWaybills.filter((item) => waybillnumberAutoComplete.value && item.waybillNumber !== waybillnumberAutoComplete.value?.waybillNumber)
        .length > 0
    );
  }

  isValidAutoComplete(waybillnumberAutoComplete: NgModel) {
    return (
      this.FilteredWaybills.filter(
        (item) => waybillnumberAutoComplete.value == '' || item.waybillNumber === waybillnumberAutoComplete.value?.wayBillNumber
      ).length > 0
    );
  }
}
