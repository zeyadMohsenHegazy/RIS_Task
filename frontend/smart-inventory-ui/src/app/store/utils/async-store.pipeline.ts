import { WritableSignal } from '@angular/core';
import { Observable, Subject, Subscription, finalize, switchMap, tap } from 'rxjs';
import {
  AsyncState,
  errorState,
  loadingState,
  successState,
} from '../models/async-state.model';

export interface AsyncStorePipelineOptions<T> {
  load$: Subject<void>;
  state: WritableSignal<AsyncState<T>>;
  request: () => Observable<T>;
  errorMessage: string;
  onSuccess?: (data: T) => void;
}

/** Wires a Subject-driven load pipeline into an AsyncState signal. */
export function connectAsyncStorePipeline<T>(
  options: AsyncStorePipelineOptions<T>,
): Subscription {
  const { load$, state, request, errorMessage, onSuccess } = options;

  return load$
    .pipe(
      tap(() => state.set(loadingState(state().data))),
      switchMap(() => request().pipe(finalize(() => undefined))),
    )
    .subscribe({
      next: (data) => {
        state.set(successState(data));
        onSuccess?.(data);
      },
      error: () => state.set(errorState(errorMessage, state().data)),
    });
}
