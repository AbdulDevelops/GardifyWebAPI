import { Component, OnInit, HostListener, OnDestroy } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { Subscription } from 'rxjs';
import { UtilityService } from 'src/app/services/utility.service';
import { ReferenceToModelClass, UserWarning } from 'src/app/models/models';
import { faEye, faCheck, faInfo } from '@fortawesome/free-solid-svg-icons';
import { StatCounterService } from 'src/app/services/stat-counter.service';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { AlertService } from 'src/app/services/alert.service';

interface Weather {
  currentTemp: any;
  wind: any;
  precipitation: any;
  pressure: any;
  humidity: any;
}
@Component({
  selector: 'app-warnungen',
  templateUrl: './warnungen.component.html',
  styleUrls: ['./warnungen.component.css']
})
export class WarnungenComponent implements OnInit,OnDestroy {
  mode: string;
  faEye = faEye; faCheck = faCheck;
  weather: Weather = {currentTemp: '', wind: '', precipitation: '', pressure: '', humidity: ''};
  subs=new Subscription;
  loading = false;
  warnings: UserWarning[] = [];
  affectedDevices = [];
  affectedPlants = [];
  warningSettings: FormGroup;
  switchButtonOn='./assets/images/Mein_Garten_Ökoelemente_Button_On.png';
  switchButtonOff='./assets/images/Mein_Garten_Ökoelemente_Button_Off.png'
  count:number;
  faInfo = faInfo;
  disabledPlantsFrost = true;
  disabledPlantsWind = true;
  disabledDevicesFrost = true;
  disabledDevicesWind = true;

  constructor(
    private themeProvider: ThemeProviderService, 
    private util: UtilityService, 
    private sc: StatCounterService,
    private formBuilder:FormBuilder,
    private alert:AlertService
    ) { }
    
  ngOnDestroy(): void {
    this.subs.unsubscribe();
    this.warningSettings = this.formBuilder.group({
      ActiveStormAlert: new FormControl(false),
      ActiveFrostAlert: new FormControl(false),});
  }

  ngOnInit() {
    this.loading=true;
    this.subs.add(this.themeProvider.getTheme().subscribe(t => this.mode = t));
    this.getwarning();
  }
  getwarning() {
    this.subs.add(this.util.getwarning().subscribe(warnings => {
      this.affectedDevices = warnings.filter(w => w.ObjectType === ReferenceToModelClass.Device);
      this.affectedPlants = warnings.filter(w => w.ObjectType === ReferenceToModelClass.Plant);
      this.setDisabledWarnings();
      this.loading=false;
    }, (err) => this.loading = false));
  }

  setDisabledWarnings() {
    this.disabledDevicesFrost = !this.affectedDevices.some(dev => dev.NotifyForFrost);
    this.disabledDevicesWind = !this.affectedDevices.some(dev => dev.NotifyForWind);
    this.disabledPlantsFrost = !this.affectedPlants.some(p => p.NotifyForFrost);
    this.disabledPlantsWind = !this.affectedPlants.some(p => p.NotifyForWind);
  }

  toggleAllPlants(forFrost = false) {
    this.loading = true;
    const newState = forFrost ? this.disabledPlantsFrost : this.disabledPlantsWind;
    this.subs.add(this.util.togglePlantsWarnings(forFrost, newState).subscribe(warnings => {
      this.affectedPlants = warnings.filter(w => w.ObjectType === ReferenceToModelClass.Plant);
      this.setDisabledWarnings();
      this.loading = false;
    }));
  }

  toggleAllDevices(forFrost = false) {
    this.loading = true;
    const newState = forFrost ? this.disabledDevicesFrost : this.disabledDevicesWind;
    this.subs.add(this.util.toggleDevicesWarnings(forFrost, newState).subscribe(warnings => {
      this.affectedDevices = warnings.filter(w => w.ObjectType === ReferenceToModelClass.Device);
      console.log(this.affectedDevices)
      this.setDisabledWarnings();
      this.loading = false;
    }));
  }

  resetWarnings() {
    this.loading = true;
    this.subs.add(this.util.resertWarnings().subscribe(warnings => {
      this.affectedDevices = warnings.filter(w => w.ObjectType === ReferenceToModelClass.Device);
      this.affectedPlants = warnings.filter(w => w.ObjectType === ReferenceToModelClass.Plant);
      this.setDisabledWarnings();
      this.loading = false;
    }, (err) => this.loading = false));
  }

  updateWarningPlantSettings(warningforPlant, forFrost: boolean) {
    this.loading=true;
    this.subs.add(this.util.updateWarningPlantNotification(warningforPlant.RelatedObjectId, forFrost).subscribe(()=> {
      this.loading=false;
      this.alert.success('Warnung wurde erfolgreich aktualisiert');
    }));
  }

  updateWarningDeviceSettings(warningforDevice: any, forFrost: boolean) {
    this.loading=true;
    this.subs.add(this.util.updateDeviceNotification(warningforDevice.RelatedObjectId, forFrost).subscribe(()=> {
      this.loading=false;
      this.alert.success('Warnung wurde erfolgreich aktualisiert');
    }));
  }
  setWarningAsReaded(relatedObjectId:number,objectType:number){
    this.loading=true;
    this.subs.add(this.util.dismissWarning(relatedObjectId,objectType).subscribe(()=>{
      this.sc.requestWarningsCount().subscribe(()=>{
        this.getwarning()
      })
    }))
  }
}
