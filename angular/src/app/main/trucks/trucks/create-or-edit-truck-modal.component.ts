import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { TrucksServiceProxy, CreateOrEditTruckDto ,TruckTrucksTypeLookupTableDto
					,TruckTruckStatusLookupTableDto
					} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { TruckUserLookupTableModalComponent } from './truck-user-lookup-table-modal.component';

@Component({
    selector: 'createOrEditTruckModal',
    templateUrl: './create-or-edit-truck-modal.component.html'
})
export class CreateOrEditTruckModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('truckUserLookupTableModal', { static: true }) truckUserLookupTableModal: TruckUserLookupTableModalComponent;
    @ViewChild('truckUserLookupTableModal2', { static: true }) truckUserLookupTableModal2: TruckUserLookupTableModalComponent;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    truck: CreateOrEditTruckDto = new CreateOrEditTruckDto();

    trucksTypeDisplayName = '';
    truckStatusDisplayName = '';
    userName = '';
    userName2 = '';

	allTrucksTypes: TruckTrucksTypeLookupTableDto[];
						allTruckStatuss: TruckTruckStatusLookupTableDto[];
					
    constructor(
        injector: Injector,
        private _trucksServiceProxy: TrucksServiceProxy
    ) {
        super(injector);
    }

    show(truckId?: string): void {

        if (!truckId) {
            this.truck = new CreateOrEditTruckDto();
            this.truck.id = truckId;
            this.truck.licenseExpirationDate = moment().startOf('day');
            this.trucksTypeDisplayName = '';
            this.truckStatusDisplayName = '';
            this.userName = '';
            this.userName2 = '';

            this.active = true;
            this.modal.show();
        } else {
            this._trucksServiceProxy.getTruckForEdit(truckId).subscribe(result => {
                this.truck = result.truck;

                this.trucksTypeDisplayName = result.trucksTypeDisplayName;
                this.truckStatusDisplayName = result.truckStatusDisplayName;
                this.userName = result.userName;
                this.userName2 = result.userName2;

                this.active = true;
                this.modal.show();
            });
        }
        this._trucksServiceProxy.getAllTrucksTypeForTableDropdown().subscribe(result => {						
						this.allTrucksTypes = result;
					});
					this._trucksServiceProxy.getAllTruckStatusForTableDropdown().subscribe(result => {						
						this.allTruckStatuss = result;
					});
					
    }

    save(): void {
            this.saving = true;

			
            this._trucksServiceProxy.createOrEdit(this.truck)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }

    openSelectUserModal() {
        this.truckUserLookupTableModal.id = this.truck.driver1UserId;
        this.truckUserLookupTableModal.displayName = this.userName;
        this.truckUserLookupTableModal.show();
    }
    openSelectUserModal2() {
        this.truckUserLookupTableModal2.id = this.truck.driver2UserId;
        this.truckUserLookupTableModal2.displayName = this.userName;
        this.truckUserLookupTableModal2.show();
    }


    setDriver1UserIdNull() {
        this.truck.driver1UserId = null;
        this.userName = '';
    }
    setDriver2UserIdNull() {
        this.truck.driver2UserId = null;
        this.userName2 = '';
    }


    getNewDriver1UserId() {
        this.truck.driver1UserId = this.truckUserLookupTableModal.id;
        this.userName = this.truckUserLookupTableModal.displayName;
    }
    getNewDriver2UserId() {
        this.truck.driver2UserId = this.truckUserLookupTableModal2.id;
        this.userName2 = this.truckUserLookupTableModal2.displayName;
    }


    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
