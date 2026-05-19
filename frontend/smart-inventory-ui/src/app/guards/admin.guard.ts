import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { NotificationService } from '../core/notifications/notification.service';
import { AuthService } from '../features/auth/auth.service';

export const adminGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const notifications = inject(NotificationService);

  if (auth.isAdmin()) {
    return true;
  }

  notifications.warning('You do not have permission to access this page.');
  return router.createUrlTree(['/home']);
};
