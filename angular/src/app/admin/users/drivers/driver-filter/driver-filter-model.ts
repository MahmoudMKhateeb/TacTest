import { CarriersForDropDownDto } from '@shared/service-proxies/service-proxies';

/**
 *  "accountNumber": "D005340",
 *  "name": "Abdullah",
 *  "surname": "Mohayddeen",
 *  "userName": "Abdullah",
 *  "emailAddress": "551337923@Tachyonhub.com",
 *  "phoneNumber": "551337923",
 *  "profilePictureId": null,
 *  "isEmailConfirmed": true,
 *  "companyName": "Tachyonhub",
 *  "nationality": "Emirati",
 *  "isActive": true,
 *  "isMissingDocumentFiles": true,
 *  "creationTime": "2022-03-07T18:08:22.5013566Z",
 *  "dateOfBirth": "1977-03-11T12:00:00Z",
 *  "rate": 4.90,
 *  "rentedStatus": "Active",
 *  "rentedShippingRequestReference": "",
 *  "assignedTruckId": 0,
 *  "assignedTruck": "",
 *  "id": 340
 */

export class DriverFilter {
  creationTime: Date;
  driverName: string;
  isActive: string;
  accountNumber: string;
  phoneNumber: string;
  selectedCarriers: CarriersForDropDownDto[];
}
