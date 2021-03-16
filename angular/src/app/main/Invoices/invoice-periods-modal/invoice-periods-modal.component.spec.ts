import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoicePeriodsModalComponent } from './invoice-periods-modal.component';

describe('InvoicePeriodsModalComponent', () => {
  let component: InvoicePeriodsModalComponent;
  let fixture: ComponentFixture<InvoicePeriodsModalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [InvoicePeriodsModalComponent],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InvoicePeriodsModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
