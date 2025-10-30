import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Contact, ContactService } from '../services/contact.service';

@Component({
  selector: 'app-contacts',
  imports: [CommonModule],
  templateUrl: './contacts.component.html'
})
export class ContactsComponent implements OnInit {
  contacts = signal<Contact[]>([]);
  isLoading = signal(false);
  errorMessage = signal('');
  selectedContact = signal<Contact | null>(null);

  constructor(
    private readonly contactService: ContactService,
    private readonly router: Router
  ) {}

  ngOnInit() {
    this.loadContacts();
  }

  loadContacts() {
    this.isLoading.set(true);
    this.errorMessage.set('');
    
    this.contactService.getContacts().subscribe({
      next: (contacts) => {
        this.contacts.set(contacts);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Failed to load contacts');
        this.isLoading.set(false);
        console.error('Error loading contacts', error);
      }
    });
  }

  selectContact(contact: Contact) {
    this.selectedContact.set(contact);
  }

  closeDetails() {
    this.selectedContact.set(null);
  }

  goToLogin() {
    this.router.navigate(['/login']);
  }
}
