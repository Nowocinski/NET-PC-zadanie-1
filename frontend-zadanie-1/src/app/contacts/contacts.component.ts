import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Contact, ContactService, CreateContactRequest, UpdateContactRequest } from '../services/contact.service';
import { AuthService } from '../services/auth.service';
import { Category, CategoryService } from '../services/category.service';
import { Subcategory, SubcategoryService } from '../services/subcategory.service';

@Component({
  selector: 'app-contacts',
  imports: [CommonModule, FormsModule],
  templateUrl: './contacts.component.html'
})
export class ContactsComponent implements OnInit {
  contacts = signal<Contact[]>([]);
  categories = signal<Category[]>([]);
  subcategories = signal<Subcategory[]>([]);
  isLoading = signal(false);
  errorMessage = signal('');
  selectedContact = signal<Contact | null>(null);
  isAuthenticated = signal(false);
  showAddForm = signal(false);
  showEditForm = signal(false);
  editingContact = signal<Contact | null>(null);
  
  newContact = {
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    birthDate: '',
    password: '',
    categoryId: ''
  };

  editContactData = {
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    birthDate: '',
    password: '',
    categoryId: ''
  };

  constructor(
    private readonly contactService: ContactService,
    private readonly router: Router,
    private readonly authService: AuthService,
    private readonly categoryService: CategoryService,
    private readonly subcategoryService: SubcategoryService
  ) {}

  ngOnInit() {
    this.isAuthenticated.set(this.authService.isAuthenticated());
    this.loadContacts();
    this.loadCategories();
    this.loadSubcategories();
  }

  loadCategories() {
    this.categoryService.getCategories().subscribe({
      next: (categories) => {
        this.categories.set(categories);
      },
      error: (error) => {
        console.error('Error loading categories', error);
      }
    });
  }

  loadSubcategories() {
    this.subcategoryService.getSubcategoriesByCategoryName('Służbowy').subscribe({
      next: (subcategories) => {
        this.subcategories.set(subcategories);
      },
      error: (error) => {
        console.error('Error loading subcategories', error);
      }
    });
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

  showEditContactForm(contact: Contact) {
    this.editingContact.set(contact);
    this.editContactData = {
      firstName: contact.firstName,
      lastName: contact.lastName,
      email: contact.email,
      phone: contact.phone,
      birthDate: contact.birthDate.split('T')[0], // Convert to YYYY-MM-DD format
      password: '',
      categoryId: contact.categoryId || ''
    };
    this.showEditForm.set(true);
  }

  hideEditContactForm() {
    this.showEditForm.set(false);
    this.editingContact.set(null);
  }

  updateContact() {
    const contact = this.editingContact();
    if (!contact) return;

    const request: UpdateContactRequest = {
      userId: contact.userId,
      firstName: this.editContactData.firstName,
      lastName: this.editContactData.lastName,
      email: this.editContactData.email,
      phone: this.editContactData.phone,
      birthDate: this.editContactData.birthDate,
      password: this.editContactData.password,
      categoryId: this.editContactData.categoryId ? this.editContactData.categoryId : undefined
    };

    this.contactService.updateContact(contact.id, request).subscribe({
      next: () => {
        // Update contact in the list
        const updatedContacts = this.contacts().map(c => 
          c.id === contact.id 
            ? { ...c, ...request, id: contact.id, birthDate: this.editContactData.birthDate }
            : c
        );
        this.contacts.set(updatedContacts);
        this.hideEditContactForm();
      },
      error: (error) => {
        this.errorMessage.set('Failed to update contact');
        console.error('Update failed', error);
      }
    });
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

  showAddContactForm() {
    this.showAddForm.set(true);
    this.resetNewContactForm();
  }

  hideAddContactForm() {
    this.showAddForm.set(false);
    this.resetNewContactForm();
  }

  resetNewContactForm() {
    this.newContact = {
      firstName: '',
      lastName: '',
      email: '',
      phone: '',
      birthDate: '',
      password: '',
      categoryId: ''
    };
  }

  addContact() {
    const request: CreateContactRequest = {
      firstName: this.newContact.firstName,
      lastName: this.newContact.lastName,
      email: this.newContact.email,
      phone: this.newContact.phone,
      birthDate: this.newContact.birthDate,
      password: this.newContact.password,
      categoryId: this.newContact.categoryId ? this.newContact.categoryId : undefined
    };

    this.contactService.createContact(request).subscribe({
      next: (contact) => {
        // Add new contact to the list
        const updatedContacts = [...this.contacts(), contact];
        this.contacts.set(updatedContacts);
        this.hideAddContactForm();
      },
      error: (error) => {
        this.errorMessage.set('Failed to create contact');
        console.error('Create failed', error);
      }
    });
  }
}
