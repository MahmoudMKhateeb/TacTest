import {
  PriceOfferChannel,
  ShippingRequestFlag,
  ShippingRequestRouteType,
  ShippingRequestStatus,
  ShippingRequestType,
} from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
export class ShippingRequestForPriceOfferGetAllInput {
  filter!: string;
  carrier!: string;
  shippingRequestId!: number;
  directRequestId!: number;
  channel!: PriceOfferChannel;
  requestType!: ShippingRequestType;
  requestFlag!: ShippingRequestFlag;
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
  isTMS: boolean = false;
}
