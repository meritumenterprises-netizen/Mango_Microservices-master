import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './app/auth-interceptor';
import { LoaderInterceptor } from './app/services/LoaderInterceptor';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { provideRouter } from '@angular/router';
import { routes } from './app/app.routes';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideToastr } from 'ngx-toastr';

bootstrapApplication(AppComponent, {
  providers: [
      provideRouter(routes), 
      provideHttpClient(withInterceptors([AuthInterceptor])), 
      provideAnimations(),
      provideToastr(),
      { provide: HTTP_INTERCEPTORS, useClass: LoaderInterceptor, multi: true }
    ],
}).catch(err => console.error(err));