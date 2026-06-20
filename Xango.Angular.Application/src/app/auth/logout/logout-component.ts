import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/AuthenticationService';

@Component({
  selector: 'app-logout-component',
  imports: [RouterModule],
  templateUrl: './logout-component.html',
  styleUrl: './logout-component.css'
})
export class LogoutComponent {
  constructor(private authService: AuthService, private router : Router) {
    if (this.authService.isUserLoggedIn()) {
      this.authService.logout();
      this.router.navigate(['/']);
    }
  }
 
}
