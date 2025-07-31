import { Component, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

import { CreateSubscriptionDto } from '../../shared/models/subscription.model';
import { SubscriptionService } from '../../shared/services/subscription.service';


@Component({
  selector: 'app-subscription-create',
  imports: [FormsModule, MatInputModule, MatButtonModule],
  templateUrl: './subscription-create.component.html',
  styleUrl: './subscription-create.component.scss'
})
export class SubscriptionCreateComponent {

  dto = signal<CreateSubscriptionDto>({stripeSubscriptionId: ''});

  constructor(private subscriptionService: SubscriptionService, private router: Router){}

  onSubmit(){
    this.subscriptionService.createSubscription(this.dto()).subscribe({
      next: (subscription) => {
        this.subscriptionService.setSubscription(subscription);
        this.router.navigate(['/tasks']);
      },
      error: (err) => console.error('Error creating subscription:', err)
    });
  }
}
