import { Component, EventEmitter, OnDestroy, OnInit, Output } from '@angular/core';
import { Subscription } from 'rxjs';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';

@Component({
  selector: 'app-header-garden-archiv',
  templateUrl: './header-garden-archiv.component.html',
  styleUrls: ['./header-garden-archiv.component.css']
})
export class HeaderGardenArchivComponent implements OnInit, OnDestroy {

@Output() showPresiEvent= new EventEmitter<boolean>()
showPresentation:boolean=false
subs=new Subscription()
mode:string

  constructor(private tp:ThemeProviderService) { }
  ngOnDestroy(): void {
    this.subs.unsubscribe()
  }

  ngOnInit(): void {
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
  }

  showPresi(event){
    this.showPresiEvent.emit(event)
    this.showPresentation=event
  }
}
