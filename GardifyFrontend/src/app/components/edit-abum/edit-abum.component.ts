import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import * as EXIF from 'exif-js';
import { Subscription } from 'rxjs';
import { AlertService } from 'src/app/services/alert.service';
import { ScannerService } from 'src/app/services/scanner.service';
import { UtilityService } from 'src/app/services/utility.service';

@Component({
  selector: 'app-edit-abum',
  templateUrl: './edit-abum.component.html',
  styleUrls: ['./edit-abum.component.css']
})
export class EditAbumComponent implements OnInit {
  findMember=false
  @ViewChild('image') imgEl: ElementRef;
  showSelectedImg=false
  uploadImage:boolean=true
  uploadForm = new FormGroup({
    headline: new FormControl('',[Validators.required]),
    location: new FormControl('',[Validators.required]),
    keywords:new FormControl('',[Validators.required]),
    creationDate:new FormControl(new Date(),[Validators.required]),
    alternativeDate:new FormControl(new Date(),[Validators.required]),
    pictureOwner:new FormControl(false),
    pictureSource:new FormControl('',[Validators.required]),
    rating:new FormControl('',[Validators.required]),
    file: new FormControl(''),
   
  });

  editForm = new FormGroup({
    headline: new FormControl('',[Validators.required]),
    showHeadline:new FormControl('',),
    showNumber:new FormControl('',),
    Description:new FormControl('',),
    file:new FormControl(''),
  
  });

  imageSrc: string;
  subs=new Subscription();
  listOfImages: any;
  selectedImages:string=null
  fileProperties: { Headline: any; Location: any; UserCreatedDate: any; Source: any; ImgOwner: any; Tags: any; AlternativeDate: any; Rating: any; };
  imageMetadata: any;
  buttonSelected: boolean;
  shareWithOthers: boolean=false;
  selectedAlbimgs: any=[];
  addImgFromGardify=false
  selectedImgIdArray: any=[];
  countSelectImgs: any;
  origin: any;
  selectedPresiId:any
  selectedAlbumId:any
  showPresiImages: boolean=false;
  presiContacts: any;

  constructor(private formBuilder:FormBuilder, private scannerService:ScannerService,
     private util:UtilityService,private activateRoute:ActivatedRoute, 
     private alert:AlertService, private route:Router) {

    this.uploadForm=this.formBuilder.group({
      headline:'',
      location:'',
      keywords:'',
      creationDate:'',
      alternativeDate:'',
      pictureOwner:'',
      pictureSource:'',
      albumName:'',
      presiName:'',
      rating:''
    })
    this.editForm=this.formBuilder.group({
      Headline:'',
      ImageIdList:'',
      ShowMode:'',
      Description:'',
      ShowHeadline:false,
      ShowPictureNumber:false,
      ShowModeArray: this.formBuilder.array(
        [{key:0,value:"Jedem"},{key:1,value:"Allen Kontakten"},{key:2,value:"Nur ausgewählten Kontakten"} ].map(item =>
          this.formBuilder.group({
            optionText: [item.value],
            answer: [false],
            key:[item.key]
          })
        )
      ),
      
      file: [],
      fileDetails: this.uploadForm
    })
    
   }

  loading:boolean

  ngOnInit(): void {
    this.loading=false
    this.uploadImage=false
    this.subs.add(this.activateRoute.queryParams.subscribe(origin=>{ 
      this.origin=origin.o;
      
      this.getSelectedAlbumImages()
     
    }))
    this.getAllAlbumImg()
   
  }

