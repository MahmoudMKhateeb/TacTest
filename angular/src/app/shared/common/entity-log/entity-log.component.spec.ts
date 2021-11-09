import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EntityLogComponent } from './entity-log.component';

describe('EntityLogComponent', () => {
  let component: EntityLogComponent;
  let fixture: ComponentFixture<EntityLogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EntityLogComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EntityLogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
