import { Component, signal } from '@angular/core';
import { HeaderComponent } from './layout/header/header.component';
import { ContainerComponent } from "./layout/container/container.component";
import { FooterComponent } from './layout/footer/footer.component';
import { CommonModule } from '@angular/common';
import { ToastrService, ToastrModule } from 'ngx-toastr';
import { LoaderComponent } from './loader-component/loader-component';
import { RouterOutlet, Router } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [HeaderComponent, ContainerComponent, FooterComponent, CommonModule, ToastrModule, LoaderComponent ],
  templateUrl: './app.component.html',
  styleUrl: "./app.component.css"
})
export class AppComponent {
  protected readonly title = signal('Xango Angular Application');

  constructor(
    private router: Router
  ) {
    const path = window.location.pathname;
    const hash = window.location.hash;

    // If hash exists, Angular hash routing can handle it
    if (hash) return;

    // If URL matches /cart/confirmation/:id, navigate programmatically
    const match = path.match(/^\/cart\/confirmation\/(\d+)$/);
    if (match) {
      const id = match[1];
      // Use Angular Router to navigate to the route
      this.router.navigate(['/cart/confirmation', id]);
    }  
  }
}
