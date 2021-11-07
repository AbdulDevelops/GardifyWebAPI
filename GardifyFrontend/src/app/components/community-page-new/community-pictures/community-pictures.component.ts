import { Component, Input, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { Community, CommunityQuestion } from 'src/app/models/models';
import { UtilityService } from 'src/app/services/utility.service';

@Component({
  selector: 'app-community-pictures',
  templateUrl: './community-pictures.component.html',
  styleUrls: ['../community-page-new.component.css']
})
export class CommunityPicturesComponent implements OnInit {

  @Input() isLoading: boolean

  articleExperts: Community;
  articleNormal: Community;
  communityQuestion: CommunityQuestion;

  subs = new Subscription();

  constructor(private util: UtilityService) { }

  ngOnInit() {
    this.subs.add(this.util.getCommunityData().subscribe(d => {
      this.articleNormal = d.filter(d => d.IsOnlyExpert === false);
      this.articleExperts = d.filter(d => d.IsOnlyExpert === true);

      this.isLoading = false;
    }));
  }

  toUrl(src): string {
    return this.util.toUrl(src, false);
  }

  getCommentsByQuestionId(questionId: number) {
    this.subs.add(this.util.getCommunityQuestionComments(questionId).subscribe(d => {
      this.communityQuestion = d
    }));
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }
}