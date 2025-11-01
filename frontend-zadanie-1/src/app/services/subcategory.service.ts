import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Subcategory {
  id: string;
  name: string;
  categoryId: string;
}

export interface CreateSubcategoryRequest {
  subcategoryName: string;
  categoryName: string;
}

@Injectable({
  providedIn: 'root'
})
export class SubcategoryService {
  private readonly apiUrl = 'http://localhost:5088/api/subcategories';

  constructor(private readonly http: HttpClient) {}

  getSubcategoriesByCategoryName(categoryName: string): Observable<Subcategory[]> {
    const params = new HttpParams().set('name', categoryName);
    return this.http.get<Subcategory[]>(this.apiUrl, { params });
  }

  createSubcategory(request: CreateSubcategoryRequest): Observable<Subcategory> {
    return this.http.post<Subcategory>(this.apiUrl, request);
  }
}
