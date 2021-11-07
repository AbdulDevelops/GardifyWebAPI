import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { EmployeesImages, StatutIcons, UserActionsFrom } from 'src/app/models/models';
import { StatCounterService } from 'src/app/services/stat-counter.service';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';

@Component({
  selector: 'app-team',
  templateUrl: './team.component.html',
  styleUrls: ['./team.component.css']
})
export class TeamComponent implements OnInit, OnDestroy {
  mode: string;
  subs= new Subscription()
  employeeList=[
    {
    EmployeeId:1,
    Title:'DR.',
    Firstname:'MARKUS',
    Lastname:'PHLIPPEN',
    Position:'Wissenschaftliche Leitung',
    PositionId:1,
    FavouritPlantLatin:'',
    FavouritPlantGerman:'',
    FavouriteCitation:'„Gardening is cheaper than therapy and you have tomatoes.”',
    profilBildSrc:''
  },
  {
    EmployeeId:2,
    Title:'DIPL. BIO.',
    Firstname:'JASMIN ',
    Lastname:'OBHOLZER',
    Position:'Wissenschaftliche Mitarbeiterin',
    PositionId:1,
    FavouritPlantLatin:'',
    FavouritPlantGerman:'',
    FavouriteCitation:'„Life‘s a garden and good friends are the flowers!” ',
    profilBildSrc:''
  },
  {
    EmployeeId:3,
    Title:'M. SC. ',
    Firstname:'MELANIE ',
    Lastname:'BAYO',
    Position:'Wissenschaftliche Mitarbeiterin',
    PositionId:1,
    FavouritPlantLatin:'Hoya carnosa',
    FavouritPlantGerman:'Porzellanblume',
    FavouriteCitation:'',
    profilBildSrc:''
  },
  {
    EmployeeId:4,
    Title:'M. SC. ',
    Firstname:'THERESE  ',
    Lastname:'LIOUVILLE',
    Position:'Wissenschaftliche Mitarbeiterin',
    PositionId:1,
    FavouritPlantLatin:'Tulipa linifolia',
    FavouritPlantGerman:'Leinblättrige Tulpe',
    FavouriteCitation:'',
    profilBildSrc:''
  },
  {
    EmployeeId:5,
    Title:'DR.',
    Firstname:'JANINE   ',
    Lastname:'SOMMER',
    Position:'Wissenschaftliche Mitarbeiterin',
    PositionId:1,
    FavouritPlantLatin:'',
    FavouritPlantGerman:'',
    FavouriteCitation:'„Blumen sind das Lächeln der Erde.“ R. W. Emerson',
    profilBildSrc:''
  },
  {
    EmployeeId:6,
    Title:' ',
    Firstname:'JANA',
    Lastname:'ADAM',
    Position:'Wissenschaftliche Mitarbeiterin',
    PositionId:1,
    FavouritPlantLatin:'',
    FavouritPlantGerman:'',
    FavouriteCitation:'„Nature is my wisest self, made visible outside of me.” ',
    profilBildSrc:''
  },
  {
    EmployeeId:7,
    Title:' ',
    Firstname:'MARTHA',
    Lastname:'AGNES',
    Position:'Wissenschaftliche Mitarbeiterin',
    PositionId:1,
    FavouritPlantLatin:'Aquilegia vulgaris',
    FavouritPlantGerman:'Gemeine Akelei',
    FavouriteCitation:'',
    profilBildSrc:''
  },
  {
    EmployeeId:8,
    Title:' ',
    Firstname:'VANESSA ',
    Lastname:'SCHMOLKE',
    Position:'Wissenschaftliche Mitarbeiterin',
    PositionId:1,
    FavouritPlantLatin:'Clematis viticella',
    FavouritPlantGerman:'Italienische Waldrebe',
    FavouriteCitation:'',
    profilBildSrc:''
  },
  {
    EmployeeId:9,
    Title:'M. SC.',
    Firstname:'KARIN',
    Lastname:'BECKER',
    Position:'Wissenschaftliche Mitarbeiterin',
    PositionId:1,
    FavouritPlantLatin:'Helianthus annuus',
    FavouritPlantGerman:'Sonnenblume',
    FavouriteCitation:'',
    profilBildSrc:''
  },
  {
    EmployeeId:10,
    Title:'DIPL.-DES. ',
    Firstname:'JUSTYNA ',
    Lastname:'SCHWERTNER',
    Position:'Leitung Grafik',
    PositionId:2,
    FavouritPlantLatin:'',
    FavouritPlantGerman:'Perückenstrauch – wegen des lustigen Namens und der „Wuschelfrisur“ im Herbst',
    FavouriteCitation:'',
    profilBildSrc:''
  },
  {
    EmployeeId:11,
    Title:'DIPL.-DES.',
    Firstname:'CAROLIN ',
    Lastname:'MOHR',
    Position:'Grafik',
    PositionId:2,
    FavouritPlantLatin:'Dahlia ‚Pooh‘',
    FavouritPlantGerman:'Halskrausendahlie ‚Pooh‘',
    FavouriteCitation:'',
    profilBildSrc:''
  },
  {
    EmployeeId:12,
    Title:'',
    Firstname:'ELLEN  ',
    Lastname:'SCHLÜTER',
    Position:'Grafik',
    PositionId:2,
    FavouritPlantLatin:'',
    FavouritPlantGerman:'',
    FavouriteCitation:'„Grün ist nicht alles, aber ohne Grün ist alles nichts.“ Hans-Hermann Bentrup',
    profilBildSrc:''
  },
  {
    EmployeeId:13,
    Title:'',
    Firstname:'MAJA   ',
    Lastname:'SAUVANT',
    Position:'Grafik',
    PositionId:2,
    FavouritPlantLatin:'Alocasia zebrina',
    FavouritPlantGerman:'Pfeilblatt',
    FavouriteCitation:'',
    profilBildSrc:''
  },
  {
    EmployeeId:14,
    Title:'',
    Firstname:'PHILINE',
    Lastname:'ANASTASOPOULOS',
    Position:'Projektmanagement',
    PositionId:3,
    FavouritPlantLatin:'Wisteria sinensis',
    FavouritPlantGerman:'Chinesischer Blauregen, Glyzinie',
    FavouriteCitation:'',
    profilBildSrc:''
  },
  {
    EmployeeId:15,
    Title:'',
    Firstname:'VALERIE',
    Lastname:'MAYER',
    Position:'Presse',
    PositionId:4,
    FavouritPlantLatin:'',
    FavouritPlantGerman:'',
    FavouriteCitation:'„Ich freue mich, wenn es regnet. Wenn ich mich nicht freue, regnet es trotzdem.“',
    profilBildSrc:''
  },
  {
    EmployeeId:16,
    Title:'',
    Firstname:'KATERINA',
    Lastname:'STEGEMANN',
    Position:'Buchhaltung ',
    PositionId:5,
    FavouritPlantLatin:'Lavandula angustifolia',
    FavouritPlantGerman:'Echter Lavendel',
    FavouriteCitation:'',
    profilBildSrc:''
  },
  {
    EmployeeId:17,
    Title:'',
    Firstname:'ANNIKA',
    Lastname:'STEINACKER',
    Position:'Social Media',
    PositionId:6,
    FavouritPlantLatin:'Aesculus hippocastanum',
    FavouritPlantGerman:'Gemeine Rosskastanie',
    FavouriteCitation:'',
    profilBildSrc:''
  },
  {
    EmployeeId:18,
    Title:'',
    Firstname:'KARL FRIEDRICH ',
    Lastname:'JOEST',
    Position:'Leitung Programmierung',
    PositionId:7,
    FavouritPlantLatin:'Olea europaea',
    FavouritPlantGerman:'Olivenbaum',
    FavouriteCitation:'',
    profilBildSrc:''
  },
  {
    EmployeeId:19,
    Title:'',
    Firstname:'KARIN',
    Lastname:'KOKOSCHKA',
    Position:'Projektmanagement Programmierung',
    PositionId:7,
    FavouritPlantLatin:'Glebionis coronaria',
    FavouritPlantGerman:'Kronen-Wucherblume',
    FavouriteCitation:'',
    profilBildSrc:''
  },
  {
    EmployeeId:20,
    Title:'',
    Firstname:'JOHANNA',
    Lastname:'HÄNICHEN',
    Position:'Geschäftsführerin',
    PositionId:8,
    FavouritPlantLatin:'',
    FavouritPlantGerman:'',
    FavouriteCitation:'„In der Einfachheit und Stille der Natur findet der Mensch die Lebenskraft.” Julius Grosse',
    profilBildSrc:''
  },
  {
    EmployeeId:21,
    Title:'',
    Firstname:'ANDREAS ',
    Lastname:'GREWE',
    Position:'Geschäftsführer',
    PositionId:8,
    FavouritPlantLatin:'Sequoiadendron giganteum ',
    FavouritPlantGerman:'Riesenmammutbaum',
    FavouriteCitation:'',
    profilBildSrc:''
  },
  {
    EmployeeId:22,
    Title:'',
    Firstname:'RALF',
    Lastname:'JOEST',
    Position:'Geschäftsführer',
    PositionId:8,
    FavouritPlantLatin:'',
    FavouritPlantGerman:'',
    FavouriteCitation:'„Gras wächst nicht schneller wenn man daran zieht.“',
    profilBildSrc:''
  },
  {
    EmployeeId:23,
    Title:'',
    Firstname:'BOB',
    Lastname:'THE DIGGER',
    Position:'Bodenanalyse',
    PositionId:8,
    FavouritPlantLatin:'',
    FavouritPlantGerman:'',
    FavouriteCitation:'„Life‘s a garden. Dig it.“',
    profilBildSrc:''
  },
]
apiCallFrom= new UserActionsFrom();

