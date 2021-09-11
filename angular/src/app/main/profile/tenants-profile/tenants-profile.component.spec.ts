import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TenantsProfileComponent } from './tenants-profile.component';

describe('TenantsProfileComponent', () => {
  let component: TenantsProfileComponent;
  let fixture: ComponentFixture<TenantsProfileComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TenantsProfileComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TenantsProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
