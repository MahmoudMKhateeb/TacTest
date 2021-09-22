import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-most-worked-with-shippers',
  templateUrl: './most-worked-with-shippers.component.html',
  styleUrls: ['./most-worked-with-shippers.component.css'],
})
export class MostWorkedWithShippersComponent {
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
}
