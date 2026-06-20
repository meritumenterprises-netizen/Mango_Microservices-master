import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { Product } from '../../dto/Product';
import { ProductService } from '../../services/ProductService';
import { AuthService } from '../../services/AuthenticationService';
import { ResponseDto } from '../../dto/ResponseDto';
import { CurrencyPipe } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { Router, RouterLink } from "@angular/router";
import Swal from 'sweetalert2';

@Component({
  selector: 'app-product-component',
  imports: [CurrencyPipe, RouterLink],
  templateUrl: './product-component.html',
  styleUrl: './product-component.css',
  providers: [ProductService]
})
export class ProductComponent {
    products$: Observable<ResponseDto> | any = null;
    response : ResponseDto | any = null;
    products: Product[] | any;
  
  constructor(
    private productService: ProductService, 
    private toastr : ToastrService,
    private router: Router) {
    this.response = new ResponseDto();
    this.products = [];
    this.refreshProducts();
  }

  refreshProducts() {
    this.products$ = this.productService.getProducts();
    this.products$.subscribe({
      next: (responseDto: any) => {
        this.response = responseDto;
      },
      error: (err: string | undefined) => {
        console.error('Error', err);
      },
      complete: () => {
        console.log('Done');
        this.products  = this.response.result;
      }});
  }

    deleteProduct(productId: number | any) {
      Swal.fire({
        title: 'Are you sure?',
        text: 'Do you really want to delete this product?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Delete',
        cancelButtonText: 'Cancel',
      }).then((result) => {
        if (result.isConfirmed) {
          this.productService.deleteProduct(productId).subscribe({
            next: (response: any) => {
              console.log('Product deleted', response);
            },
            error: (err: any) => {
              console.error('Error', err);
              this.toastr.error(err, 'Error');
            },
            complete: () => {
              console.log('Done');
              this.toastr.success("Product deleted");
              this.refreshProducts();
            },
          });
        }
      });
    }

}
