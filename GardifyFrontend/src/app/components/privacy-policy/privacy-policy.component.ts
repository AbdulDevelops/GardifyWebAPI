import { Component, OnInit, OnDestroy } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-privacy-policy',
  templateUrl: './privacy-policy.component.html',
  styleUrls: ['./privacy-policy.component.css']
})
export class PrivacyPolicyComponent implements OnInit,OnDestroy{
  mode : string;
  subscription=new Subscription()
  constructor( private themeProvider:ThemeProviderService) { }
  ngOnDestroy(): void {
    this.subscription.unsubscribe()
  }

  ngOnInit() {
    this.subscription.add(this.themeProvider.getTheme().subscribe(t => this.mode = t));
  }

}
