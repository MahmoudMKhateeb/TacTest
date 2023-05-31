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
  topWorstRated: GetTopOWorstRatedTenantsOutput[] = [
    // {
    //   id: 164,
    //   name: 'shipperPlus',
    //   numberOfRequests: 10,
    //   rating: 3.3,
    // },
    // {
    //   id: 165,
    //   name: 'shipperProPlus',
    //   numberOfRequests: 15,
    //   rating: 4.3,
    // },
    // {
    //   id: 166,
    //   name: 'shipperTest',
    //   numberOfRequests: 8,
    //   rating: 4.0,
    // },
    // {
    //   id: 167,
    //   name: 'shipperTestPlus',
    //   numberOfRequests: 20,
    //   rating: 5.0,
    // },
  ];
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
