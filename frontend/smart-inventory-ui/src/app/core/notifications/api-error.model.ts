export type ApiErrorCategory =
  | 'unauthorized'
  | 'validation'
  | 'notFound'
  | 'forbidden'
  | 'network'
  | 'server'
  | 'client'
  | 'unknown';

export interface ParsedApiError {
  category: ApiErrorCategory;
  title: string;
  message: string;
  fieldMessages: string[];
  statusCode: number;
}
