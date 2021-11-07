import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { ActivatedRoute, Router } from '@angular/router';
import { PlantSearchService, slugify } from 'src/app/services/plant-search.service';
import { faDotCircle, faChevronDown } from '@fortawesome/free-solid-svg-icons';
import { FormGroup, FormBuilder } from '@angular/forms';
import { UtilityService } from 'src/app/services/utility.service';
import { Todo, ReferenceToModelClass, Plant, UserActionsFrom } from 'src/app/models/models';
import { StatCounterService } from 'src/app/services/stat-counter.service';
import { Subject, Subscription } from 'rxjs';
import { AuthGuard } from 'src/app/guards/auth.guard';
import { AuthService } from 'src/app/services/auth.service';
import { AlertService } from 'src/app/services/alert.service';
import { HttpParams } from '@angular/common/http';
import { EditorChangeContent, EditorChangeSelection } from 'ngx-quill';

@Component({
  selector: 'app-plant-search-details',
  templateUrl: './plant-search-details.component.html',
  styleUrls: ['./plant-search-details.component.css']
})
export class PlantSearchDetailsComponent implements OnInit,OnDestroy {

  @Input() item:any;
  @Input() element:any;
mode:string;
baseUrl:string="https://gardify.de/intern/";
result:any;
faDotCircle=faDotCircle
faChevronDown= faChevronDown
showEditMenu=false
  public myRegex4= new RegExp(/\{k]|\[k]|\[\/k]/g)
    
    plantImageModal;
  exclusioncriteria: any;
  particularity: any;
  decoaspects: any;
  augmentation: any;
  use: any;
  waterneeds: any;
  care: any;
  winterhardness: any;
  ground: any;
  growth: any;
  cropPlant: any;
  cut: any;
  light: any;
  location: any;
  fruitcolor: any;
  fruits: any;
  flowersized: any;
  flowershaped: any;
  flowering: any;
  flowercolours: any;
  foliage: any;
  leafcolour: any;
  autumncolouring: any;
  leafrythm: any;
  leafshape: any;
  leafmargin: any;
  sheetposition: any;
  blossoms: any;
  fertilisation: any;
  addForm: FormGroup;
  gardens: any;
  todoList: any;
  plantSiblings: any;
  description:"";
  newTodosForm:FormGroup;
  winterHardness:any
  newTodo = new Todo();
  referenceType=ReferenceToModelClass.UserPlant
  plantSiblingId: any=[];
  plantSiblingDetails: any=[];
  userPlantTrigger: any[] = [];
  origin: any;
  selectedPlant: Plant;
  clicked:boolean=false;
  plantBadges = {
    BeesId: 447,
    BirdsId: 322,
    InsectsId: 321,
    BioId: 445,
    ButterfliesId: 531,
    Domestic: 530,
    WaterSaving:346
  };
  MONTHS = {
    1: 'Jan', 2: 'Feb', 3: 'Mär', 4: 'Apr', 5: 'Mai', 6: 'Jun',
    7: 'Jul', 8: 'Aug', 9: 'Sep', 10: 'Okt', 11: 'Nov', 12: 'Dez'
  };
  selectedPlantFromGarden:any

