import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { TaskListComponent } from './tasks/task-list/task-list.component';
import { TaskCreateComponent } from './tasks/task-create/task-create.component';
import { SubscriptionCreateComponent } from './subscriptions/subscription-create/subscription-create.component';

export const routes: Routes = [
    {path: 'login', component: LoginComponent},
    {path: 'register', component: RegisterComponent},
    {path: 'tasks', component: TaskListComponent},
    {path: 'tasks/create', component: TaskCreateComponent},
    {path: 'subscriptions/create', component: SubscriptionCreateComponent},
    {path: '', redirectTo: '/login', pathMatch: 'full'}
];
