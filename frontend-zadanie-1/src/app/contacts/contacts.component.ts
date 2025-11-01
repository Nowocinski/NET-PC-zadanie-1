import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Contact, ContactService, CreateContactRequest, UpdateContactRequest } from '../services/contact.service';
import { AuthService } from '../services/auth.service';
import { Category, CategoryService } from '../services/category.service';
import { Subcategory, SubcategoryService, CreateSubcategoryRequest } from '../services/subcategory.service';

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
    categoryId: '',
    subcategoryId: '',
    newSubcategoryName: ''
  };

  editContactData = {
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    birthDate: '',
    password: '',
    categoryId: '',
    subcategoryId: '',
    newSubcategoryName: ''
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
      categoryId: contact.categoryId || '',
      subcategoryId: contact.subcategoryId || '',
      newSubcategoryName: ''
    };
    this.showEditForm.set(true);
  }

  isSluzbowyCategory(categoryId: string): boolean {
    const category = this.categories().find(c => c.id === categoryId);
    return category?.name === 'Służbowy';
  }

  isInnyCategory(categoryId: string): boolean {
    const category = this.categories().find(c => c.id === categoryId);
    return category?.name === 'Inny';
  }

  hideEditContactForm() {
    this.showEditForm.set(false);
    this.editingContact.set(null);
  }

  updateContact() {
    const contact = this.editingContact();
    if (!contact) return;

    // If "Inny" category is selected and new subcategory name is provided, create it first
    if (this.isInnyCategory(this.editContactData.categoryId) && this.editContactData.newSubcategoryName) {
      const categoryName = this.categories().find(c => c.id === this.editContactData.categoryId)?.name;
      if (!categoryName) return;

      const subcategoryRequest: CreateSubcategoryRequest = {
        subcategoryName: this.editContactData.newSubcategoryName,
        categoryName: categoryName
      };

      this.subcategoryService.createSubcategory(subcategoryRequest).subscribe({
        next: (subcategory) => {
          // Use the newly created subcategory
          this.updateContactWithData(contact, subcategory.id);
        },
        error: (error) => {
          this.errorMessage.set('Failed to create subcategory');
          console.error('Subcategory creation failed', error);
        }
      });
    } else {
      const subcategoryId = this.editContactData.subcategoryId && this.editContactData.subcategoryId.trim() !== '' 
        ? this.editContactData.subcategoryId 
        : undefined;
      this.updateContactWithData(contact, subcategoryId);
    }
  }

  private updateContactWithData(contact: Contact, subcategoryId?: string) {
    const categoryId = this.editContactData.categoryId && this.editContactData.categoryId.trim() !== '' 
      ? this.editContactData.categoryId 
      : undefined;

    const request: UpdateContactRequest = {
      userId: contact.userId,
      firstName: this.editContactData.firstName,
      lastName: this.editContactData.lastName,
      email: this.editContactData.email,
      phone: this.editContactData.phone,
      birthDate: this.editContactData.birthDate,
      password: this.editContactData.password,
      categoryId: categoryId,
      subcategoryId: subcategoryId
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
      categoryId: '',
      subcategoryId: '',
      newSubcategoryName: ''
    };
  }

  addContact() {
    // If "Inny" category is selected and new subcategory name is provided, create it first
    if (this.isInnyCategory(this.newContact.categoryId) && this.newContact.newSubcategoryName) {
      const categoryName = this.categories().find(c => c.id === this.newContact.categoryId)?.name;
      if (!categoryName) return;

      const subcategoryRequest: CreateSubcategoryRequest = {
        subcategoryName: this.newContact.newSubcategoryName,
        categoryName: categoryName
      };

      this.subcategoryService.createSubcategory(subcategoryRequest).subscribe({
        next: (subcategory) => {
          // Use the newly created subcategory
          this.createContactWithData(subcategory.id);
        },
        error: (error) => {
          this.errorMessage.set('Failed to create subcategory');
          console.error('Subcategory creation failed', error);
        }
      });
    } else {
      const subcategoryId = this.newContact.subcategoryId && this.newContact.subcategoryId.trim() !== '' 
        ? this.newContact.subcategoryId 
        : undefined;
      this.createContactWithData(subcategoryId);
    }
  }

  private createContactWithData(subcategoryId?: string) {
    const request: CreateContactRequest = {
      firstName: this.newContact.firstName,
      lastName: this.newContact.lastName,
      email: this.newContact.email,
      phone: this.newContact.phone,
      birthDate: this.newContact.birthDate,
      password: this.newContact.password,
      categoryId: this.newContact.categoryId ? this.newContact.categoryId : undefined,
      subcategoryId: subcategoryId
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
