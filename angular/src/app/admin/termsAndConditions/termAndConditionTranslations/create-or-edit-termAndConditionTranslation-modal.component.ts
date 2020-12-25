import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { TermAndConditionTranslationsServiceProxy, CreateOrEditTermAndConditionTranslationDto ,TermAndConditionTranslationTermAndConditionLookupTableDto
					} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditTermAndConditionTranslationModal',
    templateUrl: './create-or-edit-termAndConditionTranslation-modal.component.html'
})
export class CreateOrEditTermAndConditionTranslationModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    termAndConditionTranslation: CreateOrEditTermAndConditionTranslationDto = new CreateOrEditTermAndConditionTranslationDto();

    termAndConditionTitle = '';

	allTermAndConditions: TermAndConditionTranslationTermAndConditionLookupTableDto[];
					
    constructor(
        injector: Injector,
        private _termAndConditionTranslationsServiceProxy: TermAndConditionTranslationsServiceProxy
    ) {
        super(injector);
    }
    
    show(termAndConditionTranslationId?: number): void {
    

        if (!termAndConditionTranslationId) {
            this.termAndConditionTranslation = new CreateOrEditTermAndConditionTranslationDto();
            this.termAndConditionTranslation.id = termAndConditionTranslationId;
            this.termAndConditionTitle = '';

            this.active = true;
            this.modal.show();
        } else {
            this._termAndConditionTranslationsServiceProxy.getTermAndConditionTranslationForEdit(termAndConditionTranslationId).subscribe(result => {
                this.termAndConditionTranslation = result.termAndConditionTranslation;

                this.termAndConditionTitle = result.termAndConditionTitle;

                this.active = true;
                this.modal.show();
            });
        }
        this._termAndConditionTranslationsServiceProxy.getAllTermAndConditionForTableDropdown().subscribe(result => {						
						this.allTermAndConditions = result;
					});
					
    }

    save(): void {
            this.saving = true;

			
			
            this._termAndConditionTranslationsServiceProxy.createOrEdit(this.termAndConditionTranslation)
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
