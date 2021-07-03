import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { InvoicePeriodsListComponent } from './invoice-periods-list.component';

describe('InvoicePeriodsListComponent', () => {
  let component: InvoicePeriodsListComponent;
  let fixture: ComponentFixture<InvoicePeriodsListComponent>;

  beforeEach(
    waitForAsync(() => {
      TestBed.configureTestingModule({
        declarations: [InvoicePeriodsListComponent],
      }).compileComponents();
    })
  );

  beforeEach(() => {
    fixture = TestBed.createComponent(InvoicePeriodsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
