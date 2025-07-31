import { Component, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { FormsModule } from '@angular/forms';

import { LoginDto } from '../../shared/models/user.model';
import { AuthService } from '../../shared/services/auth.service';



@Component({
  selector: 'app-login',
  imports: [FormsModule, MatInputModule, MatButtonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {

  dto = signal<LoginDto>({email: '', password: ''});

  constructor(private authService: AuthService, private router: Router){}

  onSubmit(){
    this.authService.login(this.dto()).subscribe({
      next: (response) => {
        if (response?.token) {
          this.authService.setToken(response.token);
          this.router.navigate(['/tasks']);
      } else {
          console.error('No token in response');
      }
      },
      error: (err) => console.error('Login failed', err)
    });
  }
  
}
