import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import {
  CarriersForDropDownDto,
  CreateOrEditAppendixDto,
  PricePackageAppendixServiceProxy,
  PricePackageProposalServiceProxy,
  SelectItemDto,
  ShippersForDropDownDto,
  ShippingRequestsServiceProxy,
  TmsPricePackageServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DestinationCompanyType } from '@app/main/pricePackages/price-package-appendix/create-or-edit-price-package-appendix/destination-company-type';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';

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
    private _tmsPricePackageServiceProxy: TmsPricePackageServiceProxy,
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
        } else {
          this.currentCompanyType = DestinationCompanyType.Carrier;
          this.loadPricePackages();
        }
      });
    }
    if (type === DestinationCompanyType.Shipper) {
      this.loadAllShippers();
    } else if (type === DestinationCompanyType.Carrier) {
      this.loadAllCarriers();
    }
    this.modal.show();
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
    let self = this;
    this.dataSource = new CustomStore({
      loadMode: 'raw',
      key: 'id',
      load(loadOptions: LoadOptions) {
        return self._tmsPricePackageServiceProxy
          .getAllForDropdown(JSON.stringify(loadOptions), self.companyId, undefined, self.appendix.id)
          .toPromise()
          .then((response) => {
            return response.data;
          });
      },
    });
  }
}
