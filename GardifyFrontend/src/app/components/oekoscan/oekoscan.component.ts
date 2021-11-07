import { Component, OnInit, OnDestroy, AfterViewInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { UtilityService } from 'src/app/services/utility.service';
import { faInfo } from '@fortawesome/free-solid-svg-icons';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ObjectType, UploadedImageResponse, UserActionsFrom } from 'src/app/models/models';
import { AuthService } from 'src/app/services/auth.service';
import { AlertService } from 'src/app/services/alert.service';
import html2canvas from 'html2canvas';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-oekoscan',
  templateUrl: './oekoscan.component.html',
  styleUrls: ['./oekoscan.component.css']
})
export class OekoscanComponent implements OnInit, OnDestroy, AfterViewInit {
  mode: string;
  subs= new Subscription();
  loading= true;
  gardenRating = 0;
  plantsRating = 0;
  areaRating = 0;
  areaRatingClass = 'area-rating-1';
  userInfo: any;
  totalArea = 0;
  gardenArea = 0;
  currentDate = new Date();
  chartData: number[];
  offsetChartData: number[];
  newMainChartData: number[];
  sendEmail: FormGroup;
  fromNameText = '';
  toNameText = '';
  flaechennutzungAlert = "Je weniger Flächen versiegelt werden desto besser. Optimal ist es, wenn so viel Außenflächen, wie möglich aus offenem Boden mit Pflanzenbewuchs bestehen.";
  oekoAlert = "Insektenhotel, Nistkästen oder Komposthaufen - je mehr Öko-Elemente im Garten realisiert werden, desto ökologischer ist dein Garten.";
  plantAlert = "Artenvielfalt ist das A und O für ein stabiles ökologisches Gleichgewicht im Garten. Je mehr verschiedene sinnvolle Pflanzen kultiviert werden, desto besser.";
  chartAlert = "In einem ökologisch wertvollen Garten sollte in jeder Jahreszeit etwas blühen, damit Insekten genügend Nahrung finden. Das Diagramm zeigt dir, ob es in deinem Garten noch große Lücken im Jahresverlauf gibt.";
  defaultText = "es ist jetzt ganz einfach, selbst aktiv etwas gegen das Insekten- und Bienensterben und für mehr Biodiversität zu tun. Privaten Gärten und Balkonen kommt dabei eine sehr wichtige Aufgabe zu, z. B. durch die richtige Auswahl der Pflanzen. Im Anhang siehst du den Ökoscan von meinem Garten, der aufwww.gardify.deerstellt wurde. Hier kannst Du ganz einfach in der Pflanzensuche insektenfreundliche Pflanzen finden und Pflanzen per Foto bestimmen. Die Anwendung ist übrigens kostenlos.  \n\nVielleicht hast du ja Lust, deinen eigenen Garten auch mal zu checken. ";

  // zeros at the ends are for padding the chart
  defaultChartData = [[2, 2, 4, 6, 7, 15,20, 20, 15, 10, 6, 4, 2, 2],
  [4, 4, 6, 10, 13, 20, 30, 30, 20, 15, 8, 5, 4, 4],
  [5, 5, 7, 12, 16, 27, 40, 40, 27, 20, 12, 7, 5, 5],
  [6, 6, 8, 15, 25, 35, 50, 50, 35, 25, 14, 8, 6, 6]
  ];
  apiCallFrom=new UserActionsFrom();

