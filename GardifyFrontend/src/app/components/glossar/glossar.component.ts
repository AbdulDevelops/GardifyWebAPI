import { Component, OnInit } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';

@Component({
  selector: 'app-glossar',
  templateUrl: './glossar.component.html',
  styleUrls: ['./glossar.component.css']
})
export class GlossarComponent implements OnInit {
  mode: string; points: number;

  constructor(private tp: ThemeProviderService) { }

  ngOnInit() {
    this.tp.getTheme().subscribe(t => this.mode = t);
  }

}
