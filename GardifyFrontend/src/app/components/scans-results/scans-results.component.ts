import { Component, OnInit, ViewChild, ElementRef, Renderer2, OnDestroy } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { ScannerService } from 'src/app/services/scanner.service';
import { ScanResult, Plant, PlaceHolder, UserActionsFrom } from 'src/app/models/models';
import { faSearch } from '@fortawesome/free-solid-svg-icons';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { AuthService } from 'src/app/services/auth.service';
import { UtilityService } from 'src/app/services/utility.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { StatCounterService } from 'src/app/services/stat-counter.service';
import { AlertService } from 'src/app/services/alert.service';
declare var jQuery:any;
import * as bootstrap from "bootstrap"; 
import { slugify } from 'src/app/services/plant-search.service';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-scans-results',
  templateUrl: './scans-results.component.html',
  styleUrls: ['./scans-results.component.css']
})
export class ScansResultsComponent implements OnInit,OnDestroy {
  mode: string;
  @ViewChild('infoPopup', { static: false }) popup: ElementRef;
  faSearch = faSearch;
  baseUrl = 'https://gardify.de/intern/';
  results = new ScanResult();
  uploadedPic: any;
  subs = new Subscription()
  selectedPlant;
  suggestInvalid: boolean = false;
  userLists = [];
  suggestImgs = [];
  selectedLists = [];
  userPlantTrigger: any[] = [];
  addNewPlant: boolean = false;
  requiredField: boolean = false;
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
  addForm: FormGroup;
  addNewList: FormGroup;
  loading = false;
  showTip = false;
  userName = '';
  apiCallFrom= new UserActionsFrom();
  constructor(private tp: ThemeProviderService, private scannerService: ScannerService, 
    private counter: StatCounterService, private router: Router, private auth: AuthService, 
    private util: UtilityService, private fb: FormBuilder, private alert: AlertService) {
      this.addForm = this.fb.group({
        PlantId: 3, // placeholder foreign key
        InitialAgeInDays: 1,
        Count: 1,
        IsInPot: false,
        Description: 'Keine Beschreibung.',
        Name: '',
        NameLatin: '',
        searchResult: '',
        isAuthor: false,
        ArrayOfUserlist:null
      });
      this.addNewList = this.fb.group({
        Name: ['',Validators.required],
        Description: ['']
      });
  }

  openSuggestModal() {
    this.addNewPlant = true;
    // $('#addPlantModal').modal('show');
    // var myElement = angular.element( document.querySelector( '#some-id' ) );
    // this.suggestModal.nativeElement.className = 'modal fade show';
  }


  closeModal() {
    this.addNewPlant = false;
    $('#addPlantModal').modal('hide');
    // var myElement = angular.element( document.querySelector( '#some-id' ) );
    // this.suggestModal.nativeElement.className = 'modal fade show';
  }

  toAddPlant(){
    return this.addNewPlant;
  }

