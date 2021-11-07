import { Component, OnInit, ViewChild, HostListener, OnDestroy, EventEmitter, ElementRef, AfterViewInit } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, FormArray, Validators } from '@angular/forms';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { Options } from 'ng5-slider';
import { faSearch, faChevronDown, faChevronUp, faSlidersH, faCircle, faInfo, faEye, faMapMarker } from '@fortawesome/free-solid-svg-icons';
import { Subject, Subscription, of, Observable, fromEvent } from 'rxjs';
import { Plant, Group, Garden, Todo, ReferenceToModelClass, ColorSort, PlaceHolder, Ad, UserActionsFrom } from 'src/app/models/models';
import { HttpParams } from '@angular/common/http';
import { map, debounceTime, distinctUntilChanged, mergeMap, delay } from 'rxjs/operators';
import { PlantSearchService, slugify } from 'src/app/services/plant-search.service';
import { UtilityService } from 'src/app/services/utility.service';
import { PagerService } from 'src/app/services/pager.service';
import { AuthService } from 'src/app/services/auth.service';
import { StatCounterService } from 'src/app/services/stat-counter.service';
import { AlertService } from 'src/app/services/alert.service';
import { ActivatedRoute, Router } from '@angular/router';

const X_COORDS_SCALE_FACTOR = 0.001221;
const Y_COORDS_SCALE_FACTOR = 0.000232;
const LONG_RANGE = [5.5, 17.5];
const LAT_RANGE = [45, 55];

interface PlantFamily {
  displayStr: string;
  val: string;
}

@Component({
  selector: 'app-plants-search',
  templateUrl: './plants-search.component.html',
  styleUrls: ['./plants-search.component.css']
})
export class PlantsSearchComponent implements OnInit, OnDestroy, AfterViewInit {
  mode: string; sticky: boolean; stickyBtn: boolean;
  loading = false; formIsBuilt = false; errorMsg: string;
  queue = false;
  IsSearching = false;

  detailSearch = false; showAddPlant = false; selectedPlant: Plant;
  faSearch = faSearch; faChevronDown = faChevronDown; faChevronUp = faChevronUp; sliders = faSlidersH;
  faCircle = faCircle; faInfo = faInfo; faEye = faEye;
  faMarker = faMapMarker;
  width = window.innerWidth;
  isMobile: boolean;
  isXsMobile:boolean;
  baseUrl = 'https://gardify.de/intern/';
  @ViewChild('stickyMenu', { static: true }) menuElement;
  @ViewChild('gardenMarker') gardenMarker: ElementRef;

  appliedFilters: any[] = [];
  plantImageModal;
  isHideSearchResult = false;

  plantsPager: any = {};
  currentPage = 1;

  userPlantTrigger: any[] = [];
  // paged items
  pagedPlants: any[];
  newTodosForm:FormGroup;
  newTodo = new Todo();
  referenceType=ReferenceToModelClass.UserPlant;

  public dummyElem = document.createElement('SPAN');
  public hiddenPre=document.createElement('pre');

  public searchText = new Subject<KeyboardEvent>();
  private subscriptions = new Subscription();
  italicRegex = new RegExp(/\{k]|\[k]|\[\/k]/g); 
  availableFiltersIds = {
    Ausschlusskriterien: 0,
    Besonderheiten: 0,
    Blattrand: 0,
    Blattfarbe: 0,
    Blattform: 0,
    Blattstellung: 0,
    Blütenfarben: 0,
    Blüten: 0,
    Blütenform: 0,
    Blütengröße: 0,
    Blütenstand: 0,
    Boden: 0,
    Früchte: 0,
    Fruchtfarbe: 0,
    Dekoaspekte: 0,
    Herbstfärbung: 0,
    Licht: 0,
    Laubrhythmus: 0,
    Düngung: 0,
    Wasserbedarf: 0,
    Vermehrung: 0,
    Winterhärte: 0,
    Schnitt: 0,
    Verwendung: 0,
    Wuchs: 0,
    Nutzpflanzen: 0
  };

  BADGES = {
    Bees: {Id: [447], title: 'Bienenfreundlich'},
    Birds: {Id: [320,322], title: 'Vogelfreundlich'},
    Insects: {Id: [321], title: 'Insektenfreundlich'},
    Bio: {Id: [445], title: 'Ökologisch wertvoll'},
    Butterflies: {Id: [531], title: 'Schmetterlings freundlich'},
    Domestic: {Id: [530], title: 'Heimische Pflanze'},
  };

  addForm: FormGroup;
  mainForm: FormGroup;
  addNewList: FormGroup;

  gardens: Garden[] = [];
  plants:  Plant[]= [];
  _groups: Group[];
  _families: PlantFamily[];
  _gardenGroups: any[];
  categories: any[];
  tags: any[];
  monthRange: Options = {
    floor: 1,
    ceil: 12,
    noSwitching: true,
    showTicks: true,
    hideLimitLabels: true,
    getSelectionBarColor: () => '#7a9d34',
    getPointerColor: () => '#7a9d34',
    translate: (value: number): string => {
      switch (value) {
        case 1: return 'Jan'; case 7: return 'Jul';
        case 2: return 'Feb'; case 8: return 'Aug';
        case 3: return 'Mär'; case 9: return 'Sep';
        case 4: return 'Apr'; case 10: return 'Okt';
        case 5: return 'Mai'; case 11: return 'Nov';
        case 6: return 'Jun'; case 12: return 'Dez';
      }
    },
  };

  growthRange: Options = {
    floor: 0,
    ceil: 800,
    hideLimitLabels: true,
    noSwitching: true,
    getSelectionBarColor: () => '#7a9d34',
    getPointerColor: () => '#7a9d34'
  };

  freezeOpts: Options = {
    floor: -45,
    ceil: 10,
    step: 5,
    rightToLeft: true,
    showSelectionBarFromValue: -45,
    showTicks: true,
    showTicksValues: true,
    hideLimitLabels: true,
    getSelectionBarColor: (value: number): string => {
      return (value <= -45) ? '#bc444c' : '#c9d434';
    },
    getPointerColor: () => '#333',
    translate: (value: number): string => value +  '°C'
  };