  public italicRegex= new RegExp(/\{k]|\[k]([\s\S]+?)\[\/k]/g);
    replacegrp="<i>$1</i>"
  loading=true;
  newUrl: any;
  firstUrl: any=null;
  subscriptions= new Subscription();
  requiredField: boolean = false;
  apiCallFrom=new UserActionsFrom();
  refresh: Subject<any> = new Subject();
  dropdownSettings = {
    singleSelection: false,
    idField: 'Id',
    textField: 'Name',
    selectAllText: 'Alle markieren',
    unSelectAllText: 'Alle entmarkieren',
    itemsShowLimit: 10,
    allowSearchFilter: false,
    noDataAvailablePlaceholderText: 'Bitte Liste anlegen, z.B. Hauptgarten, Vorgarten oder Topfpflanzen'
    
  };
  dropdownList = [];
  userLists = [];
  selectedLists = [];
  updatePlantMode: string;
  focused: boolean;
  blured: boolean;
  constructor(private tp: ThemeProviderService,private activateRoute:ActivatedRoute, private router: Router,
    private plantSearch: PlantSearchService,private fb: FormBuilder, private util:UtilityService, private alert: AlertService,private auth:AuthService,
    private counter: StatCounterService) {
      this.addForm = this.fb.group({
        GardenId: null, PlantId: null,
        InitialAgeInDays: 0,
        Count: 1,
        IsInPot: false,
        Todos: null,
        ArrayOfUserlist:null
      });
     }
  ngOnDestroy(): void {
    this.subscriptions.unsubscribe()
  }

  ngOnInit() {
    this.subscriptions.add(this.auth.getUserObservable().subscribe(u => {
      this.user = u;
    }));
    this.loading=true
   
    this.subscriptions.add(this.tp.getTheme().subscribe(t => this.mode = t));
    this.subscriptions.add(this.activateRoute.queryParams.subscribe((d) => this.origin = d.o));

    
    this.getPlant();
    // this.getGardens();
    this.getPlantSiblings()
    this.getUserLists()
    
  }
  getSelectedPlantFromGarden(){
    this.selectedPlantFromGarden=JSON.parse(localStorage.getItem('CurrentPlant'))
    
  }
  user: any;

  get isLoggedIn() {
    return this.auth.isLoggedIn();
  }
  get isTestAccount(){
    return this.auth.isTestAccount();
  }
  get demoMode() {
    return this.user && this.user.Email.includes('UserDemo');
  }
  private getGardens() {
    this.subscriptions.add(this.util.getGardens().subscribe(t => {
      this.gardens = t;
      if (this.gardens.length > 0) {
        this.subscriptions.add(this.util.getUserLists().subscribe(lists => {
          this.userLists = lists;
        }));
      }
    }));
  }

  updatePlant(editCount:boolean=false) {
    /*  if (!this.selectedCountList) {
       return;
     } */
     //this.selectedPlant.UserListId = this.selectedCountList.Id;
     this.subscriptions.add(this.util.updateUserPlant(this.selectedPlantFromGarden.UserPlant,editCount).subscribe(
       
       () => {
         this.counter.requestPlantsCount().subscribe((data) => this.counter.setPlantsCount(data));
         this.router.navigate(['/meingarten']);
       }
     ));
   }
   deleteUserPlantFromAllUserlists(userPlantId,gardenId)
   {
     this.loading=true;
     this.subscriptions.add(
       this.util.deleteUserPlantFromAllUserlists(userPlantId,gardenId)
       .subscribe(()=> {
        
         this.counter.requestPlantsCount().subscribe((data) => this.counter.setPlantsCount(data));
         this.counter.requestTodosCount().subscribe((data) => this.counter.setTodosCount(data));
         this.router.navigate(['/meingarten']);
         this.loading=false;
       }));
     
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
  getUserLists(){
    this.subscriptions.add(this.util.getUserLists().subscribe(lists => {
      this.dropdownList=lists;
     
    }));
  }
  toUrl(url, small = false) {
    
      return this.util.toUrl(url, small);
    
  }
  isActive(todo) {
    return !todo.Ignored && new Date(todo.DateEnd) >= new Date();
  }
  markDone(todo) {
    this.subscriptions.add(this.util.markTaskDone(todo.Id,todo.Finished,todo.Id).subscribe(() => {
     
    }));
    this.refresh.next();
  }
  replaceUrl(url){
    this.clicked=true
    this.firstUrl=this.toUrl(url,false)
    setTimeout(()=>{    
      this.clicked = false;
 }, 3000);
    return this.firstUrl;
  }
  addNewTodo() {
    this.newTodo.ReferenceId=this.item.Id;
    this.newTodo.ReferenceType=this.referenceType;
    for (let index = 0; index <  this.item.TodoTemplates.length; index++) {
      const element = this.item.TodoTemplates[index].Description;
      this.description=+" "+element;
    }

    this.newTodo.Description=this.description;        

    this.subscriptions.add(this.util.addTodo(JSON.stringify(this.newTodo))
    .subscribe(r => {
      this.newTodo = new Todo();
      this.counter.requestTodosCount().subscribe(data => this.counter.setTodosCount(data));
    }));
  }
  setStatus() {
    (this.selectedLists.length > 0) ? this.requiredField = true : this.requiredField = false;
  }

  onItemSelect(item: any) {
    //Do something if required
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
  }
  hasBadgeById(id) {
    if (this.particularity) {
      return this.particularity.filter(t => t.Id === id).length > 0;
    }
  }

  get hasBadges() {
    return this.particularity.some(t => Object.values(this.plantBadges).indexOf(t.Id) > 0);
  }
  
  clearArray(el?){
    if(this.plantSiblingId.length>0){
      this.plantSiblingId.length=0
      this.plantSiblingDetails.length=0
    }
    if(el){
      this.scroll(el)
    }
  }
  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }
  getPlantSiblings() {
    this.subscriptions.add(this.activateRoute.params.subscribe(routerId=>{
      this.clearArray()
      this.plantSearch.GetPlantSiblings(routerId.id).subscribe(sibs => {
        this.plantSiblings = sibs.filter(sib => sib.Id);
        this.plantSiblings.forEach((p) => {
          p.PlantUrl = slugify(p.NameGerman || p.NameLatin);
        });
      });
    }));
  }

