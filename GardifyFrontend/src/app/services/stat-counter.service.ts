import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { AuthService } from './auth.service';
import { tap, map } from 'rxjs/operators';
import { UtilityService } from './utility.service';
const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type':  'application/json'
  })
};

@Injectable({
  providedIn: 'root'
})
export class StatCounterService {
  private todosURL = environment.todosURL;
  private plantsURL = environment.gardenURL;
  private shopCartUrl=environment.shopCartURL;
  private plantDocUrl= environment.plantDocURL;
  private searchQueriesURL=environment.searchQueriesURL
  private todos = new BehaviorSubject({AllTodos: 0, AllTodosOfTheMonth: 0,Open:0,Finished:0});
  todosCount$ = this.todos.asObservable();
  private warnings = new BehaviorSubject(0);
  warningsCount$ = this.warnings.asObservable();
  private plants = new BehaviorSubject({Sorts: 0, Total: 0});
  plantsCount$ = this.plants.asObservable();
  private shopCartEntries= new BehaviorSubject(0);
  shopCartEntriesCount$ = this.shopCartEntries.asObservable();
  private ecoElementsCount = new BehaviorSubject(0);
  ecoCount$ = this.ecoElementsCount.asObservable();
  private bonusPoints = new BehaviorSubject(0);
  bonusPoints$ = this.bonusPoints.asObservable();
  private answers= new BehaviorSubject(0);
  answersCount$= this.answers.asObservable();
  private wishlistEntries = new BehaviorSubject(0) ;
  wishlistEntriesCount$= this.wishlistEntries.asObservable()
  private searchqueries = new BehaviorSubject(0) ;
  searchqueries$= this.searchqueries.asObservable()
  private deviceCount= new BehaviorSubject(0)
  deviceCount$=this.deviceCount.asObservable();

  constructor(private http: HttpClient, private auth: AuthService, private utils: UtilityService) { }




  requestEcoCount():Observable<any>{
    if (this.auth.isLoggedIn()) {
     return this.http.get<number>(`${this.plantsURL}countCheckedEcoEl`)
    .pipe(tap(data => this.setEcoElementsCount(data)));
    }
    return this.ecoCount$;
  }

  setEcoElementsCount(newCount) {
    this.ecoElementsCount.next(newCount);
  }

  requestDevicesCount():Observable<any>{
    if (this.auth.isLoggedIn()) {
     return this.utils.deviceCount().pipe(tap(c=>{this.setDevicesCount(c)}))
    }
    return this.deviceCount$;
  }

  setDevicesCount(number) {
    this.deviceCount.next(number);
  }

  requestTodosCount(): Observable<any> {
    if (this.auth.isLoggedIn()) {
      return this.http.get<any>(`${this.todosURL}count`, {params: new HttpParams().set('period', 'month')})
                      .pipe(tap(data => this.setTodosCount(data)));
    }
    return this.todosCount$;
  }

  setTodosCount(data) {
    this.todos.next(data);
  } 
  requestSearchqueries(): Observable<any> {
    return this.http.get<any>(`${this.searchQueriesURL}`);
  }

  setSearchqueries(data) {
    this.searchqueries.next(data);
  } 
  requestPlantsCount(): Observable<any> {
    if (this.auth.isLoggedIn()) {
      return this.http.get<number>(`${this.plantsURL}count`)
                      .pipe(tap(data => this.setPlantsCount(data)));
    }
    return this.plantsCount$;
  }
  setPlantsCount(data) {
    this.plants.next(data);
  }
  requestAnswersCount(){
    if (this.auth.isLoggedIn()) {
      return this.http.get<number>(`${this.plantDocUrl}/count`)
                      .pipe(tap(data => this.setAnswersCount(data)));
    }
    return this.answersCount$;
  }
  setAnswersCount(data){
    this.answers.next(data);
  }
  requestShopCartCounter():Observable<any>{
    if (this.auth.isLoggedIn()) {
    return this.http.get<number>(`${this.shopCartUrl}shopCartCount`)
                    .pipe(tap(data=> this.setShopCartCounter(data)))
    }
    return this.shopCartEntriesCount$;
  }
  setShopCartCounter(data){
    this.shopCartEntries.next(data)
  }
  requestWishListEntriesCounter():Observable<any>{
    if(this.auth.isLoggedIn()){
      return this.http.get<number>(`${this.shopCartUrl}wishlistEntriesCount`)
                      .pipe(tap(data=>this.setWishlistEntriesCount(data)))
    }
  }
  setWishlistEntriesCount(data){
    this.wishlistEntries.next(data)
  }
  setBonusCounter(p) {
    this.bonusPoints.next(p);
  }

  requestBonusCounter(): Observable<any> {
    if (this.auth.isLoggedIn()) {
      return this.http.get<any>(`${environment.pointsURL}`)
                    .pipe(tap(data=> this.setBonusCounter(data.Points)),
                      map(data => data.Points));
    }
    return this.bonusPoints$;
  }

  requestWarningsCount(): Observable<any> {
     if (this.auth.isLoggedIn()) {
       return this.utils.getwarning().pipe(tap(data=> this.setWarningsCount(data.filter(w => !w.Dismissed).length)));
     }
    return this.warningsCount$;
  }
  setWarningsCount(count) {
    this.warnings.next(count);
  }

  createStatEntry(type: 'teamview' | 'adclick', apiCallFrom:any): Observable<any> {
    return this.http.get<any>(`${environment.userURL}/${type}`,apiCallFrom);
  }
}
