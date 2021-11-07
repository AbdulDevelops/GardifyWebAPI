import { HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { UserActionsFrom } from 'src/app/models/models';
import { AlertService } from 'src/app/services/alert.service';
import { AuthService } from 'src/app/services/auth.service';
import { PlantSearchService, slugify } from 'src/app/services/plant-search.service';
import { StatCounterService } from 'src/app/services/stat-counter.service';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { UtilityService } from 'src/app/services/utility.service';

@Component({
  selector: 'app-latest-plants',
  templateUrl: './latest-plants.component.html',
  styleUrls: ['./latest-plants.component.css']
})
export class LatestPlantsComponent implements OnInit {
  monthNames = [
    'Januar', 'Februar', 'März', 'April', 'Mai', 'Juni',
    'Juli', 'August', 'September', 'Oktober', 'November', 'Dezember'
  ];
  BADGES = {
    Bees: {Id: [447], title: 'Bienenfreundlich'},
    Birds: {Id: [320,322], title: 'Vogelfreundlich'},
    Insects: {Id: [321], title: 'Insektenfreundlich'},
    Bio: {Id: [445], title: 'Ökologisch wertvoll'},
    Butterflies: {Id: [531], title: 'Schmetterlings freundlich'},
    Domestic: {Id: [530], title: 'Heimische Pflanze'},
  };
  displayedPlants = [];
  userLists = [];
  selectedLists = [];
  loading = true; 
  mode: string;
  subs = new Subscription();
  selectedMonth: string;
  selectedYear: string;
  selectedPlant: any;
  demoMode = true;
  addNewList: FormGroup;
  addForm: FormGroup;
  requiredField: boolean;
  apiCallFrom=new UserActionsFrom();
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

  constructor(
    private search: PlantSearchService, 
    private util: UtilityService,
    private fb: FormBuilder,
    private auth: AuthService,
    private counter: StatCounterService,
    private alert: AlertService,
    private tp: ThemeProviderService) { 
      this.addNewList = this.fb.group({
        Name: ['', Validators.required],
        Description: ['']
      });
      this.addForm = this.fb.group({
        PlantId: null,
        InitialAgeInDays: 0,
        Count: 1,
        IsInPot: false,
        Todos: null,
        ArrayOfUserlist:null
      });
    }

  ngOnInit(): void {
    this.selectedMonth = this.monthNames[new Date().getMonth()];
    this.selectedYear = new Date().getFullYear().toString();
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
    this.subs.add(this.auth.getUserObservable().subscribe(u => {
      this.demoMode = u && u.Email.includes('UserDemo');
    }));
    this.getPlantsOfMonth(this.selectedMonth, this.selectedYear);
  }

  getPlantsOfMonth(month: string, year: string) {
    this.loading = true;
    this.selectedMonth = month;
    this.selectedYear = year;
    const selMonth = this.monthNames.indexOf(month) + 1;
    const selYear = year;
    this.subs.add(this.search.getLatestPlants(selMonth, selYear).subscribe(p => {
      this.loading = false;
      this.displayedPlants = p || [];
      this.displayedPlants.forEach((p,i) => {
        if (p.Synonym) { p.Synonym = p.Synonym.replace(/\[k]|\[k]/g, '<i>').replace(/\[\/k\]/g, '</i>'); }
        if (p.Description) { p.Description = p.Description.replace(/\[k]|\[k]/g, '<i>').replace(/\[\/k\]/g, '</i>'); }
        if (p.NameLatin) { p.NameLatin = p.NameLatin.replace(/\[k]|\[k]/g, '<i>').replace(/\[\/k\]/g, '</i>'); }
        p.PlantUrl = slugify(p.NameGerman || p.NameLatin);
      });
    }));
  }

  getPlantLists() {
    this.subs.add(this.util.getPlantLists(this.selectedPlant.Id).subscribe(lists => {
      this.userLists = lists;
      this.selectedLists = lists.filter(l => l.ListSelected);
    }));
  }

  createList() {
    let params = new HttpParams();
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
    this.subs.add(this.util.createUserList(this.addNewList.value,params).subscribe(lists => {
      this.userLists = lists;
      this.selectedLists = lists.filter(l => l.ListSelected);
      this.addNewList.reset();
    }));
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

  setStatus() {
    (this.selectedLists.length > 0) ? this.requiredField = true : this.requiredField = false;
  }

  setClass() {
    this.setStatus();
    if (this.selectedLists.length > 0) { return 'validField' }
    else { return 'invalidField' }
  }

  borrowPlant() {
    let alertTriggered = false;
    const userPlantTrigger = [];
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
        userPlantTrigger.push(temp);
      })
      this.addForm.patchValue({
        ArrayOfUserlist: userPlantTrigger
      });
        this.subs.add(this.util.addUserPlantToProp(JSON.stringify(this.addForm.value)).subscribe(
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

  get displayedMonths(): any[] {
    const today = new Date();
    let d: Date;
    let month: string;
    let year: number;
    const months = [];

    for(let i = 6; i > 0; i -= 1) {
      d = new Date(today.getFullYear(), today.getMonth() - i + 1, 1);
      month = this.monthNames[d.getMonth()];
      year = d.getFullYear();
      months.push({month, year});
    }
    console.log(months);
    return months;
  }

  get isLoggedIn() {
    return this.auth.isLoggedIn();
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

  toUrl(url, small = true) {
    return this.util.toUrl(url, small);
  }
}
