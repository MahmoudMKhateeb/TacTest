import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DriversSubmittedDocumentsComponent } from './drivers-submitted-documents.component';

describe('DriversSubmittedDocumentsComponent', () => {
  let component: DriversSubmittedDocumentsComponent;
  let fixture: ComponentFixture<DriversSubmittedDocumentsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DriversSubmittedDocumentsComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DriversSubmittedDocumentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
