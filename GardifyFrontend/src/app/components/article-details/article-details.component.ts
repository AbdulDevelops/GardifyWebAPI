import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { ShopItem, UserActionsFrom } from 'src/app/models/models';
import { UtilityService } from 'src/app/services/utility.service';
import { StatCounterService } from 'src/app/services/stat-counter.service';
import { AlertService } from 'src/app/services/alert.service';
import { AuthService } from 'src/app/services/auth.service';
import { Subscription } from 'rxjs';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-article-details',
  templateUrl: './article-details.component.html',
  styleUrls: ['./article-details.component.css'],

})
export class ArticleDetailsComponent implements OnInit, OnDestroy {
  @Input() item: ShopItem;
  mode: string;
  itemId: number;
  itemCount = 1;
  totalPrice: number;
  lastPurchased: any;
  lastViewed: any;
  artViewedByOtherUser:any;
  articleImageModal: any;
  subs=new Subscription()
  apiCallFrom=new UserActionsFrom();

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private themeProvider: ThemeProviderService, 
    private alert: AlertService,
    private auth: AuthService,
    private statCounter: StatCounterService,
    private util: UtilityService) { }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

  ngOnInit() {
    this.subs.add(this.themeProvider.getTheme().subscribe(t => this.mode = t));
    this.subs.add(this.activatedRoute.params.subscribe(params => {
      this.itemId = params.id;
      this.getProduct(this.itemId);
      
      if (this.auth.isLoggedIn()) {
        this.getPurchases();
        this.getLastViewedArticles();
        this.getArtViewedByOtherUsersList(params.id);
        this.createLastViewvedArt(params.id);
      }
    }));
  }

  createLastViewvedArt(articleId){
    this.subs.add(this.util.createLastViewedArt(articleId).subscribe())
  }
  getProduct(itemId: number): void {
    this.subs.add(this.util.getArticle(itemId)
      .subscribe(item => {
        this.item = item;
        this.item.ExpertTip = this.item.ExpertTip?.replace(/\n/g, '<br />');
        this.item.Description = this.item.Description?.replace(/\n/g, '<br />');
        this.item.Description = this.parseTextLinks(this.item.Description);
        this.totalPrice = this.item.NormalPrice;
      }));
  }

  parseTextLinks(text: string) {
    const urlRegex = /(https?:\/\/[^\s]+)/g;
    text = text.replace(urlRegex, '<a class="text-green" href="$1">$1</a>');
    return text;
  }

  getPurchases() {
    this.subs.add(this.util.getLastPurchases().subscribe(ps => this.lastPurchased = ps));
  }

  getLastViewedArticles(){
    this.subs.add(this.util.getLastViewedArticles().subscribe(l => {
      this.lastViewed = l;
    }));
  }
  getArtViewedByOtherUsersList(articleId: number){
    this.subs.add(this.util.getArticlesViewedByOthersUsers(articleId).subscribe(a=>
      {
        this.artViewedByOtherUser=a
      }))
  }
  
  addToCart(item: ShopItem) {
    if (!this.isLoggedIn) {
      this.alert.error("Du musst angemeldet sein, um fortzufahren")
      this.router.navigate(['/']);
    }
    if(this.isTestAccount && this.isLoggedIn){
      this.alert.error("Du musst dich vollständig anmelden, um fortzufahren")
      this.router.navigate(['/register']);
    }
    let params = new HttpParams();
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
    this.subs.add(this.util.addToCart(item.Id,false,params).subscribe(() => {
      this.util.getShopEntries().subscribe(t => {
        this.util.toggleCart(true);
        this.statCounter.requestShopCartCounter().subscribe(data=>this.statCounter.setShopCartCounter(data));
        this.router.navigate(['/kasse']);
      });
    }));
  }

  get isLoggedIn() {
    return this.auth.isLoggedIn();
  }
  get isTestAccount(){
    return this.auth.isTestAccount();
  }
  
  addToWishlist(item: ShopItem) {
    if (this.auth.isLoggedIn()) {
      this.subs.add(this.util.addToWishlist(item.Id).subscribe(() => {
        this.statCounter.requestWishListEntriesCounter().subscribe(data=>this.statCounter.setWishlistEntriesCount(data))
        this.alert.success('Artikel wurde zur Wunschliste hinzugefügt');
      }));
    } else {
      this.router.navigate(['/new']);
    }
  }

  stepCount(step: number) {
    if (this.itemCount < 2 && step < 0) {
      return;
    }
    this.itemCount += step;
    this.totalPrice = this.item.NormalPrice * this.itemCount;
  }

  padNr(id: number) {
    return ('000000'+id).slice(-6);
  }

  toUrl(url) {
    return this.util.toUrl(url, false);
  }

  goBack() {
    this.util.goBack();
  }

  get totalLessThanMin() {
    return this.totalPrice < 15;
  }
}
