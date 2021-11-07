import { HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { UtilityService } from 'src/app/services/utility.service';

@Component({
  selector: 'app-other-presentation',
  templateUrl: './other-presentation.component.html',
  styleUrls: ['./other-presentation.component.css']
})
export class OtherPresentationComponent implements OnInit {
  subs=new Subscription;
  mode: any;
  presentation: any=[];

  constructor(private tp:ThemeProviderService, private util:UtilityService) { }
  ngOnDestroy(): void {
    this.subs.unsubscribe() 
   }

  ngOnInit(): void {
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
    this.showSinglePresi()
    this.showOtherPresi()
  }
  showSinglePresi(presiId=1){
    this.util.getUserPresentationsById(presiId).subscribe(presi=>{
     this.presentation=presi
    })
  }
showOtherPresi(){
  const showAll = false 
  const showContact = true
  const showAcceptedContact = false
  let params= new HttpParams()
  params=params.append('showAll',showAll.toString())
  params=params.append('showContact',showContact.toString())
  params=params.append('showAcceptedContact',showAcceptedContact.toString())
  this.subs.add(this.util.getOtherPresi(params).subscribe(res=>{
    this.presentation=res
  }))
 
}

  toUrl(url, small = true) {
    return this.util.toUrl(url, small);
  }
}