  hardinessTicksArray = [0, 9, 20, 31, 41, 50, 60, 70, 81, 90];
  hardinessValue = 0;
  hardinessLevels = [{value:'Z11'},{value:'Z10'},{value:'Z9'},{value:'Z8'},{value:'Z7'},{value:'Z6'},{value:'Z5'},{value:'Z4'},{value:'Z3'},{value:'Z2'}];  // make object with ids
  hardinessOpts: Options = {
    floor: 0,
    ceil: 100,
    step: 0.3,
    readOnly: false,
    showTicks: true,
    showTicksValues: true,
    showSelectionBarFromValue: 100,
    ticksArray: this.hardinessTicksArray,
    hideLimitLabels: true,
    getSelectionBarColor: (value: number): string => {
      return (value <= -45) ? '#bc444c' : '#c9d434';
    },
    getPointerColor: () => '#333',
    getLegend: (value: number): string => {
      if (value === 31) { return this.hardinessLevels[this.hardinessTicksArray.indexOf(value)].value + '<span class="star">*<span>'; }
      if (value === 41) { return this.hardinessLevels[this.hardinessTicksArray.indexOf(value)].value + '<span class="star">**<span>'; }
      if (value === 50) { return this.hardinessLevels[this.hardinessTicksArray.indexOf(value)].value + '<span class="star">***<span>'; }
      return this.hardinessLevels[this.hardinessTicksArray.indexOf(value)].value;
    }
  };
  manualRefresh: EventEmitter<void> = new EventEmitter<void>();

  freezeLevelsIds = [
    '',
    '433,286,287,288,289,290,291,292,293,294',
    '433,286,287,288,289,290,291,292,293',
    '433,286,287,288,289,290,291,292',
    '433,286,287,288,289,290,291',
    '433,286,287,288,289,290',
    '433,286,287,288,289,290',
    '433,286,287,288,289',
    '433,286,287,288',
    '433,286,287',
    '433,286',
    '433,285'
  ];

  _places = []; _waterReq = []; _usage = []; _growthType = []; _blossoms = []; _cropPlant = [];
  _blossomsSize = []; _groundType = []; _fruitType = []; _fertil = []; _trim = []; _breeding = [];
  _specifics = []; _exclusions = []; _autumnClr = []; _deco = []; _foliage = []; _flShape = []; _colors = [];
  _leafShape = []; _leafPos = []; _leafClr = []; _leafMargin = []; _blossomStat = []; _fruitClr = [];
  
  _ecoTags = [
    {t: 'Bienenfreundlich', id: 447, img: 'assets/main-icons/Bienenfreundlich_weiß.svg'},
    {t: 'Insektenfreundlich', id: 321, img: 'assets/main-icons/Insektenfreundlich_weiß.svg'},
    {t: 'Vogelfreundlich', id: '320,322', img: 'assets/main-icons/Vogelfreundlich_weiß.svg'},
    {t: 'Schmetterlingsfreundlich', id: 531, img: 'assets/main-icons/Schmetterlingsfreundlich_weiß.svg'},
    {t: 'Ökologisch wertvoll', id: 445, img: 'assets/main-icons/Biologisch_wertvoll_weiß.svg'},
    {t: 'Heimische Pflanze', id: 530, img: 'assets/main-icons/Heimische_Pflanze_weiß.svg'},
    {t: 'Wassersparende Pflanzen', id:346, img:'assets/main-icons/Wassersparende_Pflanzen_Icon.svg'}
  ];
  _sortTags=[
    {t:'Neueste Pflanzen', id:1},
    {t:'Älteste Pflanzen', id:2},
    {t:'Pflanzen von A-Z', id:3},
    {t:'Pflanzen von Z-A', id:4}
  ]
  populated: boolean;
  description: '';
  count: number;
  requiredField = false;
  dropdownSettings = {
    singleSelection: false,
    idField: 'Id',
    textField: 'Name',
    selectAllText: 'Alle markieren',
    unSelectAllText: 'Alle entmarkieren',
    itemsShowLimit: 10,
    allowSearchFilter: false,
    closeDropDownOnSelection: true,
    noDataAvailablePlaceholderText: 'Bitte Liste anlegen, z.B. Hauptgarten, Vorgarten oder Topfpflanzen'
  };
  userLists = [];
  selectedLists = [];
  ecoSearch = false;
  frostSearch = false;
  showStickyCtrls = true;
  coords = {
    Longtitude: 0,
    Latitude: 0
  };
  superCategories = [];
  queryParam = '';
  sortPlant: any;
  ad: Ad;
  adBetween:any;
  adSides:any;
  test:any="remove [k]me[/k]"
  sortedPlants: any;
  apiCallFrom=new UserActionsFrom();
  source: any;
  @ViewChild("something") something:ElementRef; 

  constructor(private themeProvider: ThemeProviderService, private fb: FormBuilder, private route: ActivatedRoute,
    private plantSearch: PlantSearchService, private util: UtilityService, private counter: StatCounterService,
    private pagerService: PagerService, private auth: AuthService,private alert: AlertService, private router: Router) { 
      
      this.ad = new Ad(
        'ca-pub-3562132666777902',
        2981533122,
        'auto',
        true,
        false,
        null
      ) 
      this.adBetween = new Ad(
        'ca-pub-3562132666777902',
        3424186569,
        'fluid',
        true,
        false,
        "-fb+5w+4e-db+86"
      ) 
      this.adSides = new Ad(
        'ca-pub-3562132666777902',
        7451899209,
        'fluid',
        true,
        true,
        null
      ) 
      this.addForm = this.fb.group({
        PlantId: null,
        InitialAgeInDays: 0,
        Count: 1,
        IsInPot: false,
        Todos: null,
        ArrayOfUserlist:null
      });

      this.addNewList = this.fb.group({
        Name: ['',Validators.required],
        Description: ['']
      });

      this.mainForm = this.fb.group({
        searchText: new FormControl(''),
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
        groups: new FormControl(''),  family: new FormControl(''), gardenGroup: new FormControl(''),
        //radios
        foliage: new FormControl(''),
        blossomsSize: new FormControl(''),
        leafMargin: new FormControl(''),
        waterReq: new FormControl(''), 
        blossomStat: new FormControl(''),
        leafShape: new FormControl(''), 
        sortTags:new FormControl(''),
        leafPos: new FormControl(''),fruitType: new FormControl(''),
      });

      this.subscriptions.add(this.searchText.pipe(
        map((event: any) => event.target.value),
        debounceTime(1000),
        distinctUntilChanged(),
        mergeMap(search => of(search).pipe(
          delay(500),
        )),
      ).subscribe(()=>this.search(0)));
  }

  

