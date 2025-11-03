import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
}

export interface RefreshTokenRequest {
  accessToken: string;
  refreshToken: string;
}

@Injectable({
  providedIn: 'root'
})
/* Serwis autoryzacji */
export class AuthService {
  private readonly apiUrl = 'http://localhost:5088/api/auth';

  constructor(private readonly http: HttpClient) {}

  /* Loguje użytkownika */
  login(email: string, password: string): Observable<LoginResponse> {
    const request: LoginRequest = { email, password };
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, request);
  }

  /* Odświeża token */
  refreshToken(): Observable<LoginResponse> {
    const accessToken = this.getAccessToken();
    const refreshToken = this.getRefreshToken();
    
    if (!accessToken || !refreshToken) {
      throw new Error('No tokens available');
    }

    const request: RefreshTokenRequest = { accessToken, refreshToken };
    return this.http.post<LoginResponse>(`${this.apiUrl}/refresh`, request);
  }

  /* Zapisuje token w local storage */
  saveTokens(accessToken: string, refreshToken: string): void {
    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);
  }

  /* Pobiera access token z local storage */
  getAccessToken(): string | null {
    return localStorage.getItem('accessToken');
  }

  /* Pobiera refresh token z local storage */
  getRefreshToken(): string | null {
    return localStorage.getItem('refreshToken');
  }

  /* Usuwa token z local storage */
  logout(): void {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
  }

  /* Sprawdza czy jest token w local storage */
  isAuthenticated(): boolean {
    return !!this.getAccessToken();
  }
}
