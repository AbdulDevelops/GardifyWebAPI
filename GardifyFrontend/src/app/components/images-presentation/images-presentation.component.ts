import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { AlertService } from 'src/app/services/alert.service';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { UtilityService } from 'src/app/services/utility.service';

@Component({
  selector: 'app-images-presentation',
  templateUrl: './images-presentation.component.html',
  styleUrls: ['./images-presentation.component.css']
})
export class ImagesPresentationComponent implements OnInit {
subs=new Subscription()
mode:string
selectedViewType:string='presentation'
presentationsImgs=[]
albumsImgs: any=[]
presentation:any
showSinglePresentation: boolean=false;
  showSingleAlbumPre: boolean=false;
  album: any;
  editAlbum=false
  shareAlbum=false
  findMember=false
  presentationId: any;
  presiContacts: any;
  constructor(private utility:UtilityService,private tp: ThemeProviderService, private alert:AlertService, private router: Router) {
    
   }

  ngOnInit(): void {
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
   this.getPresentationsImgs()
   this.getAllAlbumImgs()
  }

  getPresentationsImgs(){
   this.subs.add( this.utility.getUserPresentations().subscribe(data=>{
    this.presentationsImgs=data
    console.log(data)
  }))
  }

  getAllAlbumImgs(){
   
    this.subs.add(this.utility.getUserAlbums().subscribe(albums=>{
      this.albumsImgs=albums
     
      
    }))
  }
  
  showSinglePresi(presiId){
    this.album=undefined
  this.showSinglePresentation=true
  this.presentation= this.presentationsImgs.filter(p=>p.PresentationId== presiId)[0]
  this.presentationId=presiId
  }
  showSingleAlbum(albumId){
    this.presentation=undefined
    this.showSingleAlbumPre=true
    this.album= this.albumsImgs.filter(a=>a.Id== albumId)[0]
    }
  viewMode(mode:string){
    this.selectedViewType=mode
  }

  deleteAlbum(albumId){
    this.subs.add(this.utility.deleteAlbumById(albumId).subscribe(res=>{
      if(res){
        this.alert.success("Album wurde erfolgreich gelöscht")
     
        this.presentation=undefined
        this.album=undefined
        this.showSingleAlbumPre=false
        this.getAllAlbumImgs()
      }else{
        this.alert.error("Ein Fehler ist aufgetretten")
      }
    }))
  }

  deletePresentation(presiId){
    this.subs.add(this.utility.deletePresentationById(presiId).subscribe(res=>{
      if(res){
        this.alert.success("Presentation wurde erfolgreich gelöscht")
        this.presentation=undefined
        this.album=undefined
        this.showSinglePresentation=false
        this.getPresentationsImgs()
      }else{
        this.alert.error("Ein Fehler ist aufgetretten")
      }
    }))
  }
  toUrl(url, small = true) {
    return this.utility.toUrl(url, small);
  }
  getPresiContacts(){
    if(this.showSinglePresentation)
    this.subs.add(this.utility.getPresiContacts(this.presentationId).subscribe(res=>{
      this.presiContacts=res
    }))
  }

  editPresentation(presi,shareMode){
    const edit=presi
    edit.Id=presi.PresentationId
    if(shareMode=="allUsers"){
      edit.ShowMode=0
    }
    if(shareMode=="usersInContact"){
      edit.ShowMode=1
    }
    if(shareMode=="selectedUsers"){
      edit.ShowMode=2
    }
    this.subs.add(this.utility.editPresentation(edit).subscribe(res=>{
      if(res!=null)
      this.alert.success("Presentation wurde erfolgreich geändert")

    }))
  }
}
