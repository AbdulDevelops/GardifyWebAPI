import { Directive, ElementRef } from '@angular/core';
import { faEye, faEyeSlash } from '@fortawesome/free-solid-svg-icons';


@Directive({
  selector: '[appAppPassword]'
})
export class AppPasswordDirective {
private shown=false;
faEyeSlash=faEyeSlash;
faEye=faEye;
  constructor(private el:ElementRef) {
    this.setup();
   }
    
    setup(){
      const parent= this.el.nativeElement.parentNode;
      const span= document.createElement('span');
      span.innerHTML="<img class= 'h_30' src='./assets/main-icons/Passwort_Auge.svg' />";
      span.addEventListener('click',(event)=>{
        this.toggle(span);
      });
      parent.appendChild(span);
    }
    toggle(span: HTMLElement){
      this.shown=!this.shown;
      if(this.shown){
      this.el.nativeElement.setAttribute('type','text');
      span.innerHTML="<img class= 'h_30' src='./assets/main-icons/Passwort_Auge.svg' />";
    }else{
      this.el.nativeElement.setAttribute('type','password');
      span.innerHTML="<img class= 'h_30' src='./assets/main-icons/Passwort_Auge.svg' />";
    }
    }
    
}

