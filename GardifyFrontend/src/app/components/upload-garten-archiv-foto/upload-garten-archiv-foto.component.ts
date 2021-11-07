import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { GardenAlbumFileToModuleViewModel } from 'src/app/models/models';
import { AlertService } from 'src/app/services/alert.service';
import { ScannerService } from 'src/app/services/scanner.service';
import { UtilityService } from 'src/app/services/utility.service';

@Component({
  selector: 'app-upload-garten-archiv-foto',
  templateUrl: './upload-garten-archiv-foto.component.html',
  styleUrls: ['./upload-garten-archiv-foto.component.css']
})
export class UploadGartenArchivFotoComponent implements OnInit {
  loading:boolean
  uploadForm = new FormGroup({
    headline: new FormControl('',[Validators.required]),
    location: new FormControl('',[Validators.required]),
    keywords:new FormControl('',[Validators.required]),
    creationDate:new FormControl('',[Validators.required]),
    alternativeDate:new FormControl('',[Validators.required]),
    pictureOwner:new FormControl(false),
    pictureSource:new FormControl('',[Validators.required]),
    rating:new FormControl('',[Validators.required]),
    file: new FormControl(''),
    albumName:new FormControl('',[Validators.required]),
    presiName:new FormControl(''),


  });
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
  imgForm = new FormGroup({
    
    file: new FormControl('', [Validators.required]),
  });
  buttonSelected:boolean
  listOfAlbums=[]
  listOfPresi=[]
  imageSrc: string;
  selectedAlbum:any
  selectedPresi:any
subs= new Subscription()
  constructor(private formBuilder:FormBuilder, private util:UtilityService,private alert: AlertService,private scannerService:ScannerService, private route:Router) {
    this.uploadForm=this.formBuilder.group({
      headline:'',
      location:'',
      keywords:'',
      creationDate:'',
      alternativeDate:'',
      pictureOwner:'',
      pictureSource:'',
      file: [],
    albumName:'',
    presiName:'',
    rating:''
    })
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
   fileProperties:any={}
  ngOnInit(): void {
    this.loading=false
    this.buttonSelected=false
    this.getUserAlbums()
    this.getUserPresi()
  }
  onSubmit() {
    if(this.selectedAlbum !='undefined' || this.selectedAlbum !=null || this.selectedPresi!='undefined' || this.selectedPresi!=null){
      
      const albImg = new FormData();
      const presImg = new FormData();
      console.log(this.uploadForm.value)
      this.fileProperties= {
        Headline:this.uploadForm.value.headline,
        Location:this.uploadForm.value.location,
        UserCreatedDate:this.uploadForm.value.creationDate,
        Source:this.uploadForm.value.pictureSource,
        ImgOwner:this.uploadForm.value.pictureOwner,
        Tags:this.uploadForm.value.keywords,
        AlternativeDate:this.uploadForm.value.alternativeDate,
        Rating:this.uploadForm.value.rating
      }
      console.log(this.fileProperties)
      
       const self = this;  // different scopes
      // TODO: Use EventEmitter with form value
     

       if(this.uploadForm.value.presiName!='' && this.uploadForm.value.presiName!=undefined){
        presImg.append('id',  this.selectedPresi.PresentationId);
        presImg.append('img', this.uploadForm.value.file);
        presImg.append('imageTitle',  this.uploadForm.value.headline);
        presImg.append('imageDescription',  this.uploadForm.value.location);
        presImg.append('imageTags',  this.uploadForm.value.keywords);
        presImg.append('imageCreatedDate',  this.uploadForm.value.creationDate);
        presImg.append('imageNote',  this.uploadForm.value.pictureSource);
         this.subs.add(this.util.uploadPresiImg(presImg).subscribe(imgId=>{
           if(imgId!=null){
            this.subs.add(this.util.AddImageToPresi(imgId,this.selectedPresi.PresentationId,this.fileProperties).subscribe(res=>{
              this.util.OrderImageInPresiTable(this.selectedPresi.PresentationId,imgId).subscribe(res=>{
                this.alert.success("Das Bild wurde erfolgreich hochgeladen")
            this.route.navigateByUrl('/', {skipLocationChange: true}).then(()=>
            this.route.navigate(['gartenarchiv']));
              })
             
            }))

          } 
         }))
       }
       if(this.uploadForm.value.albumName!=undefined && this.uploadForm.value.albumName!='' ){
        let selectedAlbumId=this.selectedAlbum.Id
        albImg.append('id',  selectedAlbumId);
        albImg.append('img', this.uploadForm.value.file);
        albImg.append('imageTitle',  this.uploadForm.value.headline);
        albImg.append('imageDescription',  this.uploadForm.value.location);
        albImg.append('imageTags',  this.uploadForm.value.keywords);
        albImg.append('imageCreatedDate',  this.uploadForm.value.creationDate);
        albImg.append('imageNote',  this.uploadForm.value.pictureSource);
         this.subs.add(this.util.uploadAlbumImg(albImg).subscribe(imgId=>{
         if(imgId!=null){
         
            this.subs.add(this.util.AddImageToAlbum(imgId,selectedAlbumId, this.fileProperties).subscribe(res=>console.log(res)))
            this.alert.success("Das Bild wurde erfolgreich hochgeladen")
            this.route.navigateByUrl('/', {skipLocationChange: true}).then(()=>
            this.route.navigate(['gartenarchiv']));
           } 
         }))
       }
    }else{
      this.alert.error("Bitte wählen Sie ein Album oder eine Präsentation aus, um fortzufahren")
    }
    
 
  }
  toggleradiobtn(){
    this.buttonSelected=!this.buttonSelected
    console.log(this.buttonSelected)
  }
  getUserAlbums(){
    this.subs.add(this.util.getUserAlbums().subscribe(al=> {
      this.listOfAlbums=al
    }))
  }
  getUserPresi(){
    this.subs.add(this.util.getUserPresentations().subscribe(al=> {
      this.listOfPresi=al
      
    }))
  }
  createNewAlbum(){
    this.loading=true
    $('#createNewAlbumModal').modal('hide');
    this.subs.add(this.util.createUserAlbum(this.albumForm.value).subscribe(a=>{
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
  get f(){
    return this.uploadForm.controls;
  }
     
  async onFileChange(event) {
    if(this.listOfAlbums.length>0 || this.listOfPresi.length>0){
      const reader = new FileReader();
      if (event.target.files.length > 0) {
        const file = event.target.files[0];
         const res = await this.scannerService.handleImageUpload(event, true);
        
         if (res.err) {
         
          return;
         }
        this.uploadForm.patchValue({
          file: res.file
        }); 
        reader.readAsDataURL(file);
       
        reader.onload = () => {
      
          this.imageSrc = reader.result as string;
        }
      }
     
    }else{
      this.alert.error("Bitte fügen Sie ein Album oder eine Präsentation hinzu, bevor Sie das Bild hochladen.")
    }
    
   
  }
}
