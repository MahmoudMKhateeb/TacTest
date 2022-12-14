export class DedicatedTruckModel {
  id: number;
  capacity: string;
  carrierName: string;
  duration: string;
  plateNumber: string;
  shippingRequestReference: string;
  status: string;
  truckType: string;
  kpi: number;
  numberOfTrips: number;
  isRequestedToReplace: boolean;
  replacementDate: string;
  replacementReason: string;
  replacementFlag: number;
  replacementIntervalInDays: number;
  originalDedicatedTruckId: number;
  originalDedicatedTruckName: string;
  invoiceId: number;
  submitInvoiceId: number;
}
