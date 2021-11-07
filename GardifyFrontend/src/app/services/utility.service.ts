import { Injectable, Output } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { NewsLetter, ObjectType, DiaryEntry, UserDevices, PlaceHolder, UploadedImageResponse, UserSettings, ReferenceToModelClass, StatisticEventTypes, UserActionsFrom, GardenAlbumFileToModuleViewModel } from '../models/models';
import { environment } from 'src/environments/environment';
import { map, tap } from 'rxjs/operators';
import { Location } from '@angular/common';
import { data, param } from 'jquery';
import { ObserversModule } from '@angular/cdk/observers';

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json'
  })
};

@Injectable({
  providedIn: 'root'
})
export class UtilityService {
  isMobile = window.matchMedia('only screen and (max-width: 760px)').matches;
  gardensCache: any;
  private previousPage = 1;
  private ecoSearch;
  private frostSearch;
  public shopSearchCache = {searchText: null, category: null};
  private detailSearch;
  private paymentResponse = new BehaviorSubject(null);
  paymentResponse$ = this.paymentResponse.asObservable();
  private showCart = new BehaviorSubject(false);
  showCart$ = this.showCart.asObservable();
  private tourStep = new BehaviorSubject(0);
  tourStep$ = this.tourStep.asObservable();
  public selectedUserPlant= new BehaviorSubject(null)
  selectedUserPlant$=this.selectedUserPlant.asObservable()
  @Output() presentations: Observable<any>;
  @Output() albums: Observable<any>;


  model=new UserActionsFrom();
  constructor(private http: HttpClient, private location: Location) {
    this.albums= new Observable(observer=>{
      this.getUserAlbums().subscribe(albums=>{
        observer.next(albums)
      })
    })
    this.presentations= new Observable(obsever=>{
      this.getUserPresentations().subscribe((presentations)=>{
        obsever.next(presentations)
      })
    })
   }

  updateSelectedPlant(data){
    this.selectedUserPlant.next(data)
    this.selectedUserPlant$.subscribe(d=>{
      localStorage.setItem('CurrentPlant',JSON.stringify (d));
     
    })
   
  }
  
