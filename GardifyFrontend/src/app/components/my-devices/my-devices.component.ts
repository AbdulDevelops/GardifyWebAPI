import { Component, OnInit, ChangeDetectorRef, HostListener, OnDestroy } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { FormBuilder, FormGroup, Validators, FormControl, FormArray } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

import { faSearch, faChevronDown, faChevronUp, faSlidersH, faEllipsisH, faCircle } from '@fortawesome/free-solid-svg-icons';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { UtilityService } from 'src/app/services/utility.service';
import { UserActionsFrom, UserDevices, UserDevList, UserList } from 'src/app/models/models';
import { from, Subscription } from 'rxjs';
import { AlertService } from 'src/app/services/alert.service';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-my-devices',
  templateUrl: './my-devices.component.html',
  styleUrls: ['./my-devices.component.css']
})
export class MyDevicesComponent implements OnInit,OnDestroy {
  mode:string
  subs = new Subscription();
  faMenu = faEllipsisH;
  faCircle = faCircle;
  addnewdevice:boolean;
  newDevice:any
  updateDeviceMode:string;
  showLists = false;
  showAddMenu;
  selectedDevice:UserDevices;
  newDeviceForm: FormGroup;
  newList: FormGroup;
  selectedList:number;
  lists:UserList[] = [];
  available:any;
  faChevronDown = faChevronDown;
  submitted: boolean;
  id: any;
  adminDev: any;
  mainForm: FormGroup;
  devices: any;
  garden:any={};
  gardenName: any;
  entryImage: any;
  invalidImg = false;
  entryImageTitle: string;
  loading=true;
  today=new Date()
  gardenBadges = {
    Bees: {Count: 0, Id: 447},
    Birds: {Count: 0, Id: 322},
    Insects: {Count: 0, Id: 321},
    Bio: {Count: 0, Id: 445},
  };
  width = window.innerWidth;
  isMobile: boolean;
  isXsMobile:boolean;
  userDevicesList: any;
  notifyForFrost: boolean;
 
  dropdownSettings : IDropdownSettings = {
    singleSelection: false,
    idField: 'Id',
    textField: 'Name',
    selectAllText: 'Alle markieren',
    unSelectAllText: 'Alle entmarkieren',
    itemsShowLimit: 10,
    allowSearchFilter: false
  };
  listOfDevices = [];
  selectedDevices = [];
  displayedPlants: any;
  itemCount: number;
  apiCallFrom=new UserActionsFrom();

  constructor(private authService: AuthService,
    private formBuilder: FormBuilder,
    private activatedRoute: ActivatedRoute,private themeproviderService:ThemeProviderService,private util:UtilityService,private cd: ChangeDetectorRef,private alert: AlertService) { 
      this.newDeviceForm=this.formBuilder.group({
        Name:[null],
         Date: [null],
         notifyForFrost: [true],
         notifyForWind: [true],
        isActive: [null],
        Note:[null],
        GardenId:null,
        AdminDevId:null,
       });
      this.newList = this.formBuilder.group({
        Name: [null, Validators.required],
        Description: [null],
        GardenId: null
      });
      this.mainForm=this.formBuilder.group({
      adminDev: new FormArray([])
      })
       
    }
  ngOnDestroy(): void {
    this.subs.unsubscribe()
  }
    @HostListener('window:resize')
    onWindowResize() {
      this.width = window.innerWidth;
     
      this.isMobile = window.innerWidth <= 990;
      this.isXsMobile = window.innerWidth <=375;
    }
  ngOnInit() {
    this.isMobile = window.innerWidth <= 768;
    this.isXsMobile=window.innerWidth<=375;
    this.subs.add(this.themeproviderService.getTheme().subscribe(t=>this.mode=t));
    this.loading=true;
    this.getAdminDevices()
    let params = new HttpParams();
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
    this.subs.add(this.util.getUserMainGarden(params).subscribe(t => {
    this.garden=t
    this.gardenName = t.Name;}))
    this.addnewdevice=true
    this.available=true
    this.subs.add(this.activatedRoute.params.subscribe(routerId => {
      this.id = routerId.id;
      this.newList.patchValue({
        GardenId: this.id
      });
      this.newDeviceForm.patchValue({
        GardenId: this.id,
        Date: this.today.toLocaleDateString()
      })
      this.util.getUserLists().subscribe(lists => {
        this.lists = lists});
      this.cd.detectChanges();
    }));
    this.getUserDevices();
    this.getUserPlants();
    this.loading=false;
  }
  getUserPlants(){
    this.subs.add(this.util.getUserPlants().subscribe(p=>{
      this.displayedPlants = p
      this.setGardenBadges();
    }))
  }
  setGardenBadges() {
    this.resetBadges();
    this.displayedPlants.forEach(pl => {
      pl.UserPlant.Badges.forEach(badge => {
        switch(badge.Id) {
          case this.gardenBadges.Insects.Id: this.gardenBadges.Insects.Count+= pl.UserPlant.Count; break;
          case this.gardenBadges.Birds.Id: this.gardenBadges.Birds.Count+= pl.UserPlant.Count; break;
          case 320: this.gardenBadges.Birds.Count+= pl.UserPlant.Count; break;
          case this.gardenBadges.Bio.Id: this.gardenBadges.Bio.Count+= pl.UserPlant.Count; break;
          case this.gardenBadges.Bees.Id: this.gardenBadges.Bees.Count+= pl.UserPlant.Count; break;
        }
      });
    });
  }
  onItemSelect(item: any) { }
  
