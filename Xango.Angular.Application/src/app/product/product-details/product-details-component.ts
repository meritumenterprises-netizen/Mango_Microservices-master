import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, NavigationEnd, RouterModule } from '@angular/router';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { ProductService } from '../../services/ProductService';
import { ResponseDto } from '../../dto/ResponseDto';
import { filter, Observable, Subject, takeUntil } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../services/AuthenticationService';
import { ShoppingCartService } from '../../services/ShoppingCartService';
import { CouponService } from '../../services/CouponService';
import { Router, RouterLink } from '@angular/router';
import {
  FormsModule,
  FormGroup,
  FormBuilder,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';

@Component({
  standalone: true,
  selector: 'app-product-details-component',
  imports: [CurrencyPipe, FormsModule, ReactiveFormsModule, CommonModule, RouterModule],
  templateUrl: './product-details-component.html',
  styleUrl: './product-details-component.css',
  providers: [ProductService, ShoppingCartService, CouponService],
})
export class ProductDetailsComponent implements OnInit, OnDestroy {
  private id: number;
  private product$: Observable<ResponseDto>;
  private shoppingCart$: Observable<ResponseDto> | null = null;
  response: ResponseDto;
  product: any;
  private _isAdmin = false;
  private destroy$ = new Subject<void>();

  constructor(
    private route: ActivatedRoute,
    public productService: ProductService,
    private toastr: ToastrService,
    private authService: AuthService,
    private shoppingCartService: ShoppingCartService,
    private couponService: CouponService,
    private router: Router,
  ) {
    this.toastr.toastrConfig.enableHtml = true;
    this.id = parseInt(this.route.snapshot.paramMap.get('id')!);
    this.response = new ResponseDto();
    this.product = null;
    this.product$ = this.productService.getProduct(this.id);
    this.product$.subscribe({
      next: (responseDto) => {
        this.response = responseDto;
        this.product = this.response.result;
        if (this.product == null) {
          toastr.error(`Product with id ${this.id} has not been found`);
        }
      },
      error: (err) => {
        console.error('Error', err);
        this.product = null;
      },
      complete: () => {
        console.log('Done');
      },
    });
  }

  onSubmit(quantity: string, stockQuantity: string) {
    console.log('Adding product to shopping cart');
    let quantityVal = parseInt(quantity);
    this.shoppingCart$ = this.shoppingCartService.addProductToCart(
      this.id,
      quantityVal,
      parseInt(stockQuantity),
    );
    this.shoppingCart$.subscribe({
      next: (responseDto) => {
        this.response = responseDto;
        if (!this.response.isSuccess) {
          throw new Error(this.response.message);
        }
        this.router.navigate(['/cart']);
      },
    });
  }

  ngOnInit(): void {
    // Run once at load
    this.checkAdmin();

    // Run every time a navigation completes
    this.router.events
      .pipe(
        filter((event) => event instanceof NavigationEnd),
        takeUntil(this.destroy$),
      )
      .subscribe(() => {
        this.checkAdmin();
      });
  }

  private checkAdmin(): void {
    this.authService.isAdmin().subscribe((result) => {
      this._isAdmin = result;
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  isLoggedIn() {
    return this.authService.isUserLoggedIn();
  }

  isAdmin(): boolean {
    return this._isAdmin;
  }
}
