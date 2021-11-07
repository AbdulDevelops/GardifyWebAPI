import { Component, OnInit, OnDestroy, ChangeDetectionStrategy, ViewChild, TemplateRef, ChangeDetectorRef, HostListener } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { UtilityService } from 'src/app/services/utility.service';
import { faEllipsisH, faListOl, faChevronLeft, faChevronRight, faChevronDown, faCircle, faCalendarAlt, faCaretRight, faCaretLeft, faInfo } from '@fortawesome/free-solid-svg-icons';
import { CalendarEvent, CalendarEventAction, CalendarDateFormatter,
  CalendarEventTimesChangedEvent, CalendarView, DAYS_OF_WEEK } from 'angular-calendar';
import { startOfDay, startOfMonth } from 'date-fns';
import { Todo, ObjectType, DiaryEntry, PlaceHolder, ReferenceToModelClass, Ad, UserActionsFrom } from 'src/app/models/models';
import { Subscription, Subject } from 'rxjs';
import { CustomCalendarDateFormatter } from './custom-calendar-formatter';
import { HttpParams } from '@angular/common/http';
import { AuthService } from 'src/app/services/auth.service';
import { Router } from '@angular/router';
import { StatCounterService } from 'src/app/services/stat-counter.service';
import { environment } from 'src/environments/environment';
import { AlertService } from 'src/app/services/alert.service';
import { slugify } from 'src/app/services/plant-search.service';
import { ActivatedRoute } from '@angular/router';

declare var $:any;
@Component({
  selector: 'app-todo-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './todo-page.component.html',
  styleUrls: ['./todo-page.component.css'],
  providers: [
    {
      provide: CalendarDateFormatter,
      useClass: CustomCalendarDateFormatter
    }
  ]
})
export class TodoPageComponent implements OnInit, OnDestroy {
  mode: string;
  showAddMenu: boolean;
  baseUrl = environment.gardifyBaseUrl;
  faCaretLeft = faCaretLeft; faCaretRight = faCaretRight; faMenu = faEllipsisH; faInfo = faInfo;
  faListOl = faListOl; faChevronDown = faChevronDown; faCircle = faCircle; faCalendar = faCalendarAlt;
  weekStartsOn = DAYS_OF_WEEK.MONDAY;
  view: CalendarView = CalendarView.Month;
  viewDate: Date = new Date();
  refresh: Subject<any> = new Subject();
  activeDayIsOpen = false;
  selectedTodoItem:Todo;
  subs = new Subscription();
  loadingScans = true;
  loadingDiary = true;
  loadingTodos = true;
  showCal = true;
  showAllTodoImg=false;
  showTodoMode = false;
  showTodoMenu = false;
  todos: Todo[] = [];
  onetodo:Todo;
  todoToView: Todo[]=[];
  todoImages:any;
  selectedTodo: Todo;
  selectedTodoCyclicId;
  diaryEntries: DiaryEntry[] = [];
  bioscanEntries: DiaryEntry[] = [];
  tasksInCurrentMonth: any[];
  userLists = [];
  selectedUserListId = 0;
  deleteMode: any;
  hide:boolean=false;
  year: number;
  month: number;
  apiCallFrom= new UserActionsFrom();
  enum_deleteTodos = [
    {name: 'Nur ausgewähltes Todo löschen',id:1},
    {name: 'Todo Zyklus löschen', id:2},
  ]
  colors: any = {
    diary: {
      primary: '#a965bd',
      secondary: '#ededed'
    },
    ownTodo: {
      primary: '#3a6188',
      secondary: '#ededed'
    },
    todo: {
      primary: '#beca3b',
      secondary: '#ededed'
    },
    bioScan: {
      primary: '#ff7f23',
      secondary: '#ededed'
    }
  };

