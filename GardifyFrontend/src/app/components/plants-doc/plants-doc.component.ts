import { Component, OnInit, HostListener, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { PagerService } from 'src/app/services/pager.service';
import { FormGroup, Validators, FormControl, FormBuilder } from '@angular/forms';
import { UtilityService } from 'src/app/services/utility.service';
import { Subscription } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { Ad, UserActionsFrom } from 'src/app/models/models';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-plants-doc',
  templateUrl: './plants-doc.component.html',
  styleUrls: ['./plants-doc.component.css']
})
export class PlantsDocComponent implements OnInit, OnDestroy {
  @ViewChild('openModal', { static: true }) openModal:ElementRef;
  mode: string;
  newPostForm: FormGroup;
  newPost = {vm: { questionText: '' }};
  loading:boolean;
  posts: any[];
  width: number;
  isMobile: boolean;
  subs = new Subscription();
  origin: any;
  currentUserPosts: any;
  ad:Ad;
  apiCallFrom=new UserActionsFrom();

  constructor(private tp: ThemeProviderService,
     private fb: FormBuilder, 
     private util: UtilityService,
     private activatedRoute:ActivatedRoute,
     public router: Router
     ) {
    this.newPostForm = this.fb.group({
      title: [null, Validators.required],
      referenceType: [null, Validators.required],
      related: [],
      allowPub: new FormControl(false)
    });
    this.ad = new Ad(
      'ca-pub-3562132666777902',
      6877184130,
      'auto',
      true,
      false,
      null
    ) 
  }
  @HostListener('window:resize')
  onWindowResize() {
    this.width = window.innerWidth;
    this.isMobile = window.innerWidth <= 990;
  }

  ngOnInit() {
    this.isMobile = window.innerWidth <= 768;
    this.loading=true;
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
    let params = new HttpParams();
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
    this.subs.add(this.util.getAllEntries(this.apiCallFrom).subscribe(t => {
      this.posts = t;
      this.loading=false;
    }));
    this.subs.add(this.activatedRoute.params.subscribe((d) => {
      this.origin = d.newPost
      if(this.origin=="newPost"){
        this.openModal.nativeElement.click();
      }
    }));
    
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  sortAnswers() {
    this.posts.forEach(post => {
      post.Answers.sort((a,b) => {
        a = new Date(a.Date);
        b = new Date(b.Date);
        return a>b ? -1 : a<b ? 1 : 0;
      });
    });
  }
 
  addNewPost() {
    this.subs.add(this.util.addFaqEntry(this.newPostForm.value).subscribe(t => {
      this.posts = t;
      this.sortAnswers();
    }));
    this.newPostForm.reset();
  }
  isAdminImg(imgDescr){
    let description=imgDescr
    if(description==null){
      return false;
    }
   return description.includes("Admin_image")
  }
  toUrl(url: string, isAdminImg) {
    return this.util.toUrl(url, false, false,false, isAdminImg);
  }
  get showUserPosts() {
    return this.router.url === '/pflanzendoc/my-posts';
  }
}
