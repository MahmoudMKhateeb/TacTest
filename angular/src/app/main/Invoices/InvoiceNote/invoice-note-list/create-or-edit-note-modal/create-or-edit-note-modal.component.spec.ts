import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateOrEditNoteModalComponent } from './create-or-edit-note-modal.component';

describe('CreateOrEditNoteModalComponent', () => {
  let component: CreateOrEditNoteModalComponent;
  let fixture: ComponentFixture<CreateOrEditNoteModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CreateOrEditNoteModalComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateOrEditNoteModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
