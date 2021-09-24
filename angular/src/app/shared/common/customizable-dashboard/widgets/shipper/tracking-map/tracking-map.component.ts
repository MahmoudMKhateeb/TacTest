import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-tracking-map',
  templateUrl: './tracking-map.component.html',
  styleUrls: ['./tracking-map.component.scss'],
})
export class TrackingMapComponent {
  zoom: 12;
  public markerOptions = {
    origin: {
      icon: 'https://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=•|2e5daa',
    },
    destination: {
      icon: 'https://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=•|2e5daa',
    },
    waypoints: { icon: 'https://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=•|2e5daa' },
  };
  public markerOptions2 = {
    origin: {
      icon: 'https://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=•|9604f3|2',
    },
    destination: {
      icon: 'https://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=•|9604f3|2',
    },
    waypoints: { icon: 'https://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=•|9604f3|2' },
  };
  route2 = [
    { latitude: 24.6925674, longitude: 46.6694302 },
    { latitude: 24.7429006, longitude: 46.6791872 },
    { latitude: 24.6754904, longitude: 46.6680345 },
    { latitude: 24.6773488, longitude: 46.7443408 },
    { latitude: 24.806488, longitude: 46.61516179999999 },
  ];
  route1 = [
    { latitude: 24.7429006, longitude: 46.6791872 },
    { latitude: 24.6925674, longitude: 46.6694302 },
    { latitude: 24.6966132, longitude: 46.7019495 },
    { latitude: 24.6925674, longitude: 46.6694302 },
  ];
  waypoints1 = [];
  waypoints2 = [];
  source1 = {
    lat: this.route1[0].latitude || undefined,
    lng: this.route1[0]?.longitude || undefined,
  };
  source2 = {
    lat: this.route2[0].latitude || undefined,
    lng: this.route2[0]?.longitude || undefined,
  };
  dest1 = {
    lat: this.route1[this.route1.length - 1].latitude || undefined,
    lng: this.route1[this.route1.length - 1]?.longitude || undefined,
  };
  dest2 = {
    lat: this.route2[this.route2.length - 1].latitude || undefined,
    lng: this.route2[this.route2.length - 1]?.longitude || undefined,
  };
  constructor() {
    for (let i = 1; i < this.route1.length - 1; i++) {
      this.waypoints1.push({
        location: {
          lat: this.route1[i].latitude,
          lng: this.route1[i].longitude,
        },
      });
    }
    for (let i = 1; i < this.route1.length - 1; i++) {
      this.waypoints2.push({
        location: {
          lat: this.route2[i].latitude,
          lng: this.route2[i].longitude,
        },
      });
    }
  }
  activeTrips = [
    {
      id: 1,
      origin: 'Riyadh',
      Dest: 'Riyadh',
      status: 'In Transit',
      waybillNumber: '2322132Fake121231',
    },
    {
      id: 2,
      origin: 'Riyadh',
      Dest: 'Riyadh',
      status: 'In Transit',
      waybillNumber: '2322132Fake121231',
    },
  ];

  trips = [
    { id: 1, source: 'Riyadh', dest: 'Riyadh', status: 'in Transit', waybillNumber: 23322112325589 },
    { id: 2, source: 'Riyadh', dest: 'Riyadh', status: 'in Transit', waybillNumber: 23322112325589 },
    { id: 3, source: 'Riyadh', dest: 'Riyadh', status: 'in Transit', waybillNumber: 23322112325589 },
    { id: 4, source: 'Riyadh', dest: 'Riyadh', status: 'in Transit', waybillNumber: 23322112325589 },
    { id: 5, source: 'Riyadh', dest: 'Riyadh', status: 'in Transit', waybillNumber: 23322112325589 },
    { id: 6, source: 'Riyadh', dest: 'Riyadh', status: 'in Transit', waybillNumber: 23322112325589 },
    { id: 7, source: 'Riyadh', dest: 'Riyadh', status: 'in Transit', waybillNumber: 23322112325589 },
    { id: 8, source: 'Riyadh', dest: 'Riyadh', status: 'in Transit', waybillNumber: 23322112325589 },
    { id: 9, source: 'Riyadh', dest: 'Riyadh', status: 'in Transit', waybillNumber: 23322112325589 },
    { id: 10, source: 'Riyadh', dest: 'Riyadh', status: 'in Transit', waybillNumber: 23322112325589 },
  ];
  renderOptions1: { polylineOptions: { strokeColor: '#9604f3' } };
  renderOptions2: { polylineOptions: { strokeColor: '#0f0e15' } };
}
