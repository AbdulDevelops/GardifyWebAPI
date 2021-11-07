import { Component, OnInit, OnDestroy } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { AuthService } from 'src/app/services/auth.service';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { faPencilAlt, faChevronDown, faChevronUp} from '@fortawesome/free-solid-svg-icons';

import { Router } from '@angular/router';
import { first } from 'rxjs/operators';
import { SwPush } from '@angular/service-worker';
import { environment } from 'src/environments/environment';
import { UtilityService } from 'src/app/services/utility.service';
import { Subscription } from 'rxjs';
import { ScannerService } from 'src/app/services/scanner.service';
import { UploadedImageResponse, PlaceHolder, UserActionsFrom } from 'src/app/models/models';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css']
})
export class SettingsComponent implements OnInit,OnDestroy {
  mode: string;
  submitted = false;
  loading = false;
  faDown = faChevronDown;
  faUp = faChevronUp;
  faPencilAlt=faPencilAlt;
  settings: FormGroup;
  editDatenForm: FormGroup;
  editGardenForm: FormGroup;
  changePassForm: FormGroup;
  userAddressForm: FormGroup;
  user: any;
  userId: any;
  subs= new Subscription();
  userInfo: any;
  images: UploadedImageResponse[] = [];
  invalidImg = false;
  invalidAdd = false;
  profilImg: any;
  apiCallFrom=new UserActionsFrom();

  cities: any;
  results: any[] = [];
  cityoptions: any[] = [];
  constructor(
    private authService: AuthService,
    private formBuilder: FormBuilder,
    readonly swPush: SwPush, 
    private auth: AuthService,
    private util: UtilityService,
    private router: Router, 
    private scanner: ScannerService,
    private themeproviderService: ThemeProviderService) {
      this.userAddressForm = this.formBuilder.group({
        City: ['', Validators.required],
        Country: ['', Validators.required],
        Street: ['', Validators.required],
        HouseNr:['', Validators.required],
        UserName: ['', Validators.required],
        FirstName: ['', Validators.required],
        LastName: ['', Validators.required],
        Zip: [0, Validators.required]
      });

      this.editGardenForm = this.formBuilder.group({
        City: ['', Validators.required],
        Country: ['', Validators.required],
        Street: ['', Validators.required],
        Zip: [0, Validators.required]
      });
    }

    ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

  ngOnInit() {
    this.subs.add(this.themeproviderService.getTheme().subscribe(m => this.mode = m));
    this.subs.add(this.authService.getUserObservable().subscribe(u => {
      this.user = u;
      this.user.Email = this.isTestAccount() ? 'UserDemo' : this.user.Email;
    }));
    
    this.setUserSettings();
    this.setUserCreds();
    this.setUserAddress();
    this.setGardenAddress();
    this.getProfilImage();
    this.getCityDetails();
  }

  setGardenAddress() {
    this.util.getGardenLocation().subscribe(g => {
      this.editGardenForm.patchValue({
        Street: g.Street,
        Zip: g.Zip + ", " + g.City,
        Country:g.Country,
        HouseNr:g.HouseNr
      });
    });
  }

  setUserCreds() {
    this.editDatenForm = this.formBuilder.group({
      Id: [this.user.UserId, [Validators.required]],
      NewEmail: ['', [Validators.required, Validators.email]],
      Password: ['']
    });
    this.changePassForm = this.formBuilder.group({
      Id: [this.user.UserId, [Validators.required]],
      OldPassword: [''],
      Password: ['', [Validators.required, Validators.minLength(6), Validators.pattern(/(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W)/)]],
      ConfirmPassword: ['']
    });
  }

  setUserAddress() {
    this.util.getUserInfo().subscribe(g => {
      this.userAddressForm.patchValue({
        Street: g.Street,
        Country:g.Country,
        HouseNr:g.HouseNr,
        UserName: g.UserName,
        FirstName: g.FirstName,
        LastName: g.LastName,
        Zip: g.Zip + ", " + g.City
      });
      this.userInfo=g
      this.userInfo.UserName = this.isTestAccount() ? 'UserDemo' : this.userInfo.UserName;
    }
    );
  }

  isTestAccount() {
    return this.auth.isTestAccount();
  }

  setUserSettings() {
    this.subs.add(this.util.getUserSettings(this.apiCallFrom).subscribe(us => {
      this.settings.patchValue({
        ActiveStormAlert: us.ActiveStormAlert,
        ActiveFrostAlert: us.ActiveFrostAlert,
        ActiveNewPlantAlert: us.ActiveNewPlantAlert,
        AlertByEmail: us.AlertByEmail,
        AlertByPush: us.AlertByPush,
        FrostDegreeBuffer: us.FrostDegreeBuffer
      });
    }));
    this.settings = this.formBuilder.group({
      UserId: [this.user.UserId],
      ActiveStormAlert: new FormControl(false),
      ActiveFrostAlert: new FormControl(false),
      ActiveNewPlantAlert: new FormControl(false),
      AlertByEmail: new FormControl(false),
      AlertByPush: new FormControl(false),
      FrostDegreeBuffer: [0]
    });
  }

