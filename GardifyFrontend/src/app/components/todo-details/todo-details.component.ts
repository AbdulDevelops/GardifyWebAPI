import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UtilityService } from 'src/app/services/utility.service';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { Todo } from 'src/app/models/models';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-todo-details',
  templateUrl: './todo-details.component.html',
  styleUrls: ['./todo-details.component.css']
})
export class TodoDetailsComponent implements OnInit,OnDestroy {
  todo: any;
  mode: string;
  loading:boolean;
  selectedTodo: Todo;
  subs= new Subscription();
  constructor(
    private tp: ThemeProviderService,
    private activatedRoute: ActivatedRoute, 
    private util: UtilityService) { }
  ngOnDestroy(): void {
   this.subs.unsubscribe()
  }

  ngOnInit() {
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
    this.getTodoById();
    //this.util.getAffiliateArticles().subscribe();
  }
  getTodoById(){
    this.subs.add(this.activatedRoute.params.subscribe(route => {
      this.util.getCyclicTodo(route.id).subscribe(todo => {
        this.todo = todo;
      });
    }));
  }
  updateTodo(todo){
    this.loading=true
    this.subs.add(this.util.updateCyclicTodo(todo.Id,todo).subscribe(() => {
      this.loading=false
    }))
  }
  todoUrl() {
    let url = '';
    if (this.todo.EntryImages.length>0) {
      url = this.todo.EntryImages[0].SrcAttr || null;
    }
    return this.util.toUrl(url);
  }

}
