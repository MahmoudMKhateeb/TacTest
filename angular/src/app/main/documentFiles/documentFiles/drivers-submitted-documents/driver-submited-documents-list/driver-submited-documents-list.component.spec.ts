import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DriverSubmitedDocumentsListComponent } from './driver-submited-documents-list.component';

describe('DriverSubmitedDocumentsListComponent', () => {
  let component: DriverSubmitedDocumentsListComponent;
  let fixture: ComponentFixture<DriverSubmitedDocumentsListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DriverSubmitedDocumentsListComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DriverSubmitedDocumentsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