  constructor(private themeProvider:ThemeProviderService, private stats: StatCounterService) { }

  ngOnInit(){
    this.subs.add(this.themeProvider.getTheme().subscribe(t=>this.mode= t))
    this.subs.add(this.stats.createStatEntry('teamview',this.apiCallFrom).subscribe());
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  getIconSrc(positionId){
    var iconSrce;
    switch(positionId){
      case 1: iconSrce= StatutIcons.Research; break;
      case 2: iconSrce=StatutIcons.Graphic;break;
      case 3: iconSrce=StatutIcons.Projectmanagement; break;
      case 4: iconSrce= StatutIcons.Press;break;
      case 5: iconSrce=StatutIcons.Accounting;break;
      case 6: iconSrce= StatutIcons.Social_Media;break;
      case 7: iconSrce= StatutIcons.Programming;break;
      case 8: iconSrce= StatutIcons.ExecutiveDirector;break;
      case 9: iconSrce=StatutIcons.SoilAnalysis;break;
    }
    return iconSrce;
  }
  getEmployeeImage(employeeId){
    var employeeImgSrc;
    switch(employeeId){
      case 1:employeeImgSrc=EmployeesImages.Employee1;break;
      case 2:employeeImgSrc=EmployeesImages.Employee2;break;
      case 3:employeeImgSrc=EmployeesImages.Employee3;break;
      case 4:employeeImgSrc=EmployeesImages.Employee4;break;
      case 5:employeeImgSrc=EmployeesImages.Employee5;break;
      case 6:employeeImgSrc=EmployeesImages.Employee6;break;

      case 7:employeeImgSrc=EmployeesImages.Employee7;break;
      case 8:employeeImgSrc=EmployeesImages.Employee8;break;
      case 9:employeeImgSrc=EmployeesImages.Employee9;break;
      case 10:employeeImgSrc=EmployeesImages.Employee10;break;
      case 11:employeeImgSrc=EmployeesImages.Employee11;break;
      case 12:employeeImgSrc=EmployeesImages.Employee12;break;
      case 13:employeeImgSrc=EmployeesImages.Employee13;break;
      case 14:employeeImgSrc=EmployeesImages.Employee14;break;
      case 15:employeeImgSrc=EmployeesImages.Employee15;break;
      case 16:employeeImgSrc=EmployeesImages.Employee16;break;
      case 17:employeeImgSrc=EmployeesImages.Employee17;break;
      case 18:employeeImgSrc=EmployeesImages.Employee18;break;
      case 19:employeeImgSrc=EmployeesImages.Employee19;break;
      case 20:employeeImgSrc=EmployeesImages.Employee20;break;
      case 21:employeeImgSrc=EmployeesImages.Employee21;break;
      case 22:employeeImgSrc=EmployeesImages.Employee22;break;
      case 23:employeeImgSrc=EmployeesImages.Employee23;break;
    }
    return employeeImgSrc;
  }
}
