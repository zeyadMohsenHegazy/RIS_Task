export interface TableColumn<T = Record<string, unknown>> {
  key: string;
  label: string;
  sortable?: boolean;
  /** Optional formatter for cell display */
  format?: (row: T) => string;
}
