import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupPeriodsListComponent } from './group-periods-list.component';

describe('GroupPeriodsListComponent', () => {
  let component: GroupPeriodsListComponent;
  let fixture: ComponentFixture<GroupPeriodsListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [GroupPeriodsListComponent],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GroupPeriodsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
