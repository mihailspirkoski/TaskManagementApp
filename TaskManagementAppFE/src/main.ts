import { bootstrapApplication } from '@angular/platform-browser';
//import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './app/shared/interceptors/auth.interceptor';
import { routes } from './app/app.routes';
import { provideAnimations } from '@angular/platform-browser/animations';
import { MatDialogModule } from '@angular/material/dialog';

bootstrapApplication(AppComponent, {
       providers: [
         provideRouter(routes),
         provideHttpClient(withInterceptors([authInterceptor])),
         provideAnimations(),
         MatDialogModule
       ]
     })
  .catch((err) => console.error(err));
