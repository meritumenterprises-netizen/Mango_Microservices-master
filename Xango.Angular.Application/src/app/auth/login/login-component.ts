import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import {
  FormsModule,
  FormGroup,
  FormBuilder,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { UserRecord } from '../../dto/UserRecord';
import { AuthService } from '../../services/AuthenticationService';
import { Observable } from 'rxjs';
import { ResponseDto } from '../../dto/ResponseDto';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login-component',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule, CommonModule],
  templateUrl: './login-component.html',
  styleUrl: './login-component.css',
})
export class LoginComponent {
  private readonly emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[A-Za-z]{2,}$/;

  // Password: ≥8 chars, 1 uppercase, 1 digit, 1 special character
  private readonly passwordPattern = /^(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{8,}$/;

  public loginForm: FormGroup;
  private user$: Observable<UserRecord> | any = null;
  private responseDto: ResponseDto | any = null;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private toastr: ToastrService,
    private router: Router,
  ) {
    this.loginForm = this.fb.group({
      // ✔ safe now
      username: ['', [Validators.required, Validators.pattern(this.emailPattern)]],
      password: [
        '',
        [Validators.required, Validators.minLength(8), Validators.pattern(this.passwordPattern)],
      ],
    });
  }

  onSubmit() {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }
    console.log(this.loginForm.value.username);
    console.log(this.loginForm.value.password);
    this.user$ = this.authService.login(
      this.loginForm.value.username,
      this.loginForm.value.password,
    );
    this.user$.subscribe({
      error: (err: any) => {
        console.error('Error', err);
        this.toastr.error(`${err.message}`, 'Error');
      },
      complete: () => {
        console.log('Done');
        if (this.authService.getUser() === null) {
          this.toastr.error(
            `Could not log in user ${this.loginForm.value.username} with given password, or user does not exist`,
          );
          return;
        }
        this.router.navigate(['/']);
      },
    });
  }

  get username() {
    return this.loginForm.get('username');
  }

  get password() {
    return this.loginForm.get('password');
  }
}
