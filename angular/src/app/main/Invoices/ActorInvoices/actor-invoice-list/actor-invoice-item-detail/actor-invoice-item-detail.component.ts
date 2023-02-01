import {Component, Injector, Input, OnInit} from '@angular/core';
import {AppComponentBase} from '@shared/common/app-component-base';
import {
    ActorInvoiceServiceProxy,
    InvoiceInfoDto,
    SubmitInvoiceInfoDto
} from '@shared/service-proxies/service-proxies';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import {LoadOptions} from '@node_modules/devextreme/data/load_options';

@Component({
  selector: 'app-actor-invoice-item-detail',
  templateUrl: './actor-invoice-item-detail.component.html',
  styleUrls: ['./actor-invoice-item-detail.component.css']
})
export class ActorInvoiceItemDetailComponent extends AppComponentBase implements OnInit {

    actorInvoiceItemsDataSource: any;
    @Input() actorInvoiceId: number;
    actorInvoiceInfo: InvoiceInfoDto;

    constructor(private _injector: Injector, private _actorInvoiceServiceProxy: ActorInvoiceServiceProxy) {
        super(_injector);
    }

    ngOnInit(): void {
        this.actorInvoiceInfo = new InvoiceInfoDto();

        let self = this;
        this.actorInvoiceItemsDataSource = {};
        this.actorInvoiceItemsDataSource.store = new CustomStore({
            load(loadOptions: LoadOptions) {
                console.log(JSON.stringify(loadOptions));
                return self._actorInvoiceServiceProxy
                    .getActorInvoiceDetails(self.actorInvoiceId)
                    .toPromise()
                    .then((response) => {
                        self.actorInvoiceInfo = response;
                        return {
                            data: response.items,
                            totalAmount: response.totalAmount,
                        };
                    })
                    .catch((error) => {
                        console.log(error);
                        throw new Error('Data Loading Error');
                    });
            },
        });
    }

}
