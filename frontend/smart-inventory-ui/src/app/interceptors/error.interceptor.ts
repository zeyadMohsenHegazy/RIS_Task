import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { SKIP_GLOBAL_ERROR_HANDLING } from '../core/http/http-context.tokens';
import { NotificationService } from '../core/notifications/notification.service';
import { environment } from '../../environments/environment';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const notifications = inject(NotificationService);
  const isApiRequest = req.url.startsWith(environment.apiUrl);
  const isLoginRequest = req.url.includes('/auth/login');
  const skipHandling = req.context.get(SKIP_GLOBAL_ERROR_HANDLING);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (!isApiRequest || skipHandling) {
        return throwError(() => error);
      }

      // Login page shows its own invalid-credentials message.
      if (isLoginRequest) {
        return throwError(() => error);
      }

      // Session expiry toast is shown from auth interceptor after logout.
      if (error.status === 401) {
        return throwError(() => error);
      }

      notifications.handleHttpError(error);
      return throwError(() => error);
    }),
  );
};
