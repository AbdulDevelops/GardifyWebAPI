import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { AlertService } from 'src/app/services/alert.service';
import { ScannerService } from 'src/app/services/scanner.service';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { UtilityService } from 'src/app/services/utility.service';

@Component({
  selector: 'app-image-overview',
  templateUrl: './image-overview.component.html',
  styleUrls: ['./image-overview.component.css']
})
export class ImageOverviewComponent implements OnInit {
  image:any
  loading: boolean;
  mode:string
  subs= new Subscription();
  imgeId:any
  editFoto:boolean
  albumForm= new FormGroup({
    Id:new FormControl(''),
    Name:new FormControl(''),
    Description:new FormControl(''),
    EntryImages:new FormControl('')

  })
  presiForm= new FormGroup({
    Headline:new FormControl(''),
    ShowHeadline:new FormControl(''),
    ShowPictureNumber:new FormControl(''),
    ShowMode:new FormControl('')
  })
  listOfAlbums: any;
  listOfPresi: any;
  deleteFoto=false
  origin: any;
  albumId: any;
  presiId: any;
  shareFoto=false
  constructor(private formBuilder:FormBuilder, private activateRoute:ActivatedRoute,private util:UtilityService,private alert: AlertService,private scannerService:ScannerService, private route:Router, private tp:ThemeProviderService) {
    this.albumForm=this.formBuilder.group({
      Id:null,
      Name:'',
      Description:'',
      EntryImages:null
    })
    this.presiForm=this.formBuilder.group({
      Headline:'',
      ShowHeadline:'',
      ShowPictureNumber:'',
      ShowMode:''
    })
   }

  ngOnInit(): void {
    this.loading=true
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
    this.subs.add(this.activateRoute.queryParams.subscribe((d) => {
      this.origin = d.o
      if(d.albumId)
      this.albumId=d.albumId
      if(d.presiId)
      this.presiId=d.presiId
    }));
   this.getImage()
  }
  getImage(){
    this.subs.add(this.activateRoute.params.subscribe(data=>{
      this.util.getImageById(data.id).subscribe(img=>this.image=img)
      this.loading=false
    }
      ))
  }
  getUserAlbums(){
    this.subs.add(this.util.getUserAlbums().subscribe(al=> {
      this.listOfAlbums=al
      console.log(this.listOfAlbums);
      
    }))
  }
  getUserPresi(){
    this.subs.add(this.util.getUserPresentations().subscribe(al=> {
      this.listOfPresi=al
      console.log(this.listOfPresi);
      
    }))
  }
  createNewAlbum(){
    this.loading=true
    console.log(this.albumForm.value)
    this.subs.add(this.util.createUserAlbum(this.albumForm.value).subscribe(a=>{
      console.log(a)
      this.loading=false
      this.alert.success("Neue Album wurde erstellt")
      this.getUserAlbums();
    }))
    
  }
  createNewPresi(){
    this.loading=true
    this.subs.add(this.util.createUserPresi(this.presiForm.value).subscribe(a=>{
      console.log(a)
      this.loading=false
      this.alert.success("Neue Präsentation wurde erstellt")
      this.getUserPresi();
    }))
  }

  deleteImageFromAlbum(){
    this.util.deleteImageFromAlbum(this.image.imgProperties.FileToModuleID,this.albumId).subscribe(res=>{
      if(res==true){
        this.alert.success("Bild wurde erfolgreich gelösch")
        this.route.navigateByUrl('/', {skipLocationChange: true}).then(()=>
            this.route.navigate(['gartenarchiv']));
            
      }else{
        this.alert.error("Ein Fehler ist aufgetretten")
      }
    })
  }
  
  deleteImageFromPresi(){
    this.util.deleteImageFromPresentation(this.image.imgProperties.FileToModuleID,this.presiId).subscribe(res=>{
      if(res==false){
        this.alert.success("Bild wurde erfolgreich gelösch")
        this.route.navigateByUrl('/', {skipLocationChange: true}).then(()=>
            this.route.navigate(['gartenarchiv']));
            
      }else{
        this.alert.error("Ein Fehler ist aufgetretten")
      }
    })
  }

  deleteImageGlobal(){
   
    this.loading=true
    
    this.subs.add(this.util.deleteImagesFromGardenArchiv([this.image.imgProperties.FileToModuleID]).subscribe((res)=>{
      if(res==true){
        this.alert.success("Bild wurde erfolgreich gelösch")
        this.route.navigateByUrl('/', {skipLocationChange: true}).then(()=>
        this.route.navigate(['gartenarchiv']));  
      }else{
        this.alert.error("Ein Fehler ist aufgetretten")
      }
      this.loading=false
    }))
    
  }

  toUrl(url, small = true) {
    return this.util.toUrl(url, small);
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe()
  }
}
