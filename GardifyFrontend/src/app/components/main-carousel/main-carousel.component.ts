import { Component, OnInit, OnDestroy } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-main-carousel',
  templateUrl: './main-carousel.component.html',
  styleUrls: ['./main-carousel.component.css']
})
export class MainCarouselComponent implements OnInit,OnDestroy {
  mode: string;
  subs=new Subscription()
  constructor(private themeProvider: ThemeProviderService) { }
  ngOnDestroy(): void {
   this.subs.unsubscribe()
  }

  public toggleTheme() {
    this.themeProvider.toggleTheme();
  }

  ngOnInit() {
    this.subs.add(this.themeProvider.getTheme().subscribe(t => this.mode = t));
  }
}
