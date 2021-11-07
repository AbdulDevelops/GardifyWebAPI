import { Component, OnInit, OnDestroy } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { PagerService } from 'src/app/services/pager.service';
import { faSearch, faArrowDown, faArrowUp, faChevronDown, faFilter } from '@fortawesome/free-solid-svg-icons';
import { UtilityService } from 'src/app/services/utility.service';
import { Subscription } from 'rxjs';
import { SearchArticlePipe } from 'src/app/pipes/search-article.pipe';
import { FormGroup, FormBuilder } from '@angular/forms';
import { StatCounterService } from 'src/app/services/stat-counter.service';
import { AlertService } from 'src/app/services/alert.service';
import { AuthService } from 'src/app/services/auth.service';
import { Router } from '@angular/router';
import { UserActionsFrom } from 'src/app/models/models';
import { HttpParams } from '@angular/common/http';
declare var gtag:Function
@Component({
  selector: 'app-shop',
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.css'],
  providers: [SearchArticlePipe]
})
export class ShopComponent implements OnInit, OnDestroy {
  mode: string;
  faArrowUp = faArrowUp; faArrowDown = faArrowDown;
  faChevronDown = faChevronDown; faSearch = faSearch;
  faFilter = faFilter;
  shopPager: any = {};
  pagedShopItem: any[];
  shopItems: any[];
  shopList: any[] = [];
  searchString: string;
  wishlistIcon:any;
  wishListedIcon:any;
  subs = new Subscription();
  filterForm: FormGroup;
  loading: Boolean;
  currentPage: number = 1;
  type:any;
  gifts = [];
  rates = [new Array(5), new Array(4), new Array(3), new Array(2), new Array(1)];
  sortTypes = [
    { type: 'ascending', name: 'Preis aufsteigend' },
    { type: 'descending', name: 'Preis absteigend' }
  ];
  categories = [];
  count: any;
  searchArticle:boolean;
  sortArtByPrice:boolean;
  searchArtByCat:boolean;
  apiCallFrom=new UserActionsFrom();
  constructor(
    private themeProvider: ThemeProviderService, 
    private formBuilder: FormBuilder, 
    private pagerService: PagerService, 
    private util: UtilityService, 
    private auth: AuthService,
    private alert: AlertService,
    private statCounter: StatCounterService,
    private router: Router
  ) {
    this.filterForm = this.formBuilder.group({
      gift: [null],
      category: [null],
      rating: [null],
      priceSort: [null]
    });
  }