  bindHardiness() {
    // get corresponding index in hardiness slider (array)
    const freezeIndex = this.getFreezeIndex();
    // multiply by 9.09 (100/11 steps) while max allowed value is 100, round to nearest .3
    this.hardinessValue = Math.round(Math.min(freezeIndex * 9.09, 100)*3)/3;
  }

  bindFreezeLvl() {
    const freezeIndex = Math.round(this.hardinessValue / 9.09);
    // recalc zone based on freezeIndex
    this.hardinessValue = Math.round(Math.min(freezeIndex * 9.09, 100)*3)/3;
    const freezeValues = [10,5,0,-5,-10,-15,-20,-25,-30,-35,-40,-45];
    this.mainForm.patchValue({
      freezeLevels: freezeValues[freezeIndex]
    });
  }

  getFreezeIndex() {
    const freezeValues = [10,5,0,-5,-10,-15,-20,-25,-30,-35,-40,-45];
    return freezeValues.indexOf(this.mainForm.value.freezeLevels);
  }
  @HostListener('window:resize')
  onWindowResize() {
    this.width = window.innerWidth;
   
    this.isMobile = window.innerWidth <= 990;
    this.isXsMobile = window.innerWidth <=425;
  }

isAdmin(){
  return this.auth.isAdmin() && this.auth.isLoggedIn();
}

ToggleShowImage(){
  this.isHideSearchResult = !this.isHideSearchResult;
}

ShowImageText(){
  return this.isHideSearchResult ? "Show" : "Hide";
}

  ngOnInit() {


    

    this.isMobile = window.innerWidth <= 990;
    this.isXsMobile = window.innerWidth <=425;
    this.subscriptions.add(this.auth.getUserObservable().subscribe(u => {
      this.user = u;
    }));
    this.currentPage = this.util.getPreviousPage();
    this.subscriptions.add(this.themeProvider.getTheme().subscribe(t => this.mode = t));
    this.subscriptions.add(this.route.queryParams.subscribe(params => {
      this.queryParam = params['qs'];
      this.isHideSearchResult = params["hide"];

    }));

    console.log(this.isHideSearchResult);

    const {ecoSearch, frostSearch, detailSearch} = this.util.getOpenFilters();
    this.detailSearch = detailSearch;
    this.ecoSearch = ecoSearch;
    this.frostSearch = frostSearch;
    this.searchPlant();
    this.setStatus();
  }

  ngAfterViewInit() {
    // set garden marker on map
    this.source = fromEvent(this.something.nativeElement, 'keyup');
    this.source.pipe(debounceTime(1200)).subscribe(c => 
    {
      this.searchText.next(this.something.nativeElement);
              // list = list.filter(item => item.label.toLocaleLowerCase().includes(this.searchedKPI.toLocaleLowerCase())).slice();
    }
    );

    if (!this.isLoggedIn) {
      return;
    }
    // this.subscriptions.add(this.util.getGardenCoords().subscribe((coords) => {
    //   if (LAT_RANGE[0] < coords.Latitude && coords.Latitude < LAT_RANGE[1]
    //     && LONG_RANGE[0] < coords.Longtitude && coords.Longtitude < LONG_RANGE[1]) {
    //       const converter = new UTMConverter();
    //       const utmCoords = converter.toUtm({coord: [coords.Longtitude, coords.Latitude]});
    //       this.coords.Longtitude = utmCoords.coord.x;
    //       this.coords.Latitude = utmCoords.coord.y;
    //       //console.log('before', utmCoords.coord.x, utmCoords.coord.y);
    //       const xCoords = (this.coords.Longtitude) / this.scaleX(this.coords.Longtitude, utmCoords.zone);
    //       const yCoords = (6094791 - this.coords.Latitude) / 291;
    //       //console.log(xCoords, yCoords);
    //       const markerData = `
    //         <circle cx="${xCoords}" cy="${yCoords}" r="30" fill="red" />
    //         <text x="${xCoords + 50}" y="${yCoords + 15}" font-size="50" fill="red">Dein Garten</text>
    //       `;
    //       this.gardenMarker.nativeElement.innerHTML = markerData;
    //   }
    // }));
  }

  scaleX(long, zone) {
    if (zone === 33) {
      return 190;
    }
    const SCALE = [
      {val: 389636, fac: 780},
      {val: 446944, fac: 638},
      {val: 585759, fac: 465},
      {val: 638335, fac: 400},
    ];
    // find closest longtitude to get scale factor
    const closest = SCALE.reduce(function(prev, curr) {
      return (Math.abs(curr.val - long) < Math.abs(prev.val - long) ? curr : prev);
    });
    return closest.fac;
  }

public searchPlant() {
  this.subscriptions.add(this.plantSearch.getTagCats().subscribe(t => {
    this.loading = true;
    this.categories = t; 
    this.populateAvFilters();
    this.plantSearch.getPlantTags().subscribe(t => {
      this.tags = t; 
      this.populateArrays();
      
      this.plantSearch.getGroups().subscribe(t => {
        this.plantSearch.getFamilies().subscribe(fam => {
          this._families = fam.map(f => {
            return {
              displayStr: f, 
              val: f
            };
          });


          this._groups = t.Groups;
          this._gardenGroups = t.GardenGroups;
          this.buildForm(this.plantSearch.getPrevious());
          this.bindHardiness();
          this.loading = false;

          this.search(this.currentPage, true);
        });
      });
    });
  }));
}
  public trackByFn(index, item) {
    return item.Id;
  }

