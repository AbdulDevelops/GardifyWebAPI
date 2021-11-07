import { Injectable } from '@angular/core';
import { UtilityService } from './utility.service';
import { PlantSearchService } from './plant-search.service';
import { Subscription } from 'rxjs';
import { UserActionsFrom } from '../models/models';
import { HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class FrostAlertService {
  todayFc: any;
  minTemp: any=[];
  tagId:any=[];
  latitude:any=50; longitude: any=7;
  subs=new Subscription;
  userPlants:any;
  plantDetails:any=[];
  tags: any[];
  _freezeLvls: any=[];
 
  apiCallFrom=new UserActionsFrom();

  constructor(private util: UtilityService,private plantSearch: PlantSearchService) { 

  }
 
  
  getFrostAlert(){
    const  frost={
      plantDetails:'',
      count:0,
      minTemp:'',
      tagId:'',
      _freezeLvls:''
    }
    
    let plant:any=[]
    let count:number=0;
    this.plantSearch.getPlantTags().subscribe(t => {
      this.tags = t; 
      this.tags.forEach(tag => {
        switch(tag.CategoryId){
          case tag.CategoryId=89: this._freezeLvls.push({t:tag.Title, id: tag.Id}); break;
        }
      });
    })
    let params = new HttpParams();
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
    this.subs.add(this.util.getDailyForecast(this.longitude,this.latitude,params).subscribe(d=>{
      this.todayFc=d.Forecasts.slice(0,1);
      this.todayFc.forEach(e=>{
        this.minTemp.push(e.MinAirTemperatureInCelsius)
      })
    }))
    frost.plantDetails=plant
    frost.count=count 
      frost.minTemp=this.minTemp
      frost.tagId=this.tagId
      frost._freezeLvls=this._freezeLvls
    return frost
  }
}
