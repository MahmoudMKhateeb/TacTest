import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActorTypesEnum, BrokerDashboardServiceProxy, GetDueDateInDaysOutput } from '@shared/service-proxies/service-proxies';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

@Component({
  selector: 'app-actor-next-doc-due-date',
  templateUrl: './actor-next-doc-due-date.component.html',
  styleUrls: ['./actor-next-doc-due-date.component.scss'],
})
export class ActorNextDocDueDateComponent extends AppComponentBase implements OnInit {
  documents: GetDueDateInDaysOutput[] = [];
  actorTypes: any[] = [];
  selectedActorShipper: ActorTypesEnum;

  constructor(injector: Injector, private brokerDashboardServiceProxy: BrokerDashboardServiceProxy, private enumService: EnumToArrayPipe) {
    super(injector);
  }

  ngOnInit(): void {
    this.actorTypes = this.enumService.transform(ActorTypesEnum).map((item) => {
      item.key = Number(item.key);
      return item;
    });
    this.selectedActorShipper = this.actorTypes[0].key;
    this.fetchData();
  }

  fetchData() {
    this.brokerDashboardServiceProxy.getNextDocumentsDueDate(this.selectedActorShipper).subscribe((res) => {
      this.documents = res;
    });
  }
}
