import { Component, OnInit, ChangeDetectorRef, HostListener, OnDestroy, ViewChild } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { UtilityService } from 'src/app/services/utility.service';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faChevronDown, faTimes, faCircle, faDotCircle, faCameraRetro, faEllipsisH, faInfo, faSearch , faPencilAlt, faEye, faUndo, faRedo, faImages, faTh, faStar} from '@fortawesome/free-solid-svg-icons';
import { UserPlant, UserList, Ad, UserActionsFrom } from 'src/app/models/models';
import { Subject, Subscription } from 'rxjs';
import { StatCounterService } from 'src/app/services/stat-counter.service';
import { environment } from 'src/environments/environment';
import { debounce } from 'src/app/guards/debounce.decorator';
import { AlertService } from 'src/app/services/alert.service';
import { AuthService } from 'src/app/services/auth.service';
import { ScannerService } from 'src/app/services/scanner.service';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { slugify } from 'src/app/services/plant-search.service';
import * as Cropper from 'cropperjs/dist/cropper';
import { HttpParams } from '@angular/common/http';
import { param } from 'jquery';
import { EditorChangeContent, EditorChangeSelection } from 'ngx-quill';
import { MyDevicesComponent } from '..';

@Component({
  selector: 'app-mygarden-details',
  templateUrl: './mygarden-details.component.html',
  styleUrls: ['./mygarden-details.component.css']
})
export class MygardenDetailsComponent implements OnInit, OnDestroy {
  @ViewChild(MyDevicesComponent, { static: false }) deviceComponent: MyDevicesComponent;
  mode: string; baseUrl = environment.gardifyBaseUrl;
  gpBaseUrl = 'https://gardify.de/intern/';
  id: number; faChevronDown = faChevronDown; faCameraRetro = faCameraRetro; faEye = faEye; faImages = faImages; faGrid = faTh;
  faDotCircle=faDotCircle; faCircle=faCircle; faTimes=faTimes; faMenu = faEllipsisH;faInfo = faInfo; faPencilAlt=faPencilAlt;
  faStar = faStar;
  faUndo = faUndo; 
  faRedo = faRedo;
  faSearch=faSearch;
  selectedTodo;
  dropdownList = [];
  selectedItems = [];
  dropdownSettings : IDropdownSettings = {
    singleSelection: true,
    idField: 'Id',
    textField: 'Name',
    selectAllText: 'Alle markieren',
    unSelectAllText: 'Alle entmarkieren',
    itemsShowLimit: 10,
    allowSearchFilter: false
  };
  editForm: FormGroup;
  newList: FormGroup;
  newUserPlantTrigger:FormGroup;
  selectedPlant: UserPlant;
  refresh: Subject<any> = new Subject();
  displayedPlants: any; 
  selectedCountList: any;
  reader = new FileReader();
  garden:any={};
  today: string ;
  todosFallig: any[];
  userPlantTrigger: any[]=[];
  show = true;
  showLists = false;
  showAddMenu = false;
  showAddDevMenu=false;
  loading=true;
  imgObj = {
    img: null,
    Title: null,
    Src: null,
    dateCreated: null,
    note: null,
    tags: null,
    description: null
  };
  showFilters = false;
  ecoSearch = false;
  appliedFilters: any[] = [];
  subs = new Subscription();
  invalidImg: boolean;
  hiddeFilter=false;
  sortMode: boolean;
  lists: UserList[] = [];
  count:number;
  currentList:any;
  currentListName;
  routerId;
  routerName;
  showPlantInList:boolean;
  updatePlantMode: string; // either 'count' or 'notes'
  selectedList: any;
  userLists = [];
  selectedLists = [];
    apiCallFrom=new UserActionsFrom();

  public myRegex4= new RegExp(/\{k]|\[k]|\[\/k]|\[\/K]/g);
  public italicRegex= new RegExp(/\{k]|\[k]|\[K]([\s\S]+?)\[\/k]|\[\/K]|\[\/K],/g);
      replacegrp="<i>$1</i>"
      
