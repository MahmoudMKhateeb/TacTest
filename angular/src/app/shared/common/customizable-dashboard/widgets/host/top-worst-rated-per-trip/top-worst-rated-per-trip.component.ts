import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { GetTopOWorstRatedTenantsOutput, TMSAndHostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { TopWorstEnum } from '@app/shared/common/customizable-dashboard/widgets/host/top-worst-rated-per-trip/top-worst-enum';
import { EditionTypesForTopWorst } from '@app/shared/common/customizable-dashboard/widgets/host/top-worst-rated-per-trip/edition-types-for-top-worst';

@Component({
  selector: 'app-top-worst-rated-per-trip',
  templateUrl: './top-worst-rated-per-trip.component.html',
  styleUrls: ['./top-worst-rated-per-trip.component.css'],
})
export class TopWorstRatedPerTripComponent extends AppComponentBase implements OnInit {
  topWorstRated: GetTopOWorstRatedTenantsOutput[] = [];
  loading = false;
  rateTypes: any[] = [];
  rateType: number = TopWorstEnum.Top;
  editionType: number = EditionTypesForTopWorst.Shipper;
  editionTypes: any[] = [];

  constructor(
    private injector: Injector,
    private enumService: EnumToArrayPipe,
    private _TMSAndHostDashboardServiceProxy: TMSAndHostDashboardServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.rateTypes = this.enumService.transform(TopWorstEnum).map((item) => {
      item.key = Number(item.key);
      return item;
    });
    this.editionTypes = this.enumService.transform(EditionTypesForTopWorst).map((item) => {
      item.key = Number(item.key);
      return item;
    });
    this.getData();
  }

  getData() {
    this.loading = true;
    this._TMSAndHostDashboardServiceProxy.getTopOWorstRatedTenants(this.rateType, this.editionType).subscribe((result) => {
      this.topWorstRated = result;
      this.loading = false;
    });
  }
}
