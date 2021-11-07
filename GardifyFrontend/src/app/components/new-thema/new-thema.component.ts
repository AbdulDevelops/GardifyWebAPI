import { Component, OnInit, OnDestroy, HostListener } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { UploadedImageResponse, UserActionsFrom } from 'src/app/models/models';
import { Subscription } from 'rxjs';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { UtilityService } from 'src/app/services/utility.service';
import { Route } from '@angular/compiler/src/core';
import { Routes, Router } from '@angular/router';
import { ScannerService } from 'src/app/services/scanner.service';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-new-thema',
  templateUrl: './new-thema.component.html',
  styleUrls: ['./new-thema.component.css']
})
export class NewThemaComponent implements OnInit,OnDestroy {
  mode:string;
  newThema:FormGroup;
  images: UploadedImageResponse[] = [];
  invalidImg = false;
    subs= new Subscription();
  width: number;
  isMobile: boolean;
  loading = false;
  apiCallFrom=new UserActionsFrom();

    constructor( private tp:ThemeProviderService, private scanner: ScannerService, private fb:FormBuilder,private util:UtilityService,private router:Router) {
      this.newThema = this.fb.group({
        QuestionText: ['Hallo, ', Validators.required],
        Thema: [null],
        Description: [null],
        Isownfoto: new FormControl(false),
      });
    }
  ngOnDestroy(): void {
   this.subs.unsubscribe()
  }
  @HostListener('window:resize')
  onWindowResize() {
    this.width = window.innerWidth;
    this.isMobile = window.innerWidth <= 990;
  }
  // Images: [null],
    ngOnInit() {
      this.isMobile = window.innerWidth <= 768;
      this.tp.getTheme().subscribe(t => this.mode = t);
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
    uploadImage(entryId){
      const imgForm = new FormData();
      this.images.forEach((img,i) => {
        imgForm.append('imageFile'+ i, img.file);
        imgForm.append('imageTitle'+ i, img.title);
      });
        imgForm.append('id', entryId);
        this.subs.add(this.util.uploadQuestImg(imgForm).subscribe(() => {
          this.newThema.reset();
          this.loading = false;
          this.router.navigate(['/pflanzendoc/', "newPost"]);
        }));
    }
    
    addQuestion() {
      this.loading = true;
      let params = new HttpParams();
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
      this.subs.add(this.util.addNewQuestion(this.newThema.value,params).subscribe(entryId => {
        this.uploadImage(entryId);
      }));
    }
  }