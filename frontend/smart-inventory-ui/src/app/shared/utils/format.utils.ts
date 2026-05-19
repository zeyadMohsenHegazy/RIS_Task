const currencyFormatter = new Intl.NumberFormat(undefined, {
  style: 'currency',
  currency: 'USD',
});

export function formatCurrency(value: number): string {
  return currencyFormatter.format(value);
}

export function formatDateTime(isoDate: string): string {
  return new Date(isoDate).toLocaleString();
}
