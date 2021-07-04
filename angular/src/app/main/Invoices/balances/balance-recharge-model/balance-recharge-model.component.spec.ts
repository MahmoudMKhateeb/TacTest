import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { BalanceRechargeModelComponent } from './balance-recharge-model.component';

describe('BalanceRechargeModelComponent', () => {
  let component: BalanceRechargeModelComponent;
  let fixture: ComponentFixture<BalanceRechargeModelComponent>;

  beforeEach(
    waitForAsync(() => {
      TestBed.configureTestingModule({
        declarations: [BalanceRechargeModelComponent],
      }).compileComponents();
    })
  );

  beforeEach(() => {
    fixture = TestBed.createComponent(BalanceRechargeModelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