  months = [
    {str: 'Januar', date: new Date (new Date('2021-01-01T00:00:00.000Z').setFullYear(new Date().getFullYear()))},
    {str: 'Februar', date: new Date (new Date('2021-02-01T00:00:00.000Z').setFullYear(new Date().getFullYear()))},
    {str: 'März', date: new Date (new Date('2021-03-01T00:00:00.000Z').setFullYear(new Date().getFullYear()))},
    {str: 'April', date: new Date (new Date('2021-04-01T00:00:00.000Z').setFullYear(new Date().getFullYear()))},
    {str: 'Mai', date: new Date (new Date('2021-05-01T00:00:00.000Z').setFullYear(new Date().getFullYear()))},
    {str: 'Juni', date: new Date (new Date('2021-06-01T00:00:00.000Z').setFullYear(new Date().getFullYear()))},
    {str: 'Juli', date: new Date (new Date('2021-07-01T00:00:00.000Z').setFullYear(new Date().getFullYear()))},
    {str: 'August', date: new Date (new Date('2021-08-01T00:00:00.000Z').setFullYear(new Date().getFullYear()))},
    {str: 'September', date: new Date (new Date('2021-09-01T00:00:00.000Z').setFullYear(new Date().getFullYear()))},
    {str: 'Oktober', date: new Date (new Date('2021-10-01T00:00:00.000Z').setFullYear(new Date().getFullYear()))},
    {str: 'November', date: new Date (new Date('2021-11-01T00:00:00.000Z').setFullYear(new Date().getFullYear()))},
    {str: 'Dezember', date: new Date (new Date('2021-12-01T00:00:00.000Z').setFullYear(new Date().getFullYear()))},
  ]
  years : any[]= []
  actions: CalendarEventAction[] = [
    {
      label: '<i class="fa fa-fw fa-pencil"></i>',
      onClick: ({ event }: { event: CalendarEvent }): void => {
        this.handleEvent('Edited', event);
      }
    },
    {
      label: '<i class="fa fa-fw fa-times"></i>',
      onClick: ({ event }: { event: CalendarEvent }): void => {
        this.events = this.events.filter(iEvent => iEvent !== event);
        this.handleEvent('Deleted', event);
      }
    }
  ];

  events: CalendarEvent[] = [];
  isMobile: boolean;
  ad: Ad;
  adBetween: Ad;
  myDate:any
  todoIsReset=true;
  show=false;
  hideResetButton: boolean=true;
  constructor(private themeProvider: ThemeProviderService,
              private auth: AuthService,
              private counter: StatCounterService,
              private cd: ChangeDetectorRef,
              private router: Router,
              private util: UtilityService,
              private alert: AlertService,
              private route: ActivatedRoute) {
                this.ad = new Ad(
                  'ca-pub-3562132666777902',
                  3349002663,
                  'fluid',
                  true,
                  false,
                  null
                );
                this.adBetween = new Ad(
                  'ca-pub-3562132666777902',
                  5163685782,
                  'fluid',
                  true,
                  false,
                  null
                );
               }

  handleEvent(action: string, event: CalendarEvent): void {
    switch(action) {
      case 'Clicked': this.showTodo(event); break;
      default: console.log(action, event);
    }
  }
  @HostListener('window:resize')
  onWindowResize() {
    this.isMobile = window.innerWidth <= 425;
  }
  ngOnInit() {

    this.route.params.subscribe(params => this.year = params['year']);
    this.route.params.subscribe(params => this.month = params['month']);

    if (this.year !== undefined && this.month !== undefined){
      this.viewDate = new Date(this.year, this.month - 1);
    } 


    console.log(this.year, this.month);
    this.subs.add(this.themeProvider.getTheme().subscribe(t => { this.mode = t; this.cd.markForCheck(); }));
    this.getListOfYear()
    this.getTodos();
    this.getDiaryEntries();
    this.getBioscanEntries();
    this.subs.add(this.util.getUserLists().subscribe(lists => {
      this.userLists = lists;
    }));
    this.hide=false
    this.showAllTodoImg=false;
    this.hideResetButton=true;
    this.isMobile = window.innerWidth <= 425;
    Promise.resolve().then(() => {
      this.viewDate = new Date();
      if (this.year !== undefined && this.month !== undefined){
        this.viewDate = new Date(this.year, this.month - 1);
      }
  
      this.myDate=this.viewDate;
    });
    this.getUserInfo();
  }
  
