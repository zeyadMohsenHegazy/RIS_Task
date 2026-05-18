/** Matches backend SmartInventorySystem.Domain.Enums.TransactionType */
export enum TransactionType {
  In = 1,
  Out = 2,
}

export interface InventoryTransactionDto {
  id: number;
  productId: number;
  productName: string;
  productSku: string;
  quantity: number;
  transactionType: TransactionType;
  transactionDate: string;
  createdByUserId: number;
  createdByUsername: string;
}

export interface InventoryHistoryQueryParams {
  pageNumber: number;
  pageSize: number;
  search?: string;
  transactionType?: TransactionType | null;
}

export interface TransactionTypeFilterOption {
  label: string;
  value: TransactionType | null;
}

export const TRANSACTION_TYPE_FILTER_OPTIONS: TransactionTypeFilterOption[] = [
  { label: 'All types', value: null },
  { label: 'Stock In', value: TransactionType.In },
  { label: 'Stock Out', value: TransactionType.Out },
];

export function getTransactionTypeLabel(type: TransactionType): string {
  return type === TransactionType.In ? 'Stock In' : 'Stock Out';
}

export interface InventoryMovementDto {
  productId: number;
  quantity: number;
}

export interface InventoryMovementDialogData {
  productId?: number;
  transactionType?: TransactionType;
}

export const TRANSACTION_TYPE_FORM_OPTIONS = [
  { label: 'Stock In', value: TransactionType.In },
  { label: 'Stock Out', value: TransactionType.Out },
] as const;
