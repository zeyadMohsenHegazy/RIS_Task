import { HttpErrorResponse } from '@angular/common/http';
import { ApiErrorCategory, ParsedApiError } from './api-error.model';

interface ApiErrorBody {
  message?: string;
  statusCode?: number;
  errors?: Record<string, string[]>;
  title?: string;
  detail?: string;
}

export function parseHttpError(error: HttpErrorResponse): ParsedApiError {
  const statusCode = error.status;
  const body = extractBody(error);
  const fieldMessages = flattenValidationErrors(body?.errors);
  const message = resolveMessage(error, body, fieldMessages);
  const category = resolveCategory(statusCode, fieldMessages.length > 0);
  const title = resolveTitle(category);

  return {
    category,
    title,
    message,
    fieldMessages,
    statusCode,
  };
}

function extractBody(error: HttpErrorResponse): ApiErrorBody | null {
  if (!error.error || typeof error.error !== 'object') {
    return null;
  }
  return error.error as ApiErrorBody;
}

function flattenValidationErrors(
  errors?: Record<string, string[]>,
): string[] {
  if (!errors) {
    return [];
  }

  return Object.entries(errors).flatMap(([field, messages]) =>
    messages.map((msg) => (field ? `${field}: ${msg}` : msg)),
  );
}

function resolveMessage(
  error: HttpErrorResponse,
  body: ApiErrorBody | null,
  fieldMessages: string[],
): string {
  if (error.status === 0) {
    return 'Unable to reach the server. Check your network connection and try again.';
  }

  if (fieldMessages.length > 0) {
    return fieldMessages.slice(0, 3).join(' · ');
  }

  if (body?.message?.trim()) {
    return body.message.trim();
  }

  if (body?.detail?.trim()) {
    return body.detail.trim();
  }

  if (body?.title?.trim()) {
    return body.title.trim();
  }

  if (typeof error.error === 'string' && error.error.trim()) {
    return error.error.trim();
  }

  return defaultMessageForStatus(error.status);
}

function resolveCategory(
  statusCode: number,
  hasValidationErrors: boolean,
): ApiErrorCategory {
  if (statusCode === 0) {
    return 'network';
  }
  if (statusCode === 401) {
    return 'unauthorized';
  }
  if (statusCode === 403) {
    return 'forbidden';
  }
  if (statusCode === 404) {
    return 'notFound';
  }
  if (hasValidationErrors || statusCode === 400 || statusCode === 422) {
    return 'validation';
  }
  if (statusCode >= 500) {
    return 'server';
  }
  if (statusCode >= 400) {
    return 'client';
  }
  return 'unknown';
}

function resolveTitle(category: ApiErrorCategory): string {
  switch (category) {
    case 'unauthorized':
      return 'Unauthorized';
    case 'validation':
      return 'Validation Error';
    case 'notFound':
      return 'Not Found';
    case 'forbidden':
      return 'Forbidden';
    case 'network':
      return 'Network Error';
    case 'server':
      return 'Server Error';
    case 'client':
      return 'Request Error';
    default:
      return 'Error';
  }
}

function defaultMessageForStatus(status: number): string {
  switch (status) {
    case 401:
      return 'You are not authorized. Please sign in again.';
    case 403:
      return 'You do not have permission to perform this action.';
    case 404:
      return 'The requested resource was not found.';
    case 400:
      return 'The request was invalid. Please check your input.';
    case 422:
      return 'The submitted data could not be processed.';
    default:
      if (status >= 500) {
        return 'An unexpected server error occurred. Please try again later.';
      }
      return 'An unexpected error occurred. Please try again.';
  }
}
