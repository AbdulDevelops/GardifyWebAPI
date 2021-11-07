import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';

import { AlertService } from '../services/alert.service';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root',
})
export class DemoGuard implements CanActivate {
  constructor(private authService: AuthService, private alert: AlertService) {}

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): boolean {
    const url: string = state.url;

    return this.checkLogin(url);
  }

  checkLogin(url: string): boolean {
    if (this.authService.isTestAccount()) { 
      this.alert.error('Diese Funktion steht leider nur zur Verfügung, wenn du registriert bist.');
      return false; 
    }

    return true;
  }
}
