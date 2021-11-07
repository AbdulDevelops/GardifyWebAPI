import { Component, OnInit, OnDestroy } from '@angular/core';
import { UtilityService } from 'src/app/services/utility.service';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { faSearch } from '@fortawesome/free-solid-svg-icons';
import { Subscription } from 'rxjs';
import { UserActionsFrom } from 'src/app/models/models';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-gardening-az',
  templateUrl: './gardening-az.component.html',
  styleUrls: ['./gardening-az.component.css']
})
export class GardeningAZComponent implements OnInit, OnDestroy {
  letters = ['A','B','C','D','E','F','G','H','I','J','K','L','M','N','O',
            'P','Q','R','S','T','U','V','W','X','Y','Z'];
  selecetedLetter;
  terms: any[] = [];
  termsCache: any[] = [];
  mode: string; 
  faSearch = faSearch;
  subs = new Subscription();
  apiCallFrom=new UserActionsFrom();

  constructor(private themeProvider: ThemeProviderService, private util: UtilityService) { }

  ngOnInit() {
    this.themeProvider.getTheme().subscribe(t => this.mode = t);
    let params = new HttpParams();
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
    this.subs.add(this.util.getLexicon(params).subscribe(t => { this.termsCache = t; this.terms = t; }));
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

  search(searchText: string) {
    searchText = searchText.toLowerCase();
    this.terms = this.termsCache.filter(t => t.Name.toLowerCase().includes(searchText) || t.Description.toLowerCase().includes(searchText))
  }

  stepLetter(step: number) {
    const currIndex = this.letters.indexOf(this.selecetedLetter);
    if (this.selecetedLetter && 0 <= currIndex + step && currIndex + step < this.letters.length) {
      this.selecetedLetter = this.letters[currIndex + step];
    }
    this.filterTerms();
  }

  selectLetter(letter) {
    if (this.selecetedLetter === letter) {
      this.selecetedLetter = undefined;
    } else {
      this.selecetedLetter = letter;
    }
    this.filterTerms();
  }

  filterTerms() {
    if (this.selecetedLetter) {
      this.terms = this.termsCache.filter(t => t.Name.startsWith(this.selecetedLetter));
    } else {
      this.terms = this.termsCache;
    }
  }

  isSelected(letter) {
    return this.selecetedLetter === letter;
  }

  toUrl(images) {
    let url = images[0].SrcAttr ? images[0].SrcAttr : '';
    return this.util.toUrl(url, false);
  }
}
