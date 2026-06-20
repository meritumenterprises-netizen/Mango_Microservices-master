import { Component } from '@angular/core';
import { Product } from '../dto/Product';
import { ProductService } from '../services/ProductService';
import { Observable } from 'rxjs';
import { CurrencyPipe } from '@angular/common';
import { ResponseDto } from '../dto/ResponseDto';
import { ToastrService } from 'ngx-toastr';
import { Pipe } from '@angular/core';
import { TruncatePipe } from '../pipes/truncate.pipe';

@Component({
  standalone: true,
  selector: 'app-home-component',
  imports: [CurrencyPipe, TruncatePipe],
  templateUrl: './home-component.html',
  styleUrl: './home-component.css',
  providers: [ProductService]
})
export class HomeComponent {
  products$: Observable<ResponseDto>;
  response : ResponseDto;
  products: any;

  constructor(private productService: ProductService, private toastr : ToastrService) {
    this.response = new ResponseDto();
    this.products = [];
    this.products$ = this.productService.getProducts();
    this.products$.subscribe({
      next: responseDto => {
        this.response = responseDto;
      },
      error: err => {
        console.error('Error', err);
        toastr.error(err, "Error");
      },
      complete: () => {
        console.log('Done');
        this.products  = this.response.result;
      }});
  }

}
