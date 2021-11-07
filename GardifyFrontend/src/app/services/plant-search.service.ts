import { Injectable } from '@angular/core';
import { Observable, timer, of, Subscription } from 'rxjs';
import { Group, Plant, PlantSearch, ScanResult, UserActionsFrom } from '../models/models';
import { HttpParams, HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { map, catchError, switchMap, shareReplay } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type':  'application/json'
  })
};

@Injectable({
  providedIn: 'root'
})
export class PlantSearchService {
  private groupURL = environment.groupURL;
  private familyURL = environment.familyURL;
  private tagCatsURL = environment.tagCatsURL;
  private tagsURL = environment.tagURL;
  private searchURL = environment.searchURL;
  private searchSiblingURL=environment.searchSiblingURL;
  private scanURL = environment.scanURL;
  private scanHistoryURL = environment.scanHistoryURL;
  
  private REFRESH_INTERVAL = 1000*60*30; // every 30 mins
  private groupsCache: Observable<Group[]>;
  private familiesCache: Observable<string[]>;
  private tagsCache: Observable<any[]>;
  private catsCache: Observable<any[]>;
  private subscriptions = new Subscription();
  categories: any[];
  private previousSearch: any;
  apiCallFrom=new UserActionsFrom();

  constructor(private http: HttpClient) { }

  getPlantsCount(params: HttpParams): Observable<any> {
    params = params.delete('take'); params = params.delete('skip');
    return this.http.get(`${this.searchURL}count`, {params: params});
  }

  setPrevious(search) {
    this.previousSearch = search;
  }

  getPrevious() {
    return this.previousSearch;
  }

  getLatestPlants(month: number, year: string): Observable<any[]>  {
    return this.http.get<any[]>(`${this.searchURL}latest/${year}/${month}`, httpOptions);
  }

  // PLANT SEARCH
  GetPlantEntry(id:number): Observable<any> {
    return this.http.get(`${this.searchURL}${id}`, httpOptions);
  }
  //Plant siblings
  GetPlantSiblings(id:number):Observable<any>{
    return this.http.get(`${this.searchSiblingURL}${id}`, httpOptions);
  }
  //Plant-Search-characteristic
  GetPlantTag(id:number):Observable<any> {
    return this.http.get(`${this.tagsURL}${id}`,httpOptions);
  }

  getSearchResults(params: HttpParams): Observable<any> {
    return this.http.get<PlantSearch>(`${this.searchURL}`, {params: params})
                      .pipe(catchError(this.handleErrors));
  }

  // fetch groups from cache if available
  getGroups(): Observable<any> {
    if (!this.groupsCache) {
      const timer$ = timer(0, this.REFRESH_INTERVAL);
      this.groupsCache = timer$.pipe(
        switchMap(_ => this.requestGroups(this.apiCallFrom)),
        shareReplay(1)
      );
    }
    return this.groupsCache;
  }

  private requestGroups(apiCallFrom:any): Observable<any> {
    return this.http.get<any>(`${this.groupURL}`,apiCallFrom);
  }

  getFamilies(): Observable<string[]> {
    if (!this.familiesCache) {
      const timer$ = timer(0, this.REFRESH_INTERVAL);
      this.familiesCache = timer$.pipe(
        switchMap(_ => this.requestFamilies()),
        shareReplay(1)
      );
    }
    return this.familiesCache;
  }

  private requestFamilies(): Observable<string[]> {
    return this.http.get<string[]>(`${this.familyURL}`);
  }

  getPlantTags(): Observable<any[]> {
    if (!this.tagsCache) {
      const timer$ = timer(0, this.REFRESH_INTERVAL);
      this.tagsCache = timer$.pipe(
        switchMap(_ => this.requestPlantTags()),
        shareReplay(1)
      );
    }
    return this.tagsCache;
  }

  private requestPlantTags(): Observable<Group[]> {
    return this.http.get<Group[]>(`${this.tagsURL}`);
  }

  getTagCats(): Observable<any[]> {
    if (!this.catsCache) {
      const timer$ = timer(0, this.REFRESH_INTERVAL);
      this.catsCache = timer$.pipe(
        switchMap(_ => this.requestTagCats()),
        shareReplay(1)
      );
    }
    return this.catsCache;
  }

  private requestTagCats(): Observable<Group[]> {
    return this.http.get<Group[]>(`${this.tagCatsURL}`);
  }

  // Plant Scan
  scanImage(img: any): Observable<ScanResult> {
    return this.http.post<ScanResult>(`${this.scanURL}`, img);
  }

  getScanHistory(): Observable<any> {
    return this.http.get(`${this.scanHistoryURL}`);
  }

  private handleErrors(error: HttpErrorResponse) {
    // return an observable with a user-facing error message
    // return throwError('Etwas ist schief gelaufen. Code: ' + error.status);
    return of<any>({err: 'Etwas ist schief gelaufen. Code: ' + error.status});
  }
  
}

export const slugify = (s: string) => {
  if (!s) {
    return s;
  }
  s = s.replace(/ö/g, 'oe').replace(/ä/g, 'ae').replace(/ü/g, 'ue').replace(/ß/g, 'ss'); // remove umlaute
  s = s.replace(/[^-\w\s]/g, ''); // remove unneeded characters
  s = s.replace(/^\s+|\s+$/g, ''); // trim leading/trailing spaces
  s = s.replace(/[-\s]+/g, '-'); // convert spaces to hyphens
  s = s.toLowerCase(); // convert to lowercase 
  return s;
};
