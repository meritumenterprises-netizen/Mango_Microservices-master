import { Component, HostListener} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Coupon } from '../../dto/Coupon';
import { CouponService } from '../../services/CouponService';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { ResponseDto } from '../../dto/ResponseDto';
import { ActivatedRoute, Router } from '@angular/router';
import { lessThanValidator, __VALIDATORS_TEST__ } from '../../validators/validators';
import { CanComponentDeactivatable } from '../../basecomponents/CanComponentDeactivatable';
import { RouterLink, RouterModule } from '@angular/router';
import Swal from 'sweetalert2';
import {
  FormsModule,
  FormGroup,
  FormBuilder,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';

@Component({
  standalone: true,
  selector: 'app-create-coupon-component',
  imports: [FormsModule, ReactiveFormsModule, CommonModule, RouterModule],
  templateUrl: './create-coupon-component.html',
  styleUrl: './create-coupon-component.css',
})
export class CreateCouponComponent extends CanComponentDeactivatable {
  couponForm: FormGroup | any = null;
  coupon$: Observable<ResponseDto> | any = null;
  response: ResponseDto | any = null;
  constructor(
    private fb: FormBuilder,
    private couponService: CouponService,
    private toastr: ToastrService,
    private route: ActivatedRoute,
    private router: Router,
  ) {
    super();
    this.response = new ResponseDto();
    this.couponForm = this.fb.group(
      {
        couponCode: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(30)]],
        discountAmount: [
          '',
          [
            Validators.required,
            Validators.min(1.0),
            Validators.max(1000.0),
            Validators.pattern(/^\d*\.?\d+$/),
          ],
        ],
        minAmount: [
          '',
          [
            Validators.required,
            Validators.min(10.0),
            Validators.max(10000.0),
            Validators.pattern(/^\d*\.?\d+$/),
          ],
        ],
      },
      {
        validators: lessThanValidator('discountAmount', 'minAmount'),
      },
    );

    this.form = this.couponForm;
        this.couponForm.patchValue({
          couponCode: "SAVE",
          discountAmount: 1,
          minAmount: 10,
        });
  }

  onSubmit() {
    if (this.couponForm.invalid) {
      this.couponForm.markAllAsTouched();
      return;
    }
    console.log(this.couponForm.value.couponCode);
    console.log(this.couponForm.value.discountAmount);
    console.log(this.couponForm.value.minAmount);
    let newCoupon: Coupon = {
      couponId: 0,
      couponCode: this.couponCode.value,
      discountAmount: this.discountAmount.value,
      minAmount: this.minAmount.value,
    };
    this.coupon$ = this.couponService.createCoupon(newCoupon);
    this.coupon$.subscribe({
      error: (err: any) => {
        console.error('Error', err);
      },
      complete: () => {
        console.log('Done');
        this.toastr.success("Coupon successfully created");
        this.couponForm.markAsPristine();
        this.router.navigate(['/coupon']);
      },
    });
  }

  get couponCode() {
    return this.couponForm.get('couponCode');
  }

  get discountAmount() {
    return this.couponForm.get('discountAmount');
  }

  get minAmount() {
    return this.couponForm.get('minAmount');
  }
}
