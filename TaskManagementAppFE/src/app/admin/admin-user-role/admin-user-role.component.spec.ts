import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminUserRoleComponent } from './admin-user-role.component';

describe('AdminUserRoleComponent', () => {
  let component: AdminUserRoleComponent;
  let fixture: ComponentFixture<AdminUserRoleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminUserRoleComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminUserRoleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
