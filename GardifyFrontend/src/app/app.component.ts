import { Component, OnInit } from '@angular/core';
import { ThemeProviderService } from './services/themeProvider.service';
import { Router, NavigationEnd, Scroll, Event } from '@angular/router';
import { AuthService } from './services/auth.service';
import { filter } from 'rxjs/operators';
import { CookieService } from 'ngx-cookie-service';

import { ViewportScroller } from '@angular/common';

declare var gtag:Function;
declare var jQuery:any;


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Gardify';
  mode: string;
  cookiesAllowed = false;
  onActivate(event) {
    let scrollToTop = window.setInterval(() => {
        let pos = window.pageYOffset;
        if (pos > 0) {
            window.scrollTo(0, pos - 20); 
        } else {
            window.clearInterval(scrollToTop);
        }
    }, 16);
 }
  constructor(
    private themeProvider: ThemeProviderService,
    private auth: AuthService,
    private viewportScroller: ViewportScroller,
    private router: Router,
    private cookies: CookieService) {
      if (!localStorage.getItem('mode')) { localStorage.setItem('mode', 'light'); }

      this.auth.cookiesAllowed$.subscribe(allow => {
        this.cookiesAllowed = allow;
        if (allow) {
          this.auth.initGA();
        }
      });
     
      const navEndEvent$ = router.events.pipe(
        filter(e => e instanceof NavigationEnd)
      );
      navEndEvent$.subscribe((e: NavigationEnd) => {
        gtag('config', 'AW-624991558', {'page_path':e.urlAfterRedirects});
      }); 
    
      this.themeProvider.getTheme().subscribe(t => {
        this.mode = t || 'light';
        if (document.body.classList.contains('light')) {
          document.body.classList.replace('light', 'dark');
        } else if (document.body.classList.contains('dark')) {
          document.body.classList.replace('dark', 'light');
        } else {
          document.body.classList.add(this.mode);
        }
      });

      if (window.document.referrer != null){
        console.log(cookies.check("referral_in"));
        if (!cookies.check("referral_in")){
          cookies.set("referral_in", window.document.referrer, 365)
        }
      }

      if (window.document.URL != null){
        console.log(window.document.URL );
        if (!cookies.check("request_in")){
          cookies.set("request_in", window.document.URL, 365)

        }
      }


      this.router.events.subscribe(event => {
        if (event instanceof NavigationEnd && this.cookiesAllowed) {
          (<any>window).gtag('config', 'UA-70030524-12', {'page_path': event.urlAfterRedirects});
          // (<any>window).gtag('set', 'page', event.urlAfterRedirects);
          // (<any>window).gtag('send', 'pageview');
        }
      });

      router.events.pipe(
        filter((e: Event): e is Scroll => e instanceof Scroll)
      ).subscribe(e => {
        if (e.position) {
          // backward navigation
          viewportScroller.scrollToPosition(e.position);
        } else if (e.anchor) {
          // anchor navigation
          viewportScroller.scrollToAnchor(e.anchor);
        } else {
          // forward navigation
          viewportScroller.scrollToPosition([0, 300]);
        }
      });
  }
}
