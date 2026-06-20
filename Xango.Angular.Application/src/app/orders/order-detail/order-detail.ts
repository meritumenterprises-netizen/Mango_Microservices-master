import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthService } from '../../services/AuthenticationService';
import { OrderService } from '../../services/OrderService';
import { ResponseDto } from '../../dto/ResponseDto';
import { ToastrService } from 'ngx-toastr';
import { OrderHeader } from '../../dto/OrderHeader';
import { OrderDetail } from '../../dto/OrderDetail';
import { ActivatedRoute, NavigationEnd, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { catchError, filter, Observable, Subject, takeUntil } from 'rxjs';
import { UserRecord } from '../../dto/UserRecord';
import { CurrencyPipe,DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Pipe } from '@angular/core';
import { SplitCamelCasePipe } from '../../pipes/split-camel-case.pipe';

@Component({
  selector: 'app-order-detail',
  templateUrl: './order-detail.html',
  styleUrl: './order-detail.css',
  imports: [CommonModule, RouterModule, SplitCamelCasePipe],
  providers: [ AuthService, OrderService ]
})
export class OrderDetailsComponent implements OnInit, OnDestroy{
  private orderId : number = 0;
  public order: OrderHeader | any;
  private order$: Observable<ResponseDto> | any = null;
  public user: UserRecord | any;
  private _isAdmin = false;
  private destroy$ = new Subject<void>();

  constructor(
    private authService : AuthService,
    private orderService: OrderService,
    private toastr : ToastrService,
    private route: ActivatedRoute,
    private router: Router,
  ) {
    this.orderId = parseInt(this.route.snapshot.paramMap.get('id') as string)!;
    this.user = this.authService.getUser();
    this.reloadOrder();
  }

  reloadOrder() {
    this.order$ = this.orderService.get(this.orderId).subscribe({
      next: (responseDto : ResponseDto | any) => {
        if (!responseDto.isSuccess) {
          this.toastr.error("Order id " + this.orderId + " has not been found");
          this.router.navigate(['/order']);
        }
       this.order = responseDto.result;
      },
      error: (err: string | undefined) => {
       console.error('Error', err);
       this.toastr.error("Order id " + this.orderId + " has not been found");
       this.router.navigate(['/order']);
     }
   });
 
  }

  onSubmit_ReadyForPickup() {
    this.order$ = this.orderService.updateOrderStatus(this.orderId, "ReadyForPickup");
    this.order$.subscribe({
      next: (responseDto: ResponseDto | any) => {
        if (responseDto.isSuccess) {
          this.reloadOrder();
          console.log("Marked order as Ready for pickup");
        }
      },
        catchError: (err: Error) => {
          this.toastr.error(err.message, "Error");
        }
    });
  }

  onSubmit_Complete() {
    this.order$ = this.orderService.updateOrderStatus(this.orderId, "Completed");
    this.order$.subscribe({
      next: (responseDto: ResponseDto | any) => {
        if (responseDto.isSuccess) {
          this.reloadOrder();
          console.log("Marked order as Complete");
        }
      },
        catchError: (err: Error) => {
          this.toastr.error(err.message, "Error");
        }
    });
  }

  onSubmit_CancelOrder() {
    this.order$ = this.orderService.updateOrderStatus(this.orderId, "Cancelled");
    this.order$.subscribe({
      next: (responseDto: ResponseDto | any) => {
        if (responseDto.isSuccess) {
          this.reloadOrder();
          console.log("Marked order as Cancelled");
        }
      },
        catchError: (err: Error) => {
          this.toastr.error(err.message, "Error");
        }
    });

  }

  onSubmit_DeleteOrder() {
    this.order$ = this.orderService.deleteOrder(this.orderId);
    this.order$.subscribe({
      next: (responseDto: ResponseDto | any) => {
        if (responseDto.isSuccess) {
          this.toastr.success("Order ID " + this.orderId + " has been deleted");
          this.router.navigate(["/order"]);
          console.log("Deleted order " + this.orderId);
        }
      },
        catchError: (err: Error) => {
          this.toastr.error(err.message, "Error");
        }
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

  isAdmin() : boolean {
    return this._isAdmin;
  }

}
