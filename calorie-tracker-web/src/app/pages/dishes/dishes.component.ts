import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import axios from 'axios';

interface Food { id: number; name: string; caloriesPer100g: number; }
interface DishItem { foodId: number; grams: number; food?: Food }
interface Dish { id: number; name: string; items: DishItem[] }

@Component({
  selector: 'app-dishes',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
  <h2>Dishes</h2>
  <div style="display:grid; gap:8px; max-width:500px">
    <input [(ngModel)]="name" placeholder="Dish name" />
    <div *ngFor="let item of newItems; let i=index" style="display:flex; gap:8px">
      <select [(ngModel)]="item.foodId">
        <option [ngValue]="undefined" disabled>Select food</option>
        <option *ngFor="let f of foods" [value]="f.id">{{f.name}} ({{f.caloriesPer100g}})</option>
      </select>
      <input [(ngModel)]="item.grams" type="number" placeholder="grams" />
      <button (click)="removeItem(i)">x</button>
    </div>
    <button (click)="addItem()">Add item</button>
    <button (click)="save()">Save dish</button>
  </div>
  <h3>My Dishes</h3>
  <ul>
    <li *ngFor="let d of dishes">
      {{d.name}} (items: {{d.items.length}})
      <button (click)="delete(d.id)">Delete</button>
    </li>
  </ul>
  `
})
export class DishesComponent {
  apiBase = 'http://localhost:5099/api';
  foods: Food[] = [];
  dishes: Dish[] = [];
  name = '';
  newItems: DishItem[] = [];

  async ngOnInit() {
    await Promise.all([this.loadFoods(), this.loadDishes()]);
  }
  token() { return localStorage.getItem('token'); }
  headers() { return { Authorization: `Bearer ${this.token()}` }; }
  async loadFoods() {
    const res = await axios.get(`${this.apiBase}/foods`, { headers: this.headers() });
    this.foods = res.data;
  }
  async loadDishes() {
    const res = await axios.get(`${this.apiBase}/dishes`, { headers: this.headers() });
    this.dishes = res.data;
  }
  addItem() { this.newItems.push({ foodId: (this.foods[0]?.id ?? 0), grams: 100 }); }
  removeItem(i: number) { this.newItems.splice(i,1); }
  async save() {
    await axios.post(`${this.apiBase}/dishes`, { name: this.name, items: this.newItems }, { headers: this.headers() });
    this.name=''; this.newItems=[];
    await this.loadDishes();
  }
  async delete(id: number) {
    await axios.delete(`${this.apiBase}/dishes/${id}`, { headers: this.headers() });
    await this.loadDishes();
  }
}