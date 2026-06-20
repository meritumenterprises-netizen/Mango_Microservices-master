import { Component, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Coupon } from '../../dto/Coupon';
import { CouponService } from '../../services/CouponService';
import { ToastrService } from 'ngx-toastr';
import { catchError, Observable } from 'rxjs';
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
  selector: 'app-edit-coupon-component',
  imports: [FormsModule, ReactiveFormsModule, CommonModule, RouterModule],
  templateUrl: './edit-coupon-component.html',
  styleUrl: './edit-coupon-component.css',
})
export class EditCouponComponent extends CanComponentDeactivatable {
  couponForm: FormGroup | any = null;
  coupon$: Observable<ResponseDto> | any = null;
  couponId: number | any;
  response: ResponseDto | any = null;
  public coupon: Coupon | any = null;
  constructor(
    private fb: FormBuilder,
    private couponService: CouponService,
    private toastr: ToastrService,
    private route: ActivatedRoute,
    private router: Router,
  ) {
    super();
    this.response = new ResponseDto();
    this.couponId = parseInt(this.route.snapshot.paramMap.get('id') as string)!;
    this.coupon$ = this.couponService.getCoupon(this.couponId);
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

    this.coupon$.subscribe({
      next: (responseDto: any) => {
        this.response = responseDto;
      },
      error: (err: string | undefined) => {
        console.error('Error', err);
      },
      complete: () => {
        console.log('Done');
        this.coupon = this.response.result;
        this.couponForm.patchValue({
          couponCode: this.coupon.couponCode,
          discountAmount: this.coupon.discountAmount,
          minAmount: this.coupon.minAmount,
        });
        this.couponForm.markAsPristine();
      },
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
    let updatedCoupon: Coupon = {
      couponId: this.couponId,
      couponCode: this.couponCode.value,
      discountAmount: this.discountAmount.value,
      minAmount: this.minAmount.value,
    };
    this.coupon$ = this.couponService.updateCounpon(updatedCoupon);
    this.coupon$.subscribe({
      error: (err: any) => {
        console.error('Error', err);
      },
      complete: () => {
          console.log('Done');
          this.couponForm.markAsPristine();
          this.toastr.success('Coupon successfully updated');
          this.router.navigate(['/coupon']);
        }
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
