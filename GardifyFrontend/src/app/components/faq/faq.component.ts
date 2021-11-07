import { Component, OnInit } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';

@Component({
  selector: 'app-faq',
  templateUrl: './faq.component.html',
  styleUrls: ['./faq.component.css']
})
export class FaqComponent implements OnInit {
  mode: string; points: number;

  constructor(private tp: ThemeProviderService) { }

  ngOnInit() {
    this.tp.getTheme().subscribe(t => this.mode = t);
  }

}