  hasBee(plant) {
    return plant.Badges.filter(b => b.Id === this.gardenBadges.Bees.Id).length > 0;
  }
  hasBird(plant) {
    return plant.Badges.filter(b => b.Id === this.gardenBadges.Birds.Id).length > 0;
  }
  hasBio(plant) {
    return plant.Badges.filter(b => b.Id === this.gardenBadges.Bio.Id).length > 0;
  }
  hasInsect(plant) {
    return plant.Badges.filter(b => b.Id === this.gardenBadges.Insects.Id).length > 0;
  }
  private addCheckboxes() {
    this.adminDev.map((o, i) => {
      const control = new FormControl(i); 
      (this.mainForm.controls.adminDev as FormArray).push(control);
    });
  }
  
  getAdminDevices(){
    this.subs.add(this.util.getAdminDevices().subscribe(d=>{
     this.listOfDevices=d;
   }))
  }
  getUserDevices(){
    this.loading=true
    this.subs.add(this.util.getUserDevices().subscribe(ud=>{
      this.userDevicesList=ud;
      this.loading=false;
    }))
  }
  onImageUpload(event) {
    const reader = new FileReader();
    if (event.target.files[0].size > 4000000) { this.invalidImg = true; }
    if (event.target.files && event.target.files.length) {
      const [file] = event.target.files;
      reader.readAsDataURL(file);
      reader.onloadend = () => {
        this.entryImage = file;
        this.entryImageTitle = file.name;
        // need to run CD since file load runs outside of zone
        this.cd.markForCheck();
      };
    } else {
      this.entryImage = null;
      this.entryImageTitle = null;
    }
  }
  get f(){
    return this.newDeviceForm.controls;
  }
  get listIsValid() {
    return this.newList.valid;
  }
  createList(){
    let params = new HttpParams();
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
    this.subs.add(this.util.createUserList(this.newList.value,params).subscribe(l=>this.lists=l))
   
  }
 /*  getCreatedList(){
    this.subs.add(this.util.createUserList(this.id).subscribe(l=>this.lists=l))
  } */
  
  deleteUserList(list:UserList){
    this.subs.add(this.util.deleteUserList(this.id,list.Id).subscribe(()=>{
      this.lists=this.lists.filter(l=>l.Id!==list.Id)
    }))
  }
  postDevice(){
    this.loading=true;
    this.submitted = true;
      // stop here if form is invalid
      if (this.newDeviceForm.invalid) {
        return;
     }
    this.newDevice=this.newDeviceForm.value;
    this.subs.add(this.util.postDevices(this.newDevice).subscribe(()=>{
      if (this.entryImageTitle) {
        const img = new FormData();
        img.append('imageFile', this.entryImage);
        img.append('imageTitle', this.entryImageTitle);
        img.append('id', "1");
        this.subs.add(this.util.uploadDeviceImage(img).subscribe(()=>{this.getUserDevices()}));
        this.loading=false;
        this.alert.success("Gerät wurde der Geräteliste hinzugefügt")
      } 
    }))
  }
  postSelectedDevice(){
    this.loading=true;
    const adminDeviceId:number[]=[];
      if(this.selectedDevices.length>0){
        this.selectedDevices.forEach(d=>{
         adminDeviceId.push(d.Id)
          })
          this.subs.add(this.util.postAdminDevices(adminDeviceId).subscribe(()=>{this.getUserDevices()}));
          this.loading=false;
          this.alert.success("Gerät wurde der Geräteliste hinzugefügt");
      }else{
        return;
      }
  }
   
  toggleDeviceMenu(device) {
    device.showMenu = !device.showMenu;
  }
  deleteUserDevice(){
    this.loading=true
    this.subs.add(this.util.deleteDevice(this.selectedDevice.Id).subscribe(()=>{
    this.getUserDevices()
    this.loading=true;
    this.alert.success("Gerät wurde erfolgreich gelöscht");
  }))
}
updateDeviceNotification(device,notification){
  this.loading=true
  if(notification=='notifyForFrost'){
    device.notifyForFrost=!device.notifyForFrost;
  }else{
    device.notifyForWind=!device.notifyForWind;
  }
  this.subs.add(this.util.updateDevice(device.Id, device).subscribe(()=>{
    this.getUserDevices()
    this.loading=false
    this.alert.success("Gerät wurde erfolgreich aktualisiert")
  }))
}
addToList(){
  this.selectedDevice.UserDevListId= this.selectedList;
  //this.updateDevice()
}

toUrl(url,small=false, autoscale, isadminDevice) {
  return this.util.toUrl(url,false, autoscale,isadminDevice);
}
resetBadges() {
  this.gardenBadges = {
    Bees: {Count: 0, Id: 447},
    Birds: {Count: 0, Id: 322},
    Insects: {Count: 0, Id: 321},
    Bio: {Count: 0, Id: 445},
  }
}
stepCount(step: number,device) {
  this.loading=true
  if (device.Count < 2 && step < 0) {
    return;
  }
  device.Count +=step;
  this.subs.add(this.util.updateDeviceCount(device).subscribe(()=>{
    this.loading=false
  }))
}
}
