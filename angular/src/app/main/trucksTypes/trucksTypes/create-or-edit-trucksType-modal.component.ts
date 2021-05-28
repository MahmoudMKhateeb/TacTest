import { Component, ViewChild, Injector, Output, EventEmitter, OnInit } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { TrucksTypesServiceProxy, CreateOrEditTrucksTypeDto, SelectItemDto, TrucksTypesTranslationDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as _ from 'lodash';

@Component({
  selector: 'createOrEditTrucksTypeModal',
  templateUrl: './create-or-edit-trucksType-modal.component.html',
})
export class CreateOrEditTrucksTypeModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;
languages: abp.localization.ILanguageInfo[];
Translations: TrucksTypesTranslationDto[];
  trucksType: CreateOrEditTrucksTypeDto = new CreateOrEditTrucksTypeDto();
  allTransportTypes: SelectItemDto[];
  constructor(injector: Injector, private _trucksTypesServiceProxy: TrucksTypesServiceProxy) {
    super(injector);
  }
  ngOnInit(): void {
    this.languages = _.filter(this.localization.languages, (l) => l.isDisabled === false);
  }
  show(trucksTypeId?: number): void {
    this.Translations=[];
    if (!trucksTypeId) {
      this.trucksType = new CreateOrEditTrucksTypeDto();
      this.trucksType.id = trucksTypeId;
      this.trucksType.transportTypeId = null;
      this.PopulateTranslations([]);
      this.active = true;
      this.modal.show();
    } else {
      this._trucksTypesServiceProxy.getTrucksTypeForEdit(trucksTypeId).subscribe((result) => {
        this.trucksType = result.trucksType;
        this.PopulateTranslations(result.trucksType.translations);
        this.active = true;
        this.modal.show();
      });
    }
    this._trucksTypesServiceProxy.getAllTransportTypeForTableDropdown().subscribe((result) => {
      this.allTransportTypes = result;
    });
  }

 private  PopulateTranslations(Translations: TrucksTypesTranslationDto[]){
   this.languages.forEach((r)=>{
let item=new TrucksTypesTranslationDto;
item.icon=r.icon;
item.language=r.name;
item.languageDisplayName=r.displayName;
item.translatedDisplayName=_.find(Translations, (t)=>t.language==r.name)?.translatedDisplayName;
this.Translations.push(item);
   });
 }
  save(): void {
    this.saving = true;
    this.trucksType.translations=this.Translations;
    if (this.trucksType.transportTypeId == -1) {
      this.notify.error(this.l('PleaseChooseATransportType'));
      return;
    }
    this._trucksTypesServiceProxy
      .createOrEdit(this.trucksType)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
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