  startTour() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
    this.tourStep.next(1);
    return this.http.post(`${environment.statsURL}`, StatisticEventTypes.GuidedTour);
  }
  startTourOnSocialPage(step) {
   
    this.tourStep.next(step);
    return this.http.post(`${environment.statsURL}`, StatisticEventTypes.GuidedTour);
  }
  endTour() {
    this.tourStep.next(0);
  }

  sendGAEvent = (category: string, action: string, label: string = null, value: number = 1) => {
    (<any>window).gtag('event', action, {
      'event_category': category,
      'event_label': label,
      'value': value
    });
  }

  sendFBQEvent = (category: string, action: string) => {
    (<any>window).fbq(category, action, {value: 1});
  }

  setGAUserId = (userId: string) => {
    (<any>window).gtag('config', 'UA-70030524-12', {
      'user_id': userId
    });
  }

  toggleCart(state) {
    this.showCart.next(state);
  }

  setPaymentResponse(newRes) {
    this.paymentResponse.next(newRes);
  }

  setPreviousPage(page: number) {
    this.previousPage = page;
  }

  setOpenFilters(ecoSearch: boolean, frostSearch: boolean, detailSearch: boolean) {
    this.ecoSearch = ecoSearch;
    this.frostSearch = frostSearch;
    this.detailSearch = detailSearch;
  }

  getOpenFilters(): { ecoSearch, frostSearch, detailSearch } {
    return { ecoSearch: this.ecoSearch, frostSearch: this.frostSearch, detailSearch: this.detailSearch };
  }

  getPreviousPage() {
    return this.previousPage;
  }

  goBack() {
    this.location.back();
  }

  toUrl(srcUrl: string, small = true, autoscale = false, isadminDevice = false,isAdminImg=false): string {
    if (!srcUrl || srcUrl.includes('Platzhalter')) {
      return PlaceHolder.Img;
    }
    let baseUrl; let adminArea = false;
    if (isadminDevice == true) {
      if (srcUrl.includes('DeviceImages')||srcUrl.includes('PlantDocAnswersImages')) {
        baseUrl = environment.production ? environment.gardifyBaseUrl : 'https://localhost:10142/';
        adminArea = true;
      }
    } else {
      if (srcUrl.includes('PlantImages') || srcUrl.includes('ArticleImages') || srcUrl.includes('EventsImages') || srcUrl.includes('NewsImages') || srcUrl.includes('LexiconTermImages') || srcUrl.includes('EcoElementImages')) {
        baseUrl = environment.production ? environment.gardifyBaseUrl : 'https://localhost:10142/';
        adminArea = true;
      } else {
        baseUrl = environment.production ? environment.gardifyBaseUrl : 'https://localhost:44328/';
      }
    }

    if (srcUrl && srcUrl.includes('nfiles')) {
      if (adminArea || isAdminImg) {
        const head = srcUrl.substring(0, srcUrl.lastIndexOf('/'));
        // TODO: handle user plants urls
        const size = small ? '/250' : '';
        return environment.gpBaseUrl + head + size + srcUrl.substring(srcUrl.lastIndexOf('/'));
      }
      if (autoscale) {
        const head = srcUrl.substring(1, srcUrl.lastIndexOf('/'));
        return baseUrl + head + '/ImgMed' + srcUrl.substring(srcUrl.lastIndexOf('/'));
      }
      return baseUrl + srcUrl.substring(srcUrl.indexOf('nfiles'));
    }
  }

  getVideos(params): Observable<any> {
    return this.http.get<any>(`${environment.videosURL}`,{ params: params});
  }

  getVideoTopics(): Observable<any> {
    return this.http.get(`${environment.videosURL}topics`);
  }

  getUserInfo(): Observable<any> {
    return this.http.get(`${environment.userURL}/userinfo/`);
  }
  resetTodo():Observable<any>{
    return this.http.put(`${environment.todosURL}resetTodos`, httpOptions);
  }
  updateUserData(data): Observable<any> {
    return this.http.put(`${environment.userURL}data`, data, httpOptions);
  }

  getCityDetails(): Observable<any>{
    return this.http.get(`${environment.citiesURL}GetCityDetails`);
  }

  addNewSuggestPlant(data): Observable<any> {
    return this.http.post(`${environment.userURL}/addPlant`, data);
  }


  submitOrder(order: any,params): Observable<any> {
    return this.http.post(`${environment.ordersUrl}submit/`, order,{params:params});
  }

  submitOrderWithInvoice(orderId, params:any): Observable<any> {
    return this.http.get(`${environment.ordersUrl}paywithinvoice/${orderId}`,{params:params});
  }

  getOrderStatus(orderId: number): Observable<any> {
    return this.http.get(`${environment.ordersUrl}${orderId}`);
  }

  GetFormData(orderId): Observable<any> {
    return this.http.get(`${environment.ordersUrl}checkout/${orderId}`);
  }

  updateOrder(order: {OrderId: number, PaidWith: string}): Observable<any> {
    return this.http.put(`${environment.ordersUrl}update/${order.OrderId}`, order);
  }

  getOrders(): Observable<any> {
    return this.http.get(`${environment.ordersUrl}`);
  }

  contactUs(form: any): Observable<any> {
    return this.http.post(`${environment.userURL}/contact/`, form);
  }

  dismissWarning(relatedObjId: number,objectType): Observable<any> {
    return this.http.get(`${environment.warningUrl}/dismiss/${relatedObjId}/${objectType}`);
  }

  hideWarning(warningId: number): Observable<any> {
    return this.http.delete(`${environment.warningUrl}/hide/${warningId}`);
  }

  addNotificationSub(subJson: PushSubscriptionJSON): Observable<any> {
    const sub = {
      EndPoint: subJson.endpoint,
      Auth: subJson.keys.auth,
      P256dh: subJson.keys.p256dh
    };
    return this.http.post(`${environment.warningUrl}/pushsub`, sub);
  }

  unSubNotifications(): Observable<any> {
    return this.http.delete(`${environment.warningUrl}/pushsub`);
  }

  getUserSettings(params:any): Observable<any> {
    return this.http.get<any>(`${environment.userURL}/settings`,{params:params});
  }

  updateUserSettings(settings: UserSettings): Observable<any> {
    return this.http.put(`${environment.userURL}/updatesettings`, settings);
  }
  uploadProfilImg(data): Observable<any> {
    return this.http.post(`${environment.userURL}/uploadProfilImg`, data);
  }
  getUserProfilImg(): Observable<any> {
    return this.http.get(`${environment.userURL}/profilImg`);
  }
  suggestPlant(data, params): Observable<any> {
    return this.http.post(`${environment.userURL}/suggest`, data, {params:params});
  }

  sendEmail(data): Observable<any> {
    return this.http.post(`${environment.userURL}/sendScanMail`, data);
  }

  // Events
  getEvents(): Observable<any> {
    return this.http.get(`${environment.eventsUrl}`);
  }

  getEvent(id): Observable<any> {
    return this.http.get(`${environment.eventsUrl}${id}`);
  }

  //FrostWarning
  getminTemp(): Observable<any> {
    return this.http.get(`${environment.warningUrl}/minTemp`);
  }

  getwarning(): Observable<any> {
    return this.http.get(`${environment.warningUrl}/warnings`);
  }

  resertWarnings(): Observable<any> {
    return this.http.get(`${environment.warningUrl}/reset`);
  }

  togglePlantsWarnings(forFrost: boolean, newState: boolean): Observable<any> {
    return this.http.get(`${environment.warningUrl}/togglePlants/${forFrost}/${newState}`);
  }

  toggleDevicesWarnings(forFrost: boolean, newState: boolean): Observable<any> {
    return this.http.get(`${environment.warningUrl}/toggleDevices/${forFrost}/${newState}`);
  }

  // WEATHER
  getForecast(longitude: any, latitude: any): Observable<any> {
    return this.http.get(`${environment.forecastURL}forecast`);
  }

  getCurrentForecast(): Observable<any> {
    return this.http.get(`${environment.forecastURL}current`);
  }

  getOnlyTodayForecast(longitude: any, latitude: any): Observable<any> {
    return this.http.get(`${environment.forecastURL}todayForecast`);
  }

  getOnlyTomorrowForecast(longitude: any, latitude: any): Observable<any> {
    return this.http.get(`${environment.forecastURL}tomorrowForecast`);
  }

  getDailyForecast(longitude: any, latitude: any,params): Observable<any> {
    return this.http.get<any>(`${environment.fcDailyURL}daily`,{params:params});
  }

  getSunsetAndSunrise(longitude: any, latitude: any): Observable<any> {
    return this.http.get(`${environment.fcDailyURL}sun`);
  }

  // TODOS
  addTodo(todoBody: any): Observable<any> {
    return this.http.post(environment.todosURL, todoBody, httpOptions);
  }

  updateTodo(todoId: number, todoBody: any): Observable<any> {
    return this.http.put(`${environment.todosURL}${todoId}/uploadTodo`, todoBody, httpOptions);
  }

  markTaskDone(taskId: number, newState: boolean,cyclicId=0): Observable<any> {
    return this.http.put(`${environment.todosURL}markfinished/${newState}/${taskId}/${cyclicId}`, httpOptions);
  }
  markCyclicTaskDone(id: number, gid: number): Observable<any> {
    return this.http.put(`${environment.todosURL}cyclicToggle/${id}/${gid}`, httpOptions);
  }
  updateCyclicTodo(todoId: number, todoBody: any): Observable<any> {
    return this.http.put(`${environment.todosURL}${todoId}/updateCyclic`, todoBody, httpOptions);
  }

  uploadTodoImg(data): Observable<any> {
    return this.http.post(`${environment.todosURL}upload`, data);
  }

  deleteTodo(todoId: number, cyclic: boolean): Observable<any> {
    return this.http.delete(`${environment.todosURL}${todoId}/${cyclic}`, httpOptions);
  }

  getTodo(todoId: number): Observable<any> {
    return this.http.get(`${environment.todosURL}${todoId}`, httpOptions);
  }

  getCyclicTodo(todoId: number): Observable<any> {
    return this.http.get(`${environment.todosURL}cyclic/${todoId}`, httpOptions);
  }

  getTodos(apiCallFrom:any,params?): Observable<any> {
    if (!params) {
      params = new HttpParams().set('period', 'month');
    }
    return this.http.get<any>(`${environment.todosURL}`, { params: params});
  }

  addUserPlantToProp(up): Observable<any> {
    return this.http.post(`${environment.userPlantsURL}prop`, up, httpOptions);
  }
  moveUserplantToAnotherList(model):Observable<any>{
    return this.http.post(`${environment.userPlantsURL}movePlant`,model,httpOptions)
  }
  moveAllUserplantsToAnotherList(model):Observable<any>{
    return this.http.post(`${environment.userPlantsURL}moveAllPlants`,model,httpOptions)
  }
  // THREADS AND POSTS
  getThreads(): Observable<any> {
    return this.http.get(`${environment.threadsURL}`, httpOptions);
  }

  getThread(threadId: number): Observable<any> {
    return this.http.get(`${environment.threadsURL}${threadId}`, httpOptions);
  }

  deleteThread(threadId: number): Observable<any> {
    return this.http.delete(`${environment.threadsURL}${threadId}`, httpOptions);
  }

  updateThread(threadId: number): Observable<any> {
    return this.http.put(`${environment.threadsURL}${threadId}`, httpOptions);
  }

  addThread(threadBody: string): Observable<any> {
    return this.http.post(environment.threadsURL, threadBody, httpOptions);
  }

  // FAQ
  getFAQEntries(): Observable<any> {
    return this.http.get(`${environment.faqURL}`, httpOptions);
  }

  GetFaqEntry(id: number): Observable<any> {
    return this.http.get(`${environment.faqURL}${id}`, httpOptions);
  }

  addFaqEntry(entry): Observable<any> {
    return this.http.post(environment.faqURL, entry, httpOptions);
  }

  addFaqAnswer(answer,apiCallFrom:any): Observable<any> {
    return this.http.post(`${environment.faqURL}answer`, answer, apiCallFrom);
  }

  //plantDoc
  addNewQuestion(entry,params:any): Observable<any> {
    return this.http.post(`${environment.plantDocURL}/newEntry`, entry, {params:params});
  }

  uploadQuestImg(data): Observable<any> {
    return this.http.post(`${environment.plantDocURL}/upload`, data);
  }

  uploadAnswerImg(data): Observable<any> {
    return this.http.post(`${environment.plantDocURL}/uploadAnswerImg`, data);
  }

  getAllEntries(params:any): Observable<any> {
    return this.http.get(`${environment.plantDocURL}/getAllEntry`, {params:params});
  }

  getEntryById(id: number): Observable<any> {
    return this.http.get(`${environment.plantDocURL}/${id}/getEntry`, httpOptions);
  }

  postAnswer(data: any): Observable<any> {
    return this.http.post(`${environment.plantDocURL}/answer`, data, httpOptions);
  }

  getTotalAnswerNumber(): Observable<any> {
    return this.http.get(`${environment.plantDocURL}/count`, httpOptions);
  }

  getCurrentUserPost(): Observable<any> {
    return this.http.get(`${environment.plantDocURL}/getCurrentUserPosts`, httpOptions);
  }

  updatePost(postId: number, postBody: any): Observable<any> {
    return this.http.put(`${environment.plantDocURL}/${postId}/updatePost`, postBody, httpOptions);
  }

  updateAnswer(answerId: number, answerBody: any): Observable<any> {
    return this.http.put(`${environment.plantDocURL}/${answerId}/updateAnswer`, answerBody, httpOptions);
  }
  getPostById(id:number):Observable<any>{
    return this.http.get(`${environment.plantDocURL}/${id}/getPostById`, httpOptions)
  }
  getAnswerById(id:number):Observable<any>{
    return this.http.get(`${environment.plantDocURL}/${id}/getAnswerById`, httpOptions)
  }

  // Community 

  getAllCommunityEntries(params:any): Observable<any> {
    return this.http.get(`${environment.communityURL}/getAllEntry`, {params:params});
  }

  // NEWS
  getAllNews(params:any): Observable<any> {
    return this.http.get(`${environment.newsURL}`,{params:params});
  }
