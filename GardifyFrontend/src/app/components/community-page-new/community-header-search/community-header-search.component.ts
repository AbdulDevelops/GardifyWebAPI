import { Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import { Router } from '@angular/router';
import { UtilityService } from 'src/app/services/utility.service';
import { CurrentUser } from '../community-page-new.component';
import { CommunityService } from '../community.service';

@Component({
  selector: 'app-community-header-search',
  templateUrl: './community-header-search.component.html',
  styleUrls: ['../community-page-new.component.css']
})
export class CommunityHeaderSearchComponent implements OnInit {

  @Output() tabButtonClicked : EventEmitter<boolean> = new EventEmitter();

  showPresentation:boolean=false

  @Input('themeMode') mode: string = "a";
  currentUser: CurrentUser;

  constructor(
    public router: Router, 
    private util: UtilityService,
    private communityService: CommunityService) { 
    
  }

  ngOnInit(): void {

    this.communityService.currentUser
    .subscribe((user: CurrentUser) => {
      this.currentUser = user;
    })

  }

  get showPresentationForm(){
    return this.router.url.includes('presentation')
  }

  get showImages(){
    if(!this.showImgDetails && !this.showPresentationForm && !this.showUploadForm){
      return true
    }
    return false
  }

  get showUploadForm() {
    return this.router.url.includes('uploadfoto') ;
  }
  get showImgDetails() {
    return this.router.url.includes('image/detail') ;
  }

  setTabChangeEmitter(isGardenTab : boolean){
    this.tabButtonClicked.emit(isGardenTab)
  }

  toUrl(src): string {
    return this.util.toUrl(src, false);
  }

}
