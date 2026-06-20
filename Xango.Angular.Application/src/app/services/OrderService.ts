import { Injectable } from '@angular/core';
import { HttpClient, HttpHandler, HttpHeaders } from '@angular/common/http';
import { Product } from '../dto/Product';
import { OrderHeader } from '../dto/OrderHeader';
import { OrderDetail } from '../dto/OrderDetail';
import { ToastrService } from 'ngx-toastr';
import { ServiceSettings } from './ServiceSettings';
import { ResponseDto } from '../dto/ResponseDto';
import { ShoppingCartHeader } from '../dto/ShoppingCartHeader';
import { catchError, tap } from 'rxjs';
import { StripeRequest } from '../dto/StripeRequest';
import { SaveCartDetails } from '../dto/SaveCartDetails';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private readonly baseUrl = ServiceSettings.ORDER_API;

  constructor(
    private http: HttpClient,
    private toastr: ToastrService 
    ) {
    toastr.toastrConfig.timeOut = 5000;
    toastr.toastrConfig.closeButton = true;
    toastr.toastrConfig.enableHtml = true;
  }

    public createOrder(shoppingCart: ShoppingCartHeader) {
      return this.http.post<ResponseDto>(`${this.baseUrl}/api/order/CreateOrder`, shoppingCart).pipe(
         tap((responseDto : ResponseDto) => {
          if (!responseDto.isSuccess) {
            throw new Error(responseDto.message);
          }
        }),
        catchError(err => {
        this.toastr.error(`Error creating order<br/><br/>` + err.message, "Error");
        throw err;
        })
      );
    }

    public createStripeSession(stripeRequest: StripeRequest) {
      return this.http.post<ResponseDto>(`${this.baseUrl}/api/order/CreateStripeSession`, stripeRequest).pipe (
        tap((responseDto: ResponseDto) => {
          if (!responseDto.isSuccess) {
            throw new Error(responseDto.message);
          }
        }),
        catchError(err => {
          this.toastr.error(`Error creating Stripe session <br/><br/>` + err.message, "Error");
          throw err;
  
        })
      )
    }

    public validateStripeSession(orderId : number) {
      return this.http.post(`${this.baseUrl}/api/order/ValidateStripeSession`, orderId) .pipe(
        tap((ResponseDto: ResponseDto | any) => {
          if (!ResponseDto.isSuccess) {
            throw new Error(ResponseDto.message);
          }
        }),
        catchError((err) => {
          this.toastr.error(err.message,"Error");
          throw err;
        })
      )
    }

    public getAll(userId: string, status?: string) {
      return this.http.get(`${this.baseUrl}/api/order/GetAll?status=` + (status == null ? "all" : status + "&userId=" + userId)) .pipe(
        tap((ResponseDto: ResponseDto | any) => {
          if (!ResponseDto.isSuccess) {
            throw new Error(ResponseDto.message);
          }
        }),
        catchError((err) => {
          this.toastr.error(err.message,"Error");
          throw err;
        })
      )
    }
 
    public get(orderId: number) {
      return this.http.get(`${this.baseUrl}/api/order/GetOrder/` + orderId).pipe(
        tap((ResponseDto: ResponseDto | any) => {
          if (!ResponseDto.isSuccess) {
            throw new Error(ResponseDto.message);
          }
        }),
        catchError((err) => {
          this.toastr.error(err.message, "Error 1");
          throw err;
        })
      )
    }

    public updateOrderStatus(orderId: number, newStatus: string ) {
      const url = `${this.baseUrl}/api/order/UpdateOrderStatus/${orderId}`;
      const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
      const body = JSON.stringify(newStatus);
    
      return this.http.post<ResponseDto>(url, body, { headers }).pipe(
        tap((response: ResponseDto) => {
          if (!response.isSuccess) {
            throw new Error(response.message);
          }
        }),
        catchError((err) => {
          this.toastr.error(err.message, 'Error');
          throw err;
        })
      );    }

    public deleteOrder(orderId: number) {
      return this.http.delete(`${this.baseUrl}/api/order/DeleteOrder/` + orderId).pipe(
        tap((ResponseDto: ResponseDto | any) => {
          if (!ResponseDto.isSuccess) {
            throw new Error(ResponseDto.message);
          }
        }),
        catchError((err) => {
          this.toastr.error(err.message, "Error");
          throw err;
        })
      )
    }
}
