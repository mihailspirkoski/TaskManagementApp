import { Component, signal, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

import { CreateSubscriptionDto, SubscriptionDto } from '../../shared/models/subscription.model';
import { SubscriptionService, } from '../../shared/services/subscription.service';
import { AuthService } from '../../shared/services/auth.service';

@Component({
  selector: 'app-subscription-create',
  standalone: true,
  imports: [FormsModule, MatInputModule, MatButtonModule],
  templateUrl: './subscription-create.component.html',
  styleUrl: './subscription-create.component.scss'
})
export class SubscriptionCreateComponent implements OnInit {
  priceId = signal<string>('price_1S5sS4Am5NUdbBIdydeGck0y'); // Replace with your Stripe Price ID
  error = signal<string | null>(null);
  private stripe: any; // Use 'any' temporarily if @types/stripe-v3 fails
  private elements: any;
  private cardElement: any;

  constructor(
    private subscriptionService: SubscriptionService,
    private authService: AuthService,
    private router: Router
  ) {
    // Initialize Stripe
    this.stripe = (window as any).Stripe('pk_test_51S5sFTAm5NUdbBIdVFwYyfh4XllupRpg6J6fmVws4MLuR3nX7JYVyha0FGlIHaSZ0fVIUqZhqMrqfa0hKZnjF9aa00YeFUZx9Y'); // Replace with your Stripe Publishable Key
    this.elements = this.stripe.elements();
    this.cardElement = this.elements.create('card');
  }

  ngOnInit() {
    if (!this.authService.getToken()) {
      this.router.navigate(['/login']);
      return;
    }
    this.cardElement.mount('#card-element');
  }

  async onSubmit() {
    try {
      const { paymentMethod, error } = await this.stripe.createPaymentMethod({
        type: 'card',
        card: this.cardElement,
        id: this.cardElement.id
      });

      if (error) {
        this.error.set(error.message);
        return;
      }

      const dto: CreateSubscriptionDto = {
        priceId: this.priceId(),
        paymentMethodId: paymentMethod.id
      };

      this.subscriptionService.createSubscription(dto).subscribe({
        next: (subscription: SubscriptionDto) => {
          this.router.navigate(['/subscriptions', subscription.stripeSubscriptionId]);
        },
        error: (err) => {
          if (err.status === 401) {
            this.authService.setToken(null);
            this.router.navigate(['/login']);
          } else {
            this.error.set('Failed to create subscription: ' + err.message);
          }
        }
      });
    } catch (err) {
      this.error.set('Stripe error: ' + (err as Error).message);
    }
  }
}