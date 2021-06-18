import { PriceOfferChannel, ShippingRequestRouteType, ShippingRequestStatus } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
export class ShippingRequestForPriceOfferGetAllInput {
  filter!: string;
  shippingRequestId!: number;
  channel!: PriceOfferChannel;
  truckTypeId!: number;
  originId!: number;
  destinationId!: number;
  fromDate!: moment.Moment | undefined;
  toDate!: moment.Moment | undefined;
  pickupFromDate!: moment.Moment | undefined;
  pickupToDate!: moment.Moment | undefined;
  routeTypeId!: ShippingRequestRouteType;
  status!: ShippingRequestStatus;
  isTachyonDeal: boolean;
}
