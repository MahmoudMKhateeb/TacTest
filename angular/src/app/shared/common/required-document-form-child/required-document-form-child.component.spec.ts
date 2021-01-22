import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RequiredDocumentFormChildComponent } from './required-document-form-child.component';

describe('RequiredDocumentFormChildComponent', () => {
  let component: RequiredDocumentFormChildComponent;
  let fixture: ComponentFixture<RequiredDocumentFormChildComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RequiredDocumentFormChildComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RequiredDocumentFormChildComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