  ngOnDestroy(): void {
    this.util.setPreviousPage(this.currentPage);
    this.util.setOpenFilters(this.ecoSearch, this.frostSearch, this.detailSearch);
    this.subscriptions.unsubscribe();
  }

  user: any;

  get isLoggedIn() {
    return this.auth.isLoggedIn();
  }

  get demoMode() {
    return this.user && this.user.Email.includes('UserDemo');
  }

  toggleStickyCtrls() {
    this.showStickyCtrls = !this.showStickyCtrls;
    // this.manualRefresh.emit();
  }
  
  toggleDetailSearch() {
    this.detailSearch = !this.detailSearch;
    this.manualRefresh.emit();
  }
  toggleSortPlant() {
    this.sortPlant = !this.sortPlant;
    this.manualRefresh.emit();
  }
  toggleEcoSearch() {
    this.ecoSearch = !this.ecoSearch;
    this.manualRefresh.emit();
  }

  toggleFrostSearch() {
    this.frostSearch = !this.frostSearch;
    this.manualRefresh.emit();
  }

  @HostListener('window:scroll', ['$event'])
  handleScroll() {
    const windowScroll = window.pageYOffset;
    if (windowScroll >= 400) {
        this.sticky = true;
    } else {
        this.sticky = false;
    }
    if (windowScroll >= 600) {
      this.stickyBtn = true;
    } else {
        this.stickyBtn = false;
    }
  }

  paginatePlants(page: number) {
    // get pager object from service
    this.plantsPager = this.pagerService.getPager(this.count, page, 10);
    // get current page of items
    this.pagedPlants = this.plants.slice(this.plantsPager.startIndex, this.plantsPager.endIndex + 1);
  }

  search(skip = this.currentPage-1, defaultSearch = false) {
    if (this.queryParam && this.plants.length) {
      this.alert.error('Die Suche muss erstmal zurückgesetzt werden');
      return;
    }
    // this.loading = true;
    if(this.IsSearching == true){
      this.queue = true;
      return;
    }
    this.IsSearching = true;
    this.plantSearch.setPrevious(this.mainForm);
    defaultSearch = false;
    const params = this.queryParam ? new HttpParams({fromString: this.queryParam}) : (defaultSearch ? null : this.populateTagsParam(skip));
    this.subscriptions.add(this.plantSearch.getPlantsCount(params).subscribe(c => {this.count = c; this.paginatePlants(skip+1);}));
    this.subscriptions.add(this.plantSearch.getSearchResults(params).subscribe(t => {
      if (t.Plants != null){
        this.plants = t.Plants;

      }
      else{
        this.plants = [];
      }
      this.sortedPlants = t.ResultSortedByInput;
      if (this.queryParam) {
        this.appliedFilters = t.AppliedFilters ? t.AppliedFilters.map(f =>({pos: f.pos, filter: {t: f.t}})) : [];
      }
      this.superCategories = t.SuperCategories;
      console.log(this.plants);

      if (this.plants != null){
        this.plants.forEach((p,i) => {
          p.PlantUrl = slugify(p.NameGerman || p.NameLatin);
          p.NameLatin = p.NameLatin.replace('[k]', '').replace('[/k]', '');
          if (p.Synonym) { p.Synonym = p.Synonym.replace(/\[k]|\[k]/g, '<i>').replace(/\[\/k\]/g, '</i>'); }
          p.Description = p.Description.replace(/\[k]|\[k]/g, '<i>').replace(/\[\/k\]/g, '</i>');
          if (p.Images[0].Comments != null){
            p.Images[0].Comments.replace('[k]', '').replace('[/k]', '')
          }
          
        });
      }
      if (this.sortedPlants != null){
        this.sortedPlants.forEach((p,i) => {
          p.PlantUrl = slugify(p.NameGerman || p.NameLatin);
          p.NameLatin = p.NameLatin.replace('[k]', '').replace('[/k]', '');
          if (p.Synonym) { p.Synonym = p.Synonym.replace(/\[k]|\[k]/g, '<i>').replace(/\[\/k\]/g, '</i>'); }
          p.Description = p.Description.replace(/\[k]|\[k]/g, '<i>').replace(/\[\/k\]/g, '</i>');
          if (p.Images[0].Comments != null){
            p.Images[0].Comments.replace('[k]', '').replace('[/k]', '')
          }
          
        });
      }
      if (this.superCategories != null){
        this.superCategories.forEach((p,i) => {
          p.NameLatin = p.NameLatin.replace('[k]', '').replace('[/k]', '');
          if (p.Synonym) { p.Synonym = p.Synonym.replace(/\[k]|\[k]/g, '<i>').replace(/\[\/k\]/g, '</i>'); }

          p.Description = p.Description.replace(/\[k]|\[k]/g, '<i>').replace(/\[\/k\]/g, '</i>');
        });
  
      }
     
      console.log("finished searching for plant 3");
      this.IsSearching = false;
      if(this.queue){
        this.queue = false;
        this.currentPage = 1;
        this.search(this.currentPage, true);
      }
      // this.loading = false;
    },
    error => console.log(error, 'PS Component')));
  }

  

  checkFoliage(filter) {
    if (this.mainForm.value.foliage == '') {
      return;
    }
    if (filter != this.mainForm.value.foliage) {
      return;
    }
      this.mainForm.patchValue({
        foliage: ''
      });
    this.search();
  }
  isCollectionCard(images){
    if (images[0].FullTitle == null){
      return false
    }
    if(images[0].FullTitle.includes("Sammelkarte"))
      return true;
    return false;
  }
  checkIfAutor(images){
    if (images[0].Author == null){
      return false
    }
    if(images[0].Author.includes("Autor nicht spezifiziert"))
      return false;
    return true;
  }
  resetSearch() {
    this.mainForm.reset();
    this.queryParam = '';
    this.mainForm.patchValue({
      freezeLevels: 10,
      monthRange: [1,12],
      growthRange: [0,800]
    });
    this.appliedFilters = [];
    this.router.navigate(['/suche']);
    this.searchPlant();
  }

