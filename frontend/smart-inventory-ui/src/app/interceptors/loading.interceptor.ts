import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { finalize } from 'rxjs';
import { SKIP_GLOBAL_LOADER } from '../core/http/http-context.tokens';
import { LoadingService } from '../core/loading/loading.service';
import { environment } from '../../environments/environment';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const loading = inject(LoadingService);
  const isApiRequest = req.url.startsWith(environment.apiUrl);
  const skipLoader = req.context.get(SKIP_GLOBAL_LOADER);

  if (!isApiRequest || skipLoader) {
    return next(req);
  }

  loading.trackStart();

  return next(req).pipe(
    finalize(() => {
      loading.trackEnd();
    }),
  );
};
