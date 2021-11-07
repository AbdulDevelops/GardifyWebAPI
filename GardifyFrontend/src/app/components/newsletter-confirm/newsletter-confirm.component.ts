import { Component, OnInit, OnDestroy } from '@angular/core';
import { UtilityService } from 'src/app/services/utility.service';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-newsletter-confirm',
  templateUrl: './newsletter-confirm.component.html',
  styleUrls: ['./newsletter-confirm.component.css']
})
export class NewsletterConfirmComponent implements OnInit,OnDestroy {
  mode: string;
  subs=new Subscription()
  constructor( private util:UtilityService,private tp: ThemeProviderService,private activatedRoute: ActivatedRoute) { }
  ngOnDestroy(): void {
    this.subs.unsubscribe()
  }

  ngOnInit() {
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
    this.subs.add(this.activatedRoute.queryParams.subscribe((params) => {
      if (params.code && params.userId) {
        const userId = params.userId;
        const code = encodeURIComponent(params.code).replace(/\s/g,'+');
        const email = encodeURIComponent(params.email).replace(/\s/g,'+');
        this.util.emailConfirm(userId,code,email).subscribe((data) => {})
          // handle token
      }
  }))
  }
}
