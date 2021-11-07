import { Component, OnInit, OnDestroy, HostListener } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { UtilityService } from 'src/app/services/utility.service';
import {DomSanitizer, Meta} from '@angular/platform-browser';
import {faChevronDown, } from '@fortawesome/free-solid-svg-icons';
import { first } from 'rxjs/operators';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Ad, InstaPost, UserActionsFrom } from 'src/app/models/models';
import { HttpParams } from '@angular/common/http';
import { PagerService } from 'src/app/services/pager.service';


declare var gtag:Function
@Component({
  selector: 'app-homepage',
  templateUrl: './homepage.component.html',
  styleUrls: ['./homepage.component.css']
})
export class HomepageComponent implements OnInit, OnDestroy {
  faChevronDown = faChevronDown;
  mode: string;
  show = true;
  showVid = false;
  showCaption = true;
  newsList = [];
  instaPostEntry:InstaPost;
  instaPostList: any[]=[];
  showInstapost=true;
  pageMode: string = ""
  subs = new Subscription();
  actualTipps: any;
  loading: boolean;
  submitted: boolean;
  demoregistrationForm: FormGroup;
  testModeRegis: boolean;
  register: boolean;
  cookiesAllowed: boolean;
  isMobile: boolean;
  selectedPost:any;
  newsImageModal:any;
  ad: Ad;
  newsPager: any={};
  count: any;
  pagedNews: any[];
  currentPage: number = 1;
  pagedInstaNews: any[];
  instaNewsPager:any={};
  apiCallFrom= new UserActionsFrom();
  constructor(
    private themeProvider: ThemeProviderService,
    private router: Router,
    private util:UtilityService,
    private authService:AuthService,
    private meta: Meta,
    private formBuilder: FormBuilder, private route: ActivatedRoute, 
    private pagerService:PagerService,
    private sanitizer: DomSanitizer
    ) { 
      this.ad = new Ad(
        'ca-pub-3562132666777902',
        5990589824,
        'fluid',
        true,
        false,
        null
      )
    }
    @HostListener('window:resize')
    onWindowResize() {
      window.innerWidth <= 990? this.isMobile=true: this.isMobile=false
    }
  ngOnInit() {
    this.subs.add(this.route.queryParams.subscribe(params => {
      this.pageMode = params["mode"];
    }));
    this.subs.add(this.themeProvider.getTheme().subscribe(t => this.mode = t));
    this.subs.add(this.authService.cookiesAllowed().subscribe(a => this.cookiesAllowed = a));
    this.meta.addTag({name: 'google-site-verification', content: '73cUMUtm3VrdsNypHe_jx7BSynUFa9GKmSmczSB3roM'});
    this.demoregistrationForm = this.formBuilder.group({
      PLZ: ['', Validators.required],
      Country: ['', Validators.required],
    });
    this.getAllNews(this.currentPage);
    this.getActualTipps();
    this.getInstaPosts(this.currentPage)

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
    this.subs.add(this.util.getAllNews(params).subscribe(n => {
      n.ListEntries.forEach(l => {
        l.Text = l.Text.replace(/\[k]|\[k]/g, '<i>').replace(/\[\/k\]/g, '</i>');
      });
      this.newsList = n.ListEntries;
    }));
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
    //this.instaPostList.slice(0,8)
      }
    ))
    this.paginateInstaNews(skip+1)
  }
  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  startTour() {
    this.subs.add(this.util.startTour().subscribe());
  }

  toggle() {
    this.show=!this.show;
  }
  getActualTipps(){
    this.subs.add(this.util.getArticles(0,8,this.apiCallFrom).subscribe(a=>{
      this.actualTipps=a.ListEntries;
    }))
  }
  toUrl(url, small = false) {
    return this.util.toUrl(url, small);
  }
  createLastViewvedArt(articleId){
    this.subs.add(this.util.createLastViewedArt(articleId).subscribe())
  }
  registerDemotest(){
    this.submitted = true;
    if (this.demoregistrationForm.invalid) { return; }
    this.loading = true;
    this.subs.add(this.authService.registerDemo(this.demoregistrationForm.value,this.apiCallFrom)
    .pipe(first())
    .subscribe(
        data => {
          if (data && data.Token) {
            localStorage.setItem('currentUser', JSON.stringify(data));
            this.authService.user.next(JSON.parse(localStorage.getItem('currentUser')));
            this.router.navigate(['/demo/meingarten']);
            setTimeout(() => {
              window.location.reload();
            },50);
          }
          this.gtag_report_conversion(this.router.url, 'AW-624991558/yufqCMq0ptYBEMa6gqoC')
          if (this.cookiesAllowed) {
            this.register=true;
            this.testModeRegis=true;
            this.router.navigate(['/demo/meingarten']);
            setTimeout(() => {
              this.util.sendGAEvent('demo', 'register');
              this.util.sendFBQEvent('track', 'DemoRegistration');
            }, 3000);
          }

        },
        error => {
          this.loading = false;
        }
    ));
    
  }
  showInstaPosts(){
    this.showInstapost = true
    return this.showInstapost
  }
  hideInstaPosts(){
    this.showInstapost = false
    return this.showInstapost
  }
  get isSocialPage() {
    return this.router.url.includes('/social') || this.router.url.includes('/gartenflora') ;
  }
  get isLoggedIn() {
    return this.authService.isLoggedIn();
  }
  get isTestRegistration(){
    return this.router.url.includes('testMode') ;
  }
  get f() {
    return this.demoregistrationForm.controls;
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
  gtag_report_conversion(url, send_to) {
    var callback = function () {
      if (typeof(url) != 'undefined') {
        // window.location = url;
      }
    };
    gtag('event', 'conversion', {
        'send_to': send_to,
        'event_callback': callback
    });
    return false;
  }
}
