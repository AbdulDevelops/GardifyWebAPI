import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { TodoCycleType, ReferenceToModelClass, Todo, NotifyType, UploadedImageResponse } from 'src/app/models/models';
import { Validators, FormGroup, FormBuilder } from '@angular/forms';
import { Subscription } from 'rxjs';
import { faChevronDown, faCircle } from '@fortawesome/free-solid-svg-icons';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { Router, ActivatedRoute } from '@angular/router';
import { StatCounterService } from 'src/app/services/stat-counter.service';
import { UtilityService } from 'src/app/services/utility.service';
import { AlertService } from 'src/app/services/alert.service';
import { ScannerService } from 'src/app/services/scanner.service';

@Component({
  selector: 'app-edit-todo',
  templateUrl: './edit-todo.component.html',
  styleUrls: ['./edit-todo.component.css']
})
export class EditTodoComponent implements OnInit,OnDestroy {
  mode: string;
  subs = new Subscription();
  newTodo: FormGroup;
  toUpdate: Todo;
  todoId: number;
  images: UploadedImageResponse[] = [];
  invalidImg = false;
  faChevronDown = faChevronDown;
  faCircle = faCircle
  constructor(
    private tp: ThemeProviderService, 
    private fb: FormBuilder,
    private activateRoute: ActivatedRoute,
    private alert: AlertService,
    private counter: StatCounterService,
    private scanner: ScannerService,
    private util: UtilityService) { 

    this.newTodo = this.fb.group({
      Title: [null, Validators.required],
      Description: [null, Validators.required],
      DateStart: [new Date().toISOString().split('T')[0]],
      DateEnd: [new Date().toISOString().split('T')[0]],
      Notification: [0],
      Cycle: [0],
      //Reminder: [null],
      ReferenceType: ReferenceToModelClass.Todo,
      Precision: 0,
      Images: [null],
      Id: 0
    });
  }
  ngOnDestroy(): void {
    this.subs.unsubscribe()
  }

  ngOnInit() {
    this.subs.add(this.activateRoute.params.subscribe(params => {
      this.todoId = params.id;
      this.getTodo();
    }));
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
  }

  getTodo() {
    this.subs.add(this.util.getTodo(this.todoId).subscribe(todo => {
      this.toUpdate = todo;
      this.newTodo.patchValue({
        ...todo,
        DateStart: new Date(todo.DateStart).toISOString().split('T')[0],
        DateEnd: new Date(todo.DateEnd).toISOString().split('T')[0]
      });
    }));
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

  editTodo() {
    if (!this.newTodo.valid) {
      return;
    }else{
        this.subs.add(
          this.util.updateTodo(this.toUpdate.Id, this.newTodo.value).subscribe(() => {
            // upload the images
            this.uploadImage();
            this.alert.success('Todo wurde bearbeitet');
          })
        );
    }
  }
  uploadImage(){
    this.images.forEach(img => {
      const imgForm = new FormData();
      imgForm.append('imageFile', img.file);
      imgForm.append('imageTitle', img.title);
      imgForm.append('id', this.toUpdate.Id.toString());
      this.subs.add(this.util.uploadTodoImg(imgForm).subscribe());
    });
  }
  toUrl(url: string) {
    this.util.toUrl(url);
  }

  get CycleType() {
    return TodoCycleType;
  }

  get NotifyType() {
    return NotifyType;
  }
}
