import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { sanitizeReturnUrl } from '../core/navigation/return-url.util';
import { AuthService } from '../features/auth/auth.service';

/** Prevents authenticated users from accessing guest-only routes (e.g. login). */
export const guestGuard: CanActivateFn = (route) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (!auth.isLoggedIn()) {
    return true;
  }

  const returnUrl = sanitizeReturnUrl(route.queryParamMap.get('returnUrl'));
  return router.parseUrl(returnUrl);
};
