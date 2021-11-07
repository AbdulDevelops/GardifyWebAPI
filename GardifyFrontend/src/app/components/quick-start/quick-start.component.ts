import { Component, OnInit } from '@angular/core';
import { faInfo } from '@fortawesome/free-solid-svg-icons';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';

@Component({
  selector: 'app-quick-start',
  templateUrl: './quick-start.component.html',
  styleUrls: ['./quick-start.component.css']
})
export class QuickStartComponent implements OnInit {
  faInfo = faInfo;
  mode: string;
  
  constructor(private tp: ThemeProviderService) { }

  ngOnInit(): void {
    this.tp.getTheme().subscribe(t => this.mode = t);
  }

}
