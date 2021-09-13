import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class ProfileComponent implements OnInit {
  constructor(private _Activatedroute: ActivatedRoute) {}

  ngOnInit(): void {
    this._Activatedroute.params.subscribe((params) => {
      console.log('The Tenants Profile Component: ', +params['id']);
      console.log(+params['id']);
      // (+) converts string 'id' to a number
      // In a real app: dispatch action to load the details here.
    });
  }
}
