import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { catchError, Observable } from 'rxjs';
import { ResponseDto } from '../../dto/ResponseDto';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../services/AuthenticationService';
import { Router } from '@angular/router';
import { RegistrationRequest } from '../../dto/RegistrationRequest';
import { notFirstOptionValidator } from '../../validators/validators';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule, CommonModule],
  templateUrl: './register-component.html',
  styleUrl: './register-component.css'
})
export class RegisterComponent {
  private readonly emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[A-Za-z]{2,}$/;

  // Password: ≥8 chars, 1 uppercase, 1 digit, 1 special character
  private readonly passwordPattern = /^(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{8,}$/;

  public registerForm: FormGroup;
  private registration$: Observable<ResponseDto> | any = null;
  

    constructor(
      private fb: FormBuilder,
      private toastr: ToastrService,
      private authService: AuthService,
      private router: Router
    ) {
    this.registerForm = this.fb.group({
      email: ['', [Validators.required, Validators.pattern(this.emailPattern)]],
      name: ['', [Validators.required, Validators.minLength(4)]],
      phone: ['', [Validators.required]],
      password: [
        '',
        [Validators.required, Validators.minLength(8), Validators.pattern(this.passwordPattern)],
      ],
      role: ['', [Validators.required, notFirstOptionValidator("--Select role--")]]
    });
  }


  onSubmit() {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }
    console.log(this.registerForm.value.email);
    console.log(this.registerForm.value.name);
    console.log(this.registerForm.value.phone);
    console.log(this.registerForm.value.password);
    console.log(this.registerForm.value.role);

    let registration : RegistrationRequest = {
      email : this.registerForm.value.email,
      name: this.registerForm.value.name,
      phoneNumber: this.registerForm.value.phone,
      password: this.registerForm.value.password,
      role: this.registerForm.value.role
    };

    this.registration$ = this.authService.register(registration
    );

    this.registration$.subscribe({
      catchError: (err: any) => {
        console.error('Error', err);
        this.toastr.error(`${err.message}`, 'Error');
      },
      complete: () => {
        console.log('Done');
        this.toastr.success("User registration successful!", "Success");
        this.router.navigate(['/']);
      },
    });

  }

get email() {
    return this.registerForm.get('email');
  }

  get name() {
    return this.registerForm.get('name');
  }

  get phone() {
    return this.registerForm.get('phone');
  }

  get password() {
    return this.registerForm.get('password');
  }  

  get role() {
    return this.registerForm.get('role');
  }
}
