import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Category {
  id: string;
  name: string;
}

@Injectable({
  providedIn: 'root'
})
/* Serwis kategorii */
export class CategoryService {
  private readonly apiUrl = 'http://localhost:5088/api/categories';

  constructor(private readonly http: HttpClient) {}

  /* Pobiera kategorie */
  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(this.apiUrl);
  }
}
