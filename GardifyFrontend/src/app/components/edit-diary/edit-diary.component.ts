import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { faCircle, faChevronDown } from '@fortawesome/free-solid-svg-icons';
import { UploadedImageResponse, ObjectType, DiaryEntry, UserActionsFrom } from 'src/app/models/models';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { AlertService } from 'src/app/services/alert.service';
import { AuthService } from 'src/app/services/auth.service';
import { UtilityService } from 'src/app/services/utility.service';
import { ActivatedRoute } from '@angular/router';
import { ScannerService } from 'src/app/services/scanner.service';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-edit-diary',
  templateUrl: './edit-diary.component.html',
  styleUrls: ['./edit-diary.component.css']
})
export class EditDiaryComponent implements OnInit, OnDestroy {
  entryId: number;
  mode: string;
  newDiary: FormGroup;
  subs = new Subscription();
  invalidImg = false;
  faCircle = faCircle;
  faChevronDown = faChevronDown;
  gardenId;
  images: UploadedImageResponse[] = [];
  entry: DiaryEntry;
  apiCallFrom=new UserActionsFrom();

  constructor(
    private tp: ThemeProviderService, 
    private fb: FormBuilder,
    private activateRoute: ActivatedRoute,
    private alert: AlertService,
    private auth: AuthService,
    private scanner: ScannerService,
    private util: UtilityService) { 

    this.newDiary = this.fb.group({
      Title: [null, Validators.required],
      Description: [null, Validators.required],
      Date: new Date().toISOString().split('T')[0],
      UserId: this.auth.getCurrentUser().UserId,
      EntryObjectId: 0,
      EntryOf: ObjectType.Garden,
      Id: 0
    });
  }

  ngOnInit() {
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
    let params = new HttpParams();
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
    this.subs.add(this.util.getUserMainGarden(params).subscribe(garden => {
      this.gardenId = garden.Id;
    }));
    this.subs.add(this.activateRoute.params.subscribe(params => {
      this.entryId = params.id;
      this.getEntry();
    }));
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  async onImageUpload(event) {
    const res = await this.scanner.handleImageUpload(event);
    if (res.err) {
      this.invalidImg = !!res.err;
      return;
    }
    this.invalidImg = false;
    this.images.push(res);
  }

  getEntry() {
    this.subs.add(this.util.getDiaryEntry(this.entryId).subscribe(entry => {
      this.entry = entry;
      this.newDiary.patchValue({
        ...entry,
        Date: new Date(entry.Date).toISOString().split('T')[0]
      });
    }));
  }

  updateDiary() {
    if (!this.newDiary.valid || !this.gardenId) {
      return;
    }
    this.newDiary.patchValue({ EntryObjectId: this.gardenId });
    this.subs.add(
      this.util.updateDiaryEntry(this.newDiary.value).subscribe((entry) => {
        // upload the images
        this.images.forEach(img => {
          const imgForm = new FormData();
          imgForm.append('imageFile', img.file);
          imgForm.append('imageTitle', img.title);
          imgForm.append('id', this.entry.Id.toString());
          this.subs.add(this.util.uploadDiaryImg(imgForm).subscribe());
        });
        this.alert.success('Eintrag wurde bearbeitet');
      })
    );
  }

  toUrl(url: string) {
    this.util.toUrl(url);
  }

}
