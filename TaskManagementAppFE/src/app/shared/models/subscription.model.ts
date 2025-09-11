export interface Subscription{
    id: number;
    userId: number;
    stripeSubscriptionId: string;
    status: string; //'Active' | 'Inactive' | 'Cancelled' | 'PastDue' | 'Unpaid' | 'Trialing' | 'Paused';
    createdAt: string;
}

export interface CreateSubscriptionDto{
    priceId: string;
    paymentMethodId: string;
}

export interface SubscriptionDto {
  id: number;
  userId: number;
  stripeSubscriptionId: string;
  status: string; //'Active' | 'Inactive' | 'Cancelled' | 'PastDue' | 'Unpaid' | 'Trialing' | 'Paused';
  createdAt: string;
  updatedAt: string;
}