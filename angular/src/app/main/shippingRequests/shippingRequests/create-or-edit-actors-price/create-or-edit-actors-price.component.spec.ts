import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateOrEditActorsPriceComponent } from './create-or-edit-actors-price.component';

describe('CreateOrEditActorsPriceComponent', () => {
  let component: CreateOrEditActorsPriceComponent;
  let fixture: ComponentFixture<CreateOrEditActorsPriceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CreateOrEditActorsPriceComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateOrEditActorsPriceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
