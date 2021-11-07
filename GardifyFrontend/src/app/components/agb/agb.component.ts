import { Component, OnInit, OnDestroy } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-agb',
  templateUrl: './agb.component.html',
  styleUrls: ['./agb.component.css']
})
export class AgbComponent implements OnInit,OnDestroy {
  mode: string;
  subs=new Subscription()
  constructor(private themeProvider: ThemeProviderService) { }

  ngOnInit() {
    this.subs.add(this.themeProvider.getTheme().subscribe(t => this.mode = t));
  }

  ngOnDestroy(){
    this.subs.unsubscribe()
  }
}
