import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';

@Component({
  selector: 'app-filter-options',
  templateUrl: './filter-options.component.html',
  styleUrls: ['./filter-options.component.css']
})
export class FilterOptionsComponent implements OnInit,OnDestroy {
mode:string
subs= new Subscription()
  constructor(private theme:ThemeProviderService) { }
  ngOnDestroy(): void {
    this.subs.unsubscribe() 
   }

  ngOnInit(): void {
    this.subs.add(this.theme.getTheme().subscribe(t => this.mode = t));
  }

}
