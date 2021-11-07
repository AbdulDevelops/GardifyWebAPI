import { Component, OnInit, OnDestroy } from '@angular/core';
import { TCErrCodes, ShopOrder } from 'src/app/models/models';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { UtilityService } from 'src/app/services/utility.service';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { StatCounterService } from 'src/app/services/stat-counter.service';

@Component({
  selector: 'app-purchase-success',
  templateUrl: './purchase-success.component.html',
  styleUrls: ['./purchase-success.component.css']
})
export class PurchaseSuccessComponent implements OnInit,OnDestroy {

  mode: string;
  errMsg: any;
  orderId: any;
  order: ShopOrder;
  loading = true;
  subs=new Subscription()
  constructor(private themeProvider: ThemeProviderService, private sc: StatCounterService,
    private util: UtilityService, private route: ActivatedRoute) { }
    ngOnDestroy(): void {
      this.subs.unsubscribe()
    }

  ngOnInit() {
    this.subs.add(this.themeProvider.getTheme().subscribe(t => this.mode = t));
    this.subs.add(this.route.params.subscribe(params => {
      this.errMsg = TCErrCodes[params['err']] || '';
      this.orderId = params['id'];
      this.subs.add(this.sc.requestShopCartCounter().subscribe(data=>this.sc.setShopCartCounter(data)));
      this.subs.add(this.util.getOrderStatus(this.orderId).subscribe(o => {
        this.order = o;
        this.loading = false;
      }));
    }));
  }

  toUrl(url){
    return this.util.toUrl(url,false);
  }

}
