import { Component, OnInit, OnDestroy } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { faCog } from '@fortawesome/free-solid-svg-icons';
import { AuthService } from 'src/app/services/auth.service';
import { Subscription } from 'rxjs';
import { StatCounterService } from 'src/app/services/stat-counter.service';
import { UtilityService } from 'src/app/services/utility.service';
import { PlaceHolder } from 'src/app/models/models';
import { data } from 'jquery';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit,OnDestroy {
  faCog = faCog;
  mode: string;
  currentUser: any;
  subs = new Subscription();
  counter = { AllTodosOfTheMonth: 0, AllTodos: 0, points: 0, warnings: 0 , queries:0};
  profilImg: any;
  searchHistory = [];
  constructor(
    private tp: ThemeProviderService,
    private authService:AuthService,
    private sc: StatCounterService,
    private util:UtilityService) { }
  ngOnDestroy(): void {
    this.subs.unsubscribe()
  }

  ngOnInit() {
    this.tp.getTheme().subscribe(t => this.mode = t);
    this.authService.getUserObservable().subscribe(u => {
      this.currentUser = u;
      this.currentUser.Name = this.authService.isTestAccount() ? 'UserDemo' : this.currentUser.Name;
    });
    if (this.isLoggedIn) {
      this.subs.add(this.sc.requestSearchqueries().subscribe(q => this.searchHistory = q));
      this.getProfilImage()
    }
    
  }
  getProfilImage(){
    this.subs.add(this.util.getUserProfilImg().subscribe(data=>{
      this.profilImg=data
    }))
  }
  get isLoggedIn() {
    return this.authService.isLoggedIn();
  }
  toUrl(url: string) {
    if (url) {
      return this.util.toUrl(url, false, false);
    }
    return PlaceHolder.Profile;
  }
}
