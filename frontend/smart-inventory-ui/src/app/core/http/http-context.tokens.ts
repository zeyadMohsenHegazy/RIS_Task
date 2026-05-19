import { HttpContextToken } from '@angular/common/http';

/** When true, the global error interceptor will not show a toast for failed requests. */
export const SKIP_GLOBAL_ERROR_HANDLING = new HttpContextToken<boolean>(() => false);

/** When true, the global loading overlay will not track this request. */
export const SKIP_GLOBAL_LOADER = new HttpContextToken<boolean>(() => false);