  tempData: number[];
  faInfo = faInfo;
  barChartOptions = {
    scaleShowVerticalLines: false,
    responsive: true,
    scales: {
      xAxes: [{
          stacked: true,
          gridLines: {
            drawOnChartArea: false,
            drawTicks: false,
            lineWidth: 3,
            color: 'rgba(0, 0, 0, 1)',
            z	: 1
            },
            ticks: {
              padding: 5//this will remove only the label
              
          }
      }],
      yAxes: [{
          stacked: true,
          

          gridLines: {
            drawOnChartArea: false,
            drawTicks: false,
            lineWidth: 3,
            color: 'rgba(0, 0, 0, 1)',
            z	: 1
          },
          ticks: {
           display: false,//this will remove only the label
           max: 60,
       }
      }]
    },
    legend: {
      display: false,
      position: 'bottom'
    },
    tooltips: {
      onlyShowForDatasetIndex: [1, 2],
      callbacks: {
        label: function(t, d) {
            const xLabel = d.datasets[t.datasetIndex].label;
            const yLabel = t.yLabel;
            // if line chart
    
            
            const limit = d.datasets[2].data[t.index];

            if (t.datasetIndex === 0){
              return false
            }
            if (limit > 0) {
              if (t.datasetIndex === 1) {
                  return  d.datasets[2].label;
                
              } else if (t.datasetIndex === 2) { return d.datasets[1].label + ': ' + (d.datasets[2].data[t.index] + d.datasets[0].data[t.index]).toString(); }
              
            }

            return xLabel +': '+yLabel;


        }
      },
      enabled: false,
      custom: function(tooltipModel) {
        // Tooltip Element
        var tooltipEl = document.getElementById('chartjs-tooltip');

        // Create element on first render
        if (!tooltipEl) {
            tooltipEl = document.createElement('div');
            tooltipEl.id = 'chartjs-tooltip';
            tooltipEl.innerHTML = '<table></table>';
            document.body.appendChild(tooltipEl);
        }

        // Hide if no tooltip
        if (tooltipModel.opacity === 0) {
            tooltipEl.style.opacity = '0';
            return;
        }

        // Set caret Position
        tooltipEl.classList.remove('above', 'below', 'no-transform');
        if (tooltipModel.yAlign) {
            tooltipEl.classList.add(tooltipModel.yAlign);
        } else {
            tooltipEl.classList.add('no-transform');
        }

        function getBody(bodyItem) {
            return bodyItem.lines;
        }

        // Set Text
        if (tooltipModel.body) {
            var titleLines = tooltipModel.title || [];
            var bodyLines = tooltipModel.body.map(getBody);

            var innerHtml = '<thead>';

            titleLines.forEach(function(title) {
                innerHtml += '<tr><th style="color: white">' + title + '</th></tr>';
            });
            innerHtml += '</thead><tbody>';

            bodyLines.forEach(function(body, i) {
       
                var colors = tooltipModel.labelColors[i];
                var greenColors = tooltipModel.labelColors[1]
                var style = '';
                if (tooltipModel.dataPoints[0].datasetIndex === 1){
                  style += 'background:' + colors.backgroundColor;
                  style += '; border-color:' + colors.borderColor;
                  style += '; border-width: 2px ;     border-width: 2px;position: absolute;left: 0;top: 4px;width: 10px;height: 10px;';
                }
                
                else if (tooltipModel.dataPoints[0].datasetIndex === 2){
                  style += '; border-width: 2px ;     border-width: 2px;position: absolute;left: 0;top: 4px;width: 10px;height: 10px;';
                  style += '; border-top: 5px solid white; border-left: 5px solid white; border-right: 5px solid rgba(143, 194, 115, 1) ; border-bottom: 5px solid rgba(143, 194, 115, 1);';
                }
                var span = '<span style="' + style + '"></span>';
                innerHtml += '<tr><td style="color: white; position: relative; padding-left: 14px;">' + span + body + '</td></tr>';
            });
            innerHtml += '</tbody>';

            var tableRoot = tooltipEl.querySelector('table');
            tableRoot.innerHTML = innerHtml;
        }

        // `this` will be the overall tooltip
        var position = this._chart.canvas.getBoundingClientRect();

        // Display, position, and set styles for font
        if (tooltipModel.dataPoints[0].datasetIndex !== 0){
          tooltipEl.style.opacity = '1';
        }
        else{
          tooltipEl.style.opacity = '0';
        }
        tooltipEl.style.position = 'absolute';
        tooltipEl.style.backgroundColor = 'rgba(0,0,0,0.5)';
        tooltipEl.style.left = position.left + window.pageXOffset + tooltipModel.caretX + 'px';
        // tooltipEl.style.transform = "translate(-50%, -120%)";

        if (tooltipModel.dataPoints[0].index < 5){
          tooltipEl.style.transform = "translate(10%, -120%)";
        }
        else if (tooltipModel.dataPoints[0].index < 10){
          tooltipEl.style.transform = "translate(-50%, -120%)";
        }
        else {
          tooltipEl.style.transform = "translate(-110%, -120%)";
        }
        
        tooltipEl.style.top = position.top + window.pageYOffset + tooltipModel.caretY + 'px';
        tooltipEl.style.transition = "top 0.25s, left 0.25s, right 0.25s, transform 0.25s";
        tooltipEl.style.fontFamily = tooltipModel._bodyFontFamily;
        tooltipEl.style.fontSize = tooltipModel.bodyFontSize + 'px';
        tooltipEl.style.fontStyle = tooltipModel._bodyFontStyle;
        tooltipEl.style.padding = tooltipModel.yPadding + 'px ' + tooltipModel.xPadding + 'px';
        tooltipEl.style.pointerEvents = 'none';
    }
    }
    
  };  
  barChartLabels: any;
  barChartType = 'line';
  barChartLegend = true;  
  barChartData = [];
  newDiary: FormGroup;
  gardenId;

