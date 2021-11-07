import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ThemeProviderService {
  private theme = localStorage.getItem('mode') ? new BehaviorSubject(localStorage.getItem('mode')) : new BehaviorSubject('light');
  currentTheme = this.theme.asObservable();

  constructor() { }

  toggleTheme() {
    if (localStorage.getItem('mode')) {
      localStorage.getItem('mode') === 'dark' ? localStorage.setItem('mode', 'light') : localStorage.setItem('mode', 'dark');
    } else {
      localStorage.setItem('mode', 'dark');
    }
    this.theme.next(localStorage.getItem('mode'));
  }

  clearTheme() {
    this.theme.next('');
  }

  getTheme(): Observable<any> {
    return this.currentTheme;
  }
}
