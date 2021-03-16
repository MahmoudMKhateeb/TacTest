import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BalanceRechargeModelComponent } from './balance-recharge-model.component';

describe('BalanceRechargeModelComponent', () => {
  let component: BalanceRechargeModelComponent;
  let fixture: ComponentFixture<BalanceRechargeModelComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [BalanceRechargeModelComponent],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BalanceRechargeModelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
