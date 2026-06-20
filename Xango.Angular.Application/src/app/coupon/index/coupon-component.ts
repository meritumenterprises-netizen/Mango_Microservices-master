import { Component } from '@angular/core';
import { Coupon } from '../../dto/Coupon';
import { CouponService } from '../../services/CouponService';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { ResponseDto } from '../../dto/ResponseDto';
import { CurrencyPipe } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-coupon-component',
  imports: [CurrencyPipe, RouterModule],
  templateUrl: './coupon-component.html',
  styleUrl: './coupon-component.css',
})
export class CouponComponent {
  coupons$: Observable<ResponseDto> | any = null;
  response: ResponseDto | any = null;
  coupons: Coupon[] | any;

  constructor(
    private couponService: CouponService,
    private toastr: ToastrService,
    private router: Router
  ) {
    this.response = new ResponseDto();
    this.coupons = [];
    this.refreshCoupons();
  }

  refreshCoupons() {
    this.coupons$ = this.couponService.getCoupons();
    this.coupons$.subscribe({
      next: (responseDto: any) => {
        this.response = responseDto;
      },
      error: (err: string | undefined) => {
        console.error('Error', err);
        this.toastr.error(err, 'Error');
      },
      complete: () => {
        console.log('Done');
        this.coupons = this.response.result;
      },
    });
  }

  deleteItem(itemId: number | any) {
    Swal.fire({
      title: 'Are you sure?',
      text: 'Do you really want to delete this coupon?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Delete',
      cancelButtonText: 'Cancel',
    }).then((result) => {
      if (result.isConfirmed) {
        this.couponService.deleteCoupon(itemId).subscribe({
          next: (response: any) => {
            console.log('Coupon deleted', response);
          },
          error: (err: any) => {
            console.error('Error', err);
            this.toastr.error(err, 'Error');
          },
          complete: () => {
            console.log('Done');
            this.refreshCoupons();
          },
        });
        
      }
    });
  }
}
