import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';

@Component({
  selector: 'app-search-in-garden-archiv',
  templateUrl: './search-in-garden-archiv.component.html',
  styleUrls: ['./search-in-garden-archiv.component.css']
})
export class SearchInGardenArchivComponent implements OnInit,OnDestroy {

listOfAlbums=['Album1','Album2']
listOfPresi=['Presi1','Presi2']
listOfYears=['2012','2013']
subs=new Subscription()
loading=false
mode:string
  constructor(private tp:ThemeProviderService) { }
  ngOnDestroy(): void {
this.subs.unsubscribe()  }

  ngOnInit(): void {
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
  }
  onSearchImg(event){

  }
}
