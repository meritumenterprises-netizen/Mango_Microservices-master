import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { RouterLink } from '@angular/router';
import { ResponseDto } from '../../dto/ResponseDto';
import { ActivatedRoute } from '@angular/router';
import { ShoppingCartService } from '../../services/ShoppingCartService';
import { catchError, Observable } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../services/AuthenticationService';
import { OrderService } from '../../services/OrderService';
import { UserRecord } from '../../dto/UserRecord';

@Component({
  selector: 'app-confirmation-component',
  imports: [RouterModule],
  templateUrl: './confirmation-component.html',
  styleUrl: './confirmation-component.css'
})
export class ConfirmationComponent {

  private orderId : number = 0;
  private $deleteCart : Observable<ResponseDto> | any = null;
  private $validateStripeSession : Observable<ResponseDto> | any = null;

  constructor(
    private shoppingCartService : ShoppingCartService,
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
    private orderService: OrderService,
    private toastr: ToastrService,
  ) {
    this.orderId = parseInt(this.route.snapshot.paramMap.get('id') as string)!;
    let user : UserRecord | null = this.authService.getUser();
    this.$deleteCart = this.shoppingCartService.deleteCart(user!.id);
    this.$deleteCart.subscribe({
      next: (responseDto: ResponseDto | any) => {
        if (responseDto.isSuccess) {
          this.$validateStripeSession = this.orderService.validateStripeSession(this.orderId);
          this.$validateStripeSession.subscribe({
            next: (responseDto: ResponseDto | any) => {
              if (responseDto.isSuccess) {
                toastr.success("Order has been placed");
                this.router.navigate(['/']);
              }
            },
            catchError: (err : Error) => {
              toastr.error(err.message, "Error");
            }
          })
        }
      },
      catchError: (err: Error) => {
        toastr.error(err.message, "Error");
      }
    }

    );
  }

}
