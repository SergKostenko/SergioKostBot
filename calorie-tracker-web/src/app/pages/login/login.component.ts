import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import axios from 'axios';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
  <h2>Login</h2>
  <form (ngSubmit)="login()" #f="ngForm" style="max-width:320px; display:grid; gap:8px">
    <input name="email" [(ngModel)]="email" placeholder="Email" required />
    <input name="password" [(ngModel)]="password" placeholder="Password" type="password" required />
    <button type="submit">Login</button>
  </form>
  <h3>Or Register</h3>
  <form (ngSubmit)="register()" #r="ngForm" style="max-width:320px; display:grid; gap:8px">
    <input name="remail" [(ngModel)]="email" placeholder="Email" required />
    <input name="rpassword" [(ngModel)]="password" placeholder="Password" type="password" required />
    <input name="fullname" [(ngModel)]="fullName" placeholder="Full name" />
    <button type="submit">Register</button>
  </form>
  <div *ngIf="error" style="color:red">{{error}}</div>
  `
})
export class LoginComponent {
  email = '';
  password = '';
  fullName = '';
  error = '';
  apiBase = 'http://localhost:5099/api';

  async login() {
    try {
      const res = await axios.post(`${this.apiBase}/auth/login`, { email: this.email, password: this.password });
      localStorage.setItem('token', res.data.token);
      window.location.href = '/log';
    } catch (e: any) {
      this.error = 'Login failed';
    }
  }
  async register() {
    try {
      const res = await axios.post(`${this.apiBase}/auth/register`, { email: this.email, password: this.password, fullName: this.fullName });
      localStorage.setItem('token', res.data.token);
      window.location.href = '/log';
    } catch (e: any) {
      this.error = 'Registration failed';
    }
  }
}