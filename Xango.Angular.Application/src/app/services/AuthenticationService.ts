// src/app/services/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { ServiceSettings } from './ServiceSettings';
import { ResponseDto } from '../dto/ResponseDto';
import { catchError, map, tap } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { UserRecord } from '../dto/UserRecord';
import { UserResponse } from '../dto/UserResponse';
import { RegistrationRequest } from '../dto/RegistrationRequest';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = ServiceSettings.AUTH_API;
  private tokenKey = 'auth_token';
  private userKey = 'user_value';

  private userResponse: UserResponse | any = null;

  constructor(
    private http: HttpClient, 
    private toastr: ToastrService
  ) {}

  login(userName: string, password: string): Observable<ResponseDto> {
    const response = this.http
      .post<ResponseDto>(`${this.apiUrl}/api/auth/login`, { userName, password })
      .pipe(
        tap((response) => {
          if (!response.isSuccess) {
            throw new Error(response.message);
          }
          this.userResponse = response.result;
          this.setToken(this.userResponse.token);
          this.setUser(this.userResponse.user);
          return response;
        }),
        catchError((error: HttpErrorResponse) => {
          console.error('Login error:', error);
          return throwError(() => error);
        }),
      );
    return response;
  }

  register(registrationRequest: RegistrationRequest): Observable<ResponseDto> {
    return this.http
      .post<ResponseDto>(`${this.apiUrl}/api/auth/register`, registrationRequest)
      .pipe(
        tap((responseDto) => {
          if (!responseDto.isSuccess) {
            throw new Error(responseDto.message);
          }
        }),
        catchError((error) => {
          console.error('Registration error:', error);
          return throwError(() => error);
        }),
      );
  }

  logout(): void {
    this.clearToken();
  }

  // === token helpers ===
  setToken(token: string): void {
    localStorage.setItem(this.tokenKey, token); // or sessionStorage
  }

  setUser(user: UserRecord): void {
    localStorage.setItem(this.userKey, JSON.stringify(this.userResponse.user));
  }

  public getUser(): UserRecord | null {
    let userRecordString: string | any = localStorage.getItem(this.userKey);
    if (userRecordString !== null) {
      return JSON.parse(userRecordString);
    }
    return null;
  }

  public isUserAdmin(): boolean {
    this.isAdmin().subscribe((result: boolean) => {
      return result;
    });
    return false;
  }

  public isUserLoggedIn(): boolean {
    return this.getUser() !== null;
  }

  public getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  clearToken(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
  }

  public isAdmin() : Observable<boolean> {
    const user = this.getUser();
    if (user == undefined) {
      return new Observable<boolean>().pipe(tap(() => false));
    }
    return this.http.get<ResponseDto>(`${this.apiUrl}/api/auth/InRole/${user?.email}/ADMIN`).pipe(
      map(r => ((r as ResponseDto).result as unknown as boolean),
      catchError(err => {
        this.toastr.error("Error loading user or user role<br/><br/>" + err.message, "Error");
        return of(false);
      }))
    );  
  }
}
