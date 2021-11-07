import { Component, OnInit, ChangeDetectionStrategy, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { ShopItem, UserActionsFrom } from 'src/app/models/models';
import { ShopService } from 'src/app/services/shop.service';
import { StatCounterService } from 'src/app/services/stat-counter.service';
import { AuthService } from 'src/app/services/auth.service';
import { Subscription } from 'rxjs';
import { FormGroup, FormBuilder, FormControl } from '@angular/forms';
import { AlertService } from 'src/app/services/alert.service';
import { UtilityService } from 'src/app/services/utility.service';
import { HttpParams } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css']
})
export class FooterComponent implements OnInit,OnDestroy {
  mode: string;
  subs = new Subscription();
  counter = {shopCartCounter:0};
  suggestForm: FormGroup;
  suggestImgs = [];
  suggestImage: any;
  loading = false;
  apiCallType=new UserActionsFrom();

  constructor(
    private themeProvider: ThemeProviderService,
    private authService: AuthService,
    private util: UtilityService,
    private cd: ChangeDetectorRef,
    private alertService: AlertService,
    private fb: FormBuilder,
    private router: Router,
    private sc: StatCounterService) { 
      this.suggestForm = this.fb.group({
        Known: new FormControl(false),
        Name: [null]
      });
    }
  
  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  ngOnInit() {
    this.subs.add(this.themeProvider.getTheme().subscribe(t => this.mode = t));
    if(this.isLoggedIn){
      this.subs.add(this.sc.requestShopCartCounter().subscribe())
      this.sc.shopCartEntriesCount$.subscribe(data=>{
        this.counter.shopCartCounter=data;
      })
    }
  }

  startTour() {
    this.subs.add(this.util.startTour().subscribe());
  }

  submit() {
    const data = new FormData();
    this.suggestImgs.forEach((img, i) => {
      data.append('imageFile' + i, img.Image);
      data.append('imageTitle' + i, img.ImageTitle);
    });
    
    data.append('known', this.suggestForm.value.Known);
    data.append('name', this.suggestForm.value.Name);
    this.loading = true;
    let params = new HttpParams();
    params = params.append('isIos', this.apiCallType.IsIos.toString());
    params = params.append('isAndroid', this.apiCallType.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallType.IsWebPage.toString());
    this.subs.add(this.util.suggestPlant(data,params).subscribe(() => {
      this.alertService.success('Vielen Dank für Ihren Vorschlag. Sie erhalten in wenigen Tagen eine Rückmeldung.');
      this.loading = false;
    }, 
    (err) => this.loading = false));
  }

  imageUpload(event) {
    const reader = new FileReader();
    if (event.target.files[0].size > 4000000) { 
      return; 
    }
    if (event.target.files && event.target.files.length) {
      const [file] = event.target.files;
      reader.readAsDataURL(file);
      reader.onloadend = () => {
        this.suggestImgs.push({Image: file, ImageTitle: file.name, Src: reader.result});
        this.cd.markForCheck();
      };
    }
  }

  get isLoggedIn() {
    return this.authService.isLoggedIn();
  }
  
  logout() {
    this.authService.logout();
  }
  get isSocialPage() {
    return this.router.url.includes('/social') || this.router.url.includes('/gartenflora') ;
  }
  
}
