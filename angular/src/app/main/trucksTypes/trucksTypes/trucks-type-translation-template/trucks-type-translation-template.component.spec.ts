import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TrucksTypeTranslationTemplateComponent } from './trucks-type-translation-template.component';

describe('TrucksTypeTranslationTemplateComponent', () => {
  let component: TrucksTypeTranslationTemplateComponent;
  let fixture: ComponentFixture<TrucksTypeTranslationTemplateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TrucksTypeTranslationTemplateComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TrucksTypeTranslationTemplateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
