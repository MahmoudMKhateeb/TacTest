import { ShippingRequestRouteType, ShippingRequestTripStatus } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
export class TrackingSearchInput {
  shipper!: string;
  carrier!: string;
  WaybillNumber!: number;
  transportTypeId!: number;
  truckTypeId!: number;
  truckCapacityId!: number;
  originId!: number;
  destinationId!: number;
  fromDate!: moment.Moment | undefined;
  toDate!: moment.Moment | undefined;
  pickupFromDate!: moment.Moment | undefined;
  pickupToDate!: moment.Moment | undefined;
  routeTypeId!: ShippingRequestRouteType;
  status!: ShippingRequestTripStatus;
  shippingRequestReferance!: string;
  packingTypeId!: number;
  goodsOrSubGoodsCategoryId!: number;
  plateNumberId!: string;
  driverNameOrMobile!: string;
  deliveryFromDate!: moment.Moment | undefined;
  deliveryToDate!: moment.Moment | undefined;
  containerNumber!: string;
  isInvoiceIssued!: boolean;
  isSubmittedPOD!: boolean;
  requestTypeId!: number;
}
