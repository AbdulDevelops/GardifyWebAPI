import { Component, OnInit, OnDestroy } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { UtilityService } from 'src/app/services/utility.service';
import { ActivatedRoute } from '@angular/router';
import { ShopOrder, TCErrCodes } from 'src/app/models/models';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-purchase-fail',
  templateUrl: './purchase-fail.component.html',
  styleUrls: ['./purchase-fail.component.css']
})
export class PurchaseFailComponent implements OnInit,OnDestroy {
  mode: string;
  errMsg: any;
  orderId: any;
  order: ShopOrder;
  loading = true;
  subs=new Subscription()
  constructor(private themeProvider: ThemeProviderService, 
    private util: UtilityService, private route: ActivatedRoute) { }
  ngOnDestroy(): void {
    this.subs.unsubscribe()
  }

  ngOnInit() {
    this.subs.add(this.themeProvider.getTheme().subscribe(t => this.mode = t));
    this.subs.add(this.route.params.subscribe(params => {
      this.errMsg = TCErrCodes[params['err']] || '';
      this.orderId = params['id'];

      this.util.getOrderStatus(this.orderId).subscribe(o => {
        this.order = o; 
        this.loading = false;
      });
    }));
  }

  toUrl(url){
    return this.util.toUrl(url,false);
  }

}
