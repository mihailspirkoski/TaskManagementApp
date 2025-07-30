import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';

import { LoginDto, RegisterDto } from '../models/user.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl = 'http://localhost:5179/api/auth';
  private token = signal<string | null>(null);

  constructor(private http: HttpClient) {
    const savedToken = localStorage.getItem('token');
    if(savedToken)
      this.token.set(savedToken);
   }

   getToken(){
    return this.token.asReadonly();
   }

   setToken(token: string | null){
    this.token.set(token);
    if(token){
      localStorage.setItem('token', token);
    }else {
      localStorage.removeItem('token');
    }
   }

   register(dto: RegisterDto): Observable<{token: string}>{
    return this.http.post<{token: string}>(`${this.apiUrl}/register`, dto);
   }

   login(dto: LoginDto): Observable<{token: string}>{
    return this.http.post<{token: string}>(`${this.apiUrl}/login`, dto);
   }

}
