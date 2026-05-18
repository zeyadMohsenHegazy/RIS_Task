import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { NotificationService } from '../core/notifications/notification.service';
import { AuthService } from '../features/auth/auth.service';
import { environment } from '../../environments/environment';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const notifications = inject(NotificationService);

  const token = auth.getToken();
  const isApiRequest = req.url.startsWith(environment.apiUrl);
  const isLoginRequest = req.url.includes('/auth/login');

  let authReq = req;
  if (token && isApiRequest && !isLoginRequest) {
    authReq = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` },
    });
  }

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && isApiRequest && !isLoginRequest && token) {
        const returnUrl =
          router.url.startsWith('/login') || router.url === '/'
            ? undefined
            : router.url;

        auth.logout(false);
        notifications.error(
          'Your session has expired. Please sign in again.',
          'Unauthorized',
        );

        void router.navigate(['/login'], {
          queryParams: returnUrl ? { returnUrl } : {},
          replaceUrl: true,
        });
      }
      return throwError(() => error);
    }),
  );
};