  constructor(private tp: ThemeProviderService, private alert: AlertService,private auth: AuthService, private fb: FormBuilder, private util: UtilityService) { 
    this.sendEmail = this.fb.group({
      email: '',
      fromMail: '',
      fromName: '',
      toName: '',
      emailText: this.defaultText
    });

  }

  confirmWindow(text){
    window.alert(text);
  }

  ngOnDestroy(): void {
   this.subs.unsubscribe();
  }
  
  updateSenderName(name) {
    this.fromNameText = name;
  }

  updateReceiverName(name) {
    this.toNameText = name;
  }

  updateTotalArea(value) {
    this.totalArea = value;
    this.updateAreaRating();
  }

  updateGardenArea(value) {
    this.gardenArea = value;
    this.updateAreaRating();
  }

  updateAreaRating() {
    if (this.totalArea > 0) {
      this.areaRating = Math.max( ( Math.min(this.gardenArea, this.totalArea)* 100.0 / this.totalArea), 0);
    }
    this.updateFlowerDurationChart();
  }

  updateFlowerDurationChart() {
    if (this.totalArea < 100) {
      this.tempData = this.defaultChartData[0];
      this.areaRatingClass = 'area-rating-1';
    } else if (this.totalArea < 500) {
      this.tempData = this.defaultChartData[1];
      this.areaRatingClass = 'area-rating-2';
    } else if (this.totalArea < 1000) {
      this.tempData = this.defaultChartData[2];
      this.areaRatingClass = 'area-rating-3';
    } else {
      this.tempData = this.defaultChartData[3];
      this.areaRatingClass = 'area-rating-4';
    }
    var maxValue = 0
    if (this.barChartData.length > 0) {
      this.barChartData[0].data = this.tempData;
      maxValue = Math.max(maxValue, Math.max(...this.defaultChartData[3].map(o=>o)));
    }

    if (this.barChartData.length > 2) {
      this.offsetChartData = [];
      this.newMainChartData = [];
      maxValue = Math.max(maxValue, Math.max(...this.chartData.map(o=>o)));
      this.barChartOptions.scales.yAxes[0].ticks.max = maxValue + 4;
      for (let index = 0; index < this.barChartData[1].data.length; index++) {
        
        if (this.chartData[index] > this.barChartData[0].data[index] ) {
          
          this.offsetChartData.push(this.chartData[index] - this.barChartData[0].data[index]);
          this.newMainChartData.push(this.barChartData[0].data[index]);
        } else {
          this.offsetChartData.push(0);
          this.newMainChartData.push(this.chartData[index]);
        }
        
      }
      this.barChartData[2].data = this.offsetChartData;
      this.barChartData[1].data = this.newMainChartData;
    }
  }

  addFlowerDurationChart() {
    this.barChartData = [];

    this.tempData = this.defaultChartData[0];

    this.barChartData.push({
      data:  this.tempData, 
      label: 'Optimale Kurve der Blühdauer', 
      type: 'line', 
      backgroundColor:'rgba(178, 204, 143, 0.2)', 
      borderColor:'rgba(178, 204, 143, 0)', 
      borderWidth: 0, 
      stack: 1,
      pointRadius: 0
    });

    this.offsetChartData = [];
    for (let index = 0; index < this.chartData.length; index++) {
        this.offsetChartData.push(0);
    }

    this.barChartData.push({ data: this.chartData, 
      label: 'Anzahl meiner blühenden Arten/Sorten', 
      type: 'bar', 
      backgroundColor:'rgba(143, 194, 115, 1)', 
      borderColor:'rgba(143, 194, 115, 0)', 
      borderWidth: 1, 
      stack: 2,
      pointRadius: 0});
    this.barChartData.push({ data: this.offsetChartData, 
      label: 'Optimum erreicht', 
      type: 'bar', 
      backgroundColor:'rgba(143, 194, 115, 0)', 
      borderColor:'rgba(143, 194, 115, 1)', 
      borderWidth: 1, 
      stack: 2,
      pointRadius: 0});
    this.updateFlowerDurationChart();
  }

  saveScan() {
    const data = {
      'gardenRating' : this.gardenRating,
      'areaRating': this.areaRating,
      'plantsRating': this.plantsRating,
      'totalArea': this.totalArea,
      'gardenArea': this.gardenArea,
      'graph': this.chartData,
      'label': this.barChartLabels,
      'userInfo' : this.userInfo,
      'date' : new Date()
    };
    this.newDiary.patchValue({ EntryObjectId: this.gardenId , Description: btoa(JSON.stringify(data))});
    let params = new HttpParams();
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
    this.subs.add(
      this.util.createDiaryEntry(this.newDiary.value,params).subscribe((entry) => {
        // uplaod the images
        this.alert.success('Eintrag wurde erstellt');
      })
    );

    // this.exportAsPDF();
  }

