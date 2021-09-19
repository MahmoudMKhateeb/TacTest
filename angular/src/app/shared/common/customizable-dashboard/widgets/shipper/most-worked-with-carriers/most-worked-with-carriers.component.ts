import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-most-worked-with-carriers',
  templateUrl: './most-worked-with-carriers.component.html',
  styleUrls: ['./most-worked-with-carriers.component.css'],
})
export class MostWorkedWithCarriersComponent implements OnInit {
  Carriers = [
    { name: 'Mahmoud', rating: 4.5, tripsCount: 4 },
    { name: 'Mahmoud', rating: 4.5, tripsCount: 4 },
    { name: 'Mahmoud', rating: 4.5, tripsCount: 4 },
    { name: 'Mahmoud', rating: 4.5, tripsCount: 4 },
    { name: 'Mahmoud', rating: 4.5, tripsCount: 4 },
    { name: 'Mahmoud', rating: 4.5, tripsCount: 4 },
    { name: 'Mahmoud', rating: 4.5, tripsCount: 4 },
    { name: 'Mahmoud', rating: 4.5, tripsCount: 4 },
  ];

  constructor() {}

  ngOnInit(): void {}
}
