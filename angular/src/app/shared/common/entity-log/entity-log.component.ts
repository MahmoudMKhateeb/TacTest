import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EntityLogListDto, EntityLogServiceProxy, EntityLogType, PagedResultDtoOfEntityLogListDto } from '@shared/service-proxies/service-proxies';
import { toPlainObject } from 'lodash';
import { EntityLogForView, LogChangeItem } from '@app/shared/common/entity-log/entityLogForView';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { isMoment } from '@node_modules/moment';

@Component({
  selector: 'app-entity-log',
  templateUrl: './entity-log.component.html',
  styleUrls: ['./entity-log.component.css'],
})
export class EntityLogComponent extends AppComponentBase implements OnInit {
  @ViewChild('modal', { static: true }) modal: ModalDirective;

  @Input() entityId: number;
  @Input() entityType: EntityLogType;
  momentHijri = isMoment;

  entityLogs: EntityLogForView[] = [];
  active = false;
  isCollapsed: boolean;
  activeId: number;

  constructor(injector: Injector, private _entityLogService: EntityLogServiceProxy) {
    super(injector);
    this.getEntityLogs();
  }

  ngOnInit(): void {
    //  to();
  }

  /**
   * opens the modal
   */
  show() {
    this.modal.show();
    this.active = true;
    this.getEntityLogs();
  }

  /**
   * closes the modal
   */
  close() {
    this.active = false;
    this.modal.hide();
  }

  getEntityLogs() {
    this._entityLogService.getAllEntityLogs(1, '945', '', 10, 0).subscribe((result) => {
      for (let dto of result.items) {
        this.entityLogs.push(this.toEntityLogForView(dto));
      }
    });
  }

  formatEntityLog(jsonData: string): LogChangeItem[] {
    let changeItems: LogChangeItem[] = [];
    console.log('jsonData', jsonData);
    changeItems = JSON.parse(jsonData);
    console.log(changeItems);
    return changeItems;
  }

  toEntityLogForView(dto: EntityLogListDto): EntityLogForView {
    let logForView: EntityLogForView = new EntityLogForView();

    logForView.transaction = dto.transaction;
    logForView.modificationTime = dto.modificationTime.format('DD-MM-yyyy h:mm a');
    logForView.modifierTenantId = dto.modifierTenantId;
    logForView.modifierUserName = dto.modifierUserName;
    logForView.modifierUserId = dto.modifierUserId;
    logForView.changesData = this.formatEntityLog(dto.changesData);

    return logForView;
  }

  objectToArray(andObject) {
    // console.log('original Data', andObject);
    let resultArray = Object.keys(andObject).map(function (namedIndex) {
      // console.log(namedIndex);
      let mz = andObject[namedIndex];
      return mz;
    });
    // console.log('resultArray', resultArray);
    return resultArray;
  }

  isValidMomentDate(input: string) {
    if (this.momentHijri(input)) {
      console.log('this is a moment:', input);
    } else {
      console.log('this is not a moment:', input);
    }
  }

  // logThis(changesData: any) {
  //   console.log('logged', changesData);
  //   // this.objectToArray(changesData).forEach((x) => {
  //   //   console.log('x', x);
  //   // });
  // }
}
