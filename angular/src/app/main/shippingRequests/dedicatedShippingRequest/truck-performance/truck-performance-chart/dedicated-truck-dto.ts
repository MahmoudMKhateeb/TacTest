import { ReplacementFlag } from '@shared/service-proxies/service-proxies';

export class DedicatedTruckDto {
  id: number;
  kpi: number;
  numberOfTrips: number;
  truckType: string;
  status: string;
  plateNumber: string;
  capacity: string;
  shippingRequestReference: string;
  carrierName: string;
  duration: string;
  isRequestedToReplace: boolean;
  replacementDate: string;
  replacementReason: string;
  replacementFlag: ReplacementFlag;
  replacementIntervalInDays: number;
  originalDedicatedTruckId: number;
  originalDedicatedTruckName: string;
}
