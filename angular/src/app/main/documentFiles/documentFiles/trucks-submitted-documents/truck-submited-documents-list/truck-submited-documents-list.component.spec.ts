import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TruckSubmitedDocumentsListComponent } from './truck-submited-documents-list.component';

describe('TruckSubmitedDocumentsListComponent', () => {
  let component: TruckSubmitedDocumentsListComponent;
  let fixture: ComponentFixture<TruckSubmitedDocumentsListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TruckSubmitedDocumentsListComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TruckSubmitedDocumentsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
