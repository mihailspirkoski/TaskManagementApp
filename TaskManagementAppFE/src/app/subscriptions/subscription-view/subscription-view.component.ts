import { Component, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';

import { Subscription } from '../../shared/models/subscription.model';
import { SubscriptionService } from '../../shared/services/subscription.service';

@Component({
  selector: 'app-subscription-view',
  imports: [MatCardModule],
  templateUrl: './subscription-view.component.html',
  styleUrl: './subscription-view.component.scss'
})
export class SubscriptionViewComponent {

  subscription = signal<Subscription | null>(null);

  constructor(private subscriptionService: SubscriptionService, private route: ActivatedRoute){
    const id = this.route.snapshot.paramMap.get('id');
    if(id){
      this.subscriptionService.getSubscription(id).subscribe({
        next: (sub) => this.subscription.set(sub),
        error: (err) => console.error('Failed to load subscription:', err)
      });
    }
  }
}
