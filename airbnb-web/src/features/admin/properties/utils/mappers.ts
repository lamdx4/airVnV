import type { PendingPropertyDto, GetPendingPropertiesResponse } from '../api/adminProperties';
import type { PendingProperty } from '../types/adminProperty';

export function toPendingProperty(dto: PendingPropertyDto): PendingProperty {
  return {
    id: dto.propertyId,
    title: dto.title,
    thumbnailUrl: dto.thumbnailUrl,
    hostName: dto.hostName,
    submittedAt: new Date(dto.submittedAt),
    status: dto.status as PendingProperty['status'],
  };
}

export function toPendingPropertyList(response: GetPendingPropertiesResponse): PendingProperty[] {
  return response.items.map(toPendingProperty);
}

export function formatCurrency(amount: number, currencyCode: string = 'USD'): string {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: currencyCode,
  }).format(amount);
}

export function formatDate(date: Date): string {
  return new Intl.DateTimeFormat('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric',
  }).format(date);
}

export function formatTimeSince(date: Date): string {
  const now = new Date();
  const diffMs = now.getTime() - date.getTime();
  const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24));

  if (diffDays === 0) return 'Today';
  if (diffDays === 1) return 'Yesterday';
  if (diffDays < 7) return `${diffDays} days ago`;
  if (diffDays < 30) return `${Math.floor(diffDays / 7)} weeks ago`;
  return formatDate(date);
}