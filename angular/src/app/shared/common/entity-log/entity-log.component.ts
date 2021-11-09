import { Component, Injector, Input, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EntityLogListDto, EntityLogServiceProxy, EntityLogType, PagedResultDtoOfEntityLogListDto } from '@shared/service-proxies/service-proxies';
import { toPlainObject } from 'lodash';
import { EntityLogForView, LogChangeItem } from '@app/shared/common/entity-log/entityLogForView';

@Component({
  selector: 'app-entity-log',
  templateUrl: './entity-log.component.html',
  styleUrls: ['./entity-log.component.css'],
})
export class EntityLogComponent extends AppComponentBase implements OnInit {
  @Input() entityId: number;
  @Input() entityType: EntityLogType;
  entityLogs: EntityLogForView[] = [];

  constructor(injector: Injector, private _entityLogService: EntityLogServiceProxy) {
    super(injector);
    this.getEntityLogs();
  }

  ngOnInit(): void {
    //  to();
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

    changeItems = JSON.parse(jsonData) as LogChangeItem[];
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
}
