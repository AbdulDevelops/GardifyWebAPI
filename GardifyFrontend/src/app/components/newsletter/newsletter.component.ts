import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { UtilityService } from 'src/app/services/utility.service';
import { NewsLetter } from 'src/app/models/models';
import { AlertService } from 'src/app/services/alert.service';
import { faPencilAlt } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-newsletter',
  templateUrl: './newsletter.component.html',
  styleUrls: ['./newsletter.component.css']
})
export class NewsletterComponent implements OnInit {
  
 faPencilAlt=faPencilAlt;
 newsletterForm:FormGroup;
 mode:string;
 submitted = false;
  loading = false;
  newsLetter:NewsLetter;
  constructor(private themeProvider: ThemeProviderService,
    private formBuilder:FormBuilder, 
    private util:UtilityService,
    private alert:AlertService
    ) { 
  
  }

  ngOnInit() {
    this.themeProvider.getTheme().subscribe(t=>this.mode=t);
    
    this.newsletterForm=this.formBuilder.group({
      LastName:['', Validators.required],
      FirstName: ['', Validators.required],
      Email: ['', [Validators.required, Validators.email]],
      
    });
  }
  get f(){
    return this.newsletterForm.controls;
  }
  postNewsLetter(){
    this.submitted = true;
    this.loading=true;
      // stop here if form is invalid
      if (this.newsletterForm.invalid) {
          this.loading=false;
          return this.alert.error("Bitte alle Felder richtig ausfüllen!");
      }
    this.newsLetter=this.newsletterForm.value;
    this.util.newsLetterregs(this.newsLetter).subscribe(res=>{
      this.loading = false;
    },
    (error) => {
      this.loading = false;
    }
      
      );
  }
 unsubscribe(){
  this.loading=true;
   this.util.unsubscribe().subscribe(res=>{
    this.loading = false;
  },
  (error) => {
    this.loading = false;
  }
    
    );
 }
}
