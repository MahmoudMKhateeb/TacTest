import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import {
  CarriersForDropDownDto,
  CreateOrEditAppendixDto,
  PricePackageAppendixServiceProxy,
  PricePackageProposalServiceProxy,
  PricePackageServiceProxy,
  SelectItemDto,
  ShippersForDropDownDto,
  ShippingRequestsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DestinationCompanyType } from '@app/main/pricePackages/price-package-appendix/create-or-edit-price-package-appendix/destination-company-type';

@Component({
  selector: 'app-create-or-edit-price-package-appendix',
  templateUrl: './create-or-edit-price-package-appendix.component.html',
  styleUrls: ['./create-or-edit-price-package-appendix.component.css'],
})
export class CreateOrEditPricePackageAppendixComponent extends AppComponentBase implements OnInit {
  appendix: CreateOrEditAppendixDto;
  @ViewChild('Modal') modal: ModalDirective;
  @Output() modalSave = new EventEmitter<void>();
  isActive: boolean;
  isLoading: boolean;
  shippers: ShippersForDropDownDto[];
  dataSource;
  companyId: number;
  proposals: SelectItemDto[];
  carriers: CarriersForDropDownDto[];
  proposalsLoading: boolean;
  shippersLoading: boolean;
  carriersLoading: boolean;
  companyType = DestinationCompanyType;
  currentCompanyType: DestinationCompanyType;

  constructor(
    private _appendixServiceProxy: PricePackageAppendixServiceProxy,
    private _shippingRequestServiceProxy: ShippingRequestsServiceProxy,
    private _proposalServiceProxy: PricePackageProposalServiceProxy,
    private _pricePackageServiceProxy: PricePackageServiceProxy,
    private injector: Injector
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.isActive = false;
    this.isLoading = false;
    this.proposalsLoading = false;
    this.shippersLoading = false;
    this.carriersLoading = false;
  }

  show(id: number, type: DestinationCompanyType = null) {
    this.isActive = true;
    this.appendix = new CreateOrEditAppendixDto();

    this.currentCompanyType = type;
    if (isNotNullOrUndefined(id)) {
      this._appendixServiceProxy.getForEdit(id).subscribe((result) => {
        this.appendix = result;
        this.companyId = result.destinationCompanyId;
        if (isNotNullOrUndefined(result.proposalId)) {
          this.currentCompanyType = DestinationCompanyType.Shipper;
          this.loadProposals();
          this.loadAllShippers();
        } else {
          this.currentCompanyType = DestinationCompanyType.Carrier;
          this.loadPricePackages();
          this.loadAllCarriers();
        }
        this.modal.show();
      });
    }
    if (!this.appendix.id && this.currentCompanyType === DestinationCompanyType.Shipper) {
      this.loadAllShippers();
    } else if (!this.appendix.id && this.currentCompanyType === DestinationCompanyType.Carrier) {
      this.loadAllCarriers();
    }

    if (!this.appendix.id) {
      this.modal.show();
    }
  }

  createOrEdit() {
    this.isLoading = true;

    this.appendix.destinationCompanyId = this.companyId;

    this._appendixServiceProxy
      .createOrEdit(this.appendix)
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe(() => {
        this.notify.success(this.l('SavedSuccess'));
        this.modalSave.emit();
        this.close();
      });
  }

  loadProposals(): void {
    if (isNotNullOrUndefined(this.companyId)) {
      this.proposalsLoading = true;
      this._proposalServiceProxy
        .getAllProposalsForDropdown(this.companyId, this.appendix.id)
        .pipe(finalize(() => (this.proposalsLoading = false)))
        .subscribe((result) => {
          this.proposals = result?.items;
        });
    }
  }

  private loadAllShippers(): void {
    this.shippersLoading = true;
    this._shippingRequestServiceProxy
      .getAllShippersForDropDown()
      .pipe(finalize(() => (this.shippersLoading = false)))
      .subscribe((res) => {
        this.shippers = res;
      });
  }

  private loadAllCarriers(): void {
    this.carriersLoading = true;
    this._shippingRequestServiceProxy
      .getAllCarriersForDropDown()
      .pipe(finalize(() => (this.carriersLoading = false)))
      .subscribe((res) => {
        this.carriers = res;
      });
  }

  close() {
    this.isActive = false;
    this.appendix = new CreateOrEditAppendixDto();
    this.companyId = undefined;
    this.modal.hide();
  }

  loadPricePackages() {
    this._pricePackageServiceProxy.getPricePackagesForCarrierAppendix(this.companyId, this.appendix.id).subscribe((result) => {
      this.dataSource = result.items;
    });
  }

  autoFillByProposal(proposalId: number) {
    this._proposalServiceProxy.getProposalAutoFillDetails(proposalId).subscribe((result) => {
      this.appendix.appendixDate = result.appendixDate;
      this.appendix.notes = result.notes;
      this.appendix.scopeOverview = result.scopeOverview;
    });
  }
}
