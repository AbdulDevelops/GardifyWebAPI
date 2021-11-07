import { HttpParams } from '@angular/common/http';
import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { AlertService } from 'src/app/services/alert.service';
import { UtilityService } from 'src/app/services/utility.service';

@Component({
  selector: 'app-find-member',
  templateUrl: './find-member.component.html',
  styleUrls: ['./find-member.component.css']
})
export class FindMemberComponent implements OnInit, OnDestroy {
  @Input() presiId;
  @Input() findInContacts=false;
subs=new Subscription()
  userList: any;
  constructor(private util:UtilityService, private alert:AlertService) { }
  ngOnDestroy(): void {
    this.subs.unsubscribe()
    throw new Error('Method not implemented.');
  }

  ngOnInit(): void {
  }
  onSearchMember(searchInput){
    const value=searchInput.target.value
    if(this.findInContacts==true){
      this.util.FindMemberInContacts(value).subscribe(res=>{
        this.userList=res;
       
      })
    }else{
      this.util.FindMember(value).subscribe(res=>{
        this.userList=res;
       
      })
    }
  
  
  }
  addExistingContact(contactName){
    const userName={UserName:contactName}
    if(this.presiId!=undefined){
      
      this.subs.add(this.util.addContact(this.presiId,userName).subscribe(c=>{
        if(c=='Success'){
          this.alert.success("Kontakt wurde erfolgreich hinzugefügt")
        }
      }))
    }else{
      this.subs.add(this.util.addContactFromGardifyUser(userName).subscribe(res=>{
        if(res!==null || res!==undefined){
          this.alert.success("Kontakt wurde erfolgreich hinzugefügt")
        }
      }))
    }
   
  }


}