  ngOnInit() {
    this.themeProvider.getTheme().subscribe(t => this.mode = t);
    this.searchString = this.util.shopSearchCache.searchText;
    this.sortArtByPrice=false;
    this.searchArtByCat=false;
    this.searchArticle=false;
    this.currentPage = this.pagerService.shopPageCache;
    if (this.searchString) {
      this.search(this.pagerService.shopPageCache);
    } else if (this.util.shopSearchCache.category) {
      this.filterForm.patchValue({category: this.util.shopSearchCache.category});
      this.searchBy(this.pagerService.shopPageCache);
    } else {
      this.getArticles(this.pagerService.shopPageCache);
    }
    this.util.getArticleCategories().subscribe(cats => {
      this.categories = cats;
      this.gifts = cats.filter(c => c.IsGiftIdea);
    });
    this.wishListedIcon='/assets/images/Wunschliste-checked.svg'
    this.wishlistIcon='/assets/images/Wunschliste-gr.svg'
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  getArticleaCount(type: string = null, catId: number = 0, searchText: string = null) {
    this.subs.add(this.util.getArticlesCount(type, catId, searchText || this.searchString).subscribe(count => {
      this.count = count; 
    }));
  }

  getArticles(skip ,defaultSearch = false) {
    this.pagerService.shopPageCache = this.currentPage;
    this.sortArtByPrice=false;
    this.searchArtByCat=false;
    this.searchArticle=false;
    this.loading = true;
    this.subs.add(this.util.getArticlesCount().subscribe(count => {
      this.count = count; 
      this.subs.add(this.util.getArticles(skip-1,8,this.apiCallFrom).subscribe(t => {
        this.shopItems = t.ListEntries;
        // this.count = this.shopItems.length;
        this.setPage(skip+1);
        this.loading = false;
      }));
    }));
  }
 
  sortArticlesByPrice(skip , defaultSearch = false) {
    this.pagerService.shopPageCache = this.currentPage;
    this.sortArtByPrice=true;
    this.searchArtByCat=false;
    this.searchArticle=false;
    this.searchString="";
    this.loading = true;
    const sortType = this.filterForm.value.priceSort;
    this.subs.add(this.util.getArticlesCount().subscribe(count => {
      this.count = count; 
      this.subs.add(this.util.getSortedArticlesByPrice(sortType,skip-1).subscribe(t => {
        this.shopItems = t.ListEntries;
        // skip=this.currentPage-1
        this.setPage(skip+1);
        this.loading = false;
      }));
    }));
  }

  searchBy(skip , defaultSearch = false) {
    this.pagerService.shopPageCache = this.currentPage;
    this.sortArtByPrice=false;
    this.searchArtByCat=true;
    this.searchArticle=false;
    const id= this.filterForm.value.category ;
    this.util.shopSearchCache.category = id;
    if (id==0) {
      this.getArticles(0);
    }else{
      if(id=="gift"){
        this.subs.add(this.util.getArticlesCount('gift').subscribe(count => {
          this.count = count; 
          this.subs.add(this.util.getArticlesByCategory(0,skip-1,true).subscribe(res => {
            this.shopItems = res.ListEntries;
            // skip=this.currentPage-1
            this.setPage(skip+1);
            this.loading = false;
          }));
        }));
        
      }else{
        this.subs.add(this.util.getArticlesCount('cat', id).subscribe(count => {
          this.count = count; 
          this.subs.add(this.util.getArticlesByCategory(id,skip-1,false).subscribe(res => {
            this.shopItems = res.ListEntries;
            // skip=this.currentPage-1
            this.setPage(skip+1);
            this.loading = false;
          }));
        }));
      }
    }
  }

  search(skip , defaultSearch = false) {
    if (this.searchString) {
      this.pagerService.shopPageCache = this.currentPage;
      this.loading = true;
      this.util.shopSearchCache.searchText = this.searchString;
      this.sortArtByPrice=false;
      this.searchArtByCat=false;
      this.searchArticle=true;
      this.subs.add(this.util.getArticlesCount('search', 0, this.searchString).subscribe(count => {
        this.count = count; 
        this.subs.add(this.util.getArticleSearchText(this.searchString,skip-1).subscribe(t => {
          this.shopItems = t.ListEntries;
          // skip=this.currentPage-1
          this.setPage(skip+1);
          this.loading = false;
        }));
      }));
    }
  }
  addToCart(item) {
    if (!this.isLoggedIn) {
      this.alert.error("Du musst angemeldet sein, um fortzufahren")
      this.router.navigate(['/']);
    }
    if(this.isTestAccount && this.isLoggedIn){
      this.alert.error("Du musst dich vollständig anmelden, um fortzufahren")
      this.router.navigate(['/register/konvert']);
    }
    this.loading = true;
    let params = new HttpParams();
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
    this.subs.add( this.util.addToCart(item.Id,false,params).subscribe(() => {
      this.util.getShopEntries().subscribe(t => {
        this.statCounter.requestShopCartCounter().subscribe(data=>this.statCounter.setShopCartCounter(data))
        this.loading = false;
        this.util.toggleCart(true);
        this.alert.success('Artikel wurde dem Warenkorb hinzugefügt');
      });
      this.gtag_report_conversion(this.router.url)
    }));
  }

  addToWishlist(itemId) {
    if (this.auth.isLoggedIn()) {
      this.subs.add(this.util.addToWishlist(itemId).subscribe(() => {
        this.statCounter.requestWishListEntriesCounter().subscribe(data=>this.statCounter.setWishlistEntriesCount(data))
        this.alert.success('Artikel wurde zur Wunschliste hinzugefügt');
      }));
    }
  }
  removedToWishlist(itemId) {
    if (this.auth.isLoggedIn()) {
      this.subs.add(this.util.deleteShopCartEntry(itemId).subscribe(() => {
        this.statCounter.requestWishListEntriesCounter().subscribe(data=>this.statCounter.setWishlistEntriesCount(data))
        this.alert.success('Artikel wurde aus der Wunschliste entfernt');
      }));
    }
  }
  toUrl(url) {
    return this.util.toUrl(url, false);
  }
  public trackByFn(index, item) {
    return item.Id;
  }

  setPage(page: number) {
    // get pager object from service
    this.shopPager = this.pagerService.getPager(this.count, this.pagerService.shopPageCache || page, 8);
    // get current page of items
    this.pagedShopItem = this.shopItems;//.slice(this.shopPager.startIndex, this.shopPager.endIndex + 1);
  }

  get isLoggedIn() {
    return this.auth.isLoggedIn();
  }
  get isTestAccount(){
    return this.auth.isTestAccount();
  }
   gtag_report_conversion(url) {
    var callback = function () {
      if (typeof(url) != 'undefined') {
        window.location = url;
      }
    };
    gtag('event', 'conversion', {
        'send_to': 'AW-624991558/nSXwCITWhdUBEMa6gqoC',
        'event_callback': callback
    });
    return false;
  }
}
