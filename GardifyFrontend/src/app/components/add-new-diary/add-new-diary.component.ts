import { Component, OnInit, OnDestroy, HostListener } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UtilityService } from 'src/app/services/utility.service';
import { forkJoin, Subscription } from 'rxjs';
import { Router } from '@angular/router';
import { ObjectType, UploadedImageResponse, UserActionsFrom } from 'src/app/models/models';
import { AuthService } from 'src/app/services/auth.service';
import { faChevronDown, faCircle, faPencilAlt } from '@fortawesome/free-solid-svg-icons';
import { AlertService } from 'src/app/services/alert.service';
import { ScannerService } from 'src/app/services/scanner.service';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-add-new-diary',
  templateUrl: './add-new-diary.component.html',
  styleUrls: ['./add-new-diary.component.css']
})
export class AddNewDiaryComponent implements OnInit, OnDestroy {
  mode: string;
  faPencilAlt=faPencilAlt;
  newDiary: FormGroup;
  subs = new Subscription();
  invalidImg = false;
  faCircle = faCircle;
  faChevronDown = faChevronDown;
  gardenId;
  images: UploadedImageResponse[] = [];
  isMobile: boolean;
  
  apiCallFrom=new UserActionsFrom();

  constructor(
    private tp: ThemeProviderService, 
    private fb: FormBuilder,
    private alert: AlertService,
    private auth: AuthService,
    private scanner: ScannerService,
    private router:Router,
    private util: UtilityService) { 

    this.newDiary = this.fb.group({
      Title: [null, Validators.required],
      Description: [null, Validators.required],
      Date: new Date().toISOString().split('T')[0],
      UserId: this.auth.getCurrentUser().UserId,
      EntryObjectId: 0,
      EntryOf: ObjectType.Garden,
    });
  }
  @HostListener('window:resize')
  onWindowResize() {
    this.isMobile = window.innerWidth <= 425;
  }
  ngOnInit() {
    this.isMobile = window.innerWidth <= 425;
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
    let params = new HttpParams();
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
    this.subs.add(this.util.getUserMainGarden(params).subscribe(garden => {
      this.gardenId = garden.Id;
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

  addDiary() {
    if (!this.newDiary.valid || !this.gardenId) {
      return;
    }
    this.newDiary.patchValue({ EntryObjectId: this.gardenId });
    let params = new HttpParams();
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
    this.subs.add(
      
      this.util.createDiaryEntry(this.newDiary.value,params).subscribe((entry) => {
        // uplaod the images
        if(entry.Id>0){
          if(this.images.length>0){
            const imgs$ = this.images.map(img => {
              const imgForm = new FormData();
              imgForm.append('imageFile', img.file);
              imgForm.append('imageTitle', img.title);
              imgForm.append('id', entry.Id);
              return this.util.uploadDiaryImg(imgForm);
            });
            if(imgs$ !==null){
              forkJoin(imgs$).subscribe(() => {
                this.newDiary.reset();
                this.alert.success('Eintrag wurde erstellt');
                this.router.navigate(['/todo']);
              });
            }else{
              this.newDiary.reset();
              this.alert.success('Eintrag wurde erstellt');
              this.router.navigate(['/todo']);
            }
          }else{
            this.newDiary.reset();
            this.alert.success('Eintrag wurde erstellt');
            this.router.navigate(['/todo']);
          }
          
         
        }else{
          this.alert.error('Eintrag konnte nicht erstellt werden');
          this.router.navigate(['/todo']);
        }
        
      })
    );
  }
}
