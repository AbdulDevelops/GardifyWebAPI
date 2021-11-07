import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { UtilityService } from 'src/app/services/utility.service';
import { NewsEntry } from 'src/app/models/models';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-newspage',
  templateUrl: './newspage.component.html',
  styleUrls: ['./newspage.component.css']
})
export class NewspageComponent implements OnInit, OnDestroy {
  mode: string;
  id: number;
  article: NewsEntry;
  articleImageModal: any;
  subs = new Subscription();
  fotoUrl:string;
  public myRegex4= new RegExp(/\{k]|\[k]([\s\S]+?)\[\/k]/g);
    replacegrp="<i>$1</i>"
  constructor(private route: ActivatedRoute, private util: UtilityService,
              private themeProvider: ThemeProviderService) { }

  ngOnInit() {
    this.route.params.subscribe(params => this.id = params['id']);
    this.themeProvider.getTheme().subscribe(t => this.mode = t);
    this.subs.add(this.util.getNewsArticle(this.id).subscribe(d => {
      this.article = d;
      this.fotoUrl=this.toUrl(d.article.EntryImages[0])
    }));
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  toUrl(src): string {
    return this.util.toUrl(src, false);
  }

  goBack() {
    this.util.goBack();
  }
}
