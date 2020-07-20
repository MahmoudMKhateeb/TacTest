import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { RoutStepsServiceProxy, CreateOrEditRoutStepDto ,RoutStepCityLookupTableDto
					,RoutStepRouteLookupTableDto
					} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
    selector: 'createOrEditRoutStepModal',
    templateUrl: './create-or-edit-routStep-modal.component.html'
})
export class CreateOrEditRoutStepModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    routStep: CreateOrEditRoutStepDto = new CreateOrEditRoutStepDto();

    cityDisplayName = '';
    cityDisplayName2 = '';
    routeDisplayName = '';

	allCitys: RoutStepCityLookupTableDto[];
						allRoutes: RoutStepRouteLookupTableDto[];
					
    constructor(
        injector: Injector,
        private _routStepsServiceProxy: RoutStepsServiceProxy
    ) {
        super(injector);
    }

    show(routStepId?: number): void {

        if (!routStepId) {
            this.routStep = new CreateOrEditRoutStepDto();
            this.routStep.id = routStepId;
            this.cityDisplayName = '';
            this.cityDisplayName2 = '';
            this.routeDisplayName = '';

            this.active = true;
            this.modal.show();
        } else {
            this._routStepsServiceProxy.getRoutStepForEdit(routStepId).subscribe(result => {
                this.routStep = result.routStep;

                this.cityDisplayName = result.cityDisplayName;
                this.cityDisplayName2 = result.cityDisplayName2;
                this.routeDisplayName = result.routeDisplayName;

                this.active = true;
                this.modal.show();
            });
        }
        this._routStepsServiceProxy.getAllCityForTableDropdown().subscribe(result => {						
						this.allCitys = result;
					});
					this._routStepsServiceProxy.getAllCityForTableDropdown().subscribe(result => {						
						this.allCitys = result;
					});
					this._routStepsServiceProxy.getAllRouteForTableDropdown().subscribe(result => {						
						this.allRoutes = result;
					});
					
    }

    save(): void {
            this.saving = true;

			
            this._routStepsServiceProxy.createOrEdit(this.routStep)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }







    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
