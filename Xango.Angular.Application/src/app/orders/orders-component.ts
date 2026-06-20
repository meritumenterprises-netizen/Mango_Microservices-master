import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd } from '@angular/router';
import { Router } from '@angular/router';
import { RouterLink } from '@angular/router';
import { AuthService } from '../services/AuthenticationService';
import { OrderService } from '../services/OrderService';
import { CommonModule, NgClass } from '@angular/common';
import { UserRecord } from '../dto/UserRecord';
import { filter, Observable, Subject, takeUntil } from 'rxjs';
import { ResponseDto } from '../dto/ResponseDto';
import { OrderHeader } from '../dto/OrderHeader';
import { SplitCamelCasePipe } from '../pipes/split-camel-case.pipe';

@Component({
  selector: 'app-orders-component',
  imports: [CommonModule, SplitCamelCasePipe],
  templateUrl: './orders-component.html',
  styleUrl: './orders-component.css'
})
export class OrdersComponent implements OnInit, OnDestroy {
  public status : string = "all";
  public user : UserRecord | null;
  private orders$ : Observable<ResponseDto> | null = null;
  public orders : OrderHeader[] | null = null;
  private _isAdmin = false;
  private destroy$ = new Subject<void>();

  constructor(
    private authService: AuthService,
    private orderService: OrderService,
    private route : ActivatedRoute,
    private router: Router
  ) {
    this.status = this.route.snapshot.paramMap.get('status') as string;
    if (this.status == null) {
      this.status = "all";
    }
    this.user = this.authService.getUser();
    this.reloadOrders();
  }

  reloadOrders() {
    this.orders$ = this.orderService.getAll(this.user!.id, this.status);
    this.orders$.subscribe({
      next: (response : ResponseDto | any) => {
        this.orders = response.result;
      }
    });

  }

  onClickPending() {
    console.log("Clicked Pending");
    this.router.navigate(['/order/pending']);
    this.status = "pending";
    this.reloadOrders();
  }

  onClickApproved() {
    console.log("Clicked Approved");
    this.router.navigate(['/order/approved']);
    this.status = "approved";
    this.reloadOrders();
  }

  onClickReadyForPickup() {
    console.log("Clicked ReadyForPickup");
    this.router.navigate(['/order/readyforpickup']);
    this.status = "readyforpickup";
    this.reloadOrders();
  }

  onClickCancelled() {
    console.log("Clicked Cancelled");
    this.router.navigate(['/order/cancelled']);
    this.status = "cancelled";
    this.reloadOrders();
  }

  onClickCompleted() {
    console.log("Clicked Complete");
    this.router.navigate(['/order/complete']);
    this.status = "completed";
    this.reloadOrders();
  }

  onClickShipped() {
    console.log("Clicked Shipped");
    this.router.navigate(['/order/shipped']);
    this.status = "shipped";
    this.reloadOrders();
  }

  onClickAll() {
    console.log("Clicked All");
    this.router.navigate(['/order/all']);
    this.status = "all";
    this.reloadOrders();
  }

  onEditOrder(orderId: number) {
    console.log("Editing order");
    this.router.navigate(['/order/details/' + orderId]);
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
