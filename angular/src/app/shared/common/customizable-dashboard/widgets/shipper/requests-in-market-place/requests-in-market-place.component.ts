import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-requests-in-market-place',
  templateUrl: './requests-in-market-place.component.html',
  styleUrls: ['./requests-in-market-place.component.css'],
})
export class RequestsInMarketPlaceComponent implements OnInit {
  constructor() {}
  Requests = [
    { ref: '123456', endDate: '12-12-2021', numberOfOffers: 6 },
    { ref: '123456', endDate: '12-12-2021', numberOfOffers: 6 },
    { ref: '123456', endDate: '12-12-2021', numberOfOffers: 6 },
    { ref: '123456', endDate: '12-12-2021', numberOfOffers: 6 },
    { ref: '123456', endDate: '12-12-2021', numberOfOffers: 6 },
    { ref: '123456', endDate: '12-12-2021', numberOfOffers: 6 },
    { ref: '123456', endDate: '12-12-2021', numberOfOffers: 6 },
    { ref: '123456', endDate: '12-12-2021', numberOfOffers: 6 },
  ];
  ngOnInit(): void {}
}
