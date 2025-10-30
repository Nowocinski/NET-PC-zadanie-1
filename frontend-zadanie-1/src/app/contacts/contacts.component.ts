import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Contact, ContactService } from '../services/contact.service';
import { AuthService } from '../services/auth.service';

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
  isAuthenticated = signal(false);

  constructor(
    private readonly contactService: ContactService,
    private readonly router: Router,
    private readonly authService: AuthService
  ) {}

  ngOnInit() {
    this.isAuthenticated.set(this.authService.isAuthenticated());
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

  editContact(contact: Contact) {
    // TODO: Implement
    console.log('Edit contact', contact);
  }

  deleteContact(contactId: string) {
    if (!confirm('Are you sure you want to delete this contact?')) {
      return;
    }

    this.contactService.deleteContact(contactId).subscribe({
      next: () => {
        // Remove contact from the list
        const updatedContacts = this.contacts().filter(c => c.id !== contactId);
        this.contacts.set(updatedContacts);
        
        // Close details if deleted contact was selected
        if (this.selectedContact()?.id === contactId) {
          this.selectedContact.set(null);
        }
      },
      error: (error) => {
        this.errorMessage.set('Failed to delete contact');
        console.error('Delete failed', error);
      }
    });
  }
}
