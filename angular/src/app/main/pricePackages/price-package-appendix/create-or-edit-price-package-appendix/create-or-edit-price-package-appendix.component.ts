import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import {
  CreateOrEditAppendixDto,
  PricePackageAppendixServiceProxy,
  PricePackageProposalServiceProxy,
  SelectItemDto,
  ShippersForDropDownDto,
  ShippingRequestsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';
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
  shipperId: number;
  proposals: SelectItemDto[];
  proposalsLoading: boolean;
  shippersLoading: boolean;

  constructor(
    private _appendixServiceProxy: PricePackageAppendixServiceProxy,
    private _shippingRequestServiceProxy: ShippingRequestsServiceProxy,
    private _proposalServiceProxy: PricePackageProposalServiceProxy,
    private injector: Injector
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.isActive = false;
    this.isLoading = false;
    this.proposalsLoading = false;
    this.shippersLoading = false;
  }

  show(id: number) {
    this.isActive = true;
    this.appendix = new CreateOrEditAppendixDto();

    if (isNotNullOrUndefined(id)) {
      this._appendixServiceProxy.getForEdit(id).subscribe((result) => {
        this.appendix = result;
        this.shipperId = result.shipperId;
        this.loadProposals();
      });
    }
    this.loadAllShippers();
    this.modal.show();
  }

  createOrEdit() {
    this.isLoading = true;

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
    if (isNotNullOrUndefined(this.shipperId)) {
      this.proposalsLoading = true;
      this._proposalServiceProxy
        .getAllProposalsForDropdown(this.shipperId)
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

  close() {
    this.isActive = false;
    this.appendix = new CreateOrEditAppendixDto();
    this.modal.hide();
  }
}
