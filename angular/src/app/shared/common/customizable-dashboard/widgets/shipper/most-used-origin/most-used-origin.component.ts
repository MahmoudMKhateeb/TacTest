import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-most-used-origin',
  templateUrl: './most-used-origin.component.html',
  styleUrls: ['./most-used-origin.component.css'],
})
export class MostUsedOriginComponent implements OnInit {
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
