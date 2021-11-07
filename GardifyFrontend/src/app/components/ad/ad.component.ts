import { AfterViewInit, Component, Input, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { Ad, UserActionsFrom } from 'src/app/models/models';
import { StatCounterService } from 'src/app/services/stat-counter.service';
import { environment } from 'src/environments/environment';

@Component({
  //selector: 'google-adsense',
  selector: 'app-ad',
  templateUrl: './ad.component.html',
  styleUrls: ['./ad.component.css']
})

export class AdComponent implements OnInit, AfterViewInit{
  @Input() ad: Ad;
  @Input() masterName: string; 
  adSlot:any
  adClient:any
  adFormat:any
  fullWidthResponsive:any
  data_ad_layout_key:any
  isVertical:any
  showAd = true;
  subs = new Subscription();
  apiCallFrom= new UserActionsFrom();
  constructor(private stats: StatCounterService) { }

  ngOnInit() {
    this.adClient=this.ad.adClient
    this.adSlot=this.ad.adSlot
    this.adFormat=this.adFormat
    this.fullWidthResponsive=this.ad.fullWidthResponsive
    this.data_ad_layout_key=this.ad.data_ad_layout_key
    this.isVertical = this.ad.isVertical
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

  ngAfterViewInit() {
    setTimeout(() => {
      try {
        (window['adsbygoogle'] = window['adsbygoogle'] || []).push({
          overlays: {bottom: true}
        });
      } catch(err) { }
    }, 1000);
  }

  onAdClick() {
    this.subs.add(this.stats.createStatEntry('adclick',this.apiCallFrom).subscribe());
  }
}