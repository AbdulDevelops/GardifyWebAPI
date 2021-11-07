import { Component, OnInit } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';

@Component({
  selector: 'app-impressum',
  templateUrl: './impressum.component.html',
  styleUrls: ['./impressum.component.css']
})
export class ImpressumComponent implements OnInit {

  mode : string;
  constructor( private themeProvider:ThemeProviderService) { }

  ngOnInit() {
    this.themeProvider.getTheme().subscribe(t => this.mode = t);
  }

}
