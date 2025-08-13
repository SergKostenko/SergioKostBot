import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import axios from 'axios';

interface Food { id: number; name: string; caloriesPer100g: number; proteinPer100g: number; carbsPer100g: number; fatPer100g: number; }

@Component({
  selector: 'app-foods',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
  <h2>Foods</h2>
  <div style="display:grid; gap:8px; max-width:420px">
    <input [(ngModel)]="form.name" placeholder="Name" />
    <input [(ngModel)]="form.caloriesPer100g" placeholder="Calories/100g" type="number" />
    <input [(ngModel)]="form.proteinPer100g" placeholder="Protein/100g" type="number" />
    <input [(ngModel)]="form.carbsPer100g" placeholder="Carbs/100g" type="number" />
    <input [(ngModel)]="form.fatPer100g" placeholder="Fat/100g" type="number" />
    <button (click)="save()">Add</button>
  </div>
  <ul>
    <li *ngFor="let f of foods">
      {{f.name}} - {{f.caloriesPer100g}} kcal/100g
      <button (click)="remove(f.id)">Delete</button>
    </li>
  </ul>
  `
})
export class FoodsComponent {
  apiBase = 'http://localhost:5099/api';
  foods: Food[] = [];
  form: any = { name: '', caloriesPer100g: 0, proteinPer100g: 0, carbsPer100g: 0, fatPer100g: 0 };

  async ngOnInit() {
    await this.load();
  }
  token() { return localStorage.getItem('token'); }
  headers() { return { Authorization: `Bearer ${this.token()}` }; }
  async load() {
    const res = await axios.get(`${this.apiBase}/foods`, { headers: this.headers() });
    this.foods = res.data;
  }
  async save() {
    await axios.post(`${this.apiBase}/foods`, this.form, { headers: this.headers() });
    this.form = { name: '', caloriesPer100g: 0, proteinPer100g: 0, carbsPer100g: 0, fatPer100g: 0 };
    await this.load();
  }
  async remove(id: number) {
    await axios.delete(`${this.apiBase}/foods/${id}`, { headers: this.headers() });
    await this.load();
  }
}