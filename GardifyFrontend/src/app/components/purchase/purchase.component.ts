import { Component, OnInit, OnDestroy } from '@angular/core';
import { UtilityService } from 'src/app/services/utility.service';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { StatCounterService } from 'src/app/services/stat-counter.service';
import { faMinus, faPlus } from '@fortawesome/free-solid-svg-icons';
import { FormBuilder, FormGroup } from '@angular/forms';
import { UserActionsFrom } from 'src/app/models/models';
import { HttpParams } from '@angular/common/http';
declare var gtag:Function
@Component({
  selector: 'app-purchase',
  templateUrl: './purchase.component.html',
  styleUrls: ['./purchase.component.css']
})
export class PurchaseComponent implements OnInit,OnDestroy {
  mode: string;
  OrderForm = {
    amount: '',
    user: '',
    key: '',
    payment_method: '',
    orderid: '',
    hash: '',
    //mobile = 1,
    testmodus: '',
    success: '',
    abort: '',
    Cart: null,
    Street: '',
    Zip: '',
    CustomerName: '',
    City: '',
    Country: '',
    InvoiceStreet: '',
    InvoiceZip: '',
    InvoiceCustomerName: '',
    InvoiceCity: '',
    InvoiceCountry: ''
  };
  subTotal = 0;
  orderId;
  shippingCosts = 0;
  amountRedeemed = 0;
  loading = true;
  payWithInvoice = false;
  faMinus=faMinus; faPlus=faPlus;
  newPaymentForm: FormGroup;
  apiCallFrom=new UserActionsFrom();
  PAYMENT_TEXT = {
    V: 'Visa',
    M: 'Mastercard',
    paypal: 'PayPal',
    sofort: 'Sofort',
    rechnung: 'Rechnung'
  };
  subs= new Subscription()
  constructor(
    private util: UtilityService, 
    private fb: FormBuilder,
    private router: Router,
    private statCounter:StatCounterService,
    private themeProvider: ThemeProviderService, 
    private route: ActivatedRoute) {
      this.newPaymentForm = this.fb.group({
        PaymentMethod: ''
      });
   }
  ngOnDestroy(): void {
    this.subs.unsubscribe()
  }

  ngOnInit() {
    this.subs.add(this.themeProvider.getTheme().subscribe(t => this.mode = t));
    this.subs.add(this.route.params.subscribe(params => {
      this.orderId = params.id;
      this.subs.add(this.util.GetFormData(params.id).subscribe(order => {
        this.OrderForm = {...order};
        this.subTotal = order.Cart.TotalNormal;
        this.shippingCosts = order.Cart.ShippingCosts;
        this.payWithInvoice = this.OrderForm.payment_method.toLowerCase() === 'rechnung';
        this.newPaymentForm.patchValue({
          PaymentMethod: this.OrderForm.payment_method
        });
        this.loading = false;
      }));
    }));
  }

  changeQuantity(entryId,increase,decrease){
    this.loading=true
    this.subs.add(this.util.changeQuantity(entryId, increase, decrease).subscribe(s => {
      this.statCounter.requestShopCartCounter().subscribe(data=>this.statCounter.setShopCartCounter(data));
      this.updateOrder();
    }));
  }

  removeItem(entryId) {
    this.subs.add(this.util.deleteShopCartEntry(entryId).subscribe(()=> {
      this.statCounter.requestShopCartCounter().subscribe(data=>this.statCounter.setShopCartCounter(data));
      this.updateOrder();
    }));
  }

  updateOrder() {
    this.loading=true;
    this.subs.add(this.util.updateOrder({OrderId: this.orderId, PaidWith: this.newPaymentForm.value.PaymentMethod}).subscribe(order => {
      this.OrderForm = {...order};
      this.subTotal = order.Cart.TotalNormal;
      this.shippingCosts = order.Cart.ShippingCosts;
      this.payWithInvoice = this.OrderForm.payment_method.toLowerCase() === 'rechnung';
      this.loading=false;
    }));
  }

  redirect() {
    let params = new HttpParams();
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
    this.subs.add(this.util.submitOrderWithInvoice(this.orderId, params).subscribe(() => {
      this.gtag_report_conversion(this.router.url)
      this.router.navigate(['/bestellung/erfolgreich/' + this.OrderForm.orderid]);
    }));
  }

  get totalNormal() {
    return this.subTotal + this.shippingCosts - this.amountRedeemed;
  }
  
  toUrl(url){
    return this.util.toUrl(url,false);
  }
 gtag_report_conversion(url) {
    var callback = function () {
      if (typeof(url) != 'undefined') {
        window.location = url;
      }
    };
    gtag('event', 'conversion', {
        'send_to': 'AW-624991558/bucvCO_VhdUBEMa6gqoC',
        'transaction_id': '',
        'event_callback': callback
    });
    return false;
  }
}
