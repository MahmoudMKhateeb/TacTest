import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { BrokerDashboardServiceProxy, BrokerInvoiceType, NextDueDateDto } from '@shared/service-proxies/service-proxies';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

@Component({
  selector: 'app-actor-next-invoice-due-date',
  templateUrl: './actor-next-invoice-due-date.component.html',
  styleUrls: ['./actor-next-invoice-due-date.component.scss'],
})
export class ActorNextInvoiceDueDateComponent extends AppComponentBase implements OnInit {
  invoices: NextDueDateDto[] = [];
  brokerInvoiceTypes: any[] = [];
  selectedBrokerInvoiceType: BrokerInvoiceType;

  constructor(injector: Injector, private brokerDashboardServiceProxy: BrokerDashboardServiceProxy, private enumService: EnumToArrayPipe) {
    super(injector);
  }

  ngOnInit(): void {
    this.brokerInvoiceTypes = this.enumService.transform(BrokerInvoiceType).map((item) => {
      item.key = Number(item.key);
      return item;
    });
    this.selectedBrokerInvoiceType = this.brokerInvoiceTypes[0].key;
    this.fetchData();
  }

  fetchData() {
    this.brokerDashboardServiceProxy.getNextInvoicesDueDate(this.selectedBrokerInvoiceType).subscribe((res) => {
      this.invoices = res;
    });
  }
}
