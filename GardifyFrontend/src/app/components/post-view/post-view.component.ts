import { Component, OnInit, OnDestroy } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { PagerService } from 'src/app/services/pager.service';
import { UtilityService } from 'src/app/services/utility.service';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-post-view',
  templateUrl: './post-view.component.html',
  styleUrls: ['./post-view.component.css']
})
export class PostViewComponent implements OnInit,OnDestroy {
  mode: string;
  id: number;
  post: any;
  comments: any[] = [
    {
      Content: 'comment 1',
      CreatedDate: Date.now()
    },
    {
      Content: 'comment 2',
      CreatedDate: Date.now()
    },
    {
      Content: 'comment 3',
      CreatedDate: Date.now()
    }
  ];

  commentsPager: any = {};
  pagedComments: any[];
  subs=new Subscription()
  constructor(private tp: ThemeProviderService, private pagerService: PagerService, private util: UtilityService, private route: ActivatedRoute) { }
  ngOnDestroy(): void {
    this.subs.unsubscribe()
  }

  ngOnInit() {
    this.subs.add(this.route.params.subscribe(params => this.id = params['id']));
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
    this.subs.add(this.util.getThread(this.id).subscribe(t => {
      this.post = t.ThreadHeader;
      this.post.Title=this.post.Title.replace(/\[k\]/g, '<i>').replace(/\[\/k\]/g, '</i>');
     
      this.comments = t.RelatedPosts;
      this.setComments(1);
    }));
  }

  setComments(page: number) {
    if (this.comments) {
      // get pager object from service
      this.commentsPager = this.pagerService.getPager(this.comments.length, page, 10);
      // get current page of items
      this.pagedComments = this.comments.slice(this.commentsPager.startIndex, this.commentsPager.endIndex + 1);
    }
  }
}
