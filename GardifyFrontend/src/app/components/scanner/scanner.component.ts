import { Component, OnInit, OnDestroy, AfterViewInit, ViewChild, ElementRef } from '@angular/core';
import { FormGroup, FormBuilder, FormControl } from '@angular/forms';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { faSearch } from '@fortawesome/free-solid-svg-icons';
import { Ad, ScanResult } from 'src/app/models/models';
import { PlantSearchService } from 'src/app/services/plant-search.service';
import { ScannerService } from 'src/app/services/scanner.service';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { AuthService } from 'src/app/services/auth.service';

declare var jQuery:any;

@Component({
  selector: 'app-scanner',
  templateUrl: './scanner.component.html',
  styleUrls: ['./scanner.component.css']
})
export class ScannerComponent implements OnInit, OnDestroy, AfterViewInit {
  mainForm: FormGroup;
  @ViewChild('infoPopup', { static: true }) popup: ElementRef;
  mode: string;
  faSearch = faSearch;
  iconhidden:boolean;
  errorMessage = '';
  plants = new ScanResult();
  searchResults = 0;
  wikilinks: string[] = [];

  files: any;
  fileSrc: any;

  tagline: string;
  valid: boolean;
  submitted: boolean;
  subs = new Subscription();
  ad:Ad
  adBetween:Ad
  adSides:Ad
  constructor(private themeProvider: ThemeProviderService, private plantSearch: PlantSearchService,
    private fb: FormBuilder, private scannerService: ScannerService,
    private router: Router, private auth: AuthService) {
      this.mainForm = this.fb.group({
        img: [],
        google: new FormControl(false),
        plantnet: new FormControl(false),
      });
      this.ad = new Ad(
        'ca-pub-3562132666777902',
        7467573047,
        'auto',
        true,
        false,
        null
      ) 
      this.adBetween = new Ad(
        'ca-pub-3562132666777902',
        6022429748,
        'auto',
        true,
        false,
        null
      ) 
      this.adSides = new Ad(
        'ca-pub-3562132666777902',
        1901007865,
        'auto',
        true,
        true,
        null
      ) 
    }

  ngOnInit() {
    this.subs.add(this.themeProvider.getTheme().subscribe(t => this.mode = t));
    if (!localStorage.getItem('showPopup')) {
      localStorage.setItem('showPopup', 'true');
    }
  }

  ngAfterViewInit() {
    setTimeout(() => {
      if (localStorage.getItem('showPopup') === 'true') {
        jQuery(this.popup.nativeElement).modal('show');
      }
    }, 800);
  }

  hidePopup() {
    localStorage.setItem('showPopup', 'false');
    jQuery(this.popup.nativeElement).modal('hide'); 
  }

  ngOnDestroy() {
    this.subs.unsubscribe()
    this.scannerService.scanResults = this.plants;
  }

  async imageUpload(event) {
    const res = await this.scannerService.handleImageUpload(event, true);
    if (res.err) {
      this.fileSrc = null;
      this.tagline = res.err;
      return;
    }
    this.mainForm.patchValue({
      img: res.file
    });
    this.fileSrc = res.src;
    this.tagline = res.file.name;
  }

  search() {
    this.submitted = true;
    this.searchResults = 0;
    const data = new FormData();
    data.append('img', this.mainForm.value.img);
   /*  data.append('google', this.mainForm.value.google);
    data.append('plantnet', this.mainForm.value.plantnet); */

    this.subs.add(this.plantSearch.scanImage(data).subscribe(t => {
      this.submitted = false;
      this.plants.GPlants = t.GPlants;
      this.plants.GImages = t.GImages;
      this.plants.PnResults = t.PnResults;

      if (this.plants.GPlants) {
        this.plants.GPlants.forEach(p => {
          const link = JSON.parse(p.Links)[3];
          if (link.length) { this.wikilinks = this.wikilinks.concat(link); }
          if (p.NameLatin) { p.NameLatin = p.NameLatin.replace(/\[k\]/g, '').replace(/\[\/k\]/g, ''); }
        });
      }
      if (this.plants.PnResults.InDb) {
        this.plants.PnResults.InDb.forEach(p => {
          p.NameLatin = p.NameLatin.replace(/\[k\]/g, '').replace(/\[\/k\]/g, '');
          p.Description = p.Description.replace(/\[k\]/g, '<i>').replace(/\[\/k\]/g, '</i>');
        });
      }
      this.router.navigate(['/scanresults']);
    },
    (error)=> {
      this.errorMessage = 'Irgendwas ist schiefgelaufen. Versuche es bitte später nochmal.';
      this.submitted = false;
    }
    ));
  }

  get isValid() {
    // return this.mainForm.value.img && (this.mainForm.value.google || this.mainForm.value.plantnet);
    return this.mainForm.value.img ;
  }

  get isAdmin() {
    return this.auth.isAdmin();
  }

  resetSearch() {
    this.mainForm.reset();
    this.submitted = false;
  }

  slugify(tag: any) {
    return tag.Description || tag.NameLatin || tag.NameGerman;
  }
}
