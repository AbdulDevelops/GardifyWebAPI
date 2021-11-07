import { Component, OnInit, HostListener, OnDestroy, ChangeDetectorRef, ViewChild, ElementRef, Renderer2 } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { UtilityService } from 'src/app/services/utility.service';
import { StatCounterService } from 'src/app/services/stat-counter.service';
import { ShopItem, UserActionsFrom, WeatherIcons } from 'src/app/models/models';
import { AuthService } from 'src/app/services/auth.service';
import { WeatherService } from 'src/app/services/weather.service';
import { faUser, faPlus } from '@fortawesome/free-solid-svg-icons';
import { AlertService } from 'src/app/services/alert.service';
import { Subscription } from 'rxjs';
import { FormGroup, FormBuilder, FormControl } from '@angular/forms';
import { ScannerService } from 'src/app/services/scanner.service';
import { startOfMonth } from 'date-fns';
import { HttpParams } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { PlantSearchService } from 'src/app/services/plant-search.service';
declare var gtag: Function;

interface Weather {
  currentTemp: any;
  wind: any;
  precipitation: any;
  pressure: any;
  humidity: any;
}

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit, OnDestroy {
  @ViewChild('video') public video: ElementRef;
  @ViewChild('canvas') public canvas: ElementRef;
  public captures:any[]
  faUser=faUser;
  faPlus = faPlus;
  public isViewable: boolean;
  viewDate: Date = new Date();
  mode: string;
  show = true;
  isMobile: boolean;
  forecastItem:any
  viewpoint: string;
  
  width = window.innerWidth;
  weather: Weather = {currentTemp: '', wind: '', precipitation: '', pressure: '', humidity: ''};
  counter = { allTodosOfTheMonth: 0, allTodosOfTheYear: 0,points: 0, plantSorts: 0, plantsTotal: 0,shopCartCounter:0,answerCount:0, wishListCount:0};
  lastUpdated: Date;
  arrayBackUrl=["mygarden-iconClicked","weather-iconClicked","search-iconClicked","scan-iconClicked","doc-iconClicked","premium-iconClicked","shop-iconClicked","az-iconClicked","oekoscan-iconClicked"]
  countFrostAlert = 0;
  subs = new Subscription();
  suggestForm: FormGroup;
  suggestInvalid: boolean = true;
  suggestImgs = [];
  suggestImage: any;
  loading = false;
  public cartItems: ShopItem[];
  user: any;
  submittedPlant = false;
  showCart = false;
  uploadedPic: any;
  fromPlantScan = false;
  countdown = {hours: 0, mins: 0, secs: 0, started: false};
  onboarded: boolean;
  ignoreMatches = false;
  shopCount = 0;
  newsCount = 0;
  videosCount = 0;
  subscript: any;
  apiCallType=new UserActionsFrom();
  steps = [
    {
      step: 1,
      title: 'MEIN GARTEN',
      text: 'Hier kannst du deine Pflanzen, Gartengeräte und ökologische Ausstattung deinem Garten (+) hinzufügen. So entsteht automatisch dein To-Do-Kalender für deinen Garten. Zudem steht dir zur Verfügung:<br /> <ul><li>Notizen zu deinen Pflanzen</li> <li>Gartenbereiche hinzufügen</li> <li>Sturmwarnungen für Schirme Markisen etc.</li> <li>Frostwarnungen zu deinen gefährdeten Pflanzen</li><li>Filter (z. B. frostgefährdet, giftig, bienenfreundlich uvm.)</li><li>Statistik deiner Pflanzen</li></ul>'
    },
    {
      step: 2,
      title: 'TO-DO KALENDER',
      text: 'Dein To-do-Kalender wird automatisch erzeugt, wenn du Pflanzen in deinen Garten kopiert hast. Er zeigt dir an, was, wann und auch wie es zu tun ist. Hake erledigte To-dos ab, füge eigene To-Dos oder Tagebucheinträge hinzu und lösche To-Dos, die du nicht mehr bekommen möchtest.'
    },
    {
      step: 3,
      title: 'GARTEN WETTER',
      text: 'Hier bekommst du standortgenaue Wetter-Infos, sowie Frost- und Sturmwarnungen für deine empfindlichen Pflanzen und deine Gartenausrüstung. Um diese Funktionen zu nutzen, ist eine Anmeldung technisch erforderlich, damit du die Wetterdaten für deinen Standort siehst.'
    },
    {
      step: 4,
      title: 'PFLANZEN SUCHE',
      text: 'Die Pflanzensuche hilft dir, die richtigen Pflanzen zu finden. Dazu kannst du alle Pflanzen in der Datenbank nach 300 Eigenschaften in Sekunden sortieren. Aus der Pflanzensuche heraus, kannst du auch Pflanzen direkt in deinen Garten kopieren.'
    },
    {
      step: 5,
      title: 'PFLANZEN SCAN',
      text: 'Du hast eine Pflanze im Garten, die du nicht kennst? Lade hier zur Pflanzenbestimmung einfach ein Foto der Pflanze hoch oder nutze deine Smartphone-Kamera. Du erhältst dann Vorschläge, um welche Pflanze es sich handeln könnte. Achte darauf, dass du nah genug herangehst und nur eine Pflanze auf dem Bild zu sehen ist. Ist die Pflanze noch nicht in unserer Datenbank, kannst du sie zur Erfassung vorschlagen.'
    },
    {
      step: 6,
      title: 'GARDIFY SHOP',
      text: 'Hier findest du von uns geprüfte Produkte für den Garten. Faire Beschreibungen helfen dir, die richtigen Produkte zu finden. Alle Dünger, Erden, Saatgut und Spritzmittel werden von uns einer kritischen Prüfung durch unser Team aus Dipl.-Biologen und Gartenexperten unterworfen. <br>Wir freuen uns, wenn ihr gardify durch Kauf in unserem Shop unterstützt. Wir garantieren euch sehr faire Preise und Bedingungen!'
    },
    {
      step: 7,
      title: 'GARTEN ÖKOSCAN',
      text: 'Jeder Garten hilft der Natur, allem voran Insekten und Vögeln. Wenn du wissen du möchtest, in welchen Monaten dein Garten besonders gut abschneidet und wie die anderen Ökobewertungen deines Gartens ausfallen, probiere mal den Öko-Scan! Das Ergebnis kannst du per Mail an Gartenfreunde verschicken oder im To-Do-Kalender abspeichern.'
    },
    {
      step: 8,
      title: 'PFLANZEN DOC',
      text: 'Deine Pflanze leidet unter einem Schädlingsbefall oder einer dir unbekannten Krankheit? Frag den Pflanzen-Doc! Suche in den Beiträgen oder eröffne ein neues Thema. Lade dazu das Foto hoch und beschreibe das Problem möglichst genau.'
    },
    {
      step: 9,
      title: 'PFLANZEN ERGÄNZEN',
      text: 'Deine gesuchte Pflanze ist nicht bei gardify aufgeführt? Schlage sie für die Datenbank vor! Bitte schickt immer ein Bild mit und gebt den Namen an, wenn ihr eine Idee habt, um welche Pflanze es sich handeln könnte.'
    },
    {
      step: 10,
      title: 'EINSTELLUNGEN',
      text: 'Über die Einstellungen verwaltest du deine Nutzerdaten, deinen Gartenstandort und die Benachrichtigungen, sowie Frost- und Sturmwarnungen.'
    }
  ];
  currentTourStep = 0;
  constraints = {
    video: {
        
        width: { ideal: 4096 },
        height: { ideal: 2160 }
    }
};
videoWidth = 0;
videoHeight = 0;
  constructor(
    private renderer: Renderer2,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private themeProvider: ThemeProviderService, 
    private alertService: AlertService, 
    private cd: ChangeDetectorRef,
    private authService: AuthService, 
    private sc: StatCounterService, 
    private fb: FormBuilder,
    private util: UtilityService,
    private weatherService: WeatherService, 
    private plantSearch: PlantSearchService,
    private scanner: ScannerService) {
      this.suggestForm = this.fb.group({
        Known: new FormControl(false),
        Name: [null]
      });
      this.captures=[];
    }

  @HostListener('window:resize')
  onWindowResize() {
    this.width = window.innerWidth;
    this.setViewpoint();
    this.isMobile = window.innerWidth <= 990;
  }

  changeTourStep(step) {
    const tourWrapper = document.getElementsByClassName('step-wrapper')[0];
    if (tourWrapper) {
      tourWrapper.scrollIntoView({behavior: 'smooth', block: 'nearest', inline: 'nearest'});
    }
    const currentTourStep = this.currentTourStep + step;
    if (this.currentTourStep < 1) { this.currentTourStep = 1; return; }
    if (this.currentTourStep > 10) { this.currentTourStep = 10; return; }

    const slideRight = step > 0 && ((currentTourStep === 7) || (this.isMobile && currentTourStep % 2 !== 0));
    const sildeLeft = step < 0 && ((currentTourStep === 6) || (this.isMobile && currentTourStep % 2 === 0));

    if (!slideRight && !sildeLeft) {
      this.currentTourStep = currentTourStep;
      return;
    }
    
    // auto scroll nav when outside view
    if (slideRight) {
      (document.getElementsByClassName('carousel-control-next')[0] as HTMLElement).click();
    }
    if (sildeLeft) {
      (document.getElementsByClassName('carousel-control-prev')[0] as HTMLElement).click();
    }

    setTimeout(() => {
      this.currentTourStep = currentTourStep;
    }, 500);
  }

  startTour() {
    this.subs.add(this.util.startTour().subscribe());
  }

  closeTour() {
    this.util.endTour();
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
    this.util.endTour();
  }

  imageFromScan(value){
    this.scanner.fromPlantScan = value
    if (value == false){
      this.scanner.allPlantResult.next(null);

    }
  }

  getImageFromScanStatus(){
    return this.scanner.fromPlantScan;
  }

  getAllScanResults(){
    return this.scanner.allPlantResult;
  }

  saveSubscription(value){
    //this.user;
  //  var UserId = faUser;
    
    if(value==0){ //monthly subscription
   
    this.subscript.IsGardifyPlusMonthly = true;
    this.subscript.IsGardifyPlusAnnually = true;
    this.subscript.IsGardifyPlusTest = true;
    this.subscript.TestStartDate = true;
    this.subscript.MonthlyEndDate = true;
    this.subscript.AnnualStartDate = true;
    this.subscript.AnnualEndDate = true;
   return this.util.updateSubscription(value);
  }else if(value==1){//annual subscription

  }else if(value==2){//possibility to test

  }
  }
  ngOnInit() {
    this.util.tourStep$.subscribe(step => this.currentTourStep = step);
    this.util.showCart$.subscribe(shouldShow => {
      this.showCart = shouldShow;
      if (shouldShow) {
        setTimeout(() => {
          this.util.toggleCart(false);
        }, 5000);
      }
    });
    this.themeProvider.getTheme().subscribe(t => this.mode = t);
    this.subs.add(this.authService.getUserObservable().subscribe(u => {
      this.user = u;
      this.onboard();
    }));
    this.isMobile = window.innerWidth <= 990;
    this.subs.add(this.scanner.uploadedPic$.subscribe(pic => this.uploadedPic = pic));

    if (this.isLoggedIn) {
      this.subs.add(this.sc.requestPlantsCount().subscribe());
      this.subs.add(this.sc.requestTodosCount().subscribe());
      this.subs.add(this.sc.requestWarningsCount().subscribe());
      this.subs.add(this.sc.requestShopCartCounter().subscribe());
      this.subs.add(this.sc.requestAnswersCount().subscribe());
      this.subs.add(this.sc.requestWishListEntriesCounter().subscribe())
    }

    if (this.isLoggedIn) {
      this.setWeatherData();
      this.isViewable = true;
    }
    this.sc.todosCount$.subscribe(data => {
      this.counter.allTodosOfTheMonth = data.AllTodosOfTheMonth;
      this.counter.allTodosOfTheYear=data.AllTodos
    });
    this.sc.plantsCount$.subscribe(data => {
      this.counter.plantSorts = data.Sorts;
      this.counter.plantsTotal = data.Total;
    });
    this.sc.shopCartEntriesCount$.subscribe(data=>{
      this.counter.shopCartCounter=data;
    })
    this.sc.warningsCount$.subscribe(data=>{
      this.countFrostAlert = data;
    })
    this.sc.bonusPoints$.subscribe(data => {
      this.counter.points = data;
    });
    this.sc.answersCount$.subscribe(data=>{
      this.counter.answerCount=data;
    });
    this.sc.wishlistEntriesCount$.subscribe(data=>{
      this.counter.wishListCount=data;
    })
    this.scanner.plantForSuggestion$.subscribe(plantName => {
      this.suggestForm.patchValue({
        Name: plantName
      });
    })

    this.subs.add(this.util.getArticlesCount().subscribe(count => {
      this.shopCount = count; 
    }));
    this.subs.add(this.util.getInstaEntries(null).subscribe(e=>{
     this.newsCount = e.data.length;
    }));
    this.subs.add(this.util.getVideos(null).subscribe(d => {
      this.videosCount = d.length;
    }));

  }

  firePopulateTodos(){
    let params = new HttpParams();
    params = params.append('period', 'month');
    params = params.append('startDate', startOfMonth(this.viewDate).toISOString());
    this.subs.add(this.util.getTodos(null, params).subscribe(()=>{
       this.subs.add(this.sc.requestTodosCount().subscribe());}
   ));
  }
  startCountdown() {
    if (!this.user.Email.includes('UserDemo')) { this.countdown.started = false; return; }
    const expiration = new Date(this.user.ExpiresUtc).getTime();
    const wid = setInterval(() => {
      const now = Date.now();
      const left = expiration - now;
      if (left < 0) {
        clearInterval(wid);
        this.logout();
      }
      this.countdown.started = true;
      this.countdown.hours = Math.floor((left / (1000 * 60 * 60)));
      this.countdown.mins = Math.floor((left % (1000 * 60 * 60)) / (1000 * 60));
      this.countdown.secs = Math.floor((left % (1000 * 60)) / 1000);
    }, 1000);
  }

  onboard() {
    this.subs.add(this.activatedRoute.queryParams.subscribe((params) => {
      if (params.code && params.userId) {
        const userId = params.userId;
        const code = encodeURIComponent(params.code).replace(/\s/g,'+');
        this.authService.confirmUser(userId, code).subscribe((data) => {
          // handle token
          if (data && data.Token) {
            localStorage.setItem('currentUser', JSON.stringify(data));
            this.authService.user.next(JSON.parse(localStorage.getItem('currentUser')));
          }
          this.countdown.started = false;
          this.onboarded = true;
          // send tracking events
          this.util.sendGAEvent('register', 'register');
          this.gtag_report_conversion(this.router.url, 'AW-624991558/uXxrCN2Ek9UBEMa6gqoC');
          // refresh
          this.router.navigate(['/meingarten']);
          this.alertService.success('Willkommen bei gardify!', true);
        },
        (err) => {
          this.router.navigate(['/']);
        });
      } else if (this.isLoggedIn) {
        this.startCountdown();
      }
    }));
  }

  private setViewpoint() {
    if (this.width < 576) {
      this.viewpoint = 'xs';
    } else if (this.width >= 576 && this.width < 768) {
      this.viewpoint = 'sm';
    } else if (this.width >= 768 && this.width < 992) {
      this.viewpoint = 'md';
    } else if (this.width >= 992 && this.width < 1200) {
      this.viewpoint = 'lg';
    } else {
      this.viewpoint = 'xl';
    }
  }

  setWeatherData() {
    // only make requests if 10 mins have passed
    const isRecent = this.lastUpdated && new Date().getTime() - this.lastUpdated.getTime() < 1000 * 60 * 10;
    if (!isRecent) {
      this.subs.add(this.util.getCurrentForecast().subscribe(d => {
        this.forecastItem = d.Forecasts.slice(0, 1);
        this.forecastItem.forEach(element => {
          this.weather.currentTemp = Math.round(element.AirTemperatureInCelsius);
          this.weather.wind = element.WindSpeedInKilometerPerHour;
          this.weather.precipitation = element.PrecipitationProbabilityInPercent ;
          this.weather.pressure = element.AirPressureAtSeaLevelInHectoPascal;
          this.weather.humidity = element.RelativeHumidityInPercent;
          this.lastUpdated = new Date();
        });
      }));
    }
  }
  public getWeatherIcon(data) {
    return this.weatherService.getWeatherIcon(data);
  }
  
  public toggleTheme() {
    this.themeProvider.toggleTheme();
  }
 
  public toggle(): void { this.isViewable = !this.isViewable; }
  

  get isLoggedIn() {
    return this.authService.isLoggedIn();
  }

  get demoMode() {
    return this.user && this.user.Email.includes('UserDemo');
  }
  
  logout() {
    this.authService.logout();
    this.counter.shopCartCounter=0;
    this.counter.allTodosOfTheMonth=0;
    this.counter.plantSorts=0;
    this.counter.plantsTotal=0;
    this.counter.points=0;
    this.counter.answerCount=0;
    this.counter.wishListCount=0
  }

  gtag_report_conversion(url, send_to) {
    const callback = function () {
      if (typeof(url) != 'undefined') {
        // window.location = url;
      }
    };
    gtag('event', 'conversion', {
        'send_to': send_to,
        'event_callback': callback
    });
    return false;
  }

  close(){
    $('#checklist1').prop('checked', false);
    $('#textarea1').val('');
    $('#file1').val('');
  }

  submit() {
    const data = new FormData();
    var allScanResult = this.getAllScanResults().value;


    if (!this.getImageFromScanStatus()){
      this.suggestImgs.forEach((img, i) => {
        data.append('imageFile' + i, img.file);
        data.append('imageTitle' + i, img.title);
      });
  
    }
    else{
          // add cached scan image
      if (this.uploadedPic && this.uploadedPic.file) {
        data.append('imageFile' + this.suggestImgs.length, this.uploadedPic.file);
        data.append('imageTitle' + this.suggestImgs.length, this.uploadedPic.title);
      }
    }

    
    data.append('known', (!!this.suggestForm.value.Name).toString());
    data.append('name', this.suggestForm.value.Name);
    data.append('ignoreMatches', this.ignoreMatches.toString());
    
    this.loading = true;
    let params = new HttpParams();
    params = params.append('isIos', this.apiCallType.IsIos.toString());
    params = params.append('isAndroid', this.apiCallType.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallType.IsWebPage.toString());

    if (allScanResult == null){
      this.subs.add(this.util.suggestPlant(data,params).subscribe((res) => {
        this.loading = false;
        this.submittedPlant = true;
        if (res.FoundMatches) {
          const cachedSearch = this.buildSearchForm();
          this.plantSearch.setPrevious(cachedSearch);
          // this.ignoreMatches = true;
          setTimeout(() => {
            // forces reloading if user is already on /suche 
            this.router.navigateByUrl('/', { skipLocationChange: true }).then(()=>
              this.router.navigate(['/suche'])
            );
            this.alertService.success('Folgende Pflanzen sind ähnlich zu deinem Vorschlag');
          });
          this.suggestForm.reset();
        } else {
          this.ignoreMatches = false;
          this.alertService.success('Dein Vorschlag wurde gesendet');
          this.suggestImgs = [];
          this.suggestForm.reset();
          this.scanner.plantForSuggestion.next(null);
          this.plantSearch.setPrevious(this.buildSearchForm());
        }
      }, 
      (err) => this.loading = false));
    }
    else{
      data.append('searchResult', allScanResult);


      this.subs.add(this.util.addNewSuggestPlant(data).subscribe((res) => {
        this.loading = false;
        this.submittedPlant = true;
        if (res.FoundMatches) {
          const cachedSearch = this.buildSearchForm();
          this.plantSearch.setPrevious(cachedSearch);
          // this.ignoreMatches = true;
          setTimeout(() => {
            // forces reloading if user is already on /suche 
            this.router.navigateByUrl('/', { skipLocationChange: true }).then(()=>
              this.router.navigate(['/suche'])
            );
            this.alertService.success('Folgende Pflanzen sind ähnlich zu deinem Vorschlag');
          });
          this.suggestForm.reset();
        } else {
          this.ignoreMatches = false;
          this.alertService.success('Dein Vorschlag wurde gesendet');
          this.suggestImgs = [];
          this.suggestForm.reset();
          this.scanner.plantForSuggestion.next(null);
          this.plantSearch.setPrevious(this.buildSearchForm());
        }
      }, 
      (err) => this.loading = false));
    }
    
  }

  buildSearchForm() {
    const form = this.fb.group({
      searchText: new FormControl(this.suggestForm.value.Name.trim().toLowerCase()),
      colors: this.fb.array([new FormControl(false)]),
      places: this.fb.array([new FormControl(false)]),
      monthRange: new FormControl([1, 12]),
      growthRange: new FormControl([0, 800]),
      freezeLevels: new FormControl(10),
      growthType: this.fb.array([new FormControl(false)]),
      cropPlant: this.fb.array([new FormControl(false)]),
      blossoms: this.fb.array([new FormControl(false)]),
      fertil: this.fb.array([new FormControl(false)]),
      trim: this.fb.array([new FormControl(false)]),
      breeding: this.fb.array([new FormControl(false)]),
      usage: this.fb.array([new FormControl(false)]),
      specifics: this.fb.array([new FormControl(false)]),
      exclusions: this.fb.array([new FormControl(false)]),
      autumnClr: this.fb.array([new FormControl(false)]),
      deco: this.fb.array([new FormControl(false)]),
      flShape: this.fb.array([new FormControl(false)]),
      groundType: this.fb.array([new FormControl(false)]),
      leafClr: this.fb.array([new FormControl(false)]),
      fruitClr: this.fb.array([new FormControl(false)]),
      ecoTags: this.fb.array([new FormControl(false)]),
      // dropdowns
      groups: new FormControl(''), fruitType: new FormControl(''), family: new FormControl(''),
      waterReq: new FormControl(''), blossomStat: new FormControl(''), leafShape: new FormControl(''), 
      leafPos: new FormControl(''),
      //radios
      foliage: new FormControl(''),
      blossomsSize: new FormControl(''),
      leafMargin: new FormControl('')
    });
    return form;
  }

  async imageUpload(event) {
    const res = await this.scanner.handleImageUpload(event);
    if (res.err) {
      return;
    }

    this.suggestImgs.push({file: res.file, title: res.file.name, src: res.src});
    this.cd.markForCheck();
  }

  removeSelectedFile(index) {
    if (index < 0) {
      // delete cached image
      this.scanner.uploadedPicSubject.next(null);
    } else {
      // Delete the item from selected images list
      this.suggestImgs.splice(index, 1);
    }
   }

   
  startCamera() {
    //Check fom device camera and request users permission
    if (!!(navigator.mediaDevices && navigator.mediaDevices.getUserMedia)) { 
      navigator.mediaDevices.getUserMedia(this.constraints).then(this.attachVideo.bind(this)).catch(this.handleError);
    } else {
        alert('Sorry, camera not available.');
    }
}
  public capture() {
   
    const video= document.getElementById('video') as HTMLVideoElement;
    this.renderer.setProperty(this.canvas.nativeElement, 'width', this.videoWidth);
    this.renderer.setProperty(this.canvas.nativeElement, 'height', this.videoHeight);
    this.canvas.nativeElement.getContext('2d').drawImage(video, 0, 0);
    this.captures.push(this.canvas.nativeElement.toDataURL("image/jpeg"));
    
    console.log(this.captures)
}
  public convertToBlob(imageURL):Blob{

    // Split the base64 string in data and contentType
    let block = imageURL.split(";");
    // Get the content type of the image
    let contentType = block[0].split(":")[1];// In this case "image/gif"
    // get the real base64 content of the file
    let realData = block[1].split(",")[1];// In this case "R0lGODlhPQBEAPeoAJosM...."

    // Convert it to a blob to upload
    let blob = this.b64toBlob(realData, contentType,null);
    
    return blob;
  }

  //display error when failure occurs
  handleError(error) {
    console.log('Error: ', error);
  }
  //attach the stream from the camera to the HTML video element
  attachVideo(stream) {
  const video= document.getElementById('video') as HTMLVideoElement
    this.renderer.setProperty(video, 'srcObject', stream);
    //add a listener
    this.renderer.listen(video, 'play', (event) => {
      this.videoHeight = video.videoHeight;
      this.videoWidth = video.videoWidth;
  });
 }
 b64toBlob(b64Data, contentType, sliceSize) {
  contentType = contentType || '';
  sliceSize = sliceSize || 512;

  var byteCharacters = atob(b64Data);
  var byteArrays = [];

  for (var offset = 0; offset < byteCharacters.length; offset += sliceSize) {
      var slice = byteCharacters.slice(offset, offset + sliceSize);

      var byteNumbers = new Array(slice.length);
      for (var i = 0; i < slice.length; i++) {
          byteNumbers[i] = slice.charCodeAt(i);
      }

      var byteArray = new Uint8Array(byteNumbers);

      byteArrays.push(byteArray);
  }

var blob = new Blob(byteArrays, {type: contentType});
return blob;
}
get isSocialPage() {
  return this.router.url.includes('/social') || this.router.url.includes('/gartenflora') ;
}

}