  createList() {
    let params = new HttpParams();
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
    this.subscriptions.add(this.util.createUserList(this.addNewList.value,params).subscribe(lists => {
      this.userLists = lists;
      this.selectedLists = lists.filter(l => l.ListSelected);
      this.addNewList.reset();
    }));
  }

  private sanitize(str: any) {
    if (str) {
      if (str.includes('(')) { str = str.substr(0, str.indexOf(' (')); }
      return str;
    }
    return '';
  }

  private buildForm(previous = null) {
    if (previous && !this.queryParam) {
      this.mainForm = previous;
      return;
    }
    //checkboxes
    this.mainForm.setControl('colors', this.fb.array(this._colors.map(c => new FormControl(false))));
    this.mainForm.setControl('places', this.fb.array(this._places.map(c => new FormControl(false))));
    this.mainForm.setControl('growthType', this.fb.array(this._growthType.map(c => new FormControl(false))));
    this.mainForm.setControl('cropPlant', this.fb.array(this._cropPlant.map(c => new FormControl(false))));
    this.mainForm.setControl('blossoms', this.fb.array(this._blossoms.map(c => new FormControl(false))));
    this.mainForm.setControl('fertil', this.fb.array(this._fertil.map(c => new FormControl(false))));
    this.mainForm.setControl('trim', this.fb.array(this._trim.map(c => new FormControl(false))));
    this.mainForm.setControl('breeding', this.fb.array(this._breeding.map(c => new FormControl(false))));
    this.mainForm.setControl('usage', this.fb.array(this._usage.map(c => new FormControl(false))));
    this.mainForm.setControl('specifics', this.fb.array(this._specifics.map(c => new FormControl(false))));
    this.mainForm.setControl('exclusions', this.fb.array(this._exclusions.map(c => new FormControl(false))));
    this.mainForm.setControl('autumnClr', this.fb.array(this._autumnClr.map(c => new FormControl(false))));
    this.mainForm.setControl('deco', this.fb.array(this._deco.map(c => new FormControl(false))));
    this.mainForm.setControl('flShape', this.fb.array(this._flShape.map(c => new FormControl(false))));
    this.mainForm.setControl('groundType', this.fb.array(this._groundType.map(c => new FormControl(false))));
    this.mainForm.setControl('leafClr', this.fb.array(this._leafClr.map(c => new FormControl(false))));
    this.mainForm.setControl('fruitClr', this.fb.array(this._fruitClr.map(c => new FormControl(false))));
    this.mainForm.setControl('ecoTags', this.fb.array(this._ecoTags.map(c => new FormControl(false))));
    this.loading = false;
    this.formIsBuilt = true;
  }

  private populateAvFilters() {
    this.categories.forEach(c => {
      Object.keys(this.availableFiltersIds).forEach(f => {
        if (f === c.Title) { this.availableFiltersIds[f] = c.Id; }
      });
    });
  }

  private populateArrays() {
    this._places = []; this._waterReq = []; this._usage = [];
    this._blossoms = []; this._breeding = []; this._trim = []; this._growthType = []; this._cropPlant = [];
    this._blossomsSize = []; this._groundType = []; this._fruitType = []; this._fertil = [];
    this._specifics = []; this._exclusions = []; this._autumnClr = []; this._deco = []; this._foliage = [];
    this._flShape = []; this._leafShape = []; this._leafPos = []; this._leafClr = []; this._leafMargin = []; 
    this._blossomStat = []; this._fruitClr = []; this._colors = [];
    
    this.tags.forEach(tag => {
      switch (tag.CategoryId) {
        case this.availableFiltersIds.Licht: this._places.push({t: tag.Title, id: tag.Id}); break;
        case this.availableFiltersIds.Blütenfarben: this._colors.push({t: tag.Title, id: tag.Id}); break;
        case this.availableFiltersIds.Blüten:  this._blossoms.push({t: tag.Title, id: tag.Id}); break;
        case this.availableFiltersIds.Blütengröße: this._blossomsSize.push({t: tag.Title, id: tag.Id}); break;
        case this.availableFiltersIds.Früchte: this._fruitType.push({t: tag.Title, id: tag.Id}); break;
        case this.availableFiltersIds.Boden: this._groundType.push({t: tag.Title, id: tag.Id}); break;
        case this.availableFiltersIds.Düngung: this._fertil.push({t: tag.Title, id: tag.Id}); break;
        case this.availableFiltersIds.Verwendung: this._usage.push({t: tag.Title, id: tag.Id}); break;
        case this.availableFiltersIds.Schnitt: this._trim.push({t: tag.Title, id: tag.Id}); break;
        case this.availableFiltersIds.Vermehrung: this._breeding.push({t: tag.Title, id: tag.Id}); break;
        case this.availableFiltersIds.Wuchs: this._growthType.push({t: tag.Title, id: tag.Id}); break;
        case this.availableFiltersIds.Nutzpflanzen: this._cropPlant.push({t: tag.Title, id: tag.Id}); break;
        case this.availableFiltersIds.Wasserbedarf: this._waterReq.push({t: tag.Title, id: tag.Id}); break;
        case this.availableFiltersIds.Besonderheiten: this._specifics.push({t: tag.Title, id: tag.Id}); break;
        case this.availableFiltersIds.Ausschlusskriterien: this._exclusions.push({t: tag.Title, id: tag.Id}); break;
        case this.availableFiltersIds.Herbstfärbung: this._autumnClr.push({t: tag.Title, id: tag.Id}); break;
        case this.availableFiltersIds.Dekoaspekte: this._deco.push({t: tag.Title, id: tag.Id}); break;
        case this.availableFiltersIds.Laubrhythmus: this._foliage.push({t: tag.Title, id: tag.Id}); break;
        case this.availableFiltersIds.Blütenform: this._flShape.push({t: tag.Title, id: tag.Id}); break;

        case this.availableFiltersIds.Fruchtfarbe : this._fruitClr.push({t: tag.Title, id: tag.Id}); break;
        case this.availableFiltersIds.Blütenstand: this._blossomStat.push({t: tag.Title, id: tag.Id}); break;
        case this.availableFiltersIds.Blattform: this._leafShape.push({t: tag.Title, id: tag.Id}); break;
        case this.availableFiltersIds.Blattfarbe: this._leafClr.push({t: tag.Title, id: tag.Id}); break;
        case this.availableFiltersIds.Blattrand: this._leafMargin.push({t: tag.Title, id: tag.Id}); break;
        case this.availableFiltersIds.Blattstellung: this._leafPos.push({t: tag.Title, id: tag.Id}); break;
      }
    });
    
    this._colors.forEach((c) => {
      c.index = Number.isInteger(parseInt(ColorSort[c.t],10))? ColorSort[c.t] : 99;
    });
    this._colors.sort((a,b) => a.index - b.index);
    this._specifics.sort((a,b)=> a.t.localeCompare(b.t));
    this._usage.sort((a,b) => a.t.localeCompare(b.t));
    this._fruitType.sort((a,b) => a.t.localeCompare(b.t));
    this._flShape.sort((a,b) => a.t.localeCompare(b.t));
    this._growthType.sort((a,b) => a.t.localeCompare(b.t));
    this._cropPlant.sort((a,b) => a.t.localeCompare(b.t));
    this.populated = true;

  }

