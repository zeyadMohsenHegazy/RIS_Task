import { DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { SEARCH_DEBOUNCE_MS } from '../constants/pagination.constants';

/** Debounced search control → callback (resets to page 1 in the callback). */
export function bindDebouncedSearch(
  control: FormControl<string>,
  destroyRef: DestroyRef,
  onSearch: (value: string) => void,
): void {
  control.valueChanges
    .pipe(
      debounceTime(SEARCH_DEBOUNCE_MS),
      distinctUntilChanged(),
      takeUntilDestroyed(destroyRef),
    )
    .subscribe(onSearch);
}
