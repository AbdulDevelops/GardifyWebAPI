import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-cookie-consent',
  templateUrl: './cookie-consent.component.html',
  styleUrls: ['./cookie-consent.component.css']
})
export class CookieConsentComponent implements OnInit {
  display = false;
  
  constructor(private auth: AuthService) { }

  ngOnInit(): void { 
    this.auth.cookiesAllowed$.subscribe(allowed => {
      if (!allowed) {
        setTimeout(() => {
          this.display = true;
        }, 5000);
      }
    });
  }

  onAccept() {
    // init GA
    this.auth.initGA();
    this.auth.allowCookies();
    this.display = false;
  }

}
