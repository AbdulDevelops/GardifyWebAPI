import { Component, ElementRef, Input, OnInit } from '@angular/core';
import { Community, CommunityAnswerList, CommunityQuestion, CommunityWithAnswers } from 'src/app/models/models';
import { Subscription } from 'rxjs';
import { UtilityService } from 'src/app/services/utility.service';
import { CurrentUser } from '../community-page-new.component';
import { CommunityService } from '../community.service';
import { UtilityTimeService } from 'src/app/services/utility-time.service';
import { DateAdapter } from 'angular-calendar';

@Component({
  selector: 'app-community-garden',
  templateUrl: './community-garden.component.html',
  styleUrls: ['../community-page-new.component.css'],
  providers: [UtilityTimeService]
})
export class CommunityGardenComponent implements OnInit {

  @Input() isLoading: boolean
  currentUser: CurrentUser;

  commentLength: number = 3;

  articleExperts: Community;
  articleNormal: Community;
  communityQuestion: CommunityQuestion;
  communityWithAnswers: CommunityWithAnswers[];
  communityWithAnswersExpert: CommunityWithAnswers[];
  subs = new Subscription();

  constructor(
    private util: UtilityService,
    private communityService: CommunityService,
    private timeUtil: UtilityTimeService
  ) { }

  ngOnInit() {
    this.communityService.currentUser
      .subscribe((user: CurrentUser) => {
        this.currentUser = user;
      })

    this.subs.add(this.util.getCommunityDataWithComments().subscribe(d => {
      this.communityWithAnswers = d.filter(d => d.Post.IsOnlyExpert === false);
      this.communityWithAnswersExpert = d.filter(d => d.Post.IsOnlyExpert === true);

      console.log("model", d);

      this.isLoading = false;
    }));

    console.log("garden component %j", this.currentUser?.imageData?.Images[0]?.SrcAttr);

  }

  toUrl(src): string {
    return this.util.toUrl(src, false);
  }

  getCommentsByQuestionId(questionId: number) {
    this.subs.add(this.util.getCommunityQuestionComments(questionId).subscribe(d => {
      this.communityQuestion = d
    }));
  }

  postComment(keyboardEvent: KeyboardEvent, communityPost: CommunityWithAnswers, adminComment : boolean) {

    var inputValue: string = (<HTMLInputElement>keyboardEvent.target).value;
    // clear input field
    (<HTMLInputElement>keyboardEvent.target).value = "";

    console.log("enter triggered for question id " + communityPost.Post.QuestionId);


    let communityAnswer: CommunityAnswerList = {
      $id: null,
      AnswerText: inputValue,
      AutorName: this.currentUser.user.Name,
      Date: new Date(),
      AnswerImages: null,
      ProfilUrl: this.currentUser.imageData.Images,
      AnswerId: null,
      IsFromAdmin: this.currentUser.user.Admin,
    };

    if (adminComment) {
      this.communityWithAnswersExpert.find(x => x.$id == communityPost.$id).CommunityAnswerList.push(communityAnswer)
    } else {
      this.communityWithAnswers.find(x => x.$id == communityPost.$id).CommunityAnswerList.push(communityAnswer)
    }

    // post answer
    let answer: Answer = {
      CommunityEntryId: communityPost.Post.QuestionId,
      AnswerText: inputValue
    };

    this.subs.add(this.util.addComment(answer).subscribe(a => {
      console.log("comment posted")
    }))

  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  beautifyTimeDate(dateString: string) {
    let date = new Date(dateString);
    return this.timeUtil.relativeTimeFromNow(date)
  }

}

interface Answer {
  CommunityEntryId: number;
  AnswerText: string;
}