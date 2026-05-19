import { computed, Signal } from '@angular/core';
import { AsyncState } from '../models/async-state.model';

export function selectLoading<T>(state: Signal<AsyncState<T>>): Signal<boolean> {
  return computed(() => state().status === 'loading');
}

export function selectError<T>(state: Signal<AsyncState<T>>): Signal<string | null> {
  return computed(() => state().error);
}

export function selectData<T>(state: Signal<AsyncState<T>>): Signal<T | null> {
  return computed(() => state().data);
}
