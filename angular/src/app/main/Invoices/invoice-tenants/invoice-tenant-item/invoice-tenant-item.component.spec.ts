import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoiceTenantItemComponent } from './invoice-tenant-item.component';

describe('InvoiceTenantItemComponent', () => {
  let component: InvoiceTenantItemComponent;
  let fixture: ComponentFixture<InvoiceTenantItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [InvoiceTenantItemComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InvoiceTenantItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