  private populateTagsParam(skip: number, paramsFromQuery: HttpParams = null): HttpParams {
    let params = !paramsFromQuery ? new HttpParams() : paramsFromQuery;
    skip = (skip-1) * 10;
    this.appliedFilters = [];
    let tags = ''; 
    let ecoloTags = ''; 
    let sortPlantsTags='';
    let freezes = '';
    let excludes = '';
    let leafColors = '';
    let autumnColors = '';
    let colors = '';
    this.mainForm.value.colors.forEach((c,i) => {if (c) {colors = colors+this._colors[i].id+','; this.appliedFilters.push({pos: 'colors', index: i, filter: this._colors[i]});}});
    this.mainForm.value.places.forEach((c,i) => {if (c) {tags = tags+this._places[i].id+','; this.appliedFilters.push({pos: 'places', index: i, filter: this._places[i]});}});
    this.mainForm.value.blossoms.forEach((c,i) => {if (c) {tags = tags+this._blossoms[i].id+','; this.appliedFilters.push({pos: 'blossoms', index: i, filter: this._blossoms[i]});}});
    this.mainForm.value.growthType.forEach((c,i) => {if (c) {tags = tags+this._growthType[i].id+','; this.appliedFilters.push({pos: 'growthType', index: i, filter: this._growthType[i]});}});
    this.mainForm.value.cropPlant.forEach((c,i) => {if (c) {tags = tags+this._cropPlant[i].id+','; this.appliedFilters.push({pos: 'cropPlant', index: i, filter: this._cropPlant[i]});}});
    this.mainForm.value.fertil.forEach((c,i) => {if (c) {tags = tags+this._fertil[i].id+','; this.appliedFilters.push({pos: 'fertil', index: i, filter: this._fertil[i]});}});
    this.mainForm.value.trim.forEach((c,i) => {if (c) {tags = tags+this._trim[i].id+','; this.appliedFilters.push({pos: 'trim', index: i, filter: this._trim[i]});}});
    this.mainForm.value.flShape.forEach((c,i) => {if (c) {tags = tags+this._flShape[i].id+','; this.appliedFilters.push({pos: 'flShape', index: i, filter: this._flShape[i]});}});
    this.mainForm.value.deco.forEach((c,i) => {if (c) {tags = tags+this._deco[i].id+','; this.appliedFilters.push({pos: 'deco', index: i, filter: this._deco[i]});}});
    this.mainForm.value.autumnClr.forEach((c,i) => {if (c) {autumnColors = autumnColors+this._autumnClr[i].id+','; this.appliedFilters.push({pos: 'autumnClr', index: i, filter: this._autumnClr[i]});}});
    this.mainForm.value.exclusions.forEach((c,i) => {if (c) {excludes = excludes+this._exclusions[i].id+','; this.appliedFilters.push({pos: 'exclusions', index: i, filter: this._exclusions[i]});}});
    this.mainForm.value.breeding.forEach((c,i) => {if (c) {tags = tags+this._breeding[i].id+','; this.appliedFilters.push({pos: 'breeding', index: i, filter: this._breeding[i]});}});
    this.mainForm.value.specifics.forEach((c,i) => {if (c) {tags = tags+this._specifics[i].id+','; this.appliedFilters.push({pos: 'specifics', index: i, filter: this._specifics[i]});}});
    this.mainForm.value.leafClr.forEach((c,i) => {if (c) {leafColors = leafColors+this._leafClr[i].id+','; this.appliedFilters.push({pos: 'leafClr', index: i, filter: this._leafClr[i]});}});
    this.mainForm.value.fruitClr.forEach((c,i) => {if (c) {tags = tags+this._fruitClr[i].id+','; this.appliedFilters.push({pos: 'fruitClr', index: i, filter: this._fruitClr[i]});}});
    this.mainForm.value.groundType.forEach((c,i) => {if (c) {tags = tags+this._groundType[i].id+','; this.appliedFilters.push({pos: 'groundType', index: i, filter: this._groundType[i]});}});
    this.mainForm.value.usage.forEach((c,i) => {if (c) {tags = tags+this._usage[i].id+','; this.appliedFilters.push({pos: 'usage', index: i, filter: this._usage[i]});}});
    freezes = this.freezeLevelsIds[this.getFreezeIndex()] || '';
    if (freezes) {
      this.appliedFilters.push({pos: 'freezeLevels', filter: {t:'Frosthärte'}});
    }
    this.mainForm.value.ecoTags.forEach((c,i) => {
      if (c) {
        ecoloTags += this._ecoTags[i].id +',';
        this.appliedFilters.push({pos: 'ecoTags', index: i, filter: this._ecoTags[i]});
      }
    });
    //radios
    if (this.mainForm.value.blossomsSize) {tags = tags+this.mainForm.value.blossomsSize.id+','; this.appliedFilters.push({pos: 'blossomsSize', filter: this.mainForm.value.blossomsSize});}
    if (this.mainForm.value.foliage) {tags = tags+this.mainForm.value.foliage.id+','; this.appliedFilters.push({pos: 'foliage', filter: this.mainForm.value.foliage});}
    if (this.mainForm.value.leafMargin) {tags = tags+this.mainForm.value.leafMargin.id+','; this.appliedFilters.push({pos: 'leafMargin', filter: this.mainForm.value.leafMargin});}

    if (this.mainForm.value.waterReq) {tags += this.mainForm.value.waterReq.id+',';this.appliedFilters.push({pos: 'waterReq', filter: this.mainForm.value.waterReq});}
    if (this.mainForm.value.sortTags) {sortPlantsTags += this.mainForm.value.sortTags.id+',';this.appliedFilters.push({pos: 'sortTags', filter: this.mainForm.value.sortTags});}
    if (this.mainForm.value.fruitType) {tags += this.mainForm.value.fruitType.id+',';this.appliedFilters.push({pos: 'fruitType', filter: this.mainForm.value.fruitType});}
    if (this.mainForm.value.leafShape) {tags += this.mainForm.value.leafShape.id+',';this.appliedFilters.push({pos: 'leafShape', filter: this.mainForm.value.leafShape});}
    if (this.mainForm.value.blossomStat) {tags += this.mainForm.value.blossomStat.id+',';this.appliedFilters.push({pos: 'blossomStat', filter: this.mainForm.value.blossomStat});}
    if (this.mainForm.value.leafPos) {tags += this.mainForm.value.leafPos.id+',';this.appliedFilters.push({pos: 'leafPos', filter: this.mainForm.value.leafPos});}
    if (this.mainForm.value.growthRange && (this.mainForm.value.growthRange[0] !== 0||this.mainForm.value.growthRange[1] !== 800)) {
      this.appliedFilters.push({pos: 'growthRange', filter:{t:`${this.mainForm.value.growthRange[0]} cm - ${this.mainForm.value.growthRange[1]} cm`}});
      
    }
    if (this.mainForm.value.monthRange && (this.mainForm.value.monthRange[0] !== 1||this.mainForm.value.monthRange[1] !== 12)) {
      this.appliedFilters.push({pos: 'monthRange', filter:{t:`Blühdauer Monat ${this.mainForm.value.monthRange[0]} bis ${this.mainForm.value.monthRange[1]}`}});
     
    }
    if (this.mainForm.value.groups) {this.appliedFilters.push({pos: 'groups', filter: this.mainForm.value.groups});}
    if (this.mainForm.value.gardenGroup) {this.appliedFilters.push({pos: 'gardenGroup', filter: this.mainForm.value.gardenGroup});}
    if (this.mainForm.value.family) {this.appliedFilters.push({pos: 'family', filter: {Name: this.mainForm.value.family.replace(/\[k\]/g,'').replace(/\[\/k\]/g,'').replace('[','').replace(']', '')}});}
    if (this.mainForm.value.searchText) {this.appliedFilters.push({pos: 'searchText', filter: {t: this.mainForm.value.searchText}});}
    params = params.append('searchText', this.sanitize(this.mainForm.value.searchText));
    params = params.append('cookieTags', tags.replace(/\,$/, ''));
    params = params.append('sortTags', sortPlantsTags.replace(/\,$/, ''));
    params = params.append('ecosTags', ecoloTags.replace(/\,$/, ''));
    params = params.append('selHmin', this.mainForm.value.growthRange[0]);
    params = params.append('groupId', this.mainForm.value.groups == null ? null : this.mainForm.value.groups.Id);
    params = params.append('family', this.mainForm.value.family == null ? '' : this.mainForm.value.family);
    params = params.append('gardenGroup', this.mainForm.value.gardenGroup == null ? '' : this.mainForm.value.gardenGroup.Id);
    params = params.append('selHmax', this.mainForm.value.growthRange[1]);
    params = params.append('freezes', freezes.replace(/\,$/, ''));
    params = params.append('excludes', excludes.replace(/\,$/, ''));
    params = params.append('colors', colors.replace(/\,$/, ''));
    params = params.append('leafColors', leafColors.replace(/\,$/, ''));
    params = params.append('autumnColors', autumnColors.replace(/\,$/, ''));
    // params = params.append('ecotags', ecoTagsStr.replace(/\,$/, ''));
    params = params.append('take', '10');
    params = params.append('skip', skip.toString());
    params = params.append('selMinMonth', this.mainForm.value.monthRange[0]);
    params = params.append('selMaxMonth', this.mainForm.value.monthRange[1]);
    //params = params.append('months', this.stringifyRange(this.mainForm.value.monthRange));

    return params;
  }

