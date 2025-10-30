import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { ContactsComponent } from './contacts/contacts.component';

export const routes: Routes = [
  { path: '', component: ContactsComponent },
  { path: 'login', component: LoginComponent }
];
