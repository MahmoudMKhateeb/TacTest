import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DriverTrackingModalComponent } from './driver-tracking-modal.component';

describe('DriverTrackingModalComponent', () => {
  let component: DriverTrackingModalComponent;
  let fixture: ComponentFixture<DriverTrackingModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DriverTrackingModalComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DriverTrackingModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
