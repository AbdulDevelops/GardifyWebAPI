import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Subscription } from 'rxjs';
import { ScannerService } from 'src/app/services/scanner.service';
import { UtilityService } from 'src/app/services/utility.service';
import * as EXIF from 'exif-js';
import { User } from 'src/app/models/models';
import { AlertService } from 'src/app/services/alert.service';
@Component({
  selector: 'app-new-presentation',
  templateUrl: './new-presentation.component.html',
  styleUrls: ['./new-presentation.component.css']
})
export class NewPresentationComponent implements OnInit {
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
  presentationForm = new FormGroup({
    headline: new FormControl('',[Validators.required]),
    allUsers: new FormControl('',),
    showHeadline:new FormControl('',),
    showNumber:new FormControl('',),
    onlyContacts:new FormControl('',),
    onlyOneContact:new FormControl(false),
    file:new FormControl('')
  });
  imageSrc: string;
  subs=new Subscription();
  listOfImages: any;
  selectedImages:string=null
  fileProperties: { Headline: any; Location: any; UserCreatedDate: any; Source: any; ImgOwner: any; Tags: any; AlternativeDate: any; Rating: any; };
  imageMetadata: any;
  buttonSelected: boolean;
  contacts: User[];
  findMember=false
  selectContat=false
  selectedContacts: any=[];
  constructor(private formBuilder:FormBuilder, private scannerService:ScannerService, private util:UtilityService, private alert:AlertService) {
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
    this.presentationForm=this.formBuilder.group({
      Headline:'',
      ShowHeadline:true,
      ShowPictureNumber:false,
      ImageIdList:'',
      ShowMode:'',
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
    this.uploadImage=true
    this.getAllAlbumImg()
    this.getAllUsersContacts()
  }
  onSubmit(){
    var showModeValue=this.presentationForm.controls['ShowModeArray'].value
    showModeValue=showModeValue.filter(s=>s.answer==true)
   
    this.presentationForm.controls['ShowMode'].patchValue(showModeValue[0].key)
    if(this.uploadImage==false){

      this.presentationForm.controls['ImageIdList'].patchValue(this.selectedImages)
      var presiObject={
        Headline:this.presentationForm.controls['Headline'].value,
        ShowHeadline:this.presentationForm.controls['ShowHeadline'].value,
        ShowPictureNumber:this.presentationForm.controls['ShowPictureNumber'].value,
        ImageIdList:this.selectedImages,
        ShowMode:this.presentationForm.controls['ShowMode'].value,
      }
      this.util.createPresentation(presiObject).subscribe(res=>{
        if(res!=undefined || res!=null){
          if(this.selectedContacts.length>0)
          this.selectedContacts.forEach(userName=>{
            this.util.addContact(res,userName).subscribe(res=>{
              if(res=="Success"){
                this.alert.success("Presentation wurde erfolgreich erstellt")
              }
            })
          })
         
        }
      }) 
    }else{
      this.presentationForm.controls['ImageIdList'].patchValue(this.selectedImages)
      var presiObject={
        Headline:this.presentationForm.controls['Headline'].value,
        ShowHeadline:this.presentationForm.controls['ShowHeadline'].value,
        ShowPictureNumber:this.presentationForm.controls['ShowPictureNumber'].value,
        ImageIdList:this.selectedImages,
        ShowMode:this.presentationForm.controls['ShowMode'].value,
      }
      this.util.createPresentation(presiObject).subscribe(res=>{
        
        if(res!=undefined || res!=null){
          this.uploadImg(res)
          if(this.selectedContacts.length>0)
          this.selectedContacts.forEach(userName=>{
            this.util.addContact(res,userName).subscribe(res=>{
              if(res=="Success"){
                this.alert.success("Presentation wurde erfolgreich erstellt")
              }
            })
          })
         
        }
       
      }) 
    }
    
  }
  uploadImg(presId) {
    
      const img = new FormData();
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
       img.append('img', this.presentationForm.value.file);
       const self = this;  // different scopes
      // TODO: Use EventEmitter with form value
      
      
        img.append('id',  "0");
        img.append('imageTitle',  this.uploadForm.value.headline);
        img.append('imageDescription',  this.uploadForm.value.location);
        img.append('imageTags',  this.uploadForm.value.keywords);
        img.append('imageCreatedDate',  this.uploadForm.value.creationDate);
        img.append('imageNote',  this.uploadForm.value.pictureSource);

       if(this.presentationForm.value.Headline!='' && this.presentationForm.value.Headline!=undefined){
         this.subs.add(this.util.uploadPresiImg(img).subscribe(imgId=>{
           if(imgId!=null){
            this.subs.add(this.util.AddImageToPresi(imgId,presId,this.fileProperties).subscribe(res=>{
              this.util.OrderImageInPresiTable(presId,imgId).subscribe(res=>{
                console.log(res)
              })
              console.log(res)
            }))

          } 
         }))
       }
       
    
  }
  async onFileChange(event) {
    const reader = new FileReader();
    const fleReader = new FileReader();
    if (event.target.files.length > 0) {
      const file = event.target.files[0];
       const res = await this.scannerService.handleImageUpload(event, true);
       console.log(res)
       if (res.err) {
       
        return;
       }
      this.presentationForm.patchValue({
        file: res.file
      }); 
      // get Imgage Metadata
      reader.addEventListener('load', (fileReaderEvent) => {
        const data = EXIF.readFromBinaryFile(reader.result as string);
        
        if (data) {
          
          console.log(data);
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
        console.log(fleReader)
        
      }
   
     
    
    }
   
   
  }
  getAllAlbumImg(){
  this.loading=true
      this.subs.add(this.util.getAllUserImages().subscribe(i=>{
        this.listOfImages=i
        console.log(this.listOfImages)
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
    const controls = this.presentationForm.get("ShowModeArray")["controls"];
    controls.forEach(control => {
      control.get("answer").setValue(false);
    });
    controls[i].get("answer").setValue(true);
    if(i==2){
      this.selectContat=true
    }else{
      this.selectContat=false
    }
  }
  getAllUsersContacts(){
    this.subs.add(this.util.getAllContacts().subscribe(res=>{
      
      this.contacts=res
      this.contacts.map((obj) => {
        obj.Selected = false;
        return obj;
    })
      console.log(this.contacts)
    }))
  }
  pushSelectedContact(userName){
    const contact={UserName:userName}
    this.selectedContacts.push(contact)
  }
  removeImgFromSelection(userName){
    this.selectedContacts.filter(u=>u!=="userName")
  }

  toUrl(url, small = true) {
    return this.util.toUrl(url, small);
  }

  get f(){
    return this.presentationForm.controls;
  }
  get fControl(){
    return this.uploadForm.controls;
  }
}