  parseCharacteristic(prop) {
    let res;
    switch(prop.CategoryId) {
      // parse size
     // case 20: res = prop.CategoryTitle.replace(/in/g, prop.Min); break;
      // parse time
      case 20: res = prop.Max===null?prop.CategoryTitle.replace(/von/g, prop.Min).replace(/bis cm/g, ``):prop.CategoryTitle.replace(/von/g, prop.Min).replace(/bis/g, `- ${prop.Max}`); break;

      case 21:
      case 22:
      case 25:
      case 26:
      case 27: 
      case 28: res = prop.CategoryTitle.replace(/Monat/, this.MONTHS[prop.Min]).replace(/Monat/, this.MONTHS[prop.Max]); break;
      // parse height
      case 23:
      case 24: res = prop.CategoryTitle.replace(/von/g, prop.Min).replace(/bis/g, `- ${prop.Max}`); break;

    }
    return res;
  }

  getPlant(){
    this.subscriptions.add(this.activateRoute.params.subscribe(routerId=>{
      this.plantSearch.GetPlantEntry(routerId.id)
      .subscribe(item =>{
        
        this.item = item;
        this.firstUrl= this.toUrl(this.item.Images[0].SrcAttr,false)
        this.clicked=false;
        if (this.item.GardenCategory) {
          this.item.PlantGroups.push(this.item.GardenCategory);
        }
        if (this.item.Synonym) {
          this.item.Synonym = this.item.Synonym.replace(/\{k]|\[k]|\[K]/g, '<i>').replace(/\[\/k\]|\[\/K\]/g, '</i>');
        }
        this.item.Description=this.item.Description.replace(/\{k]|\[K]|\[k]/g, '<i>').replace(/\[\/k\]|\[\/K\]/g, '</i>');
        this.exclusioncriteria=this.item.PlantTags.filter(e=>e.CategoryId===128)
        this.particularity=this.item.PlantTags.filter(e=>e.CategoryId===108)
        this.decoaspects=this.item.PlantTags.filter(e=>e.CategoryId===121)
        this.use=this.item.PlantTags.filter(e=>e.CategoryId===97)
        this.augmentation=this.item.PlantTags.filter(e=>e.CategoryId===95)
        this.fertilisation=this.item.PlantTags.filter(e=>e.CategoryId===94)
        this.cut=this.item.PlantTags.filter(e=>e.CategoryId===92)
        this.waterneeds=this.item.PlantTags.filter(e=>e.CategoryId===91)
        this.care=this.item.PlantTags.filter(e=>e.CategoryId===90)
        this.care = this.care.concat(item.PlantCharacteristics.filter(c => [25,26,27].includes(c.CategoryId)));
        this.winterhardness=this.item.PlantTags.filter(e=>e.CategoryId===89)
        this.ground=this.item.PlantTags.filter(e=>e.CategoryId===87)
        this.light=this.item.PlantTags.filter(e=>e.CategoryId===86)
        this.growth=this.item.PlantTags.filter(e=>e.CategoryId===74)
        this.growth = this.growth.concat(item.PlantCharacteristics.filter(c => [23,24].includes(c.CategoryId)));
        this.cropPlant=this.item.PlantTags.filter(e=>e.CategoryId===137)
        this.cropPlant = this.cropPlant.concat(item.PlantCharacteristics.filter(c => [28].includes(c.CategoryId)));
        this.location=this.item.PlantTags.filter(e=>e.CategoryId===85)
        this.fruitcolor=this.item.PlantTags.filter(e=>e.CategoryId===71)
        this.fruits=this.item.PlantTags.filter(e=>e.CategoryId===68)
        this.fruits = this.fruits.concat(item.PlantCharacteristics.filter(c => [20,21].includes(c.CategoryId)));
        this.flowersized=this.item.PlantTags.filter(e=>e.CategoryId===67)
        this.flowering=this.item.PlantTags.filter(e=>e.CategoryId===66)
        this.flowershaped=this.item.PlantTags.filter(e=>e.CategoryId===65)
        this.flowercolours=this.item.PlantTags.filter(e=>e.CategoryId===64)
        this.foliage=this.item.PlantTags.filter(e=>e.CategoryId===52)
        this.leafrythm=this.item.PlantTags.filter(e=>e.CategoryId===53)
        this.leafcolour=this.item.PlantTags.filter(e=>e.CategoryId===55)
        this.autumncolouring=this.item.PlantTags.filter(e=>e.CategoryId===56)
        this.leafshape=this.item.PlantTags.filter(e=>e.CategoryId===57)
        this.leafmargin=this.item.PlantTags.filter(e=>e.CategoryId===58)
        this.sheetposition=this.item.PlantTags.filter(e=>e.CategoryId===59)
        this.blossoms=this.item.PlantTags.filter(e=>e.CategoryId===60)
        this.winterHardness=this.item.PlantTags.filter(e=>e.CategoryId===89)
        this.blossoms = this.blossoms.concat(item.PlantCharacteristics.filter(c => [22].includes(c.CategoryId)));
        this.loading=false
      }, (err) => {
        this.router.navigate(['/404']);
      });
     
    }));
   
    }
  addToCart(item){
    this.loading=true
    let params = new HttpParams();
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
    this.subscriptions.add(this.util.addToCart(item.Id,false,params).subscribe(()=>{
      this.util.toggleCart(true);
      this.counter.requestShopCartCounter().subscribe(data=>this.counter.setShopCartCounter(data))
      this.loading=false
    }))
    
    }
  getPlantLists() {
    this.subscriptions.add(this.util.getPlantLists(this.selectedPlant.Id).subscribe(lists => {
      this.userLists = lists;
      this.selectedLists = lists.filter(l => l.ListSelected);
      this.setClass();
    }));
}
isCollectionCard(images){
  if(images[0].FullTitle.includes("Sammelkarte"))
    return true;
  return false;
}
checkIfAutor(images){
  if(images[0].Author.includes("Autor nicht spezifiziert"))
    return false;
  return true;
}
moveplantEvent($event){
  if($event==true)
    this.router.navigate(['/meingarten']);
    
}
}
