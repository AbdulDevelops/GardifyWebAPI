import { Component, OnInit, OnDestroy } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { Subscription } from 'rxjs';
import { UtilityService } from 'src/app/services/utility.service';
import { faChevronDown } from '@fortawesome/free-solid-svg-icons';
import { Ad, UserActionsFrom } from 'src/app/models/models';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-videos',
  templateUrl: './videos.component.html',
  styleUrls: ['./videos.component.css']
})
export class VideosComponent implements OnInit, OnDestroy {
  mode: string;
  faChevronDown = faChevronDown;
  videos = [];
  displayedVideos = [];
  topics: string[] = [];
  uniqueTopics = [];
  apiCallFrom=new UserActionsFrom();

  subs = new Subscription();
  ad: Ad;
  constructor(private themeProvider: ThemeProviderService, 
    private util: UtilityService, 
    private sanitizer: DomSanitizer) { 
      this.ad = new Ad(
        'ca-pub-3562132666777902',
        6299417877,
        'fluid',
        true,
        false,
        null
      )
    }

  ngOnInit() {
    let params = new HttpParams();
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
    this.subs.add(this.themeProvider.getTheme().subscribe(t => this.mode = t));
    this.subs.add(this.util.getVideos(params).subscribe(d => {
      this.videos = d;
      this.videos.forEach(v => {
        v.YTLink = this.sanitizer.bypassSecurityTrustResourceUrl(this.embed(v.YTLink));
        if (v.Tags) {
          this.topics.push(v.Tags);
        }
        v.Text=this.urlify(v.Text)
      });
      setTimeout(() => {
        this.uniqueTopics = Array.from(new Set([].concat.apply([], this.topics))).sort();
      });
      this.displayedVideos = this.videos;
    }));
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  search(text) {
    if (!text) { this.displayedVideos = this.videos; return; }
    this.displayedVideos = this.videos.filter(v => v.Tags.includes(text));
  }

  sort(text) {
    switch(text) {
      case 'date': this.displayedVideos = this.videos.sort((a,b) => a.Date > b.Date ? 1 : -1); break;
      case 'view': this.displayedVideos = this.videos.sort((a,b) => a.ViewCount > b.ViewCount ? 1 : -1); break;
      default: this.displayedVideos = this.videos; break;
    }
  }

  embed(ytLink: string) {
    const BASE = 'https://www.youtube.com/embed/';
    const OPTIONS = '?rel=0&modestbranding=1';
    return BASE + ytLink.split('watch?v=')[1].split('&')[0] + OPTIONS;
  }
  urlify(text:string){
    var urlRegex = /(https?:\/\/[^\s]+)/g;
    return text.replace(urlRegex, '<a target="_blank" href="$1">$1</a>')
  }
}
