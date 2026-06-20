import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, NavigationEnd, RouterModule } from '@angular/router';
import { filter, Subject, takeUntil } from 'rxjs';
import { AuthService } from '../../services/AuthenticationService';

@Component({
  selector: 'app-header',
  standalone: true,
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
  imports: [RouterModule]
})
export class HeaderComponent implements OnInit, OnDestroy {

  private _isAdmin = false;
  private destroy$ = new Subject<void>();

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit(): void {

    // Run once at load
    this.checkAdmin();

    // Run every time a navigation completes
    this.router.events
      .pipe(
        filter(event => event instanceof NavigationEnd),
        takeUntil(this.destroy$)
      )
      .subscribe(() => {
        this.checkAdmin();
      });
  }

  private checkAdmin(): void {
    this.authService.isAdmin().subscribe(result => {
      this._isAdmin = result;
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public getUserName(): string | null {
    return this.authService.getUser()?.email ?? null;
  }

  public isUserLoggedIn(): boolean {
    return this.authService.isUserLoggedIn();
  }

  public isAdmin(): boolean {
    return this._isAdmin;
  }
}
