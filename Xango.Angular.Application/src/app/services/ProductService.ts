// product.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHandler } from '@angular/common/http';
import { Observable, catchError, of, tap } from 'rxjs';
import { ServiceSettings } from './ServiceSettings';
import { ResponseDto } from '../dto/ResponseDto';
import { AppComponent } from '../app.component';
import { ToastrService } from 'ngx-toastr';
import { Product } from '../dto/Product';

@Injectable()
export class ProductService {
  private readonly baseUrl = ServiceSettings.PRODUCT_API;

  constructor(
    private http: HttpClient,
    private toastr: ToastrService 
    ) {
    toastr.toastrConfig.timeOut = 5000;
    toastr.toastrConfig.closeButton = true;
    toastr.toastrConfig.enableHtml = true;
  }

  public getProducts() {
    return this.http.get<ResponseDto>(`${this.baseUrl}/api/product`).pipe(
       tap((responseDto) => {
        if (!responseDto.isSuccess) {
          throw new Error(responseDto.message);
        }
      }),
      catchError(err => {
        this.toastr.error("Error loading products<br/><br/>" + err.message, "Error");
        throw err;
      })
    );
  }

  public getProduct (productId : number) {
    return this.http.get<ResponseDto>(`${this.baseUrl}/api/product/${productId}`).pipe(
       tap((responseDto) => {
        if (!responseDto.isSuccess) {
          throw new Error(responseDto.message);
        }
      }),
      catchError(err => {
      this.toastr.error(`Error loading product with product id ${productId}<br/><br/>` + err.message, "Error");
      throw err;
      })
    );
  }

  public createProduct(product: Product) {
    return this.http.post<ResponseDto>(`${this.baseUrl}/api/product/`, product).pipe(
       tap((responseDto) => {
        if (!responseDto.isSuccess) {
          throw new Error(responseDto.message);
        }
      }),
      catchError(err => {
      this.toastr.error(`Error creating product<br/><br/>` + err.message, "Error");
      throw err;
      })
    );
  }

  public updateProduct(product: Product) {
    return this.http.put<ResponseDto>(`${this.baseUrl}/api/product`, product).pipe(
       tap((responseDto) => {
        if (!responseDto.isSuccess) {
          throw new Error(responseDto.message);
        }
      }),
      catchError(err => {
      this.toastr.error(`Error updating product with product id ${product.productId}<br/><br/>` + err.message, "Error");
      throw err;
      })
    );
  }

  public deleteProduct(productId : number) {
      return this.http.delete<ResponseDto>(`${this.baseUrl}/api/product/${productId}`).pipe(
      tap((responseDto) => {
        if (!responseDto.isSuccess) {
          throw new Error(responseDto.message);
        }
      }),
      catchError(err => {
        this.toastr.error("Error deleting product<br/><br/>" + err.message, "Error");
        throw err;
      })
    );

  }
}
