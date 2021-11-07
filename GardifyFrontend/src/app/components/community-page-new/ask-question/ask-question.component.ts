import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, NgForm, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { AlertService } from 'src/app/services/alert.service';
import { ScannerService } from 'src/app/services/scanner.service';
import { UtilityService } from 'src/app/services/utility.service';
import { CurrentUser } from '../community-page-new.component';
import { CommunityService } from '../community.service';

@Component({
  selector: 'app-ask-question',
  templateUrl: './ask-question.component.html',
  styleUrls: ['./ask-question.component.css']
})
export class AskQuestionComponent implements OnInit {

  @ViewChild('f', { static: false }) questionForm: NgForm;

  subs = new Subscription();
  submitted = false;
  isExpert = false;
  currentUser: CurrentUser;
  imageSrc: string;
  askAdmin : boolean = false;

  uploadForm = new FormGroup({
    commentText: new FormControl('', [Validators.required]),
    file: new FormControl(''),
  });

  constructor(
    private util: UtilityService,
    private communityService: CommunityService,
    private scannerService: ScannerService,
    private formBuilder: FormBuilder,
    private alert: AlertService,
    private route: ActivatedRoute
  ) {

    this.uploadForm = this.formBuilder.group({
      commentText: '',
      file: [],
    })

  }

  ngOnInit(): void {

    this.communityService.currentUser
      .subscribe((user: CurrentUser) => {
        this.currentUser = user;
      })

    this.communityService.valueObs.subscribe(data => {

      console.log('Trigger from ComponentB');
      console.log(data);

    });

    this.route.queryParams.subscribe(data => {
      this.askAdmin = this.convertToBoolean(data['askAdmin'])
      console.log(this.askAdmin);
    })


  }


  onSubmit() {
    if (this.imageSrc != '') {

      this.submitted = true;

      var question = {
        "QuestionAuthorId": this.currentUser?.user?.UserId,
        "QuestionText": this.uploadForm.value.commentText,
        "IsOnlyExpert": this.askAdmin,
        "Thema": ""
      };

      this.subs.add(this.util.postQuestion(question).subscribe(questionId => {
        console.log("question posted")
        this.uploadImage(questionId);
      }))

    } else {
      this.alert.error("Bitte Bild auswählen")
    }
  }

  uploadImage(questionId: string) {

    const albImg = new FormData();

    albImg.append('id', questionId);
    albImg.append('imageTitle', '');
    albImg.append('imageFile', this.uploadForm.value.file);

    console.log("questionid", questionId);
    console.log("imageTitle", this.uploadForm.value.file);
    console.log("imageFile", questionId);

    this.subs.add(this.util.uploadCommunityQuestionImg(albImg).subscribe(imgId => {
      if (imgId != null) {
        this.alert.success("Das Bild wurde erfolgreich hochgeladen")
      }
    }))

    this.uploadForm.reset();
    this.imageSrc = null

  }


  async onFileChange(event) {
    const reader = new FileReader();
    if (event.target.files.length > 0) {
      const file = event.target.files[0];
      const res = await this.scannerService.handleImageUpload(event, true);

      if (res.err) {

        return;
      }
      this.uploadForm.patchValue({
        file: res.file
      });
      reader.readAsDataURL(file);

      reader.onload = () => {

        this.imageSrc = reader.result as string;
      }
    }

  }

  convertToBoolean(input: string): boolean | undefined {
    try {
        return JSON.parse(input);
    }
    catch (e) {
        return undefined;
    }
}

}