  public removeFilter(filter: any) {
    if (this.queryParam) {
      this.alert.error('Die Suche muss erstmal zurückgesetzt werden');
      return;
    }
    const key = filter.pos; const index = filter.index;
    var undefined = void(0);
    if (index !== undefined) {
      this.mainForm.setControl(key, this.fb.array(this['_'+key].map((c,i) => {
        return (i === index)? new FormControl(false) : new FormControl(this.mainForm.value[key][i]);
      })));
    } else if (key === 'growthRange') {
      this.mainForm.setControl(key, new FormControl([0,800]));
    } else if (key === 'freezeLevels') {
      this.mainForm.setControl(key, new FormControl(10));
      this.hardinessValue = 0;
    } else if (key === 'monthRange') {
      this.mainForm.setControl(key, new FormControl([1,12]));
    } else {
      this.mainForm.setControl(key, new FormControl(''));
    }
    this.search(0);
  }

  sortPlants() {
    this.plants.sort((a, b) => a.NameGerman.localeCompare(b.NameGerman));
    this.paginatePlants(1);
  }
  setStatus() {
    (this.selectedLists.length > 0) ? this.requiredField = true : this.requiredField = false;
  }

  onItemSelect(item: any) {
    //Do something if required
    const ddl = document.getElementsByClassName('multiselect-dropdown')[0].children[1];
    ddl.setAttribute('hidden', 'false');

    this.setClass();
  }
  onSelectAll(items: any) {
    //Do something if required
    this.setClass();
  }

