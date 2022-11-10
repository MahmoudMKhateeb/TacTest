import { Component, ViewChild, Injector, Output, EventEmitter, OnInit } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';

import { CommonLookupServiceProxy, ISelectItemDto, SelectItemDto, ActorInvoiceServiceProxy, ActorsServiceProxy, ActorTypesEnum } from '@shared/service-proxies/service-proxies';
@Component({
  selector: 'Actor-Invoices-ondemand-model',
  templateUrl: './Actor-Invoices-ondemand-model.component.html',
  styleUrls: ['./Actor-Invoices-ondemand-model.component.scss']
})
export class ActorInvoiceDemandModelComponent extends AppComponentBase  {
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;

  active = false;
  saving = false;
  Actor: number;
  AllActors: ISelectItemDto[];
  Waybills: SelectItemDto[];
  SelectedWaybills: SelectItemDto[];
  ActorType: ActorTypesEnum;

  constructor(injector: Injector, private _actorService: ActorsServiceProxy) {
    super(injector);
  }


  show(): void {
    this.Actor = undefined;
    this.Waybills = undefined;
    this.SelectedWaybills = undefined;
    this.active = true;
    //this.GetAllActors();
    this.modal.show();
  }

  save(): void {
    this.saving = true;

    if (this.Actor) {
    } else {
      this.Actor = undefined;
    }
    if (!this.Actor) return;

    //check if normal invoice
      this._actorService
        .generateActorInvoice(this.Actor, this.SelectedWaybills)
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
    this.ActorType=undefined;
    this.AllActors=[];
    this.SelectedWaybills=[];
    this.modal.hide();
  }

  // GetAllActors() {
  //   this._actorService.getAllActorsForDropDown(this.ActorType).subscribe((result) => {
  //     this.AllActors = result;
  //   });
  // }

  GetActorsByType(){
    this._actorService.getAllActorsForDropDown(this.ActorType).subscribe((result) => {
      this.AllActors = result;
    });
  }

  GetAllWaybillsForActor(): void {    
    this._actorService.getAllUnInvoicedWaybillsForActor(this.Actor).subscribe((res) => {
      this.Waybills = res;
    });
  }
}
