import { Component, Injector, OnInit } from '@angular/core';
import { PricePackageProposalServiceProxy, ProposalStatus } from '@shared/service-proxies/service-proxies';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { AppComponentBase } from '@shared/common/app-component-base';
import Swal from 'sweetalert2';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

@Component({
  selector: 'app-price-packages-proposal',
  templateUrl: './price-packages-proposal.component.html',
  styleUrls: ['./price-packages-proposal.component.css'],
})
export class PricePackagesProposalComponent extends AppComponentBase implements OnInit {
  constructor(
    private injector: Injector,
    private _pricePackagesProposalServiceProxy: PricePackageProposalServiceProxy,
    private enumToArrayPipe: EnumToArrayPipe
  ) {
    super(injector);
  }

  dataSource: any;
  proposalStatuses = this.enumToArrayPipe.transform(ProposalStatus);
  proposalStatus = ProposalStatus;

  ngOnInit(): void {
    this.getAllPricePackagesProposals();
  }

  getAllPricePackagesProposals() {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        return self._pricePackagesProposalServiceProxy
          .getAll(JSON.stringify(loadOptions))
          .toPromise()
          .then((response) => {
            return {
              data: response.data,
              totalCount: response.totalCount,
              summary: response.summary,
              groupCount: response.groupCount,
            };
          })
          .catch((error) => {
            throw new Error('Data Loading Error');
          });
      },
    });
  }

  Accept(id: number) {
    Swal.fire({
      title: 'Are you sure?',
      text: 'Please Confirm Accepting The Proposal',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Yes, Accept it!',
    }).then((result) => {
      if (result.isConfirmed) {
        //Accept Service Goes Here (input is Ready in input)
        this._pricePackagesProposalServiceProxy.confirmProposal(id).subscribe(() => {
          Swal.fire('Accepted!', 'Proposal Were Successfully Accepted.', 'success');
          this.getAllPricePackagesProposals();
        });
      }
    });
  }

  Reject(id: number) {
    Swal.fire({
      title: 'Are you sure?',
      text: 'Please Confirm Rejecting The Proposal',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Yes, Reject it!',
    }).then((result) => {
      if (result.isConfirmed) {
        //Reject Service Goes Here (input is Ready in input)
        this._pricePackagesProposalServiceProxy.rejectProposal(id).subscribe(() => {
          Swal.fire('Rejected!', 'Proposal Were Successfully Rejected.', 'success');
          this.getAllPricePackagesProposals();
        });
      }
    });
  }

  getStatusDisplayName(options): string {
    let tableStatusId = options.data.status;
    let displayName = this.proposalStatuses.find((x) => x.key == tableStatusId).value;
    return this.l(displayName);
  }
}
