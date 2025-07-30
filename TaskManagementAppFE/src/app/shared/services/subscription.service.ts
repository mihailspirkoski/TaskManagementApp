import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { CreateSubscriptionDto, Subscription } from '../models/subscription.model';
import { Observable } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class SubscriptionService {

  private apiUrl = 'http://localhost:5179/api/subscription';
  private subscription = signal<Subscription | null>(null);

  constructor(private http: HttpClient) { }

  getSubscriptionSignal(){
    return this.subscription.asReadonly();
  }

  setSubscription(subscription: Subscription | null){
    this.subscription.set(subscription);
  }

  createSubscription(dto: CreateSubscriptionDto): Observable<Subscription>{
    return this.http.post<Subscription>(this.apiUrl, dto);
  }

  getSubscription(stripeSubscriptionId: string): Observable<Subscription>{
    return this.http.get<Subscription>(`${this.apiUrl}/${stripeSubscriptionId}`);
  }

}
