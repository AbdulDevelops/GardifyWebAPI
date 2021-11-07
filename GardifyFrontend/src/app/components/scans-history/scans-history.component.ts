import { Component, OnInit, OnDestroy } from '@angular/core';
import { UtilityService } from 'src/app/services/utility.service';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { PlantSearchService } from 'src/app/services/plant-search.service';
import { Plant, ScanResult } from 'src/app/models/models';
import { faChevronDown, faChevronUp } from '@fortawesome/free-solid-svg-icons';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-scans-history',
  templateUrl: './scans-history.component.html',
  styleUrls: ['./scans-history.component.css']
})
export class ScansHistoryComponent implements OnInit,OnDestroy {
  baseUrl = 'https://gardify.de/intern/';
  mode: string;
  results = [];
  faDown = faChevronDown;
  faUp = faChevronUp;
  subs=new Subscription()
  constructor(private themeProvider: ThemeProviderService, private util: UtilityService, 
    private plantSearch: PlantSearchService) { }
  ngOnDestroy(): void {
    this.subs.unsubscribe()
  }

  ngOnInit() {
    this.subs.add(this.themeProvider.getTheme().subscribe(t => this.mode = t));
    this.subs.add(this.plantSearch.getScanHistory().subscribe(t => this.results = t));
  }

  toUrl(src: string) {
    return this.util.toUrl(src);
  }

}
