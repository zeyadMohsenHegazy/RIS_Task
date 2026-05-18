export type LoadStatus = 'idle' | 'loading' | 'success' | 'error';

export interface AsyncState<T> {
  status: LoadStatus;
  data: T | null;
  error: string | null;
}

export function idleState<T>(): AsyncState<T> {
  return { status: 'idle', data: null, error: null };
}

export function loadingState<T>(previous?: T | null): AsyncState<T> {
  return { status: 'loading', data: previous ?? null, error: null };
}

export function successState<T>(data: T): AsyncState<T> {
  return { status: 'success', data, error: null };
}

export function errorState<T>(message: string, previous?: T | null): AsyncState<T> {
  return { status: 'error', data: previous ?? null, error: message };
}