  ngOnInit() {
    this.newDiary = this.fb.group({
      Title: 'Mein Ökoscan',
      Description: '',
      Date: new Date().toISOString().split('T')[0],
      UserId: this.auth.getCurrentUser().UserId,
      EntryObjectId: 0,
      EntryOf: ObjectType.BioScan,
    });
    this.updateAreaRating();
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
    let params = new HttpParams();
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
    this.subs.add(this.util.getUserMainGarden(params).subscribe(garden => {
      this.gardenId = garden.Id;
    }));
    this.subs.add(this.util.getUserInfo().subscribe(g => {
      this.userInfo=g;
      this.userInfo.UserName = this.userInfo.UserName.includes('UserDemo') ? 'Demo-Konto' : this.userInfo.UserName;
    }));
    this.subs.add(this.util.getUserMainGardenRating(params).subscribe(r => {
      this.gardenRating = r || 0;
    }));
    this.subs.add(this.util.UserPlantsRating().subscribe(r => {
      this.plantsRating = r || 0;
    }));
    this.checkFormValue()
  }

  ngAfterViewInit() {
    this.subs.add(this.util.UserPlantsFlowerDurationChart().subscribe(r => {
      this.loading = false;
      // add placeholder data to push chart bars inside, probably not the best way to do it
      r.unshift({Month: -1, PlantCount: 0});
      r.push({Month: -1, PlantCount: 0});

      this.chartData = r.map(f => f.PlantCount);

      this.addFlowerDurationChart();
      this.barChartLabels = r.map(f => this.monthConverter(f.Month));
      
      //setTimeout(() => {
      //  const ctx = (document.getElementById('canvas') as HTMLCanvasElement).getContext('2d').createLinearGradient(0, 0, 0, 600);
      //  ctx.addColorStop(0.0, '#fcffa4');
      //  ctx.addColorStop(0.3, '#f98e08');
      //  ctx.addColorStop(0.7, '#bb3754');
      //  ctx.addColorStop(1, '#570f6d');
  
      //  this.barChartData[1].backgroundColor = ctx;
      //});
    }, (reason) => {
      this.loading = false;

      this.alert.error('Fehler ist aufgetreten');
    }));
  }

  monthConverter(month) {
    switch (month) {
      case 1:
        return 'Jan.';
      case 2:
        return 'Feb.';
      case 3:
        return 'März';
      case 4:
        return 'April';
      case 5:
        return 'Mai';
      case 6:
        return 'Juni';
      case 7:
        return 'Juli';
      case 8:
        return 'Aug.';
      case 9:
        return 'Sept.';
      case 10:
        return 'Okt.';
      case 11:
        return 'Nov.';
      case 12:
        return 'Dez.';
      default:
        return '';
    }
  }

  exportAsPDF() {
    const data = document.getElementById('MyDIv');  
    // html2canvas(data).then(canvas => {
 
    html2canvas(data, {scrollY: -window.scrollY}).then(canvas => {  
      // Few necessary setting options 
      const contentDataURL = canvas.toDataURL('image/jpg');
      const data = new FormData();
      
      data.append('email', this.sendEmail.value.email);
      data.append('fromMail', this.sendEmail.value.fromMail);
      data.append('fromName', this.sendEmail.value.fromName);
      data.append('toName', this.sendEmail.value.toName);
      data.append('emailText', this.checkLinkSpace(this.sendEmail.value.emailText));
      data.append('image', contentDataURL);
      
      this.loading = true;
      this.subs.add(this.util.sendEmail(data).subscribe((res) => {
        this.alert.success('E-Mail gesendet');
        this.loading = false;
      }, (reason) => {
        this.alert.error('Fehler ist aufgetreten');
        this.loading = false;

      }));
      // let pdf = new jspdf('p', 'pt', [imgWidth, imgHeight]); // A4 size page of PDF  
      // var position = 0;  
      // pdf.addImage(contentDataURL, 'JPG', 0, position, imgWidth, imgHeight)  
      
      // pdf.save('MYPdf.pdf'); // Generated PDF   
    }); 
  }

  checkFormValue(){
    this.sendEmail.patchValue({
      emailText: this.checkLinkSpace( this.sendEmail.value.emailText)
    });
  }

  checkLinkSpace(text){
    if (!text.includes(" www.gardify.de ")){
      return text.replace("www.gardify.de", " www.gardify.de ")
    }
    return text
  }
}
