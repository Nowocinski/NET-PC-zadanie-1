import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Contact {
  id: string;
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  birthDate: string;
  password: string;
  categoryName?: string;
  subcategoryName?: string;
}

export interface CreateContactRequest {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  birthDate: string;
  categoryId?: string;
  subcategoryId?: string;
  password: string;
}

export interface UpdateContactRequest {
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  birthDate: string;
  categoryId?: string;
  subcategoryId?: string;
  password: string;
}

@Injectable({
  providedIn: 'root'
})
export class ContactService {
  private readonly apiUrl = 'http://localhost:5088/api/contacts';

  constructor(private readonly http: HttpClient) {}

  /* Pobiera kontakty */
  getContacts(): Observable<Contact[]> {
    return this.http.get<Contact[]>(this.apiUrl);
  }

  /* Dodaje kontakt */
  createContact(request: CreateContactRequest): Observable<Contact> {
    return this.http.post<Contact>(this.apiUrl, request);
  }

  /* Aktualizuje kontakt */
  updateContact(id: string, request: UpdateContactRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, request);
  }

  /* Usuwa kontakt */
  deleteContact(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
