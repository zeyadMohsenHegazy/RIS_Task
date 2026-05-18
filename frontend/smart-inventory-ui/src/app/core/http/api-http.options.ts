import { HttpContext } from '@angular/common/http';
import { SKIP_GLOBAL_LOADER } from './http-context.tokens';

/** HTTP options for data reads that show page-level skeletons (skip global overlay). */
export function dataRequestOptions(): { context: HttpContext } {
  return {
    context: new HttpContext().set(SKIP_GLOBAL_LOADER, true),
  };
}
