import { Injectable, PLATFORM_ID, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { AuthResponse, LoginRequest, RegisterRequest, TokenInfo, UserProfile } from '../models/auth.model';
import { isPlatformBrowser } from '@angular/common';

declare var google: any;

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly TOKEN_KEY = 'auth_tokens';
  private readonly API_URL = `${environment.apiUrl}/auth`;
  
  private currentUserSubject = new BehaviorSubject<UserProfile | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    this.loadStoredUser();
  }

  private getLocalStorage(): Storage | null {
    return isPlatformBrowser(this.platformId) ? window.localStorage : null;
  }

  private getAuthHeaders(): HttpHeaders {
    const tokens = this.getStoredTokens();
    return new HttpHeaders({
      'Authorization': `Bearer ${tokens?.accessToken || ''}`
    });
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.API_URL}/login`, request)
      .pipe(
        tap(response => this.handleAuthResponse(response))
      );
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.API_URL}/register`, request)
      .pipe(
        tap(response => this.handleAuthResponse(response))
      );
  }

  logout(): void {
    const storage = this.getLocalStorage();
    if (storage) {
      storage.removeItem(this.TOKEN_KEY);
    }
    this.currentUserSubject.next(null);
    this.router.navigate(['/auth/login']);
  }

  refreshToken(): Observable<AuthResponse> {
    const tokens = this.getStoredTokens();
    if (!tokens?.refreshToken) {
      throw new Error('No refresh token available');
    }

    return this.http.post<AuthResponse>(`${this.API_URL}/refresh-token`, {
      refreshToken: tokens.refreshToken
    }).pipe(
      tap(response => this.handleAuthResponse(response))
    );
  }

  getStoredTokens(): TokenInfo | null {
    const storage = this.getLocalStorage();
    if (!storage) return null;
    
    const tokens = storage.getItem(this.TOKEN_KEY);
    return tokens ? JSON.parse(tokens) : null;
  }

  isAuthenticated(): boolean {
    const tokens = this.getStoredTokens();
    if (!tokens) return false;
    
    return tokens.expiresAt > Date.now();
  }

  private handleAuthResponse(response: AuthResponse): void {
    const tokenInfo: TokenInfo = {
      accessToken: response.accessToken,
      refreshToken: response.refreshToken,
      expiresAt: new Date(response.expiresAt).getTime()
    };
    
    const storage = this.getLocalStorage();
    if (storage) {
      storage.setItem(this.TOKEN_KEY, JSON.stringify(tokenInfo));
    }

    if (response.user) {
      this.currentUserSubject.next(response.user);
    } else {
      this.loadUserProfile();
    }
  }

  private loadUserProfile(): void {
    this.http.get<UserProfile>(`${this.API_URL}/profile`, {
      headers: this.getAuthHeaders()
    }).subscribe({
      next: (profile) => {
        this.currentUserSubject.next(profile);
      },
      error: (error) => {
        console.error('Error loading user profile:', error);
        if (error.status === 401) {
          this.logout();
        }
      }
    });
  }

  private loadStoredUser(): void {
    if (this.isAuthenticated()) {
      this.loadUserProfile();
    }
  }

  loginWithGoogle(): void {
    if (isPlatformBrowser(this.platformId)) {
      google.accounts.id.initialize({
        client_id: environment.googleClientId,
        callback: this.handleGoogleSignIn.bind(this)
      });

      google.accounts.id.prompt();
    }
  }

  private handleGoogleSignIn(response: any): void {
    window.location.href = `${this.API_URL}/google-login`;
  }
} 