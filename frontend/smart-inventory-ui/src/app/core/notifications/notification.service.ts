import { HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { ActiveToast, ToastrService } from 'ngx-toastr';
import { ParsedApiError } from './api-error.model';
import { parseHttpError } from './http-error.parser';

export interface NotificationOptions {
  title?: string;
  timeOut?: number;
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private readonly toastr = inject(ToastrService);

  success(message: string, title = 'Success', options?: NotificationOptions): ActiveToast<unknown> {
    return this.toastr.success(message, title, this.toOptions(options));
  }

  error(message: string, title = 'Error', options?: NotificationOptions): ActiveToast<unknown> {
    return this.toastr.error(message, title, this.toOptions(options));
  }

  warning(message: string, title = 'Warning', options?: NotificationOptions): ActiveToast<unknown> {
    return this.toastr.warning(message, title, this.toOptions(options));
  }

  info(message: string, title = 'Info', options?: NotificationOptions): ActiveToast<unknown> {
    return this.toastr.info(message, title, this.toOptions(options));
  }

  /** Display a parsed API error (validation fields, server message, etc.). */
  showApiError(parsed: ParsedApiError, options?: NotificationOptions): void {
    const title = options?.title ?? parsed.title;
    const timeOut = parsed.category === 'validation' ? 6000 : options?.timeOut;

    if (parsed.fieldMessages.length > 3) {
      const summary = parsed.fieldMessages.slice(0, 3).join(' · ');
      const extra = parsed.fieldMessages.length - 3;
      this.error(`${summary} (+${extra} more)`, title, { timeOut });
      return;
    }

    this.error(parsed.message, title, { timeOut });
  }

  /** Parse and display an HTTP error response. */
  handleHttpError(error: HttpErrorResponse, options?: NotificationOptions): ParsedApiError {
    const parsed = parseHttpError(error);
    this.showApiError(parsed, options);
    return parsed;
  }

  private toOptions(options?: NotificationOptions): Record<string, unknown> {
    if (!options?.timeOut) {
      return {};
    }
    return { timeOut: options.timeOut };
  }
}
