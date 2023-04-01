import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class DashboardCustomizationService {
  acceptedColor = 'rgba(105, 228, 94, 0.89)';
  rejectedColor = '#d82631';
  paidColor = '#dc2434';
  unpaidColor = '#d7dadc';

  setColors(isCurrentUserBroker: boolean) {
    if (isCurrentUserBroker) {
      this.acceptedColor = '#000000';
      this.rejectedColor = '#DA1A32';
      this.paidColor = '#D7DADC';
      this.unpaidColor = '#1C1C1C';
    } else {
      this.acceptedColor = 'rgba(105, 228, 94, 0.89)';
      this.rejectedColor = '#d82631';
      this.paidColor = '#dc2434';
      this.unpaidColor = '#d7dadc';
    }
  }
}
