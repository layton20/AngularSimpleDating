import { Component, inject, signal, Signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../core/services/account-service';

@Component({
  selector: 'app-nav',
  imports: [FormsModule],
  templateUrl: './nav.html',
  styleUrl: './nav.css'
})
export class Nav {
  protected accountService = inject(AccountService);
  protected creds: any = {};
  protected loggedIn = signal(false);

  login() {
    this.accountService.login(this.creds).subscribe({
      next: response => {
        console.log(response);
        this.loggedIn.set(true);
        this.creds = {};
      },
      error: error => alert('Login failed: ' + error.message)
    });
  }

  logout() {
    this.loggedIn.set(false);
    this.accountService.logout();
  }
}
