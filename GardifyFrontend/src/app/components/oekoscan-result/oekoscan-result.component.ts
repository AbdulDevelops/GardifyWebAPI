import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { Subscription } from 'rxjs';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { ActivatedRoute } from '@angular/router';
import { faInfo } from '@fortawesome/free-solid-svg-icons';

declare var jQuery:any;

@Component({
  selector: 'app-oekoscan-result',
  templateUrl: './oekoscan-result.component.html',
  styleUrls: ['./oekoscan-result.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class OekoscanResultComponent implements OnInit {

  subs=new Subscription();
  mode:string;
  dataValue: any;

  loading= false;
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
  fromNameText = '';
  toNameText = '';
  flaechennutzungAlert = "Je weniger Flächen versiegelt werden desto besser. Optimal ist es, wenn so viel Außenflächen, wie möglich aus offenem Boden mit Pflanzenbewuchs bestehen.";
  oekoAlert = "Insektenhotel, Nistkästen oder Komposthaufen - je mehr Öko-Elemente im Garten realisiert werden, desto ökologischer ist dein Garten.";
  plantAlert = "Artenvielfalt ist das A und O für ein stabiles ökologisches Gleichgewicht im Garten. Je mehr verschiedene sinnvolle Pflanzen kultiviert werden, desto besser.";
  chartAlert = "In einem ökologisch wertvollen Garten sollte in jeder Jahreszeit etwas blühen, damit Insekten genügend Nahrung finden. Das Diagramm zeigt dir, ob es in deinem Garten noch große Lücken im Jahresverlauf gibt.";
  defaultText = 'es ist jetzt ganz einfach, selbst aktiv etwas gegen das Insekten- und Bienensterben und für mehr Biodiversität zu tun. Privaten Gärten und Balkonen kommt dabei eine sehr wichtige Aufgabe zu, z. B. durch die richtige Auswahl der Pflanzen. Im Anhang siehst du den Ökoscan von meinem Garten, der auf www.gardify.de erstellt wurde. Hier kannst Du ganz einfach in der Pflanzensuche insektenfreundliche Pflanzen finden und Pflanzen per Foto bestimmen. Die Anwendung ist übrigens kostenlos.  \n\nVielleicht hast du ja Lust, deinen eigenen Garten auch mal zu checken. ';

  // zeros at the ends are for padding the chart
  defaultChartData = [[2, 2, 4, 6, 7, 15,20, 20, 15, 10, 6, 4, 2, 2],
  [4, 4, 6, 10, 13, 20, 30, 30, 20, 15, 8, 5, 4, 4],
  [5, 5, 7, 12, 16, 27, 40, 40, 27, 20, 12, 7, 5, 5],
  [6, 6, 8, 15, 25, 35, 50, 50, 35, 25, 14, 8, 6, 6]
  ];
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
  gardenId;

  constructor(private tp:ThemeProviderService, private activatedRoute:ActivatedRoute) 
  {
    // $('.hide-page').hide();
    this.subs.add(this.activatedRoute.queryParams.subscribe(params =>{
      this.dataValue= JSON.parse(atob(params.data));
      }
      ))
   }

   ngOnDestroy(): void {
    this.subs.unsubscribe();
  
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

  ngOnInit(): void {
    this.currentDate = this.dataValue.date;
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
    this.areaRating = this.dataValue.areaRating ? this.dataValue.areaRating : 0;
    this.userInfo = this.dataValue.userInfo;
    this.gardenRating = this.dataValue.gardenRating;
    this.plantsRating = this.dataValue.plantsRating;
    this.totalArea = this.dataValue.totalArea;
    this.gardenArea = this.dataValue.gardenArea;
    var tempChartData = this.dataValue.graph;
    var tempLabel = this.dataValue.label;
    if (tempChartData.length < 13){
      tempChartData.unshift(0);
      tempChartData.push(0);
      tempLabel.unshift('');
      tempLabel.push('');
    }
    this.chartData = tempChartData;
    this.addFlowerDurationChart();
    this.barChartLabels = tempLabel;
    // this.exportAsPDF();
  }

  // exportAsPDF()
  // {
  //   let data = document.getElementById('MyDIv');  
  //   // html2canvas(data).then(canvas => {
  //   //   const contentDataURL = canvas.toDataURL('image/png')  
  //   //   // let pdf = new jspdf('l', 'cm', 'a4'); //Generates PDF in landscape mode
  //   //    let pdf = new jspdf('p', 'cm', 'a4'); //Generates PDF in portrait mode
  //   //   pdf.addImage(contentDataURL, 'PNG', 0, 0, 21, 20);  
  //   //   pdf.save('Filename.pdf');   
  //   // }); 
  //   // var data = document.getElementById('contentToConvert');  
  //   html2canvas(data, {scrollY: -window.scrollY}).then(canvas => {  
  //     // Few necessary setting options 
  //     var imgWidth = 208;   
  //     var pageHeight = 295;    
  //     var imgHeight = canvas.height * imgWidth / canvas.width;  
  //     var heightLeft = imgHeight;  

  //     const contentDataURL = canvas.toDataURL('image/png')  
  //     let pdf = new jspdf('p', 'mm', 'a4'); // A4 size page of PDF  
  //     var position = 0;  
  //     pdf.addImage(contentDataURL, 'PNG', 0, position, imgWidth, imgHeight)  
  //     pdf.save('MYPdf.pdf'); // Generated PDF   
  //   }); 

    
  // }

}
