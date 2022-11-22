import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import {
  CreateOrEditProposalDto,
  PricePackageProposalServiceProxy,
  ShippersForDropDownDto,
  ShippingRequestsServiceProxy,
  TmsPricePackageServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';

@Component({
  selector: 'app-create-or-edit-price-packege-proposal',
  templateUrl: './create-or-edit-price-packege-proposal.component.html',
  styleUrls: ['./create-or-edit-price-packege-proposal.component.css'],
})
export class CreateOrEditPricePackegeProposalComponent extends AppComponentBase implements OnInit {
  @ViewChild('Modal') modal: ModalDirective;
  @Output() modalSave: EventEmitter<void> = new EventEmitter<void>();
  isFormActive: boolean;
  isFormLoading: boolean;
  pricePackageProposal: CreateOrEditProposalDto;
  shippers: ShippersForDropDownDto[];
  dataSource: any;

  constructor(
    injector: Injector,
    private _shippingRequestServiceProxy: ShippingRequestsServiceProxy,
    private _proposalServiceProxy: PricePackageProposalServiceProxy,
    private _tmsPricePackageServiceProxy: TmsPricePackageServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.pricePackageProposal = new CreateOrEditProposalDto();
    this.loadAllShippers();
  }

  show(id?: number) {
    //this is edit
    if (id) {
      this.pricePackageProposal.id = id;
      this._proposalServiceProxy.getForEdit(id).subscribe((res) => {
        this.pricePackageProposal = res;
        this.loadAllPricePkgForTable();
      });
    }
    this.isFormActive = true;
    this.modal.show();
  }

  close() {
    this.pricePackageProposal = new CreateOrEditProposalDto();
    this.isFormActive = false;
    this.modal.hide();
  }

  /**
   * loads All Shippers
   * @private
   */
  private loadAllShippers(): void {
    this._shippingRequestServiceProxy.getAllShippersForDropDown().subscribe((res) => {
      this.shippers = res;
    });
  }

  loadAllPricePkgForTable(): CustomStore {
    let self = this;
    return (this.dataSource = new CustomStore({
      loadMode: 'raw',
      key: 'id',
      load(loadOptions: LoadOptions) {
        return self._tmsPricePackageServiceProxy
          .getAllForDropdown(JSON.stringify(loadOptions), self.pricePackageProposal.shipperId, self.pricePackageProposal.id)
          .toPromise()
          .then((response) => {
            return response.data;
          });
      },
    }));
  }

  createOrEdit() {
    this.isFormLoading = true;
    this._proposalServiceProxy.createOrEdit(this.pricePackageProposal).subscribe(() => {
      this.notify.success(this.l('Success'));
      this.isFormLoading = false;
      this.modalSave.emit();
      this.close();
    });
  }
}
