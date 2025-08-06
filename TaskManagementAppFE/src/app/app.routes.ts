import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { TaskListComponent } from './tasks/task-list/task-list.component';
import { TaskCreateComponent } from './tasks/task-create/task-create.component';
import { SubscriptionCreateComponent } from './subscriptions/subscription-create/subscription-create.component';
import { TaskEditComponent } from './tasks/task-edit/task-edit.component';
import { SubscriptionViewComponent } from './subscriptions/subscription-view/subscription-view.component';

export const routes: Routes = [
    {path: 'login', component: LoginComponent},
    {path: 'register', component: RegisterComponent},
    {path: 'tasks', component: TaskListComponent},
    {path: 'tasks/create', component: TaskCreateComponent},
    {path: 'tasks/edit/:id', component: TaskEditComponent},
    {path: 'subscriptions/:id', component: SubscriptionViewComponent},
    {path: 'subscription/create', component: SubscriptionCreateComponent},
    {path: '', redirectTo: '/login', pathMatch: 'full'},
    {path: '**', redirectTo: '/login'}
];
