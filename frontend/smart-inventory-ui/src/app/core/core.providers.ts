import { EnvironmentProviders } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideToastr } from 'ngx-toastr';
import { authInterceptor, errorInterceptor, loadingInterceptor } from '../interceptors';

/** Core application providers — import in app.config.ts */
export function provideCore(): EnvironmentProviders[] {
  return [
    provideHttpClient(
      withInterceptors([authInterceptor, loadingInterceptor, errorInterceptor]),
    ),
    provideToastr({
      timeOut: 4000,
      positionClass: 'toast-top-right',
      preventDuplicates: true,
    }),
  ];
}