// InstaEntries
  getInstaEntries(params:any): Observable<any> {
    return this.http.get(`${environment.newsURL}/getInstaPost`, {params:params});
  }
  getInstaImages(imageId:any):Observable<any>{
    return this.http.get(`${environment.newsURL}/getInstaImage/${imageId}`,httpOptions)
  }
  getNewsArticle(id: number): Observable<any> {
    return this.http.get(`${environment.newsURL}${id}`, httpOptions);
  }

  uploadArticleImg(data): Observable<any> {
    return this.http.post(`${environment.newsURL}upload`, data);
  }

  // PLANTS AND GARDENS
  getGardenCoords(): Observable<any> {
    return this.http.get(`${environment.gardenURL}coords`, httpOptions);
  }

  getGardenLocation(): Observable<any> {
    return this.http.get(`${environment.gardenURL}location`, httpOptions);
  }
  updateGardenLocation(data): Observable<any> {
    return this.http.put(`${environment.gardenURL}location`, data, httpOptions);
  }

  getGarden(id: number): Observable<any> {
    return this.http.get(`${environment.gardenURL}${id}`, httpOptions);
  }

  getUserMainGarden(params:any): Observable<any> {
    return this.http.get(`${environment.gardenURL}main`, {params:params});
  }

  getUserMainGardenRating(params:any): Observable<any> {
    return this.http.get(`${environment.gardenURL}ratingTotalEcoEl`, {params:params});
  }

  getGardensLite(): Observable<any> {
    return this.http.get(`${environment.gardenURL}`, httpOptions);
  }

  getGardens(): Observable<any> {
    return this.http.get(`${environment.gardenURL}details`, httpOptions);
  }

  uploadGardenImg(data): Observable<any> {
    return this.http.post(`${environment.gardenURL}upload`, data);
  }

  deleteGardenImg(id): Observable<any> {
    return this.http.delete(`${environment.gardenURL}deleteimg/${id}`, httpOptions);
  }

  updateGardenSort(data): Observable<any> {
    return this.http.put(`${environment.gardenURL}sort`, data, httpOptions);
  }

  updateGarden(garden: any): Observable<any> {
    return this.http.put(`${environment.gardenURL}${garden.Id}`, garden, httpOptions);
  }

  deleteGarden(id: number): Observable<any> {
    return this.http.delete(`${environment.gardenURL}${id}`, httpOptions);
  }

  addGarden(garden: any): Observable<any> {
    return this.http.post(`${environment.gardenURL}`, garden, httpOptions);
  }

  getGardenPlant(id: number): Observable<any> {
    return this.http.get(`${environment.gardenURL}${id}`, httpOptions);
  }

  // User Lists
  getUserLists(): Observable<any> {
    return this.http.get<any>(`${environment.userListsURL}`, httpOptions);
  }

  getPlantLists(plantId): Observable<any> {
    return this.http.get<any>(`${environment.userListsURL}plantlists/${plantId}`, httpOptions);
  }

  createUserList(list: any,params): Observable<any> {
    return this.http.post<any>(`${environment.userListsURL}create`, list,{params:params});
  }

  updatePlantInUserList(list: any): Observable<any> {
    return this.http.put<any>(`${environment.userListsURL}update/${list.Id}`, list, httpOptions);
  }

  updateUserList(list: any): Observable<any> {
    return this.http.put<any>(`${environment.userListsURL}updatelist`, list, httpOptions);
  }

  deleteUserList(gardenId: number, listId: number): Observable<any> {
    return this.http.delete<any>(`${environment.userListsURL}${gardenId}/${listId}`, httpOptions);
  }

  getUserList(gardenId: number, listId: number): Observable<any> {
    return this.http.get<any>(`${environment.userListsURL}${gardenId}/${listId}`, httpOptions);
  }

  // Ecolist
  getUserEcoList(): Observable<any> {
    return this.http.get<any>(`${environment.ecolistUrl}ecoelements`, httpOptions);
  }

  updateEcoElement(elem): Observable<any> {
    return this.http.put<any>(`${environment.ecolistUrl}updateEcoelements`, elem, httpOptions);
  }

  // User Plants
  getUserPlants(): Observable<any> {
    return this.http.get<any>(`${environment.userPlantsURL}`, httpOptions);
  }

  getUserPlantByUserListId(listId: number): Observable<any> {
    return this.http.get<any>(`${environment.userPlantsURL}UserPlantByUserListId/${listId}`, httpOptions);
  }

  addUserPlant(plant): Observable<any> {
    return this.http.post(environment.userPlantsURL, plant, httpOptions);
  }

  postUserPlantTrigger(data: any[]): Observable<any> {
    return this.http.post(`${environment.userPlantsURL}userPlantToUserList`, data, httpOptions);
  }

  updateUserPlant(plant, editCount:boolean): Observable<any> {
    return this.http.put(`${environment.userPlantsURL}${editCount}`, plant, httpOptions);
  }
  updateWarningPlantNotification(id: number, forFrost: boolean): Observable<any> {
    return this.http.get(`${environment.userPlantsURL}updateUserPlantNotification/${id}/${forFrost}`, httpOptions);
  }
  uploadUserPlantImg(data): Observable<any> {
    return this.http.post(`${environment.userPlantsURL}upload`, data);
  }

  deleteUserPlant(id, gardenId): Observable<any> {
    return this.http.delete(`${environment.userPlantsURL}${id}/${gardenId}`, httpOptions);
  }

  deleteUserPlantFromUserList(userPlantId, userListId): Observable<any> {
    return this.http.delete(`${environment.userPlantsURL}deleteUserPlantFromUserList/${userPlantId}/${userListId}`, httpOptions);
  }
  deleteUserPlantFromAllUserlists(userPlantId, gardenId): Observable<any> {
    return this.http.delete(`${environment.userPlantsURL}deleteUserPlantFromAllUserList/${userPlantId}/${gardenId}`, httpOptions);
  }
  userPlantsfilter(params: HttpParams): Observable<any> {
    return this.http.get(`${environment.userPlantsURL}filterPlant`, { params: params });
  }

  UserPlantsRating(): Observable<any> {
    return this.http.get<any>(`${environment.userPlantsURL}ratingPlant`, httpOptions);
  }

  UserPlantsFlowerDurationChart(): Observable<any> {
    return this.http.get<any>(`${environment.userPlantsURL}durationRatingPlant`, httpOptions);
  }

  // DIARY
  getDiaryEntries(month, year): Observable<any> {
    return this.http.get(`${environment.diaryURL}?m=${month}&y=${year}`, httpOptions);
  }

  getBioScanEntries(month, year): Observable<any> {
    return this.http.get(`${environment.diaryURL}/BioScan?m=${month}&y=${year}`, httpOptions);
  }

  getDiaryEntry(entryId: number): Observable<any> {
    return this.http.get(`${environment.diaryURL}/${entryId}`, httpOptions);
  }

  createDiaryEntry(body: DiaryEntry, params): Observable<any> {
    return this.http.post(`${environment.diaryURL}`, body,{params:params});
  }

  updateDiaryEntry(body: DiaryEntry) {
    return this.http.put(`${environment.diaryURL}/${body.Id}`, body, httpOptions);
  }

  uploadDiaryImg(data): Observable<any> {
    return this.http.post(`${environment.diaryURL}/upload`, data);
  }

  deleteDiaryEntry(id: number) {
    return this.http.delete(`${environment.diaryURL}/${id}`, httpOptions);
  }

  // PREMIUM CONTENT
  getUserPoints(userId: string): Observable<any> {
    return this.http.get(`${environment.pointsURL}${userId}`, httpOptions);
  }

  getEarnedPoints(userId: string): Observable<any> {
    return this.http.get(`${environment.pointsURL}history/${userId}`, httpOptions);
  }

  getspentPoints(userId: string): Observable<any> {
    return this.http.get(`${environment.pointsURL}historyspent/${userId}`, httpOptions);
  }

  // VON A-Z
  getLexicon(params:any): Observable<any> {
    return this.http.get(`${environment.lexiconURL}`,{params:params});
  }

  //NewsLetter
  newsLetterregs(regs: NewsLetter): Observable<NewsLetter> {
    const body = JSON.stringify(regs);
    return this.http.post<NewsLetter>(`${environment.newsletterURL}`, body, httpOptions);
  }

  unsubscribe(): Observable<any> {
    return this.http.post<any>(`${environment.newsletterURL}/unsubscribe`, httpOptions);
  }

  emailConfirm(userId: string, code: any, email: any): Observable<any> {
    return this.http.get<any>(`${environment.newsletterURL}/confirm?userId=${userId}&code=${code}&email=${email}`, httpOptions);
  }

  //shop
  getArticles(skip: number, take: number = 8, params): Observable<any> {
    return this.http.get(`${environment.articleURL}/${skip}/${take}`,{params:params});
  }

  getArticlesCount(type: string = null, catId: number = 0, searchText: string = null): Observable<any> {
    return this.http.get(`${environment.articleURL}count/${type}/${catId}/${searchText}`, httpOptions);
  }

  getSortedArticlesByPrice(sortType: string, skip: number): Observable<any> {
    return this.http.get(`${environment.articleURL}SortedArticlesByPrice/${skip}`, { params: new HttpParams().set('sortType', sortType) });
  }

  getArticlesByCategory(id: number, skip: number, giftCat: boolean): Observable<any> {
    return this.http.get(`${environment.articleURL}category/${id}/${skip}/${giftCat}`, httpOptions);
  }

  getArticleCategories(): Observable<any> {
    return this.http.get(`${environment.articleURL}categories`, httpOptions);
  }

  getArticle(id: number): Observable<any> {
    return this.http.get(`${environment.articleURL}articleById/${id}`, httpOptions);
  }

  getArticleSearchText(searchText: string, skip: number): Observable<any> {
    return this.http.get(`${environment.articleURL}search/${skip}`, { params: new HttpParams().set('searchText', searchText) });
  }

  getLastPurchases() {
    return this.http.get(`${environment.articleURL}purchases`, httpOptions);
  }

  getLastViewedArticles() {
    return this.http.get(`${environment.articleURL}LastViewedArticles`, httpOptions);
  }

  createLastViewedArt(articleId: number) {
    return this.http.post(`${environment.articleURL}createLastViewedArt/${articleId}`, httpOptions);
  }

  getArticlesViewedByOthersUsers(id: number) {
    return this.http.get(`${environment.articleURL}viewedby/${id}`, httpOptions);
  }

  //ShopCart
  addToCart(articleId: number, fromWishlist = false,params): Observable<any> {
    return this.http.post(`${environment.shopCartURL}addToShopCart/${articleId}/${fromWishlist}`, {params:params},httpOptions);
  }

  addToWishlist(articleId: number): Observable<any> {
    return this.http.post(`${environment.shopCartURL}addToWishlist/${articleId}`, httpOptions);
  }

  getAffiliateArticles() {
    return this.http.get(`${environment.articleURL}affiliate`, httpOptions);
  }

  getShopEntries(): Observable<any> {
    return this.http.get(`${environment.shopCartURL}shopCartEntries`, httpOptions);
  }

  deleteShopCartEntry(articleId: number) {
    return this.http.delete(`${environment.shopCartURL}deleteEntry/${articleId}`, httpOptions);
  }

  moveToWishlist(itemId): Observable<any> {
    return this.http.put(`${environment.shopCartURL}shopCartEntries/wish/${itemId}`, httpOptions);
  }

  changeQuantity(articleId: number, increase: boolean, decrease: boolean): Observable<any> {
    return this.http.post(`${environment.shopCartURL}changeQuantity/${articleId}/${increase}/${decrease}`, httpOptions);
  }

  //Devices
  getAdminDevices(): Observable<any> {
    return this.http.get(`${environment.devicesURL}AdminDevices`, httpOptions);
  }

  getUserDevices(): Observable<any> {
    return this.http.get(`${environment.devicesURL}`, httpOptions);
  }

  postDevices(device: UserDevices): Observable<UserDevices> {
    const body = JSON.stringify(device);
    return this.http.post<UserDevices>(`${environment.devicesURL}postDevice`, body, httpOptions);
  }

  postAdminDevices(arrayOfId: number[]): Observable<any> {
    return this.http.post<any>(`${environment.devicesURL}postAdminDevice`, arrayOfId, httpOptions);
  }

  deleteDevice(id: number) {
    return this.http.delete(`${environment.devicesURL}/${id}`, httpOptions);
  }
  deviceCount():Observable<number>{
    return this.http.get<number>(`${environment.devicesURL}/count`,httpOptions)
  }
  updateDevice(id: number, device: any): Observable<any> {
    return this.http.put(`${environment.devicesURL}/update/${id}`, device, httpOptions);
  }
  updateDeviceNotification(id: number, forFrost: boolean): Observable<any> {
    return this.http.get(`${environment.devicesURL}/flipNotification/${id}/${forFrost}`, httpOptions);
  }
  updateDeviceCount(device:any):Observable<any>{
    return this.http.put(`${environment.devicesURL}/updateCount`, device, httpOptions);
  }
  createUserDevList(newlist: any): Observable<any> {
    return this.http.post<any>(`${environment.userDevicesListsURL}create`, newlist, httpOptions);
  }

  getCreatedDevList(id: number): Observable<any> {
    return this.http.get(`${environment.userDevicesListsURL}${id}`, httpOptions);
  }

  uploadDeviceImage(data): Observable<any> {
    return this.http.post(`${environment.devicesURL}upload`, data);
  }
  //queries
  saveSearchQueries(data: string):Observable<any>{
    return this.http.post(`${environment.searchQueriesURL}`, {QueryString: data})
  }

  updateSubscription(data:any): Observable<any> {
    return this.http.post(`${environment.subscriptionURL}/saveGardifyPlusSubscription`, data, httpOptions);
  }