  setClass() {
    this.setStatus();
    if (this.selectedLists.length > 0) { return 'validField' }
    else { return 'invalidField' }
  }
  borrowPlant() {
    let alertTriggered = false;
    this.userPlantTrigger = [];
    if(this.selectedLists.length>0){
      this.addForm.patchValue({
        PlantId: this.selectedPlant.Id,
        InitialAgeInDays: 1
      });
      this.selectedLists.forEach(e=>{
        const temp = {
          UserPlantId: 1,
          UserListId: e.Id
        };
        this.userPlantTrigger.push(temp);
      })
      this.addForm.patchValue({
        ArrayOfUserlist:this.userPlantTrigger
      });
        this.subscriptions.add(this.util.addUserPlantToProp(JSON.stringify(this.addForm.value)).subscribe(
          (res) => {
            if (!alertTriggered) {
              alertTriggered = true;
              this.alert.success('Pflanze wurde dem Garten hinzugefügt');
            }
              this.selectedPlant.IsInUserGarden = true;

            this.counter.requestPlantsCount().subscribe(data => this.counter.setPlantsCount(data));

            // wait on (async) todo population
            setTimeout(() => {
              this.counter.requestTodosCount().subscribe(data=>this.counter.setTodosCount(data));
            }, 500);
          }));
     
      this.addForm.patchValue({
        PlantId: null,
        InitialAgeInDays: 0,
        Count: 1,
        IsInPot: false,
        Todos: null
      });
    } else {
      this.alert.error('Bitte Liste auswählen!');
    }

  }

  selectPlant(plant: any) {
    this.selectedPlant = plant;
    $(window).on('hide.bs.modal', () => {
      this.addForm.patchValue({
        PlantId: null,
        InitialAgeInDays: 0,
        Count: 1,
        IsInPot: false,
        Todos: null
      });
    }); 
  }

  toUrl(url, small = true) {
    return this.util.toUrl(url, small);
  }

  getPlantLists() {
      this.subscriptions.add(this.util.getPlantLists(this.selectedPlant.Id).subscribe(lists => {
        this.userLists = lists;
        this.selectedLists = lists.filter(l => l.ListSelected);
      }));
  }

  // this keeps the form of html tags
  public decodeEntities(value: string): string {
    if (!value) { return ''; }
      
    this.hiddenPre.innerHTML = value.replace(/</g,'&lt;');
    // innerText depends on styling as it doesn't display hidden elements.
    // Therefore, it's better to use textContent not to cause unnecessary reflows.
    return this.hiddenPre.textContent;
  }

  // this removes all html tags
  public decodeHtml(text: string): string {
    let ret = '';
    this.dummyElem.innerHTML = text;
    document.body.appendChild(this.dummyElem);
    ret = this.dummyElem.textContent; // just grap the decoded string which contains the desired HTML tags
    document.body.removeChild(this.dummyElem);

    return ret;
  }

  removeExcludeExtraText(i){
    return this._exclusions[i].t;
  }
  
  hasBee(plant) {
    return plant.Badges.filter(b => this.BADGES.Bees.Id.includes(b.Id)).length > 0;
  }
  hasBird(plant) {
    return plant.Badges.filter(b => this.BADGES.Birds.Id.includes(b.Id)).length > 0;
  }
  hasBio(plant) {
    return plant.Badges.filter(b => this.BADGES.Bio.Id.includes(b.Id)).length > 0;
  }
  hasInsect(plant) {
    return plant.Badges.filter(b => this.BADGES.Insects.Id.includes(b.Id)).length > 0;
  }
  hasButterfly(plant) {
    return plant.Badges.filter(b => this.BADGES.Butterflies.Id.includes(b.Id)).length > 0;
  }
  hasDomestic(plant) {
    return plant.Badges.filter(b => this.BADGES.Domestic.Id.includes(b.Id)).length > 0;
  }
  saveQueries(appliedFilters:any[]) {
    this.loading = true;
    const params = this.populateTagsParam(0);
    
    if(appliedFilters.length>0) {
      this.subscriptions.add(this.util.saveSearchQueries(params.toString()).subscribe());
      this.loading = false;
    } else {
      return this.alert.error('Keine Suchkriterien angewendet');
    }
  }
  // helper methods
  get places() { return this.mainForm.get('places') as FormArray; }
  get colors() { return this.mainForm.get('colors') as FormArray; }
  get groundType() { return this.mainForm.get('groundType') as FormArray; }
  get blossoms() { return this.mainForm.get('blossoms') as FormArray; }
  get blossomsSize() { return this.mainForm.get('blossomsSize') as FormArray; }
  get growthType() { return this.mainForm.get('growthType') as FormArray; }
  get cropPlant() { return this.mainForm.get('cropPlant') as FormArray; }
  get trim() { return this.mainForm.get('trim') as FormArray; }
  get fertil() { return this.mainForm.get('fertil') as FormArray; }
  get breeding() { return this.mainForm.get('breeding') as FormArray; }
  get usage() { return this.mainForm.get('usage') as FormArray; }
  get specifics() { return this.mainForm.get('specifics') as FormArray; }
  get exclusions() { return this.mainForm.get('exclusions') as FormArray; }
  get autumnClr() { return this.mainForm.get('autumnClr') as FormArray; }
  get deco() { return this.mainForm.get('deco') as FormArray; }
  get fruitClr() { return this.mainForm.get('fruitClr') as FormArray; }
  get leafMargin() { return this.mainForm.get('leafMargin') as FormArray; }
  get leafClr() { return this.mainForm.get('leafClr') as FormArray; }
  get flShape() { return this.mainForm.get('flShape') as FormArray; }
  get foliage() { return this.mainForm.get('foliage') as FormArray; }
  get fruitType() { return this.mainForm.get('fruitType') as FormArray; }
  get blossomStat() { return this.mainForm.get('blossomStat') as FormArray; }
  get leafPos() { return this.mainForm.get('leafPos') as FormArray; }
  get leafShape() { return this.mainForm.get('leafShape') as FormArray; }
  get waterReq() { return this.mainForm.get('waterReq') as FormArray; }
  get sortTags() { return this.mainForm.get('sortTags') as FormArray; }
  get ecoTags() { return this.mainForm.get('ecoTags') as FormArray; }
}