  getSelectedAlbumImages(){
    this.subs.add(this.activateRoute.params.subscribe(data=>{
     
      if(this.origin==="album"){
        this.getAlbumImgsById(data)
      }else if(this.origin==="presentation"){
        this.getUserPresiById(data)
        this.getPresiContacts(data.id)
      }
     
      this.loading=false
    }
      ))
  }
  getUserPresiById(data){
    this.util.getUserPresentationsById(data.id).subscribe(presi=>{
      this.selectedAlbimgs=presi
      this.selectedPresiId=presi[0].PresentationId 
      const presiName=presi[0].Headline
      this.editForm.controls['Headline'].patchValue(presiName)
    })
  }
  getAlbumImgsById(data){
    this.util.getAlbumImagesById(data.id).subscribe(album=>{
      this.selectedAlbimgs=album
      this.selectedAlbumId=album[0].Id
      const albumName=album[0].Name
      this.editForm.controls['Headline'].patchValue(albumName)
      this.editForm.controls['Description'].patchValue(album[0].Description)
    })
  }
  onSubmit(){
    var showModeValue=this.editForm.controls['ShowModeArray'].value
    showModeValue=showModeValue.filter(s=>s.answer==true)
   
    this.editForm.controls['ShowMode'].patchValue(showModeValue[0].key)
    this.editForm.controls['ImageIdList'].patchValue(this.selectedImages)
    const presiObject={
      Headline:this.editForm.controls['Headline'].value,
      ImageIdList:this.selectedImages,
      ShowMode:this.editForm.controls['ShowMode'].value,
      ShowHeadline:this.editForm.controls['ShowHeadline'].value,
      ShowPictureNumber:this.editForm.controls['ShowPictureNumber'].value,
      Id:this.selectedPresiId
    }
    console.log(presiObject)
    const albumObject={
      Name:this.editForm.controls['Headline'].value,
      Description:this.editForm.controls['Description'].value,
      Id:this.selectedAlbumId
    }
    if(this.uploadImage==false){

      if(this.origin==="album"){
        
        this.util.editAlbum(albumObject).subscribe(res=>{
          this.route.navigate(['/gartenarchiv/images/presentation']);    
        }) 
      }else if(this.origin==="presentation"){
        this.util.editPresentation(presiObject).subscribe(res=>{
          this.route.navigate(['/gartenarchiv/images/presentation']);    
            }) 
        
      }
      
    }else{
      
      if(this.origin==="album"){
        this.util.editAlbum(albumObject).subscribe(res=>{
          if(this.imageSrc ){
            this.uploadImg(this.selectedAlbumId)

          }
        }) 
        
      }else if(this.origin==="presentation"){
        this.util.editPresentation(presiObject).subscribe(res=>{
          if(this.imageSrc ){
            this.uploadImg(this.selectedPresiId)

          }
        }) 
        
      }
  
    }

    if(this.addImgFromGardify==false){
      this.deleteSelectedImgs()

    }
   
  }
  uploadImg(id) {
    
      const img = new FormData();
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
       img.append('img', this.editForm.value.file);
       const self = this;  // different scopes
      // TODO: Use EventEmitter with form value
      
      
       
        img.append('imageTitle',  this.uploadForm.value.headline);
        img.append('imageDescription',  this.uploadForm.value.location);
        img.append('imageTags',  this.uploadForm.value.keywords);
        img.append('imageCreatedDate',  this.uploadForm.value.creationDate);
        img.append('imageNote',  this.uploadForm.value.pictureSource);

       if(this.editForm.value.Headline!='' && this.editForm.value.Headline!=undefined){
        if(this.origin==="album"){
          img.append('id',  this.selectedAlbumId);
          this.subs.add(this.util.uploadAlbumImg(img).subscribe(imgId=>{
            if(imgId!=null){
            
               this.subs.add(this.util.AddImageToAlbum(imgId,this.selectedAlbumId, this.fileProperties).subscribe(res=>{}))
               this.alert.success("Das Bild wurde erfolgreich hochgeladen")
               this.route.navigateByUrl('/', {skipLocationChange: true}).then(()=>
               this.route.navigate(['gartenarchiv/images/presentation']));
              } 
            }))
         
        }else if(this.origin==="presentation"){
          img.append('id',  id);
          this.subs.add(this.util.uploadPresiImg(img).subscribe(imgId=>{
            if(imgId!=null){
             this.subs.add(this.util.AddImageToPresi(imgId,id,this.fileProperties).subscribe(res=>{
               this.util.OrderImageInPresiTable(id,imgId).subscribe(res=>{
                this.route.navigate(['/gartenarchiv/images/presentation']);    

               })
             }))
 
           } 
          }))
        }
         
       }
       
    
  }
  async onFileChange(event) {
    const reader = new FileReader();
    const fleReader = new FileReader();
    if (event.target.files.length > 0) {
      const file = event.target.files[0];
       const res = await this.scannerService.handleImageUpload(event, true);
       if (res.err) {
       
        return;
       }
      this.editForm.patchValue({
        file: res.file
      }); 
      // get Imgage Metadata
      reader.addEventListener('load', (fileReaderEvent) => {
        const data = EXIF.readFromBinaryFile(reader.result as string);
        
        if (data) {
          this.getExif(data)
        } else {
          console.log(null);
        }
      });
      reader.readAsArrayBuffer(file);
      fleReader.readAsDataURL(file);
     
      fleReader.onload = () => {
        this.showSelectedImg=true
        this.imageSrc = fleReader.result as string;
        
      }
    }
  }
  getAllAlbumImg(){
  this.loading=true
      this.subs.add(this.util.getAllUserImages().subscribe(i=>{
        this.listOfImages=i
        
        this.loading=false
      }))
    
  }
  selectImgToAdd(image){
    this.selectedImages=this.selectedImages+image.Id+","

  }

