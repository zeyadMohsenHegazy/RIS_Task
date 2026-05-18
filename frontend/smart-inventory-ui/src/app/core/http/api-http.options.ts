import { HttpContext } from '@angular/common/http';
import {
  SKIP_GLOBAL_ERROR_HANDLING,
  SKIP_GLOBAL_LOADER,
} from './http-context.tokens';

/** GETs that show page skeletons — skip the full-screen overlay. */
export function dataRequestOptions() {
  return {
    context: new HttpContext().set(SKIP_GLOBAL_LOADER, true),
  };
}

/** Mutations with local button spinners — skip the full-screen overlay. */
export function mutationRequestOptions() {
  return {
    context: new HttpContext().set(SKIP_GLOBAL_LOADER, true),
  };
}

export function silentRequestOptions() {
  return {
    context: new HttpContext()
      .set(SKIP_GLOBAL_LOADER, true)
      .set(SKIP_GLOBAL_ERROR_HANDLING, true),
  };
}