//Garden Archiv
  getUserAlbums():Observable<any>{
    return this.http.get(`${environment.imageApiUrl}/album`, httpOptions);
  }
   getAllUserImages():Observable<any>{
    return this.http.get(`${environment.imageApiUrl}/allImages`, httpOptions);
  }
  getUserPresentations():Observable<any>{
    return this.http.get(`${environment.imageApiUrl}/mypresentation`, httpOptions);
  }
  getUserPresentationsById(id:number):Observable<any>{
    return this.http.get(`${environment.imageApiUrl}/mypresentation/${id}`, httpOptions);
  }
  getUserAlbumById(albumId:number):Observable<any>{
    return this.http.get(`${environment.imageApiUrl}/album/${albumId}`, httpOptions);
  }
  createUserAlbum(data:any):Observable<any>{
    return this.http.post(`${environment.imageApiUrl}/album`, data,httpOptions)
  }
  createUserPresi(data:any):Observable<any>{
    return this.http.post(`${environment.imageApiUrl}/presentation`, data,httpOptions)
  }
  sortImages(params):Observable<any>{
    return this.http.get(`${environment.imageApiUrl}/sortImages`,{params:params} );
  }
  FilterAlbumImgByDate(params):Observable<any>{
    return this.http.get(`${environment.imageApiUrl}/filterImages`,{params:params} );
  }
  uploadAlbumImg(data:any):Observable<any>{
    return this.http.post(`${environment.imageApiUrl}/upload`,data );

  }
  uploadPresiImg(data:any):Observable<any>{
    return this.http.post(`${environment.imageApiUrl}/presentation/upload`,data);

  }
  AddImageToPresi(imageId,presiId,data:any):Observable<any>{
    return this.http.post(`${environment.imageApiUrl}/${imageId}/${presiId}/addImageToPresi`,data );
  }
  OrderImageInPresiTable(presiId,imageId):Observable<any>{
    return this.http.post(`${environment.imageApiUrl}/presentation/addImage/${presiId}/${imageId}`,httpOptions );
  }
  AddImageToAlbum(imageId,albumId,data:any):Observable<any>{
    return this.http.post(`${environment.imageApiUrl}/${imageId}/${albumId}`,data );
  }
  getAlbumImagesById(albumId):Observable<any>{
    return this.http.get(`${environment.imageApiUrl}/album/${albumId}`,httpOptions);
  }
  getImageById(imageId):Observable<any>{
    return this.http.get(`${environment.imageApiUrl}/image/${imageId}`,httpOptions);
  }
  deleteImageFromAlbum(imageId,albumId):Observable<any>{
    return this.http.delete(`${environment.imageApiUrl}/${imageId}/${albumId}`,httpOptions)
  }
  deleteImageFromPresentation(imageId,presiId):Observable<any>{
    return this.http.delete(`${environment.imageApiUrl}/presentation/deleteImage/${presiId}/${imageId}`,httpOptions)
  }
  createPresentation(dataForm:any):Observable<any>{
    return this.http.post(`${environment.imageApiUrl}/presentation`,dataForm)
  }
  deleteImagesFromGardenArchiv(imagesIds:any[]):Observable<any>{
    return this.http.post<any>(`${environment.imageApiUrl}/deleteImageGlobal`,imagesIds)
  }
  deleteAlbumById(albumId):Observable<any>{
    return this.http.delete(`${environment.imageApiUrl}/album/${albumId}`,httpOptions)
  }

  deletePresentationById(presiId):Observable<any>{
    return this.http.delete(`${environment.imageApiUrl}/presentation/${presiId}`,httpOptions)
  }
  GetUserPresentationById(presiId):Observable<any>{
    return this.http.get(`${environment.imageApiUrl}/myPresentation/${presiId}`,httpOptions);
  }
  editPresentation(editForm):Observable<any>{
    return this.http.put(`${environment.imageApiUrl}/presentation`,editForm);
  }
  editAlbum(editForm):Observable<any>{
    return this.http.put(`${environment.imageApiUrl}/album`,editForm);
  }

  FindMember(searchUserName):Observable<any>{
    return this.http.get<any>(`${environment.imageApiUrl}/findmember/${searchUserName}`, httpOptions)
  }
  FindMemberInContacts(searchUserName):Observable<any>{
    return this.http.get<any>(`${environment.imageApiUrl}/findmemberInContact/${searchUserName}`, httpOptions)
  }

  addContact(presId, contactName):Observable<any>{
    return this.http.post(`${environment.imageApiUrl}/presentation/addContact/${presId}`, contactName)

  }
  addContactFromGardifyUser(contactName):Observable<any>{
    return this.http.post(`${environment.userURL}addtocontact`, contactName)

  }
  getPresiContacts(presiId):Observable<any>{
    return this.http.get<any>(`${environment.imageApiUrl}/presentation/getcontact/${presiId}`, httpOptions)
  }
  getAllContacts(){
    return this.http.get<any>(`${environment.userURL}getcontact`, httpOptions)
  }

  getOtherPresi(params){
    return this.http.get<any>(`${environment.imageApiUrl}/otherpresentation`, {params:params})
  }

  //Community
  getCommunityData():Observable<any>{
    return this.http.get(`${environment.communityURL}/getallentry`, httpOptions);
  }
  getCommunityDataWithComments():Observable<any>{
    return this.http.get(`${environment.communityURL}/getAllEntryWithAnswers`, httpOptions);
  }
  getCommunityQuestionComments(questionId):Observable<any>{
    return this.http.get(`${environment.communityURL}/${questionId}/getentry`,httpOptions);
  }
  addComment(data:any):Observable<any>{
    return this.http.post(`${environment.communityURL}/answer`, data,httpOptions)
  }
  postQuestion(data:any):Observable<any>{
    return this.http.post(`${environment.communityURL}/newEntry`, data,httpOptions)
  }
  uploadCommunityQuestionImg(data:any):Observable<any>{
    return this.http.post(`${environment.communityURL}/uploadQuestionImage`,data );

  }
}
