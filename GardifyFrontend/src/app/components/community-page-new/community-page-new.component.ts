import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';

import { Ad, Detail } from 'src/app/models/models';
import { AuthService } from 'src/app/services/auth.service';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { UtilityService } from 'src/app/services/utility.service';
import { CommunityService } from './community.service';

@Component({
  selector: 'app-community-page-new',
  templateUrl: './community-page-new.component.html',
  styleUrls: ['./community-page-new.component.css'],
  providers: [CommunityService]
})
export class CommunityPageNewComponent implements OnInit {
  ad: Ad;
  mode: string;
  subs= new Subscription()
  currentUser = {} as CurrentUser;

  gardenTab : boolean = true
  loading=true

  tester: string = "hello tester";


  constructor(
    private themeProvider: ThemeProviderService,
    private authService: AuthService,
    private util: UtilityService,
    private communityService: CommunityService
    ) {
    this.ad = new Ad(
      'ca-pub-3562132666777902',
      6877184130,
      'auto',
      true,
      false,
      null
    )
  }

  ngOnInit() {
    this.loading = false; // TODO loader should be 

    this.subs.add(this.themeProvider.getTheme().subscribe(t => this.mode = t));

    this.subs.add(this.authService.getUserObservable().subscribe(u => {
      this.currentUser.user = u;
      this.communityService.currentUser.next(this.currentUser);
    }));

    this.subs.add(this.util.getUserProfilImg().subscribe(d => {
      this.currentUser.imageData = d;
      this.communityService.currentUser.next(this.currentUser);
    }));

    this.communityService.valueObs.next("linked successfully");

    console.log("user data", this.currentUser);
  }

  getCurrentTab(isGardenTab: boolean) {
    this.gardenTab = isGardenTab;
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }
  
}

export interface CurrentUser{
  user?: User ; 
  imageData?: ImageData;
}

export interface User {
  $id: string;
  Token: string;
  ExpiresUtc: Date;
  UserId: string;
  Email: string;
  Name: string;
  Admin: boolean;
  Street: string;
  HouseNr: string;
  PLZ: number;
  City: string;
  Country: string;
  FirstName: string;
  LastName: string;
  Signature: string;
}

export interface ImageData {
  $id: string;
  Images: Detail[];
}