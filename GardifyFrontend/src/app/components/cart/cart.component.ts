import { Component, OnInit, OnDestroy, ViewChild, ElementRef, Input } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { UtilityService } from 'src/app/services/utility.service';
import { faMinus, faPlus, faInfo } from '@fortawesome/free-solid-svg-icons';
import { StatCounterService } from 'src/app/services/stat-counter.service';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';
import { AlertService } from 'src/app/services/alert.service';
import { UserActionsFrom } from 'src/app/models/models';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit,OnDestroy {
  @ViewChild("wunschliste") MyProp: ElementRef;
  @Input() popup = false;
mode:string;
readonly MIN_ORDER_AMOUNT = 10;
shippingCosts = 0;
shopCartEntries = [];
wishlist = [];
faMinus=faMinus; faPlus=faPlus; faInfo = faInfo;
loading=true;
totalNormal: any;
userPoints = 0;
apiCallFrom=new UserActionsFrom();

subs= new Subscription()
  constructor(
    private themeProvider: ThemeProviderService,
    private util:UtilityService,
    private statCounter:StatCounterService,
    public router: Router,
    private alert: AlertService,
  ) { }
  ngOnDestroy(): void {
    this.subs.unsubscribe()
  }

  closeCart() {
    this.util.toggleCart(false);
  }
  
  ngOnInit() {
    this.subs.add(this.themeProvider.getTheme().subscribe(t => this.mode = t));
    this.getShopCartEntries();
    this.subs.add(this.statCounter.requestBonusCounter().subscribe(p => this.userPoints = p));
  }
  showWishlistPart(){
    this.MyProp.nativeElement.scrollIntoView();
  }
  getShopCartEntries(){
    this.loading=true
    
    this.subs.add(this.util.getShopEntries().subscribe(t=>{
      this.shopCartEntries=t.ShopCartEntries.EntriesList;
      this.totalNormal=t.ShopCartEntries.TotalNormal;
      this.shippingCosts = t.ShopCartEntries.ShippingCosts;
      this.wishlist = t.WishListEntries;
      if(this.router.url.includes('wunschliste')){
        this.showWishlistPart();
      }
     this.loading=false;
    }));
  }
  removeItem(entryId){
    this.subs.add(this.util.deleteShopCartEntry(entryId).subscribe(()=>{
      this.getShopCartEntries();
      this.statCounter.requestShopCartCounter().subscribe(data=>this.statCounter.setShopCartCounter(data))
      this.statCounter.requestWishListEntriesCounter().subscribe(data=>this.statCounter.setWishlistEntriesCount(data))
    }));
  }
  moveToWishlist(itemId) {
    this.subs.add(this.util.moveToWishlist(itemId).subscribe(()=>{
      this.getShopCartEntries();
      this.statCounter.requestShopCartCounter().subscribe(data=>this.statCounter.setShopCartCounter(data))
      this.statCounter.requestWishListEntriesCounter().subscribe(data=>this.statCounter.setWishlistEntriesCount(data))
    }));
  }
  changeQuantity(entryId,increase,decrease){
    this.loading=true
    this.subs.add(this.util.changeQuantity(entryId, increase, decrease).subscribe(s => {
      this.statCounter.requestShopCartCounter().subscribe(data=>this.statCounter.setShopCartCounter(data))
      this.shopCartEntries=s.EntriesList;
      this.totalNormal=s.TotalNormal;
      this.shippingCosts = s.ShippingCosts;
      this.wishlist =this.wishlist;
      this.loading=false;
    }));
  }
  addToCart(itemId) {
    this.loading = true;
    let params = new HttpParams();
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
    this.subs.add( this.util.addToCart(itemId, true,params).subscribe(() => {
      this.util.getShopEntries().subscribe(t => {
        this.shopCartEntries=t.ShopCartEntries.EntriesList;
        this.totalNormal=t.ShopCartEntries.TotalNormal;
        this.shippingCosts = t.ShopCartEntries.ShippingCosts;
        this.wishlist = t.WishListEntries;
        this.statCounter.requestShopCartCounter().subscribe(data=>this.statCounter.setShopCartCounter(data))
        this.loading = false;
        this.alert.success('Artikel wurde dem Warenkorb hinzugefügt');
      });
    }));
  }
  toUrl(url){
    return this.util.toUrl(url,false);
  }

  get minLeft(): number {
    return this.MIN_ORDER_AMOUNT - this.totalNormal;
  }

  get enoughOrderAmount(): boolean {
    return this.totalNormal >= this.MIN_ORDER_AMOUNT;
  }

  get totalCartItems() {
    const res = this.shopCartEntries.reduce(( sum, { Quantity }) => sum + Quantity , 0);
    return res;
  }
}