  resetTodos(){
    this.loadingTodos=true;
    this.subs.add(this.util.resetTodo().subscribe(()=>{
      this.getTodos();
      this.getUserInfo();
      this.loadingTodos=false;
      this.alert.success("To-dos wurden erfolgreich aktualisiert")
      
    }))
  }
  getListOfYear(){
    const number= 10
    var currentYear= new Date().getFullYear()
    for(var i=0; i<number; i++){
      this.years.push({date:currentYear})
      currentYear=currentYear+1
    }
  }
  setYear(step: number) {
    this.viewDate = new Date(
      this.viewDate.getFullYear()+step,
      this.viewDate.getMonth(),
      this.viewDate.getDate());
  }
  setMonth(step: number) {
    const year = this.viewDate.getFullYear();
    const month = this.viewDate.getMonth();
    const day = this.viewDate.getDate();
    this.viewDate = new Date(year , month + step, day);
  }
  dayClicked(date:Date){
  let allDayEvents=[];
  this.todoToView =[];
  allDayEvents=this.events.filter(event => event.start.getDate()===date.getDate()&& event.start.getUTCMonth()===date.getUTCMonth()&&event.start.getUTCFullYear()===date.getUTCFullYear())
   allDayEvents.forEach(e=>{
     this.todoToView.push(this.showTodo(e))
   });
   this.showTodoMode = true;
  }
  scroll(el: HTMLElement) {
    if(el){
      el.scrollIntoView();
    }
    
  }
  getDiaryEntries(clearEvents = false) {
    this.util.getDiaryEntries(this.viewDate.getMonth()+1, this.viewDate.getFullYear()).subscribe(data => {
      this.diaryEntries = data.ListEntries || [];
      if (clearEvents) {
        this.events = this.events.filter(e => e.meta !== 'diary');
      }
      this.setDiaryEvents();
      this.loadingDiary = false;
      this.setCurrentMonthTasks();
      this.refresh.next();
    });
  }

  getBioscanEntries(clearEvents = false){
    this.util.getBioScanEntries(this.viewDate.getMonth()+1, this.viewDate.getFullYear()).subscribe(data => {
      this.bioscanEntries = data.ListEntries || [];
      this.bioscanEntries.forEach(e => {
        const scanData = JSON.parse(atob(e.Description));
        const scanDataStr = `
          Begrünte Fläche: ${scanData.gardenArea} m<sup>2</sup>, Außenfläche: ${scanData.totalArea} m<sup>2</sup> <br>
          Flächennutzung: (${Math.round(scanData.areaRating)}%) <br>
          Ökokriterien: (${Math.round(scanData.gardenRating)}%) <br>
          Pflanzenvielfalt: (${Math.round(scanData.plantsRating)}%) <br>
        `;
        e.DescriptionStr = scanDataStr;
      });
      if (clearEvents) {
        this.events = this.events.filter(e => e.meta !== 'bioscan');
      }
      this.setBioscanEvents();
      this.loadingScans = false;
      this.setCurrentMonthTasks();
      this.refresh.next();
    });
  }

  getTodos(clearEvents = true) {
    let params = new HttpParams();
    params = params.append('period', 'month');
    params = params.append('startDate', startOfMonth(this.viewDate).toISOString());
    params = params.append('gid', this.selectedUserListId.toString());
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
    this.loadingTodos = true;
    this.events = [];
    console.log(params);
    this.subs.add(this.util.getTodos(null, params).subscribe(todos => {
      this.todos = todos.TodoList || [];
      this.todos.forEach(todo => {
        // todo.Description = todo.Description.replace(/\{k]|\[k]/g, '<i>').replace(/\[\/k\]/g, '</i>');
        todo.Description = todo.Description;

        if (todo.RelatedPlantName != null){
          todo.RelatedPlantName=slugify( todo.RelatedPlantName)

        }
      });
      if (clearEvents) {
        this.events = this.events.filter(e => e.meta === 'diary');
      }
      this.setTodoEvents();
      this.loadingTodos = false;
      this.setCurrentMonthTasks();
      this.refresh.next();
    }, (err) => {
      this.loadingTodos = false;
    }));
  }
getUserInfo(){
  this.subs.add(this.util.getUserInfo().subscribe(userinfo=>{
   
    if(userinfo.ResetTodo==0){
      this.todoIsReset=false
      
    }else if(userinfo.ResetTodo==1){
      this.todoIsReset=true
      
    }

  }))
}
  groupedTasks() {
    const map = new Map();
    if (this.tasksInCurrentMonth) {
      this.tasksInCurrentMonth.forEach((item) => {
        const key = item.Date ? new Date(item.Date).getDate() : new Date(item.DateStart).getDate();
        const collection = map.get(key);
        if (!collection) {
          map.set(key, [item]);
        } else {
          collection.push(item);
        }
      });
    }
    return map;
  }

