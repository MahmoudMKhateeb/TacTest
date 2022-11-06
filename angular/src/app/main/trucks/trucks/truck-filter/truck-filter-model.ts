import { CarriersForDropDownDto, ISelectItemDto } from '@shared/service-proxies/service-proxies';

export class TruckFilter {
  plateNumber: string;
  creationDate: Date;
  transportTypeId: number;
  selectedCarrier: CarriersForDropDownDto[];
  selectedTruckTypes: ISelectItemDto[];
  selectedCapacity: ISelectItemDto[];
  istmaraNumber: string;
  insuranceNumber: number;
  truckStatusId: number;
}
