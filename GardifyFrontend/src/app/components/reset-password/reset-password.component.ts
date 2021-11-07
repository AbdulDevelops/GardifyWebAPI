import { Component, OnInit, OnDestroy } from '@angular/core';
import { Validators, FormGroup, FormBuilder } from '@angular/forms';
import { Subscription } from 'rxjs';
import { StatCounterService } from 'src/app/services/stat-counter.service';
import { AuthService } from 'src/app/services/auth.service';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit,OnDestroy {
  resetForm: FormGroup;
  submitted = false;
  loading = false;
  mode: string;
  subs = new Subscription();
  userId: string;
  resetCode: string;

  constructor(private authService: AuthService, private route: ActivatedRoute,
    private formBuilder: FormBuilder, private sc: StatCounterService,
    private router: Router, private themeproviderService: ThemeProviderService) { 
    }
  ngOnDestroy(): void {
   this.subs.unsubscribe()
  }
  
  ngOnInit() {
    this.authService.logout(false);
    this.subs.add(this.themeproviderService.getTheme().subscribe(m => this.mode = m));
    this.userId = this.route.snapshot.queryParams['UserId'];
    this.resetCode = this.route.snapshot.queryParams['c'];
    this.resetCode = this.resetCode.replace(/\s/g, '+');
    this.resetForm = this.formBuilder.group({
      Password: ['', Validators.required],
      ConfirmPassword: ['', Validators.required],
      UserId: this.userId,
      Code: this.resetCode
    });
  }

  get f() { 
    return this.resetForm.controls;
  }

  onSubmit() {
    this.submitted = true;
    if (this.resetForm.invalid) { return; }
    this.loading = true;
    this.subs.add(this.authService.resetPassword(this.resetForm.value)
    .subscribe(
        () => {
          this.router.navigate(['/']);
          this.loading = false;
        },
        (error) => {
          this.loading = false;
        }
    ));
  }
}
