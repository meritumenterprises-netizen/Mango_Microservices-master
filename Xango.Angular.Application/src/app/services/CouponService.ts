import { Injectable } from '@angular/core';
import { HttpClient, HttpHandler } from '@angular/common/http';
import { ResponseDto } from '../dto/ResponseDto';
import { Observable, catchError, of, tap } from 'rxjs';
import { ServiceSettings } from './ServiceSettings';
import { ToastrService } from 'ngx-toastr';
import { Coupon } from '../dto/Coupon';

@Injectable({
  providedIn: 'root'
})
export class CouponService {
  private readonly baseUrl = ServiceSettings.COUPON_API;

    constructor(public http: HttpClient, public toastr: ToastrService ) {
      toastr.toastrConfig.timeOut = 5000;
      toastr.toastrConfig.closeButton = true;
      toastr.toastrConfig.enableHtml = true;
  }

  public getCoupons() : Coupon[] | any {
      return this.http.get<ResponseDto>(`${this.baseUrl}/api/coupon`).pipe(
       tap((responseDto) => {
        if (!responseDto.isSuccess) {
          throw new Error(responseDto.message);
        }
      }),
      catchError(err => {
        this.toastr.error("Error loading coupons<br/><br/>" + err.message, "Error");
        throw new Error(err);
      })
    );
  }

  public getCoupon(id: number) : Coupon | any {
      return this.http.get<ResponseDto>(`${this.baseUrl}/api/coupon/${id}`).pipe(
       tap((responseDto) => {
        if (!responseDto.isSuccess) {
          throw new Error(responseDto.message);
        }
      }),
      catchError(err => {
        this.toastr.error("Error loading coupons<br/><br/>" + err.message, "Error");
        throw new Error(err);
      })
    );
  }

  
  public getCouponByCode(code : string) : Coupon | any {
    return this.http.get<ResponseDto>(`${this.baseUrl}/api/coupon/GetByCode/${code}`).pipe(
      tap ((responseDto) => {
        if (!responseDto.isSuccess) {
          throw new Error(responseDto.message);
        }
      }),
      catchError((err) => {
        this.toastr.error(err, "Error");
        throw new Error(err);
      })
    );
  }

  public createCoupon(coupon : Coupon) {
      return this.http.post<ResponseDto>(`${this.baseUrl}/api/coupon`, coupon).pipe(
       tap((responseDto) => {
        if (!responseDto.isSuccess) {
          throw new Error(responseDto.message);
        }
      }),
      catchError(err => {
        this.toastr.error("Error creating coupons\r\n" + err, "Error");
        throw new Error(err);
      })
    );
  }

  public updateCounpon(coupon: Coupon | any ) {
      return this.http.put<ResponseDto>(`${this.baseUrl}/api/coupon`, coupon).pipe(
      tap((responseDto) => {
        if (!responseDto.isSuccess) {
          throw new Error(responseDto.message);
        }
      }),
      catchError(err => {
        this.toastr.error("Error updating coupon\r\n" + err, "Error");
        throw new Error(err);
      })
    );
  }

  public deleteCoupon(id : number) {
      return this.http.delete<ResponseDto>(`${this.baseUrl}/api/coupon/${id}`).pipe(
      tap((responseDto) => {
        if (!responseDto.isSuccess) {
          throw new Error(responseDto.message);
        }
      }),
      catchError(err => {
        this.toastr.error("Error deleting coupon\r\n" + err, "Error");
        throw new Error(err);
      })
    );
  }
}
