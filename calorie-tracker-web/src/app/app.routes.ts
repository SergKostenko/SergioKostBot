import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'log', pathMatch: 'full' },
  { path: 'login', loadComponent: () => import('./pages/login/login.component').then(m => m.LoginComponent) },
  { path: 'foods', loadComponent: () => import('./pages/foods/foods.component').then(m => m.FoodsComponent) },
  { path: 'dishes', loadComponent: () => import('./pages/dishes/dishes.component').then(m => m.DishesComponent) },
  { path: 'log', loadComponent: () => import('./pages/log/log.component').then(m => m.LogComponent) },
  { path: '**', redirectTo: 'log' }
];
