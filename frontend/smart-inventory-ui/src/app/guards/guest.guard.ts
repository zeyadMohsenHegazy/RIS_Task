import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../features/auth/auth.service';

/** Prevents authenticated users from accessing guest-only routes (e.g. login). */
export const guestGuard: CanActivateFn = (route) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (!auth.isAuthenticated()) {
    return true;
  }

  const returnUrl = route.queryParamMap.get('returnUrl') ?? '/dashboard';
  return router.parseUrl(returnUrl);
};
