import { Component, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule, MatIconButton } from '@angular/material/button';

import { RegisterDto } from '../../shared/models/user.model';
import { AuthService } from '../../shared/services/auth.service';


@Component({
  selector: 'app-register',
  imports: [FormsModule, MatInputModule, MatButtonModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {

  dto = signal<RegisterDto>({email: '', password: ''});

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit() {
    this.authService.register(this.dto()).subscribe({
      next: (response) => {
        if (response?.token) {
          this.authService.setToken(response.token);
          this.router.navigate(['/tasks']);
        } else {
          console.error('No token in response');
        }
      },
      error: (err) => console.error('Registration failed', err)
    });
  }
}
