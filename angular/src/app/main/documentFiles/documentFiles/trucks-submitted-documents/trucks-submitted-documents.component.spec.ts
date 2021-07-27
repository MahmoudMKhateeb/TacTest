import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TrucksSubmittedDocumentsComponent } from './trucks-submitted-documents.component';

describe('TrucksSubmittedDocumentsComponent', () => {
  let component: TrucksSubmittedDocumentsComponent;
  let fixture: ComponentFixture<TrucksSubmittedDocumentsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TrucksSubmittedDocumentsComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TrucksSubmittedDocumentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
