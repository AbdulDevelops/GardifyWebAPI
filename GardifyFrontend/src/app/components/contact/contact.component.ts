import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { UtilityService } from 'src/app/services/utility.service';
import { faChevronDown, faInfo } from '@fortawesome/free-solid-svg-icons';
import { AlertService } from 'src/app/services/alert.service';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.css']
})
export class ContactComponent implements OnInit,OnDestroy {
  contactForm: FormGroup;
  subs = new Subscription();
  faChevronDown = faChevronDown;
  mode = '';
  faInfo = faInfo;
  currentUser: any;
  loading:boolean
  constructor(
    private tp: ThemeProviderService, 
    private util: UtilityService,
    private alert: AlertService,
    private router: Router,
    private authService:AuthService,
    private fb: FormBuilder) {
      this.contactForm = this.fb.group({
        Subject: ['', Validators.required],
        FirstName: ['', Validators.required],
        Email: ['',[Validators.required, Validators.email] ],
        Text: ['', Validators.required]
      });
    }
  ngOnDestroy(): void {
    this.subs.unsubscribe()
  }

  ngOnInit() {
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
    if(this.isLoggedIn)
    this.authService.getUserObservable().subscribe(u => {
      this.currentUser = u;
      this.currentUser.Name = this.authService.isTestAccount() ? 'UserDemo' : this.currentUser.Name;
    });
  }

  send() {
    this.loading=true
    this.subs.add(this.util.contactUs(this.contactForm.value).subscribe(() => {
      this.alert.success('Deine Nachricht wurde versendet!');
      this.loading=false
      this.router.navigate(['/home']);
    })); 
  }
  get isLoggedIn() {
    return this.authService.isLoggedIn();
  }
}
