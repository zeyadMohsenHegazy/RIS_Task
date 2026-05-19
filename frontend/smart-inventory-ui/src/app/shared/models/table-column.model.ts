export interface TableColumn<T = object> {
  key: string;
  label: string;
  sortable?: boolean;
  /** Optional formatter for cell display */
  format?: (row: T) => string;
}
