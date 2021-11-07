import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { first } from 'rxjs/operators';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { StatCounterService } from 'src/app/services/stat-counter.service';
import { Subscription } from 'rxjs';
import { UtilityService } from 'src/app/services/utility.service';
import { AlertService } from 'src/app/services/alert.service';
import { CookieService } from 'ngx-cookie-service';
import { UserActionsFrom } from 'src/app/models/models';
declare var gtag:Function
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit,OnDestroy {
  loginForm: FormGroup;
  forgotForm: FormGroup;
  submitted = false;
  confirmForgot = false;
  loading = false;
  displayCheckMsg = false;
  mode: string;
  subs = new Subscription();
  simpleMode = true;
  emailResent = false;
  displayMsg=false;
  apiCallType=new UserActionsFrom();

  constructor(private authService: AuthService, private utils: UtilityService,
    private formBuilder: FormBuilder, private sc: StatCounterService,
    private router: Router, private themeproviderService: ThemeProviderService,
    private alertService: AlertService, private cookies: CookieService) { 
      
    }
  ngOnDestroy(): void {
    this.subs.unsubscribe()
  }

  ngOnInit() {
    if (this.router.url.includes('login')) {
      this.authService.logout(false);
    }
    this.subs.add(this.themeproviderService.getTheme().subscribe(m => this.mode = m));
    this.loginForm = this.formBuilder.group({
      Email: ['', Validators.required], // has UserName if in simpleMode
      Password: ['', Validators.required],
      Model: new UserActionsFrom(),
      RememberMe: new FormControl(false)
    });
    this.forgotForm = this.formBuilder.group({
      Email: ['', Validators.required]
    });
    console.log(this.apiCallType)
  }

  get isLoggedIn() {
    return this.authService.isLoggedIn();
  }

  resendEmail() {
    this.loading = true;
    this.subs.add(this.authService.forgot(this.forgotForm.value).subscribe(data => {
      this.emailResent = true;
      this.loading = false;
    },
    (error) => {
      this.loading = false;
    }));
  }

  toggleMode() {
    this.simpleMode = !this.simpleMode;
  }

  get f() { 
    return this.loginForm.controls;
  }

  updateEmail(email){
    this.loginForm.patchValue({
      Email: email
      
    });
  }
  resendConfEmail(){
    this.loading = true;
    this.subs.add(this.authService.sendConfMail(this.loginForm.value).subscribe(data => {
      this.loading = false;
      this.alertService.success("Wir haben dir eine neue Bestätigungsmail versendet!")
    },
    (error) => {
      this.loading = false;
      this.displayMsg=false;
    }));
  }
  updatePassword(password){
    this.loginForm.patchValue({
     
      Password: password
      
    });
  }

  checkFormValue(){
    this.submitted = true;
    this.loading = true;
    // && !this.loginForm.invalid
    console.log("check if valid");
    if (!this.confirmForgot && !this.loginForm.invalid) {
      this.subs.add(this.authService.login(this.loginForm.value, this.simpleMode)
      .pipe(first())
      .subscribe(
          data => {
            
            console.log("check if token exist");

            // login successful if there's a jwt token in the response
            if (data && data.Token) {
              localStorage.setItem('currentUser', JSON.stringify(data));
              this.cookies.set('gardify_user', JSON.stringify(data), null , null, ".gardify.de");
              this.authService.user.next(JSON.parse(localStorage.getItem('currentUser')));
            }
            const redirectUrl = this.authService.redirectUrl || '/';
            // this.utils.sendGAEvent('login', 'login');
            // this.utils.setGAUserId(data.UserId);
            
            this.router.navigate(['/news']); // TODO: user redirectUrl
            this.subs.add(this.sc.requestPlantsCount().subscribe());
            this.subs.add(this.sc.requestTodosCount().subscribe());
            this.subs.add(this.sc.requestWarningsCount().subscribe());
            this.subs.add(this.sc.requestShopCartCounter().subscribe());
            this.gtag_report_conversion(this.router.url)
            this.loading = false;
            setTimeout(() => {
              window.location.reload();  
            },500);
           
          },
          (error) => {
            
            if(error.error.Message.includes("Bitte bestätige erst deine Emailadresse.")){
              this.displayMsg=true;
            }
            this.loading = false;
          }
      ));
    } else {
      this.subs.add(this.authService.forgot(this.forgotForm.value).subscribe(data => {
        this.displayCheckMsg = true;
        this.loading = false;
      },
      (error) => {
        this.loading = false;
      }));
    }

  }

  onSubmit() {
    this.submitted = true;
    this.loading = true;
    // && !this.loginForm.invalid
    if (!this.confirmForgot && !this.loginForm.invalid) {
      this.subs.add(this.authService.login(this.loginForm.value, this.simpleMode)
      .pipe(first())
      .subscribe(
          data => {
            
            // login successful if there's a jwt token in the response
            if (data && data.Token) {
              localStorage.setItem('currentUser', JSON.stringify(data));
              this.cookies.set('gardify_user', JSON.stringify(data));
              this.authService.user.next(JSON.parse(localStorage.getItem('currentUser')));
            }
            const redirectUrl = this.authService.redirectUrl || '/';
            // this.utils.sendGAEvent('login', 'login');
            // this.utils.setGAUserId(data.UserId);
            
            this.router.navigate(['/meingarten']); // TODO: user redirectUrl
            this.alertService.success('Willkommen bei gardify!', true);
            this.subs.add(this.sc.requestPlantsCount().subscribe());
            this.subs.add(this.sc.requestTodosCount().subscribe());
            this.subs.add(this.sc.requestWarningsCount().subscribe());
            this.subs.add(this.sc.requestShopCartCounter().subscribe());
            this.gtag_report_conversion(this.router.url)
            this.loading = false;
            setTimeout(() => {
              window.location.reload();  
            },500);
           
          },
          (error) => {
            
            this.loading = false;
          }
      ));
    } else {
      this.subs.add(this.authService.forgot(this.forgotForm.value).subscribe(data => {
        this.displayCheckMsg = true;
        this.loading = false;
      },
      (error) => {
        this.loading = false;
      }));
    }
  }
  get loginRoute(){
    return this.router.url.includes('login') ;
  }
  gtag_report_conversion(url){
    
      var callback = function () {
        if (typeof(url) != 'undefined') {
          window.location = url;
        }
      };
      gtag('event', 'conversion', {
          'send_to': 'AW-624991558/URvxCLKrptYBEMa6gqoC',
          'event_callback': callback
      });
      return false;
  }
  
}