  stepBuffer(step) {
    this.settings.patchValue({
      FrostDegreeBuffer: this.settings.value.FrostDegreeBuffer + step
    });
  }

  toggleNotifications() {
    if (this.swPush.isEnabled && this.settings.value.AlertByPush) {
      this.swPush.requestSubscription({
          serverPublicKey: environment.VAPID_KEY
      })
      .then(sub =>  this.subs.add(this.util.addNotificationSub(sub.toJSON()).subscribe()))  // needs testing
      .catch(err => console.error('Could not subscribe to notifications', err));
    } else {
      this.subs.add(this.util.unSubNotifications().subscribe());
    }
  }

  updateGardenLocation() {
    if(this.editGardenForm.value.Zip.length > 0){
      var data = this.editGardenForm.value.Zip.split(",");
      this.editGardenForm.value.Zip = data[0];
      this.editGardenForm.value.City = data[1];
      }
    this.subs.add(this.util.updateGardenLocation(this.editGardenForm.value).subscribe());
  }

  updateUserData() {
    if(this.userAddressForm.value.Zip.length > 0){
    var data = this.userAddressForm.value.Zip.split(",");
    this.userAddressForm.value.Zip = data[0];
    this.userAddressForm.value.City = data[1];
    }
    this.subs.add(this.util.updateUserData(this.userAddressForm.value).subscribe(() => {
      this.authService.updateUser$({Name: `${this.userAddressForm.value.FirstName} ${this.userAddressForm.value.LastName}`});
    }));
  }

  updateUserSettings() {
    this.subs.add(this.util.updateUserSettings(this.settings.value).subscribe());
  }

  updateUserCreds() {
    this.submitted = true;
    if (this.editDatenForm.invalid) { return; }
    this.loading = true;

    this.subs.add(this.authService.updateUser(this.user.UserId, this.editDatenForm.value)
    .pipe(first())
    .subscribe(
      // on success (empty response), ask to verify new email
        data => {
          if (data && data.Message) {
            return;
          }
          this.router.navigate(['/einstellungen']);
        },
        error => {
          this.loading = false;
        }
    ));
  }

  getCityDetails(){
    this.subs.add(this.util.getCityDetails().subscribe(data=>{
      this.cities=data;
    }))
  }
  searchZip(data){
    this.results = [];
    if(data.length > 0 && this.cities){
    for (var val of this.cities) {  
    if((val.ZipCode + '').startsWith(data)){
      this.results.push(val); 
    }
    }
    //So that it wont overload
    this.cityoptions = this.results;}
    
  }
  changePassword() {
    this.submitted = true;
    if (this.changePassForm.invalid) { return; }
    this.loading = true;

    this.subs.add(this.authService.changeUserPassword(this.user.UserId, this.changePassForm.value)
    .pipe(first())
    .subscribe(
        data => {
          if (data && data.Message) {
            return;
          }
          this.router.navigate(['/']);
        },
        error => {
          this.loading = false;
        }
    ));
  }

  onDeleteUser() {
    this.submitted = true;
    this.loading = true;
    this.subs.add(this.authService.deleteUser(this.user.UserId).subscribe( d => {
      this.authService.logout()
      this.router.navigate(['/deletionconfirmation']);
    }));
  }

  public toggleTheme() {
    this.themeproviderService.toggleTheme();
  }
  getProfilImage(){
    this.subs.add(this.util.getUserProfilImg().subscribe(data=>{
      this.profilImg=data
    }))
  }
  async onImageUpload(event) {
    const res = await this.scanner.handleImageUpload(event);
    if (res.err) {
      this.invalidImg = !!res.err;
      return;
    }
    this.invalidImg = false;
    this.images.push(res);
    this.uploadImage();
  }
  uploadImage(){
    this.loading = true;
    this.images.forEach(img => {
      const imgForm = new FormData();
      imgForm.append('imageFile', img.file);
      imgForm.append('imageTitle', img.title);
      this.subs.add(this.util.uploadProfilImg(imgForm).subscribe(()=>{
        this.getProfilImage()
        this.loading = false;
      }));
    });
  }

  get hasEmail() {
    return this.user.Email.indexOf('@') > 0;
  }
  
  get f() {
    return this.editDatenForm.controls;
  }

  get d() {
    return this.userAddressForm.controls;
  }

  get control() {
    return this.changePassForm.controls;
  }

  get gControls() {
    return this.editGardenForm.controls;
  }
  toUrl(url: string) {
    if (url) {
      return this.util.toUrl(url, false, false);
    }
    return PlaceHolder.Profile;
  }
}
