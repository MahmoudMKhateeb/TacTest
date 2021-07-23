import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViweTruckDocumentsModalComponent } from './viwe-truck-documents-modal.component';

describe('ViweTruckDocumentsModalComponent', () => {
  let component: ViweTruckDocumentsModalComponent;
  let fixture: ComponentFixture<ViweTruckDocumentsModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ViweTruckDocumentsModalComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ViweTruckDocumentsModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
