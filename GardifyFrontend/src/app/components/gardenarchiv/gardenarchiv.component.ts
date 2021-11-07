import { HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, Subscription } from 'rxjs';
import { AlertService } from 'src/app/services/alert.service';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { UtilityService } from 'src/app/services/utility.service';

@Component({
  selector: 'app-gardenarchiv',
  templateUrl: './gardenarchiv.component.html',
  styleUrls: ['./gardenarchiv.component.css']
})
export class GardenarchivComponent implements OnInit {
loading:boolean=true
mode:string
subs= new Subscription();
selectedSortType:string
listOfImages:any
  sortedImgs: any;
  showPresentation:boolean=false
  editMode=false
  deleteFoto=false
  imgSelected=false
  selectedImgIdArray=[]
  countSelectImgs=0;
  selectAll=false;
  filterSelected=false
  constructor(private util:UtilityService,private tp: ThemeProviderService,public router: Router,private alert:AlertService ) { 
    
  }

  ngOnInit(): void {
    this.loading=true
    this.selectedSortType="all"
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
    this.getAllAlbumImg();
  }
  showPresi(event){
    this.showPresentation=event
  }

  getAllAlbumImg(){
    this.selectedSortType="all"
    this.sortedImgs=[]
      this.subs.add(this.util.getAllUserImages().subscribe(i=>{
        this.listOfImages=i
        this.loading=false
      }))
    this.loading=false
  }
  sortImgs(sortType){
    this.loading=true
    this.listOfImages=[]
    this.selectedImgIdArray=[]
    this.selectedSortType=sortType
    let params = new HttpParams();
    params = params.append('sortType', sortType.toString());
    this.subs.add(this.util.sortImages(params).subscribe(s=>{
      this.sortedImgs=s
      this.loading=false
    }))
  }
  filterByDate(date, filterMode){
    this.listOfImages=[]
    this.selectedSortType=filterMode
    let params = new HttpParams();
    params = params.append('inpuDate', date);
    params = params.append('filterMode', filterMode.toString());
    this.subs.add(this.util.FilterAlbumImgByDate(params).subscribe(s=>{
      this.sortedImgs=s
      console.log(s)
    }))
  }
  getImageById(imageId:number){
    this.subs.add(this.util.getImageById(imageId).subscribe(img=>{
    }))
  }

  pushSelectedImgId(imgId){
    this.selectedImgIdArray.push(imgId)
    this.setCountSelectedImages(+1)
  }

  removeImgFromSelection(imgId){
    this.selectedImgIdArray= this.selectedImgIdArray.filter(i=>i!=imgId)
    this.setCountSelectedImages(-1)
  }
  setCountSelectedImages(number){
  return this.countSelectImgs = this.countSelectImgs+number

  }

  deleteSelectedImgs(){
    if(this.selectedImgIdArray.length>0){
      this.loading=true
      console.log(this.selectedImgIdArray)
      this.subs.add(this.util.deleteImagesFromGardenArchiv(this.selectedImgIdArray).subscribe((res)=>{
        if(res==true){
          this.countSelectImgs=0
          this.alert.success("Bild wurde erfolgreich gelösch")
              this.getAllAlbumImg()
        }else{
          this.alert.error("Ein Fehler ist aufgetretten")
        }
        this.loading=false
      }))
    }
    
  }

  selectAllImgs(){
    this.countSelectImgs=0
    this.selectedImgIdArray=[]
    this.loading=true
    if(this.selectedSortType=='all'){
      let currentListOfImages=this.listOfImages
      currentListOfImages.forEach(element => {
        element.highlighted=true;
        this.selectedImgIdArray.push(element.Id)
        this.countSelectImgs=this.countSelectImgs+1
      });
      this.listOfImages=currentListOfImages
    }else{
      let currentListOfImages=this.sortedImgs
      currentListOfImages.forEach(element => {
        element.forEach(item => {
          item.highlighted=true;
        this.selectedImgIdArray.push(item.Id)
        this.countSelectImgs=this.countSelectImgs+1
        });
        
      });
      this.sortedImgs=currentListOfImages
    }
    
    this.loading=false;
    console.log(this.selectedImgIdArray)
  }

  resetImgSelection(){
    this.countSelectImgs=0
    this.loading=true
    this.editMode=false ;this.selectAll=false
    if(this.selectedSortType=='all'){
      this.getAllAlbumImg()
    }else{
      this.sortImgs(this.selectedSortType)
    }
    
  }

  toUrl(url, small = true) {
    return this.util.toUrl(url, small);
  }
  ngOnDestroy(): void {
    this.subs.unsubscribe()
  }
  get showUploadForm() {
    return this.router.url.includes('uploadfoto') ;
  }
  get showImgDetails() {
    return this.router.url.includes('image/detail') ;
  }
  get showPresentationForm(){
    return this.router.url.includes('images/presentation')
  }
  get newPresentationForm(){
    return this.router.url.includes('new/presentation')
  }
  get editPresiOrAlb(){
    return this.router.url.includes('images/edit-album')
  }
  get showImages(){
    if(!this.showImgDetails && !this.showPresentationForm && !this.showUploadForm && !this.newPresentationForm && !this.editPresiOrAlb){
      return true
    }
    return false
  }
}
