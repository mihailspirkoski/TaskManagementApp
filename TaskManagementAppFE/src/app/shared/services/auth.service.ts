import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';

import { LoginDto, RegisterDto } from '../models/user.model';
import { Observable } from 'rxjs';

export interface JwtClaims {
  [key: string]: any; // Allow dynamic keys
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'?: string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'?: string;
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'?: string;
}


@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl = 'https://localhost:7298/api/auth';
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

   getClaims(): JwtClaims | null {
    const token = this.token();
    if (!token) {
      return null;
    }
    try {
      const payload = token.split('.')[1]; // Get payload
      const decoded = atob(payload); // Decode Base64
      return JSON.parse(decoded) as JwtClaims;
    } catch (error) {
      console.error('Failed to decode JWT:', error);
      return null;
    }
  }

  getClaim(key: string): any {
    const claims = this.getClaims();
    return claims ? claims[key] : null;
  }

  getUserId(): string | null {
    return this.getClaim('http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier');
  }

   register(dto: RegisterDto): Observable<{token: string}>{
    return this.http.post<{token: string}>(`${this.apiUrl}/register`, dto);
   }

   login(dto: LoginDto): Observable<{token: string}>{
    return this.http.post<{token: string}>(`${this.apiUrl}/login`, dto);
   }

}