  ngOnDestroy(): void {
   this.subs.unsubscribe()
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

  selectPlant(plant: any) {
    // check if we're adding a PlantNet-plant (no Id) or a gardify-plant (has Id)
    this.showTip = false;
    this.addNewPlant = plant.Id > 0 ? false : true;
    this.selectedPlant = plant;
  }

  onItemSelect(item: any) {
    const ddl = document.getElementsByClassName('multiselect-dropdown')[0].children[1];
    ddl.setAttribute('hidden', 'false');
    this.setClass();
  }

  onSelectAll(items: any) {
    this.setClass();
  }

  setClass() {
    this.setStatus();
    if (this.selectedLists.length > 0) { return 'validField' }
    else { return 'invalidField' }
  }
  setStatus() {
    (this.selectedLists.length > 0) ? this.requiredField = true : this.requiredField = false;
  }

  getPlantLists() {
    this.subs.add(this.util.getPlantLists(0).subscribe(lists => {
      this.userLists = lists;
      this.selectedLists = lists.filter(l => l.ListSelected);
    }));
  }

  ngOnInit() {

    this.subs.add(this.auth.getUserObservable().subscribe(u => {
      this.user = u;
    }));
    this.subs.add(this.auth.user$.subscribe(u => this.userName = u.Name.split(' ')[0]));
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
    this.subs.add(this.scannerService.uploadedPic$.subscribe(pic => this.uploadedPic = pic));
    this.scannerService.scanResults ? this.results = this.scannerService.scanResults:
                                      this.router.navigate(['/scanner']);

    if (this.results && this.results.PnResults && this.results.PnResults.InDb) {
      this.results.PnResults.InDb.forEach((p) => {
          p.NameLatin = p.NameLatin.replace('[k]', '').replace('[/k]', '');
          if (p.Synonym) { p.Synonym = p.Synonym.replace(/\[k]|\[k]/g, '<i>').replace(/\[\/k\]/g, '</i>'); }
          p.Description = p.Description.replace(/\[k]|\[k]/g, '<i>').replace(/\[\/k\]/g, '</i>');
        p.PlantUrl = slugify(p.NameGerman || p.NameLatin);
      });
    }
  }


  checkIfExist(plant){

    var returnValue = !this.results.PnResults.InDb.some(e => e.NameLatin.toLowerCase().includes(plant.species.scientificNameWithoutAuthor.toLowerCase()))


    return returnValue;
  }

  selectPlantForSuggestion(plant) {
    this.scannerService.plantForSuggestion.next(plant.species.scientificNameWithoutAuthor);
    if (this.results.PnResults && this.results.PnResults.results)
    {
      var mappedList = "";
      this.results.PnResults.results.forEach(res => {
        mappedList += res.species.scientificNameWithoutAuthor + "\n";
      });
      this.scannerService.allPlantResult.next(mappedList);

    }
  }

  imageFromScan(value){
    this.scannerService.fromPlantScan = value
    if (value == false){
      this.scannerService.allPlantResult.next(null);

    }
  }



  // called when adding plant from gardify DB
  borrowPlant() {
    this.userPlantTrigger = [];
    this.loading = true;
    if (this.selectedLists.length>0) {
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
      this.selectedLists.forEach(e => {
        this.subs.add(this.util.addUserPlantToProp(this.addForm.value).subscribe((res) => {
          this.subs.add(this.counter.requestPlantsCount().subscribe(data => this.counter.setPlantsCount(data)));
      
          this.selectedPlant.IsInUserGarden = true;
          //this.closeModal();
          this.loading = false;
          this.alert.success('Pflanze wurde dem Garten hinzugefügt');

          // this.showTip = true;
        }));
      });
    } else {
      //this.closeModal();
      this.alert.error('Bitte Liste auswählen!');
      this.loading = false;
    }
  }

  // called when creating a new plant from PlantNet data
  addPlant(){
    this.userPlantTrigger = [];
    this.loading = true;
    if (this.selectedLists.length>0) {
      const data = new FormData();
      var newName = this.selectedPlant.species.commonNames.length > 0? this.selectedPlant.species.commonNames[0] : this.selectedPlant.species.scientificNameWithoutAuthor;
    
      this.addForm.patchValue({
        Name: newName,
        NameLatin: this.selectedPlant.species.scientificNameWithoutAuthor,
        isAuthor: this.suggestInvalid
      });
      var uploadedImage = this.dataURItoBlob(this.uploadedPic.src.split(",")[1], newName.split(" ")[0] + ".jpeg");
      data.append('Name', this.addForm.value.Name);
      data.append('NameLatin', this.addForm.value.NameLatin);
      data.append('imageFile', uploadedImage);
      data.append('imageTitle', newName.split(" ")[0] + ".jpg");
      data.append('count', this.addForm.value.Count);
      data.append('age', this.addForm.value.InitialAgeInDays);
      data.append('isInPot', this.addForm.value.IsInPot);
      data.append('isAuthor', this.addForm.value.isAuthor);
      this.selectedLists.forEach(e=>{
        const temp = {
          UserPlantId: 1,
          UserListId: e.Id
        };
        this.userPlantTrigger.push(temp);
      })
      data.append('ArrayOfUserlist', JSON.stringify( this.userPlantTrigger));


      
      if (this.results.PnResults && this.results.PnResults.results)
      {
        var mappedList = "";
        this.results.PnResults.results.forEach(res => {
          mappedList += res.species.scientificNameWithoutAuthor + "\n";
        });
        data.append('searchResult', mappedList);

      }


      this.selectedLists.forEach(e=> {
        this.subs.add(this.util.addNewSuggestPlant(data).subscribe((res) => {
          this.counter.requestPlantsCount().subscribe(data => this.counter.setPlantsCount(data));
          // const userPlantTrigger = [];
          //   const temp = {
          //     UserPlantId: res.Id,
          //     UserListId: e.Id
          //   };
          //   userPlantTrigger.push(temp);
          //   this.closeModal();
          // this.subs.add(this.util.postUserPlantTrigger(userPlantTrigger).subscribe());
          //this.closeModal();
          this.loading = false;
          this.showTip = true;
        }, (reason) => {
          // this.openSuggestModal();
          this.closeModal();
          this.alert.error('Pflanze existiert nicht in Gardify, Sie können eine neue Pflanzen vorschlagen');
          this.openSuggestModal();
          this.loading = false;
        }));
      });
    } else {
      //this.closeModal();
      this.alert.error('Bitte Liste auswählen!');
      this.loading = false;
    }
  }

   suggestAgree(input){
    this.suggestInvalid = input;
  }

  hidePopup() {
    localStorage.setItem('showPopup', 'false');
    jQuery(this.popup.nativeElement).modal('hide'); 
  }

  get dbResults() {
    return this.results.PnResults && this.results.PnResults.InDb ? this.results.PnResults.InDb.length : 0;
  }

  get pnResults() {
    return this.results.PnResults && this.results.PnResults.results ? this.results.PnResults.results.length : 0;
  }

  toUrl(plant: Plant) {
    const head = plant.Images[0].SrcAttr.substring(0, plant.Images[0].SrcAttr.lastIndexOf('/'));
    
    return this.baseUrl+head+'/250'+ plant.Images[0].SrcAttr.substring(plant.Images[0].SrcAttr.lastIndexOf('/'));
  }

  user: any;

  get isLoggedIn() {
    return this.auth.isLoggedIn();
  }

  get demoMode() {
    return this.user && this.user.Email.includes('UserDemo');
  }

  get phImg() {
    return PlaceHolder.Img;
  }

  imageUpload(event) {
    const reader = new FileReader();
    if (event.target.files[0].size > 4000000) { 
      return this.alert.error('Die Bildgröße muss kleiner oder gleich 4Mb sein'); 
    }
    if (event.target.files && event.target.files.length) {
      const [file] = event.target.files;
      this.suggestImgs = [];
      reader.readAsDataURL(file);
      reader.onloadend = () => {
        this.suggestImgs.push({Image: file, ImageTitle: file.name, Src: reader.result});
        // this.cd.markForCheck();
        // this.attachment.nativeElement.value = '';
      };
    }
  }

  dataURItoBlob(dataURI: string, imageName: string){
      const byteString: string = window.atob(dataURI);
      const arrayBuffer: ArrayBuffer = new ArrayBuffer(byteString.length);
      const int8Array: Uint8Array = new Uint8Array(arrayBuffer);
      for (let i = 0; i < byteString.length; i++) {
        int8Array[i] = byteString.charCodeAt(i);
      }
      const blob = new Blob([int8Array], { type: "image/jpeg" });
      const file = new File([blob], imageName, {
        type: "image/jpeg"
      });

      return file;
  }

  removeSelectedFile(index) {
    // Delete the item from selected images list
    this.suggestImgs.splice(index, 1);
   
   }
}
