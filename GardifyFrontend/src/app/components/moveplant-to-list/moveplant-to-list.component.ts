import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { AlertService } from 'src/app/services/alert.service';
import { UtilityService } from 'src/app/services/utility.service';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';

@Component({
  selector: 'app-moveplant-to-list',
  templateUrl: './moveplant-to-list.component.html',
  styleUrls: ['./moveplant-to-list.component.css']
})
export class MoveplantToListComponent implements OnInit {
  selectedItems = [];
  selectedListIds: any=[];
  @Input() selectedPlant
  @Input() dropdownList
  @Output() moveplantEvent=new EventEmitter<boolean>()
  userPlants: any;
  mode: any;
  constructor(private util:UtilityService, private router:Router, private alert:AlertService,private tp: ThemeProviderService) { }
  subs=new Subscription()
  dropdownSettings : IDropdownSettings = {
    singleSelection: true,
    idField: 'Id',
    textField: 'Name',
    selectAllText: 'Alle markieren',
    unSelectAllText: 'Alle entmarkieren',
    itemsShowLimit: 10,
    allowSearchFilter: false
  };
  ngOnInit(): void {
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
  }
  ngOnDestroy(): void {
    this.subs.unsubscribe()
  }
  onItemSelect(item: any) {
    this.selectedListIds.push(item);
  }
  movePlant(){
    if(this.selectedItems.length>0){
      if(!this.selectedPlant.ListIds.includes(this.selectedItems[0].Id)){
        const newlistId=this.selectedItems[0].Id
        const userplantId=this.selectedPlant.UserPlant.Id
        const moveModel={UserplantId:userplantId,NewListId:newlistId}
        this.subs.add(
          this.util.moveUserplantToAnotherList(moveModel).subscribe((newUserplantList)=>{
            this.userPlants=newUserplantList
            this.moveplantEvent.emit(true)
            
            
          })
        )
      }else{
        this.alert.error("Diese Pflanze ist bereits in dieser Liste verfügbar")
      }
      }
  }
}
