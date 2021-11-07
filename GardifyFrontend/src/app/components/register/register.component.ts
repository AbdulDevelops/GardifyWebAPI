import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { AuthService } from 'src/app/services/auth.service';
import { first } from 'rxjs/operators';
import { Router, ActivatedRoute } from '@angular/router';
import { faInfoCircle } from '@fortawesome/free-solid-svg-icons';
import { UtilityService } from 'src/app/services/utility.service';
import { Subscription } from 'rxjs';
import { UserActionsFrom } from 'src/app/models/models';
import { MustMatch } from 'src/app/_helpers/must-match.validators';
declare var gtag:Function
declare var fbq: Function;
@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit,OnDestroy {
  mode: string;
  submitted = false;
  simpleMode = true;
  loading = false;
  registrationForm: FormGroup;
  faInfoCircle = faInfoCircle;
  register=false;
  subs= new Subscription();
  cookiesAllowed;
  testModeRegis: boolean;
  newsLetterAbo=false;
  apiCallFrom= new UserActionsFrom();
  constructor(private formBuilder: FormBuilder, private tp: ThemeProviderService,
    private authService: AuthService, private router: Router, private utils: UtilityService) {
  }
  ngOnDestroy(): void {
    this.subs.unsubscribe()
  }
  ngOnInit() {
    this.register=this.router.url.includes('danke');
    if (this.router.url.includes('danke')) {
      this.utils.sendFBQEvent('track', 'CompleteRegistration');
    }
    this.testModeRegis=false;
    this.subs.add(this.authService.cookiesAllowed().subscribe(a => this.cookiesAllowed = a));
    this.subs.add(this.tp.getTheme().subscribe(m => this.mode = m));
    this.registrationForm = this.formBuilder.group({
      Firstname: ['', Validators.required],
      Lastname: ['', Validators.required],
      Street: ['', Validators.required],
      HouseNr: ['', Validators.required],
      PLZ: ['', [Validators.required,Validators.minLength(4)]],
      City: ['', Validators.required],
      Country: ['', Validators.required],
      UserName: ['', [Validators.required, Validators.minLength(4)]],
      Gender:['', Validators.required],
      Email: ['', [Validators.required, Validators.email]],
      Password: ['', [Validators.required, Validators.minLength(6), Validators.pattern(/(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W)/)]],
      ConfirmPassword: ['', Validators.required],

      model: new UserActionsFrom()
    },{
      validator: MustMatch('Password', 'ConfirmPassword')} 
    );
    
    if (this.isConverting) {
      this.registrationForm.get('Country').disable(); 
      this.registrationForm.get('PLZ').disable(); 




    }
  }


   toggleMode() {
    this.simpleMode = !this.simpleMode;
    if (this.simpleMode) {
      this.registrationForm.get('FirstName').disable();
      this.registrationForm.get('Lastname').disable();
      this.registrationForm.get('Street').disable();
      this.registrationForm.get('HouseNr').disable();
      this.registrationForm.get('City').disable();
      this.registrationForm.get('Gender').disable();
      this.registrationForm.get('Country').disable();
    } else {
      this.registrationForm.get('FirstName').enable();
      this.registrationForm.get('Lastname').enable();
      this.registrationForm.get('Street').enable();
      this.registrationForm.get('HouseNr').enable();
      this.registrationForm.get('City').enable();
      this.registrationForm.get('Gender').enable();
      this.registrationForm.get('Email').enable();
      this.registrationForm.get('Country').enable(); 
   }
  } 
  registerDemo(){
    this.submitted = true;
    //if (this.registrationForm.invalid) { return; }
    this.loading = true;
    this.subs.add(this.authService.registerDemo(this.registrationForm.value,this.apiCallFrom)
    .pipe(first())
    .subscribe(
        data => {
          if (data && data.Token) {
            localStorage.setItem('currentUser', JSON.stringify(data));
            this.authService.user.next(JSON.parse(localStorage.getItem('currentUser')));
            this.router.navigate(['/demo/meingarten']);
            setTimeout(() => {
              window.location.reload();
            },50);
          }
          this.gtag_report_conversion(this.router.url, 'AW-624991558/yufqCMq0ptYBEMa6gqoC')
          if (this.cookiesAllowed) {
            this.utils.sendGAEvent('register', 'register');
            this.register=true;
            this.testModeRegis=true;
            this.router.navigate(['/register/danke']);
          }

        },
        error => {
          this.loading = false;
        }
    ));
    
  }
  onSubmit() {
    this.submitted = true;
    if (this.isConverting) {
      this.subs.add(this.authService.convert(this.registrationForm.value,this.apiCallFrom)
      .pipe(first())
      .subscribe(
          data => {
            if (data && data.Token) {
              localStorage.setItem('currentUser', JSON.stringify(data));
              this.authService.user.next(JSON.parse(localStorage.getItem('currentUser')));
            }
            this.register=true;
            
            this.router.navigate(['/register/danke']);
            this.loading = false;
          },
          error => {
            this.loading = false;
          }
      ));
      return;
    }
    this.isTestRegistration?this.simpleMode=false:this.simpleMode=true;
    if (!this.simpleMode ) {
       this.registerDemo() 
      }else
      {
      this.loading = true;
      console.log(this.registrationForm.value)
      this.subs.add(this.authService.register(this.registrationForm.value, this.simpleMode,this.newsLetterAbo)
      .pipe(first())
      .subscribe(
          data => {
            if (data && data.Token) {
              localStorage.setItem('currentUser', JSON.stringify(data));
              this.authService.user.next(JSON.parse(localStorage.getItem('currentUser')));
            }
            this.register=true;
            this.router.navigate(['/register/danke']);
            this.loading = false;
          },
          error => {
            this.loading = false;
          }
      ));
    }
   
  }
  
  get f() {
    return this.registrationForm.controls;
  }

  get isConverting() {
    return this.router.url.includes('konvert') ;
  }

  get isTestRegistration(){
    return this.router.url.includes('testMode') ;
  }
  get registerRoute(){
    return this.router.url.includes('register') ;
  }
   gtag_report_conversion(url, send_to) {
    var callback = function () {
      if (typeof(url) != 'undefined') {
        // window.location = url;
      }
    };
    gtag('event', 'conversion', {
        'send_to': send_to,
        'event_callback': callback
    });
    return false;
  }
}
