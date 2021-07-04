import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { BalancesListComponent } from './balances-list.component';

describe('BalancesListComponent', () => {
  let component: BalancesListComponent;
  let fixture: ComponentFixture<BalancesListComponent>;

  beforeEach(
    waitForAsync(() => {
      TestBed.configureTestingModule({
        declarations: [BalancesListComponent],
      }).compileComponents();
    })
  );

  beforeEach(() => {
    fixture = TestBed.createComponent(BalancesListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
