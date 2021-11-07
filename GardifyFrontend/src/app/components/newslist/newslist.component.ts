import { Component, OnInit, OnDestroy, HostListener } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { Ad, InstaPost, NewsEntry, UserActionsFrom } from '../../models/models';
import { UtilityService } from 'src/app/services/utility.service';
import { Subscription } from 'rxjs';
import { TestBed } from '@angular/core/testing';
import { DomSanitizer } from '@angular/platform-browser';
import { HttpParams } from '@angular/common/http';
import { PagerService } from 'src/app/services/pager.service';

@Component({
  selector: 'app-newslist',
  templateUrl: './newslist.component.html',
  styleUrls: ['./newslist.component.css']
})
export class NewslistComponent implements OnInit, OnDestroy {
  master = 'Master';
  mode: string;
  newsList: NewsEntry[];
  instaPostEntry:InstaPost;
  instaPostList: any[]=[];
  showInstapost=true;
  isMobile= false;
  width = window.innerWidth;
  subs = new Subscription();
  selectedPost:any;
  newsImageModal:any;
  apiCallFrom= new UserActionsFrom();
  public myRegex4= new RegExp(/\{k]|\[k]([\s\S]+?)\[\/k]/g);
    replacegrp="<i>$1</i>"
    ad: Ad;
  newsPager: any={};
  count: any;
  pagedNews: any[];
  currentPage: number = 1;
  pagedInstaNews: any[];
  instaNewsPager:any={};
@HostListener('window:resize')
onWindowResize() {
  window.innerWidth <= 990? this.isMobile=true: this.isMobile=false
}
  constructor(private themeProvider: ThemeProviderService, private util: UtilityService, private sanitizer: DomSanitizer, private pagerService:PagerService) { 
     this.ad = new Ad(
      'ca-pub-3562132666777902',
      5990589824,
      'fluid',
      true,
      false,
      null
    )
  } 

  ngOnInit() {
    this.themeProvider.getTheme().subscribe(t => this.mode = t);
   
   this.getAllNews(this.currentPage)
    this.getInstaPosts(this.currentPage)
    this.onWindowResize() 
  }
  getAllNews(skip){
    this.newsList=[]
    let params = new HttpParams();
    var page=skip-1
    var take=20
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
    params = params.append('skip', page.toString());
    params = params.append('take', take.toString());

    this.subs.add(this.util.getAllNews(params).subscribe(
      d => this.newsList = d.ListEntries));
    this.paginateNews(skip+1)
  }
  getInstaPosts(skip){
    this.instaPostList=[]
    let params = new HttpParams();
    var page=skip-1
    var take=20
    params = params.append('skip', page.toString());
    params = params.append('take', take.toString());
    this.subs.add(this.util.getInstaEntries(params).subscribe(e=>{
       e.data.forEach(element => {
      this.instaPostEntry ={Text:element.caption,
         MediaEntry:this.sanitizer.bypassSecurityTrustResourceUrl(element.media_url),
          Media_Type:element.media_type, 
          Expanded:false, 
          Date:element.timestamp}
      
      this.instaPostList.push(this.instaPostEntry)
    })
    this.paginateInstaNews(skip+1)
  }
    ))
  }
  ngOnDestroy() {
    this.subs.unsubscribe();
  }
  showInstaPosts(){
    this.showInstapost = true
    return this.showInstapost
  }
  hideInstaPosts(){
    this.showInstapost = false
    return this.showInstapost
  }
  toUrl(image): string {
    return this.util.toUrl(image ? image.SrcAttr : null, false);
  }
  paginateNews(page: number) {
    // get pager object from service
    this.newsPager = this.pagerService.getPager(this.count, page, 20);
    // get current page of items
    this.pagedNews = this.newsList;
  }
  paginateInstaNews(page: number) {
    // get pager object from service
    this.instaNewsPager = this.pagerService.getPager(this.count, page, 20);
    // get current page of items
    this.pagedInstaNews = this.instaPostList;
  }
}