  toggleradiobtn(){
    this.buttonSelected=!this.buttonSelected
    console.log(this.buttonSelected)
  }
  
  private getExif(allMetaData) {
    
    this.imageMetadata = allMetaData;
   
    if(this.imageMetadata!=undefined ){
      
      this.uploadForm.controls['keywords'].patchValue(this.imageMetadata.ImageDescription?this.imageMetadata.ImageDescription:'')
      this.uploadForm.controls['creationDate'].patchValue(this.imageMetadata.DateTime?this.imageMetadata.DateTime:'')
      this.uploadForm.controls['alternativeDate'].patchValue(this.imageMetadata.DateTime?this.imageMetadata.DateTime: this.formatDate('2008/05/09 14:03:06'))
      this.uploadForm.controls['pictureSource'].patchValue(this.imageMetadata.Artist?this.imageMetadata.Artist:'')

    }
  }
  
  private formatDate(date) {
    const d = new Date(date);
    let month = '' + (d.getMonth() + 1);
    let day = '' + d.getDate();
    const year = d.getFullYear();
    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;
    return [year, month, day].join('-');
  }

  updateAnswer(i) {
    const controls = this.editForm.get("ShowModeArray")["controls"];
    controls.forEach(control => {
      control.get("answer").setValue(false);
    });
    controls[i].get("answer").setValue(true);
    this.shareWithOthers=true
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
              this.route.navigate(['/gartenarchiv/images/presentation']);    

        }else{
          this.alert.error("Ein Fehler ist aufgetretten")
        }
        this.loading=false
      }))
    }
    
  }

uploadImageMode(){
  this.uploadImage=true;
  this.addImgFromGardify=false;
  this.showPresiImages=false
  this.selectedImgIdArray=[]
}
viewMode(){
  this.uploadImage=false;
  this.addImgFromGardify=false;
  this.imageSrc=null
  this.showSelectedImg=false
  this.showPresiImages=true

}
getPresiContacts(id){
  if(this.origin==="presentation")
  this.subs.add(this.util.getPresiContacts(id).subscribe(res=>{
    this.presiContacts=res
  }))
}
  toUrl(url, small = true) {
    return this.util.toUrl(url, small);
  }

  get f(){
    return this.editForm.controls;
  }
  get fControl(){
    return this.uploadForm.controls;
  }
}



