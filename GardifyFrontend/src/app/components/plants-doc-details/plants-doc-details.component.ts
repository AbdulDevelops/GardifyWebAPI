import { Component, OnInit, HostListener, OnDestroy } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { ActivatedRoute } from '@angular/router';
import { UtilityService } from 'src/app/services/utility.service';
import { FaqEntry, UploadedImageResponse } from 'src/app/models/models';
import { AuthService } from 'src/app/services/auth.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Subscription } from 'rxjs';
import { StatCounterService } from 'src/app/services/stat-counter.service';
import { ScannerService } from 'src/app/services/scanner.service';
import { AlertService } from 'src/app/services/alert.service';

@Component({
  selector: 'app-plants-doc-details',
  templateUrl: './plants-doc-details.component.html',
  styleUrls: ['./plants-doc-details.component.css']
})
export class PlantsDocDetailsComponent implements OnInit,OnDestroy {
  item:any;
  mode: string;
  width: number;
  isMobile: boolean;
  answerForm:FormGroup;
  fileSrc = [];
  tagline: any;
  valid: boolean;
  answerMode = false;
  user;
  subs=new Subscription();
  images: UploadedImageResponse[] = [];
  invalidImg = false;
  answers: any[];
  loading:boolean;
  fotoUrl:string;
  selectedAnswer:any;
  answerItem: any;
  postItem: any;
  constructor(
    private tp: ThemeProviderService, 
    private auth: AuthService,
    private scanner: ScannerService,
    private fb:FormBuilder,
    private activateRoute:ActivatedRoute, 
    private util:UtilityService,
    private sc:StatCounterService,
    private alert: AlertService) {
    this.answerForm = this.fb.group({
      AuthorId: null,
      AnswerText: null,
      PlantDocEntryId: null
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

  ngOnInit() {
    this.loading=true
    this.isMobile = window.innerWidth <= 768;
    this.subs.add(this.auth.getUserObservable().subscribe(user => this.user = user));
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
    const id = +this.activateRoute.snapshot.paramMap.get('id')
    this.getPost(id);
  }

  getPost(id): void {
    
    this.subs.add(this.util.getEntryById(id)
      .subscribe(item => {
        this.item = item;
        if(this.isAdminImg(item.PlantDocViewModel.Images[0]?.FullDescription)){
          this.fotoUrl=this.toUrl(item.PlantDocViewModel.Images[0]?.SrcAttr,false,false,false,true)
        }else{
          this.fotoUrl=this.toUrl(item.PlantDocViewModel.Images[0]?.SrcAttr,false,false,false,false)
        }
        this.answers=item.PlantDocAnswerList
        this.subs.add(this.sc.requestAnswersCount().subscribe(data=>this.sc.setAnswersCount(data)))
        this.loading=false
      }));
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
    this.images.forEach(img => {
      const imgForm = new FormData();
      imgForm.append('imageFile', img.file);
      imgForm.append('imageTitle', img.title);
      imgForm.append('id', entryId);
      this.subs.add(this.util.uploadAnswerImg(imgForm).subscribe());
    });
  }

  postAnswer() {
    this.loading=true
    this.answerMode = false;
    const questionId=this.item.PlantDocViewModel.QuestionId
    this.answerForm.patchValue({
      AuthorId:this.user.UserId,
      PlantDocEntryId: questionId,
    })
    if(this.answerForm.value && questionId>0){
      
      this.subs.add(this.util.postAnswer(this.answerForm.value).subscribe((answerId)=>{
        if(answerId>0){
          this.uploadImage(answerId)
        };
        this.getPost(questionId)
      }))
    }
  }

  toggleAnswerMode() {
    if (this.auth.isTestAccount()) {
      this.alert.error('Diese Funktion steht leider nur zur Verfügung, wenn du registriert bist.');
      return;
    }
    this.answerMode = true;
  }

  updateAnswer(selectedAnswer:any){
    if(selectedAnswer!=null){
      this.subs.add(this.util.updateAnswer(selectedAnswer.AnswerId,selectedAnswer).subscribe())
    }
  }
  getAnswerById(id:number){
    this.subs.add(this.util.getAnswerById(id).subscribe(data=>this.answerItem=data))
  }
  getPostById(id:number){
    this.subs.add(this.util.getPostById(id).subscribe(data=>this.postItem=data))
  }
  print(descip){
    console.log(descip)
  }
  isAdminImg(imgDescr){
    let description=imgDescr
    this.print(description)
    return description.includes("Admin_image")
   }
  toUrl(url: string,small,autoscale,adminArea,AdminImg) {
    return this.util.toUrl(url,small,autoscale,adminArea,AdminImg);
  }
}
