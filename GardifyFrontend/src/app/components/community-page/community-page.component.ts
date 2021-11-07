import { Component, OnInit, OnDestroy } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-community-page',
  templateUrl: './community-page.component.html',
  styleUrls: ['./community-page.component.css']
})
export class CommunityPageComponent implements OnInit,OnDestroy {
  mode: string;
  subs= new Subscription()
  constructor(private themeProvider: ThemeProviderService) { }
  ngOnDestroy(): void {
    this.subs.unsubscribe()
  }

  ngOnInit() {
    this.subs.add(this.themeProvider.getTheme().subscribe(t => this.mode = t));
  }
}
