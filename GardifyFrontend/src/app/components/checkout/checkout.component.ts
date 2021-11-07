import { Component, OnInit, OnDestroy } from '@angular/core';
import { StatCounterService } from 'src/app/services/stat-counter.service';
import { UtilityService } from 'src/app/services/utility.service';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { faMinus, faPlus, faPen, faChevronDown, faPencilAlt } from '@fortawesome/free-solid-svg-icons';
import { FormGroup, FormBuilder, Validators, AbstractControl } from '@angular/forms';
import { AlertService } from 'src/app/services/alert.service';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { UserActionsFrom } from 'src/app/models/models';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.css']
})
export class CheckoutComponent implements OnInit,OnDestroy {
  mode: string;
  readonly MIN_ORDER_AMOUNT = 10;
  faChevronDown = faChevronDown;
  faEdit = faPen;
  faPencilAlt=faPencilAlt;
  loading: boolean;
  subTotal;
  shippingCosts = 0;
  amountRedeemed = 0;
  shopCartEntries = [];
  faMinus=faMinus; faPlus=faPlus;
  invoiceAddressForm: FormGroup;
  shipAddressForm: FormGroup;
  orderSubmitted: boolean;
  orderNr: string;
  hasAgreed = false;
  diffAddress = false;
  subs= new Subscription()
  apiCallFrom=new UserActionsFrom();

  constructor(
    private themeProvider: ThemeProviderService,
    private util: UtilityService,
    private alert: AlertService,
    private router: Router,
    private statCounter: StatCounterService,
    private fb: FormBuilder
  ) { 
    this.invoiceAddressForm = this.fb.group({
      InvoiceStreet: ['', [Validators.required, this.ValidateNoPlaceholder]],
      InvoiceHouseNr: ['', Validators.required],
      InvoiceCity: ['', [Validators.required, this.ValidateNoPlaceholder]],
      InvoiceZip: ['', Validators.required],
      InvoiceCustomerName: ['', [Validators.required, this.ValidateNoPlaceholder]],
      InvoiceCountry: ['', [Validators.required, this.ValidateNoPlaceholder]],
      PaymentMethod: ['', Validators.required]
    });
    this.shipAddressForm = this.fb.group({
      Street: ['', this.RequiredIfDiffAddress],
      HouseNr: [0, this.RequiredIfDiffAddress],
      City: ['', this.RequiredIfDiffAddress],
      Zip: ['', this.RequiredIfDiffAddress],
      Name: ['', this.RequiredIfDiffAddress],
      Country: ['', this.RequiredIfDiffAddress]
    });
  }
  ngOnDestroy(): void {
    this.subs.unsubscribe()
  }

  ngOnInit() {
    this.subs.add(this.themeProvider.getTheme().subscribe(t => this.mode = t));
    this.getShopCartEntries();
    this.subs.add(this.util.getUserInfo().subscribe(info => {
      this.invoiceAddressForm.patchValue({
        InvoiceZip: info.Zip,
        InvoiceCity: info.City,
        InvoiceStreet: info.Street,
        InvoiceHouseNr: info.HouseNr,
        InvoiceCustomerName: info.FirstName + ' ' + info.LastName,
        InvoiceCountry: info.Country
      });
    }));
  }

  submitOrder() {
    this.orderSubmitted = true;
    if (!this.hasAgreed || !this.enoughOrderAmount || (this.diffAddress && this.shipAddressForm.invalid)) {
      return;
    }
    let params = new HttpParams();
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
    this.subs.add(this.util.submitOrder({...this.invoiceAddressForm.value, ...this.shipAddressForm.value},params).subscribe(res => {
      this.router.navigate(['/bestellung/'+ res.OrderId]);
      if (res.Msg) {
        this.alert.success(res.Msg);
      }
    }));
  }

  getShopCartEntries(){
    this.loading=true
    this.subs.add(this.util.getShopEntries().subscribe(t => {
      this.shopCartEntries=t.ShopCartEntries.EntriesList;
      this.subTotal =t.ShopCartEntries.TotalNormal;
      this.shippingCosts = t.ShopCartEntries.ShippingCosts;
      this.loading=false;
    }));
  }

  removeItem(entryId){
    this.subs.add(this.util.deleteShopCartEntry(entryId).subscribe(()=>{
      this.getShopCartEntries();
      this.statCounter.requestShopCartCounter().subscribe(data=>this.statCounter.setShopCartCounter(data))
    }));
  }

  changeQuantity(entryId,increase,decrease){
    this.subs.add(this.util.changeQuantity(entryId, increase, decrease).subscribe(s => {
      this.shopCartEntries=s.EntriesList;
      this.subTotal=s.TotalNormal;
      this.shippingCosts = s.ShippingCosts;
    }));
  }

  ValidateNoPlaceholder(control: AbstractControl) {
    if (!control.value.includes('Platzhalter') && control.value !== 0) {
      return null;
    }
    return { hasPlaceholder: true };
  }

  RequiredIfDiffAddress(control: AbstractControl) {
    if ( (!!control.value && control.value !== '' && control.value !== 0)) {
      return null;
    }
    return { required: true };
  }

  get totalNormal() {
    return this.subTotal + this.shippingCosts - this.amountRedeemed;
  }

  get minLeft(): number {
    return this.MIN_ORDER_AMOUNT - this.subTotal;
  }

  get enoughOrderAmount(): boolean {
    return this.subTotal >= this.MIN_ORDER_AMOUNT;
  }

  get vat() {
    let vat = 0;
    this.shopCartEntries.forEach(e => {
      vat += (e.ArticleView.NormalPrice - (e.ArticleView.NormalPrice / (1+e.ArticleView.VAT))) * e.Quantity;
    });
    return vat;
  }

  toUrl(url){
    return this.util.toUrl(url,false);
  }

  get totalCartItems() {
    const res = this.shopCartEntries.reduce(( sum, { Quantity }) => sum + Quantity , 0);
    return res;
  }

  get f() {
    return this.invoiceAddressForm.controls;
  }

  get sh() {
    return this.shipAddressForm.controls;
  }

  get form() {
    return this.invoiceAddressForm.value;
  }

}
