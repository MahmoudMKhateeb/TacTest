import { Component, EventEmitter, Injector, Input, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  CreateUserDelegationDto,
  NameValueDto,
  FindUsersInput,
  CommonLookupServiceProxy,
  UserDelegationServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { CommonLookupModalComponent } from '@app/shared/common/lookup/common-lookup-modal.component';
import { finalize } from 'rxjs/operators';
import * as moment from 'moment';
import { DateType } from '../common/hijri-gregorian-datepicker/consts';
import { NgForm } from '@angular/forms';
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'createNewUserDelegation',
  templateUrl: './create-new-user-delegation-modal.component.html',
})
export class CreateNewUserDelegationModalComponent extends AppComponentBase {
  @ViewChild('userDelegationModal', { static: true }) modal: ModalDirective;
  @ViewChild('userLookupModal', { static: true }) userLookupModal: CommonLookupModalComponent;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;
  selectedUsername = '';
  startTime: any;
  endTime: any;
  selectedDateType: DateType = DateType.Hijri; // or DateType.Gregorian
  @Input() parentForm: NgForm;
  @ViewChild('userForm', { static: false }) userForm: NgForm;
  minGreg: NgbDateStruct;
  minHijri: NgbDateStruct;
  maxGreg: NgbDateStruct;
  maxHijri: NgbDateStruct;

  userDelegation: CreateUserDelegationDto = new CreateUserDelegationDto();

  constructor(
    injector: Injector,
    private _userDelegationService: UserDelegationServiceProxy,
    private _commonLookupService: CommonLookupServiceProxy
  ) {
    super(injector);
  }

  validateDates($event: NgbDateStruct, type) {
    if (type == 'startTime') this.startTime = $event;
    if (type == 'endTime') this.endTime = $event;

    this.minGreg = this.dateFormatterService.ToGregorianDateStruct(this.startTime, 'D/M/YYYY');
    this.minHijri = this.dateFormatterService.ToHijriDateStruct(this.startTime, 'D/M/YYYY');

    this.maxGreg = this.dateFormatterService.ToGregorianDateStruct(this.endTime, 'D/M/YYYY');
    this.maxHijri = this.dateFormatterService.ToHijriDateStruct(this.endTime, 'D/M/YYYY');
  }

  show(): void {
    this.active = true;
    this.userDelegation = new CreateUserDelegationDto();

    this.userLookupModal.configure({
      title: this.l('SelectAUser'),
      dataSource: (skipCount: number, maxResultCount: number, filter: string, tenantId?: number) => {
        let input = new FindUsersInput();
        input.filter = filter;
        input.excludeCurrentUser = true;
        input.maxResultCount = maxResultCount;
        input.skipCount = skipCount;
        input.tenantId = tenantId;
        return this._commonLookupService.findUsers(input);
      },
    });

    this.modal.show();
  }

  showCommonLookupModal(): void {
    this.userLookupModal.show();
  }

  userSelected(user: NameValueDto): void {
    this.userDelegation.targetUserId = parseInt(user.value);
    this.selectedUsername = user.name;
  }

  save(): void {
    this.saving = true;

    let input = new CreateUserDelegationDto();
    input.targetUserId = this.userDelegation.targetUserId;
    //input.startTime = moment(this.userDelegation.startTime).startOf('day');
    input.endTime = moment(this.userDelegation.endTime).endOf('day');
    if (this.startTime != null && this.startTime != undefined)
      input.startTime = this.GetGregorianAndhijriFromDatepickerChange(this.startTime).GregorianDate.startOf('day');

    this._userDelegationService
      .delegateNewUser(input)
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
    this.selectedUsername = '';
    this.modal.hide();
  }
}
