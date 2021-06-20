import { ShippingRequestRouteType, ShippingRequestTripStatus } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
export class TrackingSearchInput {
  shipper!: string;
  carrier!: string;
  truckTypeId!: number;
  originId!: number;
  destinationId!: number;
  fromDate!: moment.Moment | undefined;
  toDate!: moment.Moment | undefined;
  pickupFromDate!: moment.Moment | undefined;
  pickupToDate!: moment.Moment | undefined;
  routeTypeId!: ShippingRequestRouteType;
  status!: ShippingRequestTripStatus;
}
