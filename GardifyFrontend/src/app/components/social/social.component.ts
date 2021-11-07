import { HttpParams } from '@angular/common/http';
import { Component, HostListener, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DomSanitizer } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { faInfoCircle } from '@fortawesome/free-solid-svg-icons';
import { Subscription } from 'rxjs';
import { first } from 'rxjs/operators';
import { InstaPost, UserActionsFrom } from 'src/app/models/models';
import { AuthService } from 'src/app/services/auth.service';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { UtilityService } from 'src/app/services/utility.service';
import { MustMatch } from 'src/app/_helpers/must-match.validators';
import { ZipCodeMustMatch } from 'src/app/_helpers/zipcode-match.validators';


@Component({
  selector: 'app-social',
  templateUrl: './social.component.html',
  styleUrls: ['./social.component.css']
})
export class SocialComponent implements OnInit {
  subs = new Subscription();
  mode:any
  play=false;
  registrationForm: FormGroup;
  loading: boolean;
  newsLetterAbo=false;
  register=false;
  faInfoCircle = faInfoCircle;
  instaPostEntry:InstaPost;
  instaPostList: any[]=[];
  showInstapost: boolean=true;
  newsList: any;
  apiCallFrom= new UserActionsFrom();
  actualTipps: any;
  show = true;
  selectedPost:any;
  isMobile: boolean;
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
      title: 'PFLANZEN SUCHE',
      text: 'Die Pflanzensuche hilft dir, die richtigen Pflanzen zu finden. Dazu kannst du alle Pflanzen in der Datenbank nach 300 Eigenschaften in Sekunden sortieren. Aus der Pflanzensuche heraus, kannst du auch Pflanzen direkt in deinen Garten kopieren.'
    },
    {
      step: 4,
      title: 'PFLANZEN SCAN',
      text: 'Du hast eine Pflanze im Garten, die du nicht kennst? Lade hier zur Pflanzenbestimmung einfach ein Foto der Pflanze hoch oder nutze deine Smartphone-Kamera. Du erhältst dann Vorschläge, um welche Pflanze es sich handeln könnte. Achte darauf, dass du nah genug herangehst und nur eine Pflanze auf dem Bild zu sehen ist. Ist die Pflanze noch nicht in unserer Datenbank, kannst du sie zur Erfassung vorschlagen.'
    },
    {
      step: 5,
      title: 'PFLANZEN DOC',
      text: 'Deine Pflanze leidet unter einem Schädlingsbefall oder einer dir unbekannten Krankheit? Frag den Pflanzen-Doc! Suche in den Beiträgen oder eröffne ein neues Thema. Lade dazu das Foto hoch und beschreibe das Problem möglichst genau.'
    },
    {
      step: 6,
      title: 'GARDIFY VIDEOS',
      text: ''
    },
  ]
  currentTourStep = 0;
  width: number;
  viewpoint: string;
  password:string;
  confirmedPassword:string;
  noMatch: boolean;
  zipCode: any;
  country: any;
  countryMatch: boolean=false;
  count: any;
  plantsPager: any;
  newsPager: any;
  constructor(
    private themeProvider: ThemeProviderService,
    private router: Router,
    private util:UtilityService,
    private authService:AuthService,
    private formBuilder: FormBuilder,
    private sanitizer: DomSanitizer
  ) {
    
   }
   @HostListener('window:resize')
   onWindowResize() {
     this.width = window.innerWidth;
     this.setViewpoint();
     this.isMobile = window.innerWidth <= 990;
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
  ngOnInit(): void {
    this.util.tourStep$.subscribe(step => this.currentTourStep = step);
    this.isMobile = window.innerWidth <= 990;
    this.subs.add(this.themeProvider.getTheme().subscribe(t => this.mode = t));
    this.registrationForm = this.formBuilder.group({
      PLZ: ['', [Validators.required,Validators.minLength(4)]],
      Country: ['', Validators.required],
      UserName: ['', [Validators.required, Validators.minLength(4)]],
      Email: ['', [Validators.required, Validators.email]],
      Password: ['', [Validators.required, Validators.minLength(6), Validators.pattern(/(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W)/)]],
      ConfirmPassword: ['', Validators.required],
      model: new UserActionsFrom()

    },
    {
      validators: [MustMatch('Password', 'ConfirmPassword'),ZipCodeMustMatch('Country','PLZ')]} 
    );
    this.register=false;
    let params = new HttpParams();
    var page=1
    var take=20
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
    params = params.append('skip', page.toString());
    params = params.append('take', take.toString());
    this.subs.add(this.util.getAllNews(params).subscribe(n => {
      n.ListEntries.forEach(l => {
        l.Text = l.Text.replace(/\[k]|\[k]/g, '<i>').replace(/\[\/k\]/g, '</i>');
      });
      this.newsList = n.ListEntries;
    }));
    this.country="Land auswählen *"
    console.log(this.country)
    this.getActualTipps();
    this.getInstaPosts(1)
  }
  socialPageReg(){

    this.loading = true;
    this.register=true;
    if(this.registrationForm.invalid){
      return;
    }
    this.subs.add(this.authService.register(this.registrationForm.value, true,this.newsLetterAbo)
    .pipe(first())
    .subscribe(
        data => {
          if (data && data.Token) {
            localStorage.setItem('currentUser', JSON.stringify(data));
            this.authService.user.next(JSON.parse(localStorage.getItem('currentUser')));
          }
         if(this.router.url.includes("social")){
          this.router.navigate(['/social/register/danke']);

         }
         else if(this.router.url.includes("flora")){
          this.router.navigate(['/gartenflora/register/danke']);

        }else{
          this.router.navigate(['/register/danke']);

        }
          this.loading = false;
        },
        error => {
          this.loading = false;
        }
    ));
  }
  getInstaPosts(skip){
    let params = new HttpParams();
    var page=skip-1
    var take=20
    params = params.append('skip', page.toString());
    params = params.append('take', take.toString());
    this.subs.add(this.util.getInstaEntries(params).subscribe(e=>{
       e.data.forEach(element => {
      this.instaPostEntry ={Text:element.caption,
         MediaEntry:this.sanitizer.bypassSecurityTrustResourceUrl(element.media_url),
          Media_Type:element.media_type, 
          Expanded:false, 
          Date:element.timestamp}
      
      this.instaPostList.push(this.instaPostEntry)
    })
    this.instaPostList.slice(0,8)
  }
    ))
  }
  
  changeTourStep(step) {
    const tourWrapper = document.getElementsByClassName('step-wrapper')[0];
    if (tourWrapper) {
      tourWrapper.scrollIntoView({behavior: 'smooth', block: 'nearest', inline: 'nearest'});
    }
    const currentTourStep = this.currentTourStep + step;
    if (this.currentTourStep < 1) { this.currentTourStep = 1; return; }
    if (this.currentTourStep > 5) { this.currentTourStep = 5; return; }

    const slideRight = step > 0 && ((currentTourStep === 3) || (this.isMobile && currentTourStep % 2 !== 0));
    const sildeLeft = step < 0 && ((currentTourStep === 5) || (this.isMobile && currentTourStep % 2 === 0));
    console.log(slideRight)
    if (!slideRight && !sildeLeft) {
      this.currentTourStep = currentTourStep;
      return;
    }
    
    // auto scroll nav when outside view
    if (slideRight) {
     
      (document.getElementsByClassName('carousel-control-next controlFirst')[0] as HTMLElement).click();
    }
    if (sildeLeft) {
      (document.getElementsByClassName('carousel-control-prev controlFirst')[0] as HTMLElement).click();
    }

    setTimeout(() => {
      this.currentTourStep = currentTourStep;
    }, 500);
   
  }
  startTour(step) {
    this.subs.add(this.util.startTourOnSocialPage(step).subscribe());
  }

  closeTour() {
    this.util.endTour();
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }
  showInstaPosts(){
    this.showInstapost = true
    return this.showInstapost
  }
  toggle() {
    this.show=!this.show;
  }
  getActualTipps(){
    this.subs.add(this.util.getArticles(0,8,this.apiCallFrom).subscribe(a=>{
      this.actualTipps=a.ListEntries;
    }))
  }
  toUrl(url, small = false) {
    return this.util.toUrl(url, small);
  }
  hideInstaPosts(){
    this.showInstapost = false
    return this.showInstapost
  }
  /* checkPasswords(group: FormGroup) { // here we have the 'passwords' group
    const password = group.get('password').value;
    const confirmPassword = group.get('confirmPassword').value;
  
    return password === confirmPassword ? null : { notSame: true }     
  } */
  
  public onChange(event: any) {
    this.confirmedPassword= event.target.value;
    this.password= this.registrationForm.controls.Password.value
    this.noMatch = this.password !== this.confirmedPassword;
  }
  /* public checkZipcode(event){
    this.zipCode=event.target.value;
    this.country=this.registrationForm.controls.Country.value
    if(this.zipCode>=1000 && this.zipCode<=9999){
      this.countryMatch=this.country!=="Deutschland";
    }
    if(this.zipCode>=9999 && this.zipCode<=99999 ){
      this.countryMatch=this.country==="Deutschland";
    }
    return this.countryMatch
  } */
  get f() {
    return this.registrationForm.controls;
  }
}
