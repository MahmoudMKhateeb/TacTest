import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-most-used-destinations',
  templateUrl: './most-used-destinations.component.html',
  styleUrls: ['./most-used-destinations.component.css'],
})
export class MostUsedDestinationsComponent implements OnInit {
  data = [
    { city: 'Jeddah', numberOfRequests: 22 },
    { city: 'Jeddah', numberOfRequests: 23 },
    { city: 'Jeddah', numberOfRequests: 24 },
    { city: 'Jeddah', numberOfRequests: 25 },
    { city: 'Jeddah', numberOfRequests: 26 },
  ];

  constructor() {}

  ngOnInit(): void {}
}
