import { Component, OnInit, OnDestroy, HostListener } from '@angular/core';
import { faCircle } from '@fortawesome/free-solid-svg-icons';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { UtilityService } from 'src/app/services/utility.service';
import { StatCounterService } from 'src/app/services/stat-counter.service';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-ecolist',
  templateUrl: './ecolist.component.html',
  styleUrls: ['./ecolist.component.css']
})
export class EcolistComponent implements OnInit,OnDestroy {
  mode: string;
  listItems = [];
  faCircle = faCircle;
  subs= new Subscription();
  isMobile: boolean;
  ecoName: string;
  constructor(
    private tp: ThemeProviderService, 
    private sc: StatCounterService,
    private util: UtilityService,
    public router: Router) { }
  ngOnDestroy(): void {
    this.subs.unsubscribe()
  }
  @HostListener('window:resize')
  onWindowResize() {
    this.isMobile = window.innerWidth <= 425;
  }
  ngOnInit() {
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
    this.isMobile = window.innerWidth <= 425;
    this.getEcoelements()
  }
  getEcoelements(){
    this.subs.add(this.util.getUserEcoList().subscribe(l => {
      this.listItems = l;
    }));
  }
  updateItem(Id: number, Checked: boolean) {
    this.subs.add(this.util.updateEcoElement({Id, Checked}).subscribe(()=>{
      this.getEcoelements()
      this.sc.requestEcoCount().subscribe(data => this.sc.setEcoElementsCount(data));

    }))
  }
  
  toUrl(url) {
    return this.util.toUrl(url, false);
  }
  
}
