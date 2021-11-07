import { Component, OnInit, OnDestroy } from '@angular/core';
import { UtilityService } from 'src/app/services/utility.service';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { UserActionsFrom } from 'src/app/models/models';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.css']
})
export class OrdersComponent implements OnInit,OnDestroy {
  mode: string;
  orders = [];
  subs =new Subscription()
  apiCallFrom=new UserActionsFrom();

  constructor(
    private util: UtilityService, 
    private tp: ThemeProviderService, 
    private router: Router) { }
  ngOnDestroy(): void {
    this.subs.unsubscribe()
  }

  ngOnInit() {
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
    this.subs.add(this.util.getOrders().subscribe(list => this.orders = list));
  }

  toUrl(url){
    return this.util.toUrl(url,false);
  }

  buyAgain(articleId) {
    let params = new HttpParams();
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
    this.util.addToCart(articleId,false,params).subscribe(() => this.router.navigate(['/kasse']));
  }

}
