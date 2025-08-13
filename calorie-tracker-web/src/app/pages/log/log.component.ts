import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import axios from 'axios';

interface Food { id: number; name: string; caloriesPer100g: number; }
interface Dish { id: number; name: string; items: { grams: number; food: Food }[] }
interface Entry { id: number; foodId?: number; dishId?: number; grams: number; calories: number }

@Component({
  selector: 'app-log',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
  <h2>Daily Log</h2>
  <div style="display:flex; gap:8px; align-items:center">
    <input type="date" [(ngModel)]="dateStr" />
    <button (click)="load()">Load</button>
  </div>
  <div style="margin-top:12px">
    <select [(ngModel)]="addType">
      <option value="food">Food</option>
      <option value="dish">Dish</option>
    </select>
    <select *ngIf="addType==='food'" [(ngModel)]="selectedFoodId">
      <option *ngFor="let f of foods" [value]="f.id">{{f.name}}</option>
    </select>
    <select *ngIf="addType==='dish'" [(ngModel)]="selectedDishId">
      <option *ngFor="let d of dishes" [value]="d.id">{{d.name}}</option>
    </select>
    <input type="number" [(ngModel)]="grams" placeholder="grams" />
    <button (click)="addEntry()">Add</button>
  </div>
  <div *ngIf="log">
    <h3>Total: {{log.totalCalories}} kcal</h3>
    <ul>
      <li *ngFor="let e of log.entries">
        <span *ngIf="e.foodId">Food #{{e.foodId}}</span>
        <span *ngIf="e.dishId">Dish #{{e.dishId}}</span>
        - {{e.grams}} g = {{e.calories}} kcal
        <button (click)="remove(e.id)">x</button>
      </li>
    </ul>
  </div>
  `
})
export class LogComponent {
  apiBase = 'http://localhost:5099/api';
  dateStr = new Date().toISOString().slice(0,10);
  addType: 'food' | 'dish' = 'food';
  selectedFoodId?: number;
  selectedDishId?: number;
  grams = 100;
  foods: Food[] = [];
  dishes: Dish[] = [];
  log: { totalCalories: number; entries: Entry[] } | null = null;

  async ngOnInit() {
    await Promise.all([this.loadFoods(), this.loadDishes()]);
    await this.load();
  }
  token() { return localStorage.getItem('token'); }
  headers() { return { Authorization: `Bearer ${this.token()}` }; }
  async loadFoods() {
    const res = await axios.get(`${this.apiBase}/foods`, { headers: this.headers() });
    this.foods = res.data;
    this.selectedFoodId = this.foods[0]?.id;
  }
  async loadDishes() {
    const res = await axios.get(`${this.apiBase}/dishes`, { headers: this.headers() });
    this.dishes = res.data;
    this.selectedDishId = this.dishes[0]?.id;
  }
  async load() {
    const res = await axios.get(`${this.apiBase}/logs/${this.dateStr}`, { headers: this.headers() });
    this.log = res.data;
  }
  async addEntry() {
    const body: any = { grams: this.grams };
    if (this.addType === 'food') body.foodId = this.selectedFoodId; else body.dishId = this.selectedDishId;
    await axios.post(`${this.apiBase}/logs/${this.dateStr}/entries`, body, { headers: this.headers() });
    await this.load();
  }
  async remove(id: number) {
    await axios.delete(`${this.apiBase}/logs/entries/${id}`, { headers: this.headers() });
    await this.load();
  }
}