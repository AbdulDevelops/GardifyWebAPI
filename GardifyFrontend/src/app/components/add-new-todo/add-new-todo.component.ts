import { Component, OnInit, ChangeDetectorRef, HostListener, OnDestroy } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { forkJoin, Subscription } from 'rxjs';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { UtilityService } from 'src/app/services/utility.service';
import { ReferenceToModelClass, TodoCycleType, NotifyType, UploadedImageResponse } from 'src/app/models/models';
import { Router, ActivatedRoute } from '@angular/router';
import { faChevronDown, faCircle, faPencilAlt } from '@fortawesome/free-solid-svg-icons';
import { StatCounterService } from 'src/app/services/stat-counter.service';
import { AlertService } from 'src/app/services/alert.service';
import { ScannerService } from 'src/app/services/scanner.service';

@Component({
  selector: 'app-add-new-todo',
  templateUrl: './add-new-todo.component.html',
  styleUrls: ['./add-new-todo.component.css']
})
export class AddNewTodoComponent implements OnInit, OnDestroy {
  mode: string;
  faPencilAlt=faPencilAlt;
  subs = new Subscription();
  newTodo: FormGroup;
  images: UploadedImageResponse[] = [];
  invalidImg = false;
  faChevronDown = faChevronDown;
  faCircle = faCircle;
  deviceId:number;
  isMobile: boolean;
  constructor(
    private tp: ThemeProviderService, 
    private fb: FormBuilder,
    private alert: AlertService,
    private counter: StatCounterService,
    private util: UtilityService,
    private scanner: ScannerService,
    private activateRoute:ActivatedRoute,
    private router: Router,
    ) { 

    this.newTodo = this.fb.group({
      Title: [null, Validators.required],
      Description: [null, Validators.required],
      DateStart: [new Date().toISOString().split('T')[0]],
      DateEnd: [new Date().toISOString().split('T')[0]],
      Notification: [0],
      Cycle: [0],
      //Reminder: [null],
      ReferenceType: ReferenceToModelClass.Todo,
      ReferenceId:0,
      Precision: 0,
      Images: [null],
    });
  }

  @HostListener('window:resize')
  onWindowResize() {
    this.isMobile = window.innerWidth <= 425;
  }
  ngOnInit() {
    this.isMobile = window.innerWidth <= 425;
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
    this.activateRoute.params.subscribe(routeId=>{
      this.deviceId=routeId.id
      if(routeId.id){
        this.newTodo.patchValue({ReferenceType:ReferenceToModelClass.Device,
                                  ReferenceId:this.deviceId})
      }
    })
  }
  ngOnDestroy(){
    this.subs.unsubscribe()
  }
get loop(){
  if(this.newTodo.value.Cycle===0){
    return false
  }
  return true
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

  addTodo() {
    if (!this.newTodo.valid) {
      return;
    }
    if((this.newTodo.value.Cycle!==0) && (this.newTodo.value.DateEnd <= this.newTodo.value.DateStart)){
       return this.alert.error('Das Startdatum muss kleiner als das Enddatum sein');
    }
    if((this.newTodo.value.Cycle!==0) && (this.newTodo.value.DateEnd <= this.newTodo.value.DateStart)){
      return this.alert.error('Das Startdatum muss kleiner als das Enddatum sein');
   }
     this.subs.add(
      
      this.util.addTodo(this.newTodo.value).subscribe((newEntryId) => {
        // uplaod the image
        if (newEntryId > 0) {
          this.counter.requestTodosCount().subscribe(data => this.counter.setTodosCount(data));
          if(this.images.length>0){
            const imgs$ = this.images.map(img => {
              const imgForm = new FormData();
              imgForm.append('imageFile', img.file);
              imgForm.append('imageTitle', img.title);
              imgForm.append('id', newEntryId);
              return this.util.uploadTodoImg(imgForm);
            });
            if(imgs$ !==null){
              forkJoin(imgs$).subscribe(() => {
                this.newTodo.reset();
                this.alert.success('Todo wurde erstellt');
                this.router.navigate(['/todo']);
              });
            }else{
              this.newTodo.reset();
              this.alert.success('Todo wurde erstellt');
              this.router.navigate(['/todo']);
            }
          }else{
            this.newTodo.reset();
            this.alert.success('Todo wurde erstellt');
            this.router.navigate(['/todo']);
          }
         
        }else{
          this.newTodo.reset();
          this.alert.error('Todo konnte nicht erstellt werden');
          this.router.navigate(['/todo']);
        }
        
      })
    ); 
  }

  get CycleType() {
    return TodoCycleType;
  }

  get NotifyType() {
    return NotifyType;
  }
 
}
