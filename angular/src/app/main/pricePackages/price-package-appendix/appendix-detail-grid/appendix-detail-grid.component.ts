import {Component, Injector, Input, OnInit} from '@angular/core';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import {LoadOptions} from '@node_modules/devextreme/data/load_options';
import {isNotNullOrUndefined} from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import {NormalPricePackagesServiceProxy, TmsPricePackageServiceProxy} from '@shared/service-proxies/service-proxies';
import {AppComponentBase} from '@shared/common/app-component-base';

@Component({
    selector: 'app-appendix-detail-grid',
    templateUrl: './appendix-detail-grid.component.html',
    styleUrls: ['./appendix-detail-grid.component.css']
})
export class AppendixDetailGridComponent extends AppComponentBase implements OnInit {
    pricePackageDataSource: any;
    @Input() appendixId: number;

    constructor(private _injector: Injector,
                private _tmsPricePackageServiceProxy: TmsPricePackageServiceProxy) {
        super(_injector);
    }

    ngOnInit(): void {
        let self = this;
        this.pricePackageDataSource = {};
        this.pricePackageDataSource.store = new CustomStore({
            load(loadOptions: LoadOptions) {
                if (!isNotNullOrUndefined(loadOptions)) {
                    loadOptions = {};
                }
                loadOptions.filter = [];
                (loadOptions.filter as any[]).push(['AppendixId', '=', self.appendixId]);
                return self._tmsPricePackageServiceProxy
                    .getAppendixPricePackages(JSON.stringify(loadOptions))
                    .toPromise()
                    .then((result) => {
                        return {
                            data: result.data,
                            totalCount: result.totalCount,
                            summary: result.summary,
                            groupCount: result.groupCount,
                        };
                    })
                    .catch(() => {
                        throw new Error('Data Loading Error');
                    });
            },
        });

    }
}
