import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs';
import { ResponseDto } from '../../dto/ResponseDto';
import { RouterModule } from '@angular/router';
import { CurrencyPipe } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CanComponentDeactivatable } from '../../basecomponents/CanComponentDeactivatable';
import { ShoppingCartHeader } from '../../dto/ShoppingCartHeader';
import { ShoppingCartDetail } from '../../dto/ShoppingCartDetail';
import { ShoppingCart } from '../../dto/ShoppingCart';
import { ShoppingCartService } from '../../services/ShoppingCartService';

import {
  FormsModule,
  FormGroup,
  FormBuilder,
  Validators,
  ReactiveFormsModule,
  Form,
} from '@angular/forms';
import { SaveCartDetails } from '../../dto/SaveCartDetails';


@Component({
  selector: 'app-checkout-cart-component',
  imports: [CurrencyPipe, FormsModule, ReactiveFormsModule, CommonModule, RouterModule],
  templateUrl: './checkout-cart-component.html',
  styleUrl: './checkout-cart-component.css',
  providers: [ShoppingCartService]
})
export class CheckoutCartComponent extends CanComponentDeactivatable {
  private shoppingCart$: Observable<ResponseDto> | null= null;
  public shoppingCartForm: FormGroup | any = null;
  public shoppingCart: ShoppingCart | any = null;
  private response: ResponseDto | any = null;
  private readonly emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[A-Za-z]{2,}$/;
      
  constructor(
    private fb: FormBuilder,
    private shoppingCartService: ShoppingCartService,
    private router: Router,
    private toastr: ToastrService,
  ) {
    super();
    this.shoppingCartForm = this.fb.group(
      {
        name : ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
        email: ['', [Validators.required, Validators.pattern(this.emailPattern)]],
        phone: ['', [Validators.required]]
      }
    );
    this.form = this.shoppingCartForm;
    this.reloadShoppingCart();
  }

  reloadShoppingCart() {
    this.shoppingCart$ = this.shoppingCartService.getShoppingCart();
    this.shoppingCart$.subscribe({
      next: (responseDto) => {
        this.response = responseDto;
      },
      error: (err) => {
        console.error('Error', err);
        this.shoppingCart = null;
      },
      complete: () => {
        console.log('Done');
        this.shoppingCart = this.response.result;
        if (this.shoppingCart == null) {
          this.toastr.error(`No shopping cart for the currently logged on user has been found`);
        }
        this.shoppingCartForm.patchValue({
          name: this.shoppingCart.cartHeader.name,
          email: this.shoppingCart.cartHeader.email,
          phone: this.shoppingCart.cartHeader.phone,
        });
        this.shoppingCartForm.markAsPristine();
      },
    });
 }

  onSubmit() {
    let saveUserDetails : SaveCartDetails = {
      name: this.shoppingCartForm.get("name").value,
      phone: this.shoppingCartForm.get("phone").value,
      email: this.shoppingCartForm.get("email").value
    } ;   
    sessionStorage.setItem('userDetails', JSON.stringify(saveUserDetails));
    this.router.navigate(['/cart/placeorder']);    
 }

 get name() {
  return this.shoppingCartForm.get('name');
}

get email() {
  return this.shoppingCartForm.get('email');
}

get phone() {
  return this.shoppingCartForm.get('phone');
}

}
