import { Injectable } from '@angular/core';
import { ScanResult, UploadedImageResponse } from '../models/models';
import heic2any from 'heic2any';
import { AlertService } from './alert.service';
import { BehaviorSubject } from 'rxjs';
import * as ExifReader from 'exifreader';

@Injectable({
  providedIn: 'root'
})
export class ScannerService {

  uploadedPic: UploadedImageResponse = { file: null, title: null, err: null, src: '', dateCreated: null };
  fromPlantScan = false;
  uploadedPicSubject = new BehaviorSubject(this.uploadedPic);
  scanResults: ScanResult;
  MAX_ALLOWED_SIZE = 25 * 1024 * 1024;
  plantForSuggestion = new BehaviorSubject(null);
  allPlantResult = new BehaviorSubject(null);
  
  constructor(private alert: AlertService) { }

  get uploadedPic$() {
    return this.uploadedPicSubject.asObservable();
  }

  get plantForSuggestion$() {
    return this.plantForSuggestion.asObservable();
  }

  get allPlantResult$() {
    return this.allPlantResult.asObservable();
  }

  async handleImageUpload(event, cacheResult = false): Promise<UploadedImageResponse> {
    const reader = new FileReader();
    const readerDataURL = new FileReader();
    const response: UploadedImageResponse = { file: null, title: null, err: null, src: '', dateCreated: null };

    // support images up to 25MB
    if (event.target.files[0].size > this.MAX_ALLOWED_SIZE) { 
      response.err = 'Das Bild ist zu groß (max. 25MB).'; 
      this.alert.error(response.err); 
      return response; 
    }

    if (!event.target.files || !event.target.files.length) { 
      response.err = 'Es wurde keine Datei ausgewählt.'; 
      this.alert.error(response.err); 
      return response; 
    }

    let file: File = event.target.files[0];

    if (this.isHEIC(file)) {
      await heic2any({ blob: file, toType: 'image/jpeg', quality: 0.5 })
      .then(res => {
        // we expect a file not a blob
        file = new File([res as Blob], file.name.replace(/\.[^.]*$/, '.jpg'), {type: 'image/jpeg'});
      })
      .catch((err: {code: string, message: string}) => {
        response.err = `Fehler bei der Konvertierung (CODE: ${err.code}). Ist die Datei eine gültige HEIC-Datei?`;
        this.alert.error(response.err); 
      });
    }

    await new Promise((resolve) => {
      let tags = {};
      reader.readAsArrayBuffer(file);

      reader.onload = () => {
        try {
          tags = ExifReader.load(reader.result as ArrayBuffer);
          delete tags['MakerNote'];
          response.dateCreated = tags['DateTimeOriginal'].description || tags['DateTime'].description;
          response.dateCreated = response.dateCreated ? response.dateCreated.split(' ')[0].replaceAll(':', '_') : new Date().toDateString();
        } catch(err) {
          console.log('failed parsing exif data');
        }
        response.file = file;
        response.title = file.name;
        readerDataURL.readAsDataURL(file);
      };

      readerDataURL.onload = () => {
        response.src = readerDataURL.result;
        resolve(readerDataURL.result);
      }
    });

    if (cacheResult) {
      this.uploadedPicSubject.next(response);
    }

    return response;
  }

  isHEIC(file: File) {
    return file.type.includes('image/heic') 
    || file.type.includes('image/heif')
    || file.name.endsWith('.heic')
    || file.name.endsWith('.heics')
    || file.name.endsWith('.heif')
    || file.name.endsWith('.heifs');
  }
}
