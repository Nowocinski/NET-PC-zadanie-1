import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Contact, ContactService } from '../services/contact.service';

@Component({
  selector: 'app-contacts',
  imports: [CommonModule],
  templateUrl: './contacts.component.html'
})
export class ContactsComponent implements OnInit {
  contacts: Contact[] = [];
  isLoading = false;
  errorMessage = '';

  constructor(private readonly contactService: ContactService) {}

  ngOnInit() {
    this.loadContacts();
  }

  loadContacts() {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.contactService.getContacts().subscribe({
      next: (contacts) => {
        this.contacts = contacts;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load contacts';
        this.isLoading = false;
        console.error('Error loading contacts', error);
      }
    });
  }
}