  setCurrentMonthTasks() {
    this.tasksInCurrentMonth = [];
    if (this.todos) {
      const todos = this.todos.filter(t => new Date(t.DateStart).getMonth() === this.viewDate.getMonth());
      this.tasksInCurrentMonth = this.tasksInCurrentMonth.concat(todos);
    }
    if (this.diaryEntries) {
      const entries = this.diaryEntries.filter(e => new Date(e.Date).getMonth() === this.viewDate.getMonth());
      this.tasksInCurrentMonth = this.tasksInCurrentMonth.concat(entries);
    }
    if(this.bioscanEntries){
      const biscan = this.bioscanEntries.filter(e => new Date(e.Date).getMonth() === this.viewDate.getMonth());
      this.tasksInCurrentMonth = this.tasksInCurrentMonth.concat(biscan);
    }
    this.tasksInCurrentMonth.sort((a,b) => {
      let first, sec;
      if (a.DateStart) {
        first = new Date(a.DateStart);
      } else {
        first = new Date(a.Date);
      }
      if (b.DateStart) {
        sec = new Date(b.DateStart);
      } else {
        sec = new Date(b.Date);
      }
      return first > sec ? 1 : -1;  
    });
    this.cd.markForCheck();
  }

  onMonthChange(date) {
    let newDate= new Date(date)
    console.log(newDate)
    this.viewDate.setMonth(newDate.getMonth()) ;
    console.log(this.viewDate)
   this.viewDate = new Date(
    this.viewDate.getFullYear(),
    this.viewDate.getMonth(),
    this.viewDate.getDate());
    this.closeOpenMonthViewDay();
    
  }
  onYearChange(date) {
    let newDate= new Date(date)
    console.log(newDate)
    this.viewDate.setFullYear(newDate.getFullYear()) ;
    this.viewDate = new Date(
      this.viewDate.getFullYear(),
      this.viewDate.getMonth(),
      this.viewDate.getDate());
    this.closeOpenMonthViewDay();
  }
  closeOpenMonthViewDay() {
    this.activeDayIsOpen = false;
    this.events = [];
    this.todoToView = [];
    this.showTodoMode = false;
    this.getTodos();
    this.getDiaryEntries();
    this.getBioscanEntries();
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  updateTodo(task) {
    if (task.ObjectId) {
      // // task is a diary entry
      this.subs.add(this.util.updateDiaryEntry(task).subscribe(() => {
        this.getDiaryEntries(true);
        this.updateCounter();
      }));
    } else {
      // task is a todo
        this.subs.add(this.util.updateTodo(task.Id,task).subscribe(() => {
          this.getTodos(true);
          this.updateCounter();
        }));
    }
    this.refresh.next();
  }

  markDone(todo) {
    this.subs.add(this.util.markTaskDone(todo.Id, todo.Finished).subscribe(() => {
      this.updateCounter();
    }));
    this.refresh.next();
  }
  updateCounter(){
    this.counter.requestTodosCount().subscribe(data => this.counter.setTodosCount(data));
  }

  selectTodo(todo) {
    this.selectedTodo = todo; 
    todo.showTodoMenuCal = false;
  }
  
  deleteTodo(task, deleteAll) {
    this.loadingTodos=true;
    if (task.ObjectId) {
      // task is a diary entry
      this.subs.add(this.util.deleteDiaryEntry(task.Id).subscribe(() => {
        this.diaryEntries = this.diaryEntries.filter(e => e.Id !== task.Id);
       this.updateCounter();
        this.selectedTodo = null;
        this.setEvents();
        this.setCurrentMonthTasks();
        this.refresh.next();
        this.loadingTodos=false;
        this.alert.success('Todo wurde erfolgreich gelöscht')
      }));
    } else {
      //task is a todo
      if(this.selectedTodo && task.CyclicId == null){
        this.subs.add(this.util.deleteTodo(task.Id,false).subscribe(() => {
          this.todos = this.todos.filter(t => t.Id !== task.Id);
          this.updateCounter();
          this.selectedTodo = null;
            this.setEvents();
            this.setCurrentMonthTasks();
            this.refresh.next();
            this.loadingTodos=false;
            this.alert.success('Todo wurde erfolgreich gelöscht')
          }));
      }else if(this.selectedTodo && task.CyclicId != null){
        if(deleteAll){
          this.subs.add(this.util.deleteTodo(task.Id,true).subscribe(() => {
            this.todos = this.todos.filter(t => t.CyclicId !== task.CyclicId);
            this.updateCounter();
            this.selectedTodo = null;
              this.setEvents();
              this.setCurrentMonthTasks();
              this.refresh.next();
              this.loadingTodos=false;
              this.alert.success('Todos wurden erfolgreich gelöscht')
            }));
        }else{
          this.subs.add(this.util.deleteTodo(task.Id,false).subscribe(() => {
            this.todos = this.todos.filter(t => t.Id !== task.Id);
            this.updateCounter();
            this.selectedTodo = null;
              this.setEvents();
              this.setCurrentMonthTasks();
              this.refresh.next();
              this.loadingTodos=false;
              this.alert.success('Todo wurde erfolgreich gelöscht')
            }));
        }
      }
    }
    this.events = this.events.filter(e => e.id !== task.Id);
    this.todoToView.forEach(t=>{
      if (t.Id === task.Id) {
        this.todoToView = [];
        this.showTodoMode = false;
      }
    })
  }

  setEvents() {
    this.events = [];
    this.setTodoEvents();
    this.setDiaryEvents();
  }

  setTodoEvents() {
    this.todos.forEach((todo: Todo) => {
      const event: CalendarEvent = {
        id: todo.CyclicId? todo.CyclicId : todo.Id,
        meta: todo.ReferenceId? 'todo' : 'own',
        start: startOfDay(todo.DateStart),
        title: todo.Title,
        color: todo.ReferenceId? this.colors.todo : this.colors.ownTodo,
        actions: this.actions
      };
      this.events.push(event);
    });
  }

  setDiaryEvents() {
    this.diaryEntries.forEach((entry: DiaryEntry) => {
      const event: CalendarEvent = {
        id: entry.Id,
        meta: 'diary',
        start: startOfDay(entry.Date),
        title: entry.Title,
        color: this.colors.diary,
        actions: this.actions
      };
      this.events.push(event);
    });
  }

  setBioscanEvents() {
    
    this.bioscanEntries.forEach((entry: DiaryEntry) => {
      const event: CalendarEvent = {
        id: entry.Id,
        meta: 'bioscan',
        start: startOfDay(entry.Date),
        title: entry.Title,
        color: this.colors.bioScan,
        actions: this.actions
      };
      this.events.push(event);
    });
  }

  showTodo(event: CalendarEvent) {
     return this.getTodoById(event.id, event.meta, event.start);
     //
  }

  getTodoById(todoId, meta,eventStart): any {
    if (meta === 'diary') {
      return this.diaryEntries.find(todo => todo.Id === todoId);
    
    } else if(meta === 'bioscan'){
      return this.bioscanEntries.find(todo => todo.Id === todoId);
    }
    
    else {
      return this.todos.find(todo => todo.CyclicId? todo.CyclicId === todoId && new Date(todo.DateStart).getDate()=== eventStart.getDate() : todo.Id===todoId && new Date(todo.DateStart).getDate()=== eventStart.getDate())
    }
  }

  editUrl(task) {
    return task.ObjectId ? '/edit/diary/' + task.Id: '/edit/todo/' + task.Id;
  }

  toUrl(url) {
    return this.util.toUrl(url);
  }
}
