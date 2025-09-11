import { HttpClient } from '@angular/common/http';
import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../shared/services/auth.service';


export interface ChangeUserRole{
  role: string;
}


@Component({
  selector: 'app-admin-user-role',
  imports: [FormsModule, MatInputModule, MatButtonModule, MatSelectModule],
  templateUrl: './admin-user-role.component.html',
  styleUrl: './admin-user-role.component.scss'
})
export class AdminUserRoleComponent implements OnInit {

  userId = signal<number>(0);
  role = signal<string>('Client');
  roles = ['Admin', 'Client', 'Subscriber'];
  error = signal<string | null>(null);

  constructor(
    private http: HttpClient,
    private authService: AuthService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    if (!this.authService.getToken()) {
      this.router.navigate(['/login']);
      return;
    }
    const try1= this.authService.getUserId();
    this.userId.set(try1 ? parseInt(try1) : 0);
    const try2 = this.authService.getClaim('http://schemas.microsoft.com/ws/2008/06/identity/claims/role');
    this.role.set(try2 ? try2 : 'Client');
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.userId.set(+id);
    }
  }

  onSubmit() {
    const dto: ChangeUserRole = { role: this.role() };
    this.http.put(`https://localhost:7298/api/admin/users/${this.userId()}/${this.role()}`, dto).subscribe({
      next: () => this.router.navigate(['/admin/users']),
      error: (err) => {
        if (err.status === 401) {
          this.authService.setToken(null);
          this.router.navigate(['/login']);
        } else {
          this.error.set('Failed to change role: ' + err.message);
        }
      }
    });
  }

}