  gardenBadges = {
    Bees: {Count: 0, Id: [447], checked: false, text: 'Bienenfreundlich'},
    Birds: {Count: 0, Id: [320,322], checked: false, text: 'Vogelfreundlich'},
    Insects: {Count: 0, Id: [321], checked: false, text: 'Insektenfreundlich'},
    Bio: {Count: 0, Id: [445], checked: false, text: 'Ökologisch wertvoll'},
    Butterflies: {Count: 0, Id: [531], checked: false, text: 'Schmetterlings freundlich'},
    Domestic: {Count: 0, Id: [530], checked: false, text: 'Heimische Pflanze'},
    WaterSaving:{Count:0, Id:[346],checked:false,text:'Wassersparende Pflanzen'}
  };
  plantsCount = {};
  selectedListIds: any=[];
  listTotalItem: any;
  imgInView = 0;
  shouldSlide;
  totalSort: any;
  subscriptions= new Subscription();
  ecoCount: number;
  frameOptions = {
    stroke: 'none',
    color: 'white'
  };
  STROKE_OPTIONS = {
    'none': 0,
    'framed thin': 20,
    'framed normal': 40,
    'framed thick': 70,
  };

  searchFilters = {
    toxic: { Id: [561], checked: false, text: 'stark giftig',Count: 0 },
    partToxic: { Id: [315], checked: false, text: 'bedingt giftig',Count: 0 },
    freezeLvl0: { Id: [294,295], checked: false, text: 'nicht frosthart',Count: 0 },
    freezeLvl1: { Id: [293], checked: false, text: 'frosthart bis -5°C',Count: 0 },
    freezeLvl2: { Id: [292], checked: false, text: 'frosthart bis -10°C' ,Count: 0},
    freezeLvl3: { Id: [285,286,287,288,289,290,291], checked: false, text: 'voll frosthart',Count: 0 }
  };
  cropper: any;
  toxiSearch: boolean;
  userPlants: any[];
  sortedImages: any[];
  setImagesSortIndex: any[]=[];
  showStickyCtrls = true;
  countActiveTodos=0;
  slideShow: boolean=true;
  gardenImageModal:any;
  ad: Ad;
  adBetween: Ad;
  adSides: Ad;
  master = 'Master';
  blured = false
  focused = false
  galerieView=false
  showFilterMenu=false
  showSortMenu=false
  showSearchBar=false
  showShareMenu=false
  addAdminDevice=false
  addUserDevice=false
  listOfDevices: any;
  deviceCount: number;
  selectedDevices=[]
  constructor(
    private tp: ThemeProviderService, 
    private util: UtilityService, 
    private fb: FormBuilder, 
    private scanner: ScannerService,
    private routeParam:ActivatedRoute,
    private counter: StatCounterService, 
    private cd: ChangeDetectorRef, 
    private alert: AlertService,
    private authService: AuthService,
    
    public router: Router) { 
      
      const now = new Date;
      this.editForm = this.fb.group({
        Name: [null, Validators.required],
        Description: [null, Validators.required],
        CardinalDirection: [],
        ShadowStrength: [],
        Inside: new FormControl(false),
        GroundType: [],
        PhType: [],
        Wetness: [],
        Id: -1
      });
      this.newList = this.fb.group({
        Name: [null, Validators.required],
        Description: [''],
        GardenId: null
      });
      this.today =now.toISOString();
      this.newUserPlantTrigger=this.fb.group({
        PlantId:[''],
        UserListId:[''],
        GardenId:[''],
        UserPlantId:[''],
      });
       this.ad = new Ad(
        'ca-pub-3562132666777902',
        1376807418,
        'auto',
        true,
        false,
        null 
      ) ;
      this.adSides = new Ad(
        'ca-pub-3562132666777902',
        2395831882,
        'auto',
        true,
        true,
        null
      ) ;
    }
   editor = {
      toolbar: [
        ['bold', 'italic', 'underline', 'strike'],        // toggled buttons
        ['blockquote', 'code-block'],
    
        [{ 'header': 1 }, { 'header': 2 }],               // custom button values
        [{ 'list': 'ordered'}, { 'list': 'bullet' }],
        [{ 'script': 'sub'}, { 'script': 'super' }],      // superscript/subscript
        [{ 'indent': '-1'}, { 'indent': '+1' }],          // outdent/indent
        [{ 'direction': 'rtl' }],                         // text direction
    
        [{ 'size': ['small', false, 'large', 'huge'] }],  // custom dropdown
        [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
    /* 
        [{ 'color': [] }, { 'background': [] }],          // dropdown with defaults from theme
        [{ 'font': [] }],
        [{ 'align': [] }],
    
        ['clean'],                                         // remove formatting button */
    
        ['link']                         // link and image, video
      ]
    };

  scrollTo(index) {
    const wrapper = document.getElementsByClassName('slide-img-wrapper')[index];
    if (wrapper) {
      wrapper.scrollIntoView({behavior: 'smooth', block: 'nearest', inline: 'nearest'});
      this.imgInView = index;
    } else {
      clearInterval(this.shouldSlide);
    }
  }
  scrollleft() {
    if (this.imgInView > 0) {
      document.getElementsByClassName('slide-img-wrapper')[--this.imgInView]
      .scrollIntoView({behavior: 'smooth', block: 'nearest', inline: 'nearest'});
      clearInterval(this.shouldSlide);
    }
  }
  scrollright() {
    if (this.imgInView < this.garden.Images.length-1) {
      document.getElementsByClassName('slide-img-wrapper')[++this.imgInView]
      .scrollIntoView({behavior: 'smooth', block: 'nearest', inline: 'nearest'});
      clearInterval(this.shouldSlide);
    }
  }

  @HostListener('window:scroll', ['$event'])
  @debounce()
  handleScroll() {
    const windowScroll = window.pageYOffset;
    if (windowScroll < 500) {
      clearInterval(this.shouldSlide);
      this.startSlideShow();
    } else {
      clearInterval(this.shouldSlide);
    }
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
    clearInterval(this.shouldSlide);
  }

  ngOnInit() {
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
    this.subs.add(this.routeParam.params.subscribe(param => {
      this.routerId=param.id;
      this.routerName=param.name;
    }));
    this.loading=true;
      this.setCheckedEcoElementCount();
      this.getDevicesCount()
      this.invalidImg = true;
      let params = new HttpParams();
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
     this.subs.add(this.util.getUserMainGarden(params).subscribe(t => {
      this.id = t.Id;
      this.garden = t;
      this.setPlantsCount();
      if(this.routerId ) {
        this.currentList=this.routerName;
      } else {
       this.getUserPlants();
      }
      this.newList.patchValue({
        GardenId: this.id
      });
     
      this.subs.add(this.util.getUserLists().subscribe(lists => {
        this.dropdownList=lists;
        this.lists = lists;
      }));
      this.cd.detectChanges();
    })); 
    this.loading=false
    this.showPlantInList=false;
    this.getAdminDevices()
  }

  setPlantsCount() {
    if(this.isLoggedIn) {
      this.subs.add(this.counter.plantsCount$.subscribe(count => {
        this.plantsCount = count;
      }));
    } else {
      this.plantsCount= {Sorts:0,Total:0};
    }
    
  }
  getAdminDevices(){
    this.subs.add(this.util.getAdminDevices().subscribe(d=>{
     this.listOfDevices=d;
   }))
  }
  get isLoggedIn() {
    return this.authService.isLoggedIn();
  }
  setCheckedEcoElementCount() {
    this.counter.requestEcoCount().subscribe(()=> {
      this.counter.ecoCount$.subscribe(count=> {
        this.ecoCount=count;
      });
    });
    
  }
  getDevicesCount(){
    this.counter.requestDevicesCount().subscribe(()=>{
      this.counter.deviceCount$.subscribe(c=>{
        this.deviceCount=c
      })
    })
  }
  getUserPlants() {
    this.subs.add(this.util.getUserPlants().subscribe(p=> {
      this.userPlants = p;
      this.userPlants.forEach((p,i) => {
        p.UserPlant.PlantUrl = slugify(p.UserPlant.Name);
        if (p.UserPlant.Synonym) { p.UserPlant.Synonym = p.UserPlant.Synonym.replace(/\{k]|\[k]|\[K]/g, '<i>').replace(/\[\/k\]|\[\/K\]/g, '</i>'); }
        p.Description = p.UserPlant.Description.replace(/\{k]|\[k]|\[K]/g, '<i>').replace(/\[\/k\]|\[\/K\]/g, '</i>');
       
      });
      
      this.displayedPlants = this.userPlants;
      this.sortTodos();
      this.setGardenBadges();
      this.loading=false;
      this.startSlideShow();
    }));
  }
  setGardenBadges() {
    this.resetBadges();
    this.displayedPlants.forEach(pl => {
      pl.UserPlant.Badges.forEach(badge => {
        for (const [key, value] of Object.entries(this.gardenBadges)) {
          if (this.gardenBadges[key].Id.includes(badge.Id)) {
            this.gardenBadges[key].Count+= pl.UserPlant.Count;
          }
        }
        for (const [key, value] of Object.entries(this.searchFilters)) {
          if (this.searchFilters[key].Id.includes(badge.Id)) {
            this.searchFilters[key].Count+= pl.UserPlant.Count;
          }
        }
      });
      
    });
  }

  toggleStickyCtrls() {
    this.showStickyCtrls = !this.showStickyCtrls;
    // this.manualRefresh.emit();
  }

  resetBadges() {
    this.gardenBadges = {
      Bees: {Count: 0, Id: [447], checked: false, text: 'Bienenfreundlich'},
      Birds: {Count: 0, Id: [320,322], checked: false, text: 'Vogelfreundlich'},
      Insects: {Count: 0, Id: [321], checked: false, text: 'Insektenfreundlich'},
      Bio: {Count: 0, Id: [445], checked: false, text: 'Ökologisch wertvoll'},
      Butterflies: {Count: 0, Id: [531], checked: false, text: 'Schmetterlings freundlich'},
      Domestic: {Count: 0, Id: [530], checked: false, text: 'Heimische Pflanze'},
      WaterSaving:{Count:0, Id:[346],checked:false,text:'Wassersparende Pflanzen'}
    };
  }

  startSlideShow() {
    if (this.garden && this.garden.Images && this.garden.Images.length > 1 && window.pageYOffset < 500 && !this.shouldSlide) {
      const upperBound = this.garden.Images.length - 1;
      this.shouldSlide =  setInterval(() => {
        switch(this.imgInView) {
          case upperBound: this.scrollTo(0); break;
          default: this.scrollTo(++this.imgInView); break;
        }
      }, 10000);
    }
  }

  sortTodos() {
    this.displayedPlants.forEach(p => {
      if (p.UserPlant.CyclicTodos) {
        p.UserPlant.CyclicTodos.sort((a,b) => (this.isActive(a) === this.isActive(b)) ? 0 : this.isActive(a)? 1 : -1);
      }
    });
  }

  hasBee(plant) {
    return plant.Badges.filter(b => this.gardenBadges.Bees.Id.includes(b.Id)).length > 0;
  }
  hasBird(plant) {
    return plant.Badges.filter(b => this.gardenBadges.Birds.Id.includes(b.Id)).length > 0;
  }
  hasBio(plant) {
    return plant.Badges.filter(b => this.gardenBadges.Bio.Id.includes(b.Id)).length > 0;
  }
  hasInsect(plant) {
    return plant.Badges.filter(b => this.gardenBadges.Insects.Id.includes(b.Id)).length > 0;
  }
  hasButterfly(plant) {
    return plant.Badges.filter(b => this.gardenBadges.Butterflies.Id.includes(b.Id)).length > 0;
  }
  hasDomestic(plant) {
    return plant.Badges.filter(b => this.gardenBadges.Domestic.Id.includes(b.Id)).length > 0;
  }
  hasWaterSaving(plant){
    return plant.Badges.filter(b => this.gardenBadges.WaterSaving.Id.includes(b.Id)).length > 0;
  }

  public filter(defaultSearch = false) {
    this.router.navigate(['/meingarten']);
    this.loading=true;

    const selectedBadges = Object.keys(this.gardenBadges).filter(o => this.gardenBadges[o].checked).map(f => this.gardenBadges[f].Id);
    const selectedFilters = Object.keys(this.searchFilters).filter(o => this.searchFilters[o].checked).map(f => this.searchFilters[f].Id);
    const query = [].concat.apply([], selectedBadges.concat(selectedFilters));

    // filter total plants based on query
    this.displayedPlants = (query.length > 0) ? this.userPlants.filter(u => this.intersect(u.UserPlant.Badges.map(b => b.Id), query).length > 0) : this.userPlants;
    this.listTotalItem = this.displayedPlants.length;
    this.count = this.displayedPlants.length;

    this.updateAppliedFilters();
    
    this.loading = false;
  }

  private updateAppliedFilters() {
    this.appliedFilters = [];
    Object.keys(this.gardenBadges).forEach(badge => {
      if (this.gardenBadges[badge].checked) {
        this.appliedFilters.push({badgeId:this.gardenBadges[badge].Id, Name:this.gardenBadges[badge].text});
      }
    });
    Object.keys(this.searchFilters).forEach(filter => {
      if (this.searchFilters[filter].checked) {
        this.appliedFilters.push({filterId:this.searchFilters[filter].Id, Name:this.searchFilters[filter].text});
      }
    });
  }

  removeFilter(filterObject:any) {
    if(filterObject.filterId) {
      this.appliedFilters = this.appliedFilters.filter(l => l.filterId !== filterObject.filterId);
      Object.keys(this.searchFilters).forEach(f => {
        if (this.searchFilters[f].Id === filterObject.filterId) {
          this.searchFilters[f].checked = false;
        }
      });
    } else if (filterObject.listId) {
      this.appliedFilters = this.appliedFilters.filter(l => l.listId !== filterObject.listId);
      this.currentListName = 'Pflanzen';
      this.displayedPlants = this.userPlants;
      this.currentList = null;
      this.filter();
    } else {
      if(filterObject.badgeId) {
        this.appliedFilters = this.appliedFilters.filter(l => l.badgeId !== filterObject.badgeId);
        Object.keys(this.gardenBadges).forEach(b => {
          if (this.gardenBadges[b].Id === filterObject.badgeId) {
            this.gardenBadges[b].checked = false;
          }
        });
      }
    }
    this.filter();
  }

  resetSearch() {
    Object.keys(this.searchFilters).forEach(k => this.searchFilters[k].checked = false);
    this.appliedFilters = [];
    this.displayedPlants = this.userPlants;
  }

  viewMyPlants() {
    this.loading=true;
    this.displayedPlants = this.userPlants;
    this.currentListName='Meine Pflanzen';
    this.listTotalItem=this.displayedPlants.length;
    this.sortTodos();
    this.setGardenBadges();
    this.setPlantsCount();
    this.resetSearch();
    this.loading=false;
  }

  navigateToMyPlants() {
    this.router.navigate(['/meingarten']);
    this.currentListName = 'Meine Pflanzen';
    //this.viewMyPlants();
    this.getUserPlants()
  }

  viewList(list: UserList) {
    this.navigateToMyPlants();
    this.loading=true;
    this.displayedPlants=this.userPlants
    // this.appliedFilters.push({listId:list.Id, Name:list.Name});
    this.displayedPlants = this.displayedPlants.filter(p => p.ListId === list.Id);
    this.sortTodos();
    this.currentListName=list.Name;
    this.listTotalItem= this.displayedPlants.length;
    this.count = this.displayedPlants.length;
    // l.UserPlantsList? this.setGardenBadges():'';
    this.loading=false;
  }

  viewListWithListId(list: UserList) {
    this.router.navigate(['/meingarten']);
    this.loading=true;
    this.displayedPlants=null;
    this.util.getUserPlantByUserListId(list.Id).subscribe(l=> {
     this.displayedPlants =l;
     this.displayedPlants.forEach((p,i) => {
      p.UserPlant.PlantUrl = slugify(p.UserPlant.Name);
      if (p.UserPlant.Synonym) { p.UserPlant.Synonym = p.UserPlant.Synonym.replace(/\{k]|\[k]|\[K]/g, '<i>').replace(/\[\/k\]|\[\/K\]/g, '</i>'); }
      p.Description = p.UserPlant.Description.replace(/\{k]|\[k]|\[K]/g, '<i>').replace(/\[\/k\]|\[\/K\]/g, '</i>');
     
    });
     this.sortTodos();
     this.currentListName=list.Name;
     this.listTotalItem=this.displayedPlants.length;
     this.count = this.displayedPlants.length;
     this.selectedList=list;
     
     this.loading=false;
    });
  }

  createNewList() {
    let params = new HttpParams();
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
    this.subs.add(this.util.createUserList(this.newList.value,params).subscribe(lists => {
      this.lists = lists;
      this.dropdownList = lists;
      this.newList.reset();
      this.newList.patchValue({
        GardenId: this.id
      });
    }));
  }

  deleteUserList(list:UserList) {
    this.subs.add(this.util.deleteUserList(this.id,list.Id).subscribe(()=> {
      this.lists=this.lists.filter(l=>l.Id!==list.Id);
      this.subs.add(this.util.getUserPlants().subscribe(p=> {
        this.userPlants = this.userPlants.filter(u=>!u.ListIds.includes(list.Id));
        this.displayedPlants = this.displayedPlants.filter(u=>!u.ListIds.includes(list.Id));
        this.navigateToMyPlants()
       
      }));
    }));
  }
  setDropdownList(){
    this.dropdownList=this.lists.filter(u=>u!==this.selectedList)
  }
  deleteUserPlantInUserList(userPlantId, listId) {
    this.loading=true;
    this.subs.add(
      this.util.deleteUserPlantFromUserList(userPlantId, listId)
                .subscribe(()=> {
                  this.displayedPlants = this.displayedPlants.filter(p => !(p.UserPlant.Id === userPlantId && p.ListIds.includes(listId)));
                  this.listTotalItem=this.displayedPlants.length;
                  this.setGardenBadges();
                  this.counter.requestPlantsCount().subscribe((data) => this.counter.setPlantsCount(data));
                  this.counter.requestTodosCount().subscribe((data) => this.counter.setTodosCount(data));
                  this.loading=false;
                }));
  }
  deleteUserPlantFromAllUserlists(userPlantId,gardenId)
  {
    this.loading=true;
    this.subs.add(
      this.util.deleteUserPlantFromAllUserlists(userPlantId,gardenId)
      .subscribe(()=> {
        this.displayedPlants = this.displayedPlants.filter(p => !(p.UserPlant.PlantId === userPlantId ));
        this.listTotalItem=this.displayedPlants.length;
        this.setGardenBadges();
        this.counter.requestPlantsCount().subscribe((data) => this.counter.setPlantsCount(data));
        this.counter.requestTodosCount().subscribe((data) => this.counter.setTodosCount(data));
        this.loading=false;
      }));
    
  }
  movePlantEvent($event){
    if($event==true)
      this.router.navigate(['/meingarten']);
      this.viewMyPlants();
         
  }
  moveAllPlants(selectedList){
    if(this.selectedItems.length>0){
      const newlistId=this.selectedItems[0].Id
      const moveModel={CurrentListId:selectedList.Id,NewListId:newlistId}
      this.subs.add(
        this.util.moveAllUserplantsToAnotherList(moveModel).subscribe((newUserplantList)=>{
          this.userPlants=newUserplantList
          this.lists=this.lists.filter(l=>l.Id!==selectedList.Id);
          this.router.navigate(['/meingarten']);
          this.viewMyPlants();
        })
      )
    }
  }
  addPlantToList() {
    this.userPlantTrigger=[];
    if(this.selectedListIds.length!==0) {
      this.selectedListIds.forEach(element => {
        this.newUserPlantTrigger.patchValue({
          UserPlantId:this.selectedPlant.Id,
          UserListId:element.Id,
          GardenId: this.id,
          PlantId:this.selectedPlant.Id,

        });
        this.userPlantTrigger.push(this.newUserPlantTrigger.value);
      });
      this.subs.add(this.util.postUserPlantTrigger(this.userPlantTrigger).subscribe(()=>this.alert.success('Pflanze wurde zu Pflanzenliste hinzugefügt')));
    }
  }

  togglePlantMenu(plant) {
    plant.showMenu = !plant.showMenu;
  }

  updatePlant(editCount:boolean=false) {
   /*  if (!this.selectedCountList) {
      return;
    } */
    //this.selectedPlant.UserListId = this.selectedCountList.Id;
    this.subs.add(this.util.updateUserPlant(this.selectedPlant,editCount).subscribe(
      
      () => {
        this.counter.requestPlantsCount().subscribe((data) => this.counter.setPlantsCount(data));
        this.getUserPlants();
      }
    ));
  }
 updateList(selectedList){
    this.subs.add(this.util.updateUserList(selectedList).subscribe())
  }
  editTodo(selectedPlant) {
    selectedPlant.CyclicTodos.forEach(todo => {
      this.subs.add(this.util.updateCyclicTodo(todo.Id,todo).subscribe());
    });
  }

  editGarden() {
    const img = new FormData();
    const self = this;  // different scopes
    const canvas = this.cropper.getCroppedCanvas({
      maxWidth: 4096,
      maxHeight: 4096
    });
    const image = document.getElementById('uploaded-img') as HTMLImageElement;
    const frameCanvas = this.getFrameCanvas(canvas, image, this.cropper);
    frameCanvas.toBlob(function (blob: Blob) {
      const imgFile = new File([blob], self.imgObj.Title, {type: 'image/jpeg'});
      
      img.append('imageFile', imgFile);
      img.append('imageTitle', imgFile);
      img.append('imageDescription', self.imgObj.description);
      img.append('imageTags', self.imgObj.tags);
      img.append('imageCreatedDate', self.imgObj.dateCreated);
      img.append('imageNote', self.imgObj.note);
      img.append('id', self.garden.Id);

      self.loading=true;
      self.subs.add(self.util.uploadGardenImg(img).subscribe(() => {
        // reset 
        self.cropper.destroy();
        self.imgObj = {img: null, Title: null, Src: null, dateCreated: null, note: null, tags: null, description: null};
        self.frameOptions = {
          stroke: 'none',
          color: 'white',
        };
        let params = new HttpParams();
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
        self.util.getUserMainGarden(params).subscribe(t => {
          self.garden = t
          self.garden.Images = t.Images;
        });
        self.loading=false;
      }));
    }, 'image/jpeg', 1);
  }

  rotateImage(deg: number) {
    if (this.cropper) {
      this.cropper.rotate(deg);
    }
  }
  setAspectRatio(ratio:number){
    if(this.cropper){
      this.cropper.setAspectRatio(ratio);
    }
  }
  getFrameCanvas(canvas, image, cropper) {
    if (this.frameOptions.stroke === 'none') {
      return canvas;
    }

    const context = canvas.getContext('2d');
    const imageWidth = cropper.getImageData().naturalWidth;
    const imageHeight = cropper.getImageData().naturalHeight;

    canvas.width = imageWidth;
    canvas.height = imageHeight;

    context.lineWidth = this.STROKE_OPTIONS[this.frameOptions.stroke];
    context.strokeStyle= this.frameOptions.color === 'white' ? '#fff' : '#000';
    context.imageSmoothingEnabled = true;
    context.drawImage(image, 0, 0, imageWidth, imageHeight);
    context.strokeRect(0, 0, canvas.width, canvas.height);
    return canvas;
  }

  deleteGardenImage(id) {
    this.subs.add(this.util.deleteGardenImg(id).subscribe(
      () => {
        this.garden.Images = this.garden.Images.filter(img => img.Id !== id);
      }));
  }

  updateSorting() {
    this.sortedImages.forEach((element,index)=> {
      element.Sort=index;
      this.setImagesSortIndex.push(element);
    });
    this.subs.add(this.util.updateGardenSort(this.setImagesSortIndex).subscribe(
      () => {
        this.garden.Images.sort((a,b) => a.Sort > b.Sort);
      }
    ));
  }
  drop(event: CdkDragDrop<any[]>) {
    moveItemInArray(this.garden.Images, event.previousIndex, event.currentIndex);
    this.sortedImages=this.garden.Images;
  }
  async imageUpload(event) {
    const res = await this.scanner.handleImageUpload(event);
    this.invalidImg = res.err !== null;
    
    if (!res.err) {
      this.imgObj.Src = res.src;
      this.imgObj.dateCreated = res.dateCreated;
      this.imgObj.img = res.file;
      this.imgObj.Title = res.file.name;
      setTimeout(() => {
        this.initCropper();
      });
    } else {
      this.imgObj.img = null;
      this.imgObj.note = null;
      this.imgObj.tags = null;
      this.imgObj.dateCreated = null;
      this.imgObj.Title = null;
      this.imgObj.description = null;
    }
  }

  initCropper() {
    const image = document.getElementById('uploaded-img') as HTMLImageElement;
    let cropBoxData;
    let canvasData;

    this.cropper = new Cropper(image, {
      autoCropArea: 0.5,
      aspectRatio:1/1,
      ready: function () {
        //Should set crop box data first here
        this.cropper.setCropBoxData(cropBoxData).setCanvasData(canvasData);
      }
    });
  }

  cancelEdit() {
    this.imgObj.img = null;
    this.imgObj.Title = null;
    this.imgObj.Src = null;
    this.imgObj.note = null;
    this.imgObj.tags = null;
    this.imgObj.dateCreated = null;
    this.imgObj.description = null;
  }

  isMain(image) {
    return image.Id === this.garden.MainImageId;
  }

  setImageMain(image) {
    this.garden.MainImageId =image.Id;
    this.subs.add(this.util.updateGarden(this.garden).subscribe());
  }

  toggleShow() {
    this.show = !this.show;
  }
  togglePlant(plant: UserPlant,editCount:boolean=false) {
    plant.IsInPot = !plant.IsInPot;
    this.subs.add(this.util.updateUserPlant(plant,editCount).subscribe());
  }
  gardenSize(garden) {
    return garden.PlantsLight.filter(e=> e.IsInPot ===true).reduce((acc, g) => acc + g.Count, 0);
  }
  
  isActive(todo) {
    return !todo.Ignored && new Date(todo.DateEnd) >= new Date();
  }
  countActiveTodo(allTodos){
    this.countActiveTodos=0;
    allTodos.forEach(element => {
      if(this.isActive(element)){
        this.countActiveTodos=+1;
      }
    });
    return this.countActiveTodos;
  }
  totalTodos(plant) {
    if (plant.CyclicTodos && plant.CyclicTodos.length > 1) {
      plant.Todos = plant.CyclicTodos.filter((thing, index, self) => self.findIndex(t => t.CyclicId === thing.CyclicId) === index);
    }
    return plant.Todos;
  }

  public trackByFn(index, item) {
    return item.Id;
  }
  
  get listIsValid() {
    return this.newList.valid;
  }

  toUrl(url: string, small = false) {
    return this.util.toUrl(url, small, true);
  }
  changedEditor(event: EditorChangeContent| EditorChangeSelection){
  
    let texteditorValue=event['editor']['root']
    console.log(texteditorValue)
  }
  onEditorBlured($event) {
    // tslint:disable-next-line:no-console
    console.log('blur', $event)
    this.focused = false
    this.blured = true
  }
  
  onEditorFocused($event) {
    // tslint:disable-next-line:no-console
    console.log('focus', $event)
    this.focused = true
    this.blured = false
  }
  
  onEditorCreated(event) {
    // tslint:disable-next-line:no-console
    console.log('editor-created', event)
  }
  
  onContentChanged(event: EditorChangeContent | EditorChangeSelection) {
    // tslint:disable-next-line:no-console
    console.log('editor-change', event)
  }
  markDone(todo) {
    this.subs.add(this.util.markTaskDone(todo.Id,todo.Finished,todo.Id).subscribe(() => {
     
    }));
    this.refresh.next();
  }
  onSelectDeviceNgModelChange($event){
    this.selectedDevices=$event
  }
  postSelectedDevice(){
    this.loading=true;
    
    console.log(this.selectedDevices)
    const adminDeviceId:number[]=[];
      if(this.selectedDevices.length>0){
        this.selectedDevices.forEach(d=>{
         adminDeviceId.push(d.Id)
          })
          this.subs.add(this.util.postAdminDevices(adminDeviceId).subscribe(()=>{ this.router.navigate(['/meingarten/myDevices/'+this.id]);}));
          this.deviceComponent?.getUserDevices()
          this.loading=false;
          this.alert.success("Gerät wurde der Geräteliste hinzugefügt");
      }else{
        return;
      }
  }
  sortPlantsDesc(){
    this.displayedPlants.sort((x,y)=>+new Date(x.UserPlant.DatePlanted)- +new Date(y.UserPlant.DatePlanted))
  }

  sortPlantsAsc(){
    this.displayedPlants.sort((x,y)=>+new Date(y.UserPlant.DatePlanted)- +new Date(x.UserPlant.DatePlanted))
  }

  updateSelectedPlant(selectedPlant){
    this.util.updateSelectedPlant(selectedPlant)
    console.log(selectedPlant)
  }

  get showChecklist() {
    return this.router.url === '/meingarten/ecolist';
  }
  get showDevicelist() {
    return this.router.url === '/meingarten/myDevices/'+this.id;
  }
  get showEcoElemt() {
    return this.router.url.includes('ecoelement') ;
  }
get showGarden(){
  if(!this.showChecklist && !this.showDevicelist && !this.showEcoElemt){
    return true
  }
  return false
}
  private intersect(a, b): any[] {
    if (a) {
      return a.filter(Set.prototype.has, new Set(b)) || [];
    }
    return [];
  }
}
