import { Component, OnInit, ChangeDetectorRef, HostListener, OnDestroy } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { UtilityService } from 'src/app/services/utility.service';
import { Chart } from 'chart.js'; //TODO: update to v3 when released to remove moment.js (guide: https://www.chartjs.org/docs/next/getting-started/v3-migration/)
import { faArrowDown, faArrowUp } from '@fortawesome/free-solid-svg-icons';
import { WeatherService } from 'src/app/services/weather.service';
import { Subscription } from 'rxjs';
import { Ad, UserActionsFrom } from 'src/app/models/models';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-forecast',
  templateUrl: './forecast.component.html',
  styleUrls: ['./forecast.component.css']
})

export class ForecastComponent implements OnInit, OnDestroy {
  forecastInterval: any;
  ad: Ad;
  constructor(private tp: ThemeProviderService, private util: UtilityService, private cdRef: ChangeDetectorRef, private weatherService: WeatherService) {
    this.isViewable = true;
    this.ad = new Ad(
      'ca-pub-3562132666777902',
      4490696542,
      'auto',
      true,
      false,
      null
    ) 
  }
  mode: string;
  faArrowUp = faArrowUp; faArrowDown = faArrowDown;
  forecast: any;
  fcfirstItem: any;
  fcDaily: any;
  fcDaily4: any;
  fcDaily3: any;
  fc3Items: any;
  fcrestItem: any;
  city: any;
  isViewable: boolean;
  isMobile: boolean;
  isXsMobile: boolean;
  forcDay: any;
  minTemp = [];
  maxTemp: number[] = [];
  rain = [];
  data: number[] = [];
  chart: any = [];
  dateformat: any;
  hour = [];
  chart2: any = [];
  ctx: [];
  daysofTheWeek = ['So', 'Mo', 'Di', 'Mi', 'Do', 'Fr', 'Sa'];
  monthsOfTheYear = ['Januar', 'Februar', 'März', 'April', 'Mai', 'Juni', 'Juli', 'August', 'September', 'Oktober', 'November', 'December',];
  dayItem: any;
  monthItem: any;
  yearItem: any;
  month: any = [];
  point = '.';
  dateString: any = [];
  dayNumber: any = [];
  daysformat: any;
  monthNumber: any = [];
  width = window.innerWidth;
  fcDaily1: any;
  dateformat1: any;
  dayItem1: any;
  dayWeek: string;
  chart3: any = [];
  chart4: any = [];
  newDayNumber: any = [];
  newHour: any = [];
  dateformatsec: any;
  monthNumbersec: any = [];
  daysformatsec: any;
  dayItemsec: any;
  dateStringsec: any = [];
  minTempsec: any = [];
  maxTempsec: any = [];
  rainsec: any = [];
  rainQuantity: any;
  isdisabled: boolean;
  newForecast: any;
  forecastRain: any;
  windOrigin: string;
  windStrength: string;
  windOriginArray: any = [];
  windStrengthArray: any = [];
  latitude: any = 50; longitude: any = 7;
  SunsetAndSunriseTime: any[];
  firstItemPT1H: any;
  currentTime = new Date();
  morningFc: any;
  middayFc: any;
  eveningFc: any;
  nightFc: any;
  fourFcinDay = [];
  tomorrowFc: any;
  array = [];
  fcrestItemPT1H: any;
  dailyweather: any;
  eightdays = [];
  loading = true;
  fullDayName: any = [];
  tomorrowDay: any;
  tomorrowMonth: any;
  dayFullString = ['Sonntag', 'Montag', 'Dienstag', 'Mittwoch', 'Donnerstag', 'Freitag', 'Samstag'];
  firsttwoDays = ['heute', 'morgen'];
  subs = new Subscription();
  // used to get better icons 
  dayRanges = [];
  fcrestItemPT1HDisplayed: any;
  forecastDisplayed: any;
  temp: any = undefined;
  apiCallFrom=new UserActionsFrom();

  @HostListener('window:resize')
  onWindowResize() {
    this.width = window.innerWidth;
    this.isMobile = window.innerWidth <= 990;
    this.isXsMobile = window.innerWidth <= 375;
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  ngOnInit() {
    this.isMobile = window.innerWidth <= 768;
    this.isXsMobile = window.innerWidth <= 375;
    this.isViewable = true;
    this.isdisabled = true;
    this.loading = true;
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
    this.getTodayForecast();
    this.getDailyForecast();
    this.showFcOfMorning();
  }

  getSunsetAndSunrise() {
    this.subs.add(this.util.getSunsetAndSunrise(this.longitude, this.latitude).subscribe(d => {
      this.SunsetAndSunriseTime = d.SunsetandSunrises;
    }));
  }

  getTodayForecast() {
    this.subs.add(this.util.getForecast(this.longitude, this.latitude).subscribe(d => {
      d.Forecasts.forEach(element => {
        this.windStrengthArray.push(this.setWindStrength(element.WindSpeedInBeaufort));
        this.windOriginArray.push(this.setWindOrigin(element.WindDirectionInDegree));
      });

      this.forecast = d.Forecasts.filter(d => d.ValidPeriod === 'PT0S');
      this.firstItemPT1H = d.Forecasts.filter(d => d.ValidPeriod === 'PT1H');

      this.fcfirstItem = d.Forecasts.filter(d => d.ValidPeriod === 'PT0S').slice(0, 1);
      this.dayWeek = this.daysofTheWeek[new Date(this.fcfirstItem[0].ValidFrom).getDay()];
      this.monthItem = this.monthsOfTheYear[new Date(this.fcfirstItem[0].ValidFrom).getMonth()];
      this.dailyweather = d.Forecasts.slice(0, 2);

      this.forecastDisplayed = this.forecast;
      this.fcrestItemPT1HDisplayed = this.firstItemPT1H;
    }));
  }

  showListRange(period: string) {
    let from = 0, to = 0;
    switch (period) {
      case 'Morgens': from = 6; to = 10; break;
      case 'Mittags': from = 11; to = 15; break;
      case 'Abends': from = 16; to = 19; break;
      case 'Nachts': from = 20; to = 24; break;
    }
    this.fcrestItemPT1HDisplayed = this.firstItemPT1H.filter(d => from <= new Date(d.ValidFrom).getHours() && new Date(d.ValidFrom).getHours() <= to);
    this.forecastInterval = this.forecast.filter(d => from <= new Date(d.ValidFrom).getHours() && new Date(d.ValidFrom).getHours() <= to);
  }

  getDailyForecast() {
    let params = new HttpParams();
    params = params.append('isIos', this.apiCallFrom.IsIos.toString());
    params = params.append('isAndroid', this.apiCallFrom.IsAndroid.toString());
    params = params.append('isWebPage', this.apiCallFrom.IsWebPage.toString());
    this.subs.add(this.util.getDailyForecast(this.longitude, this.latitude,params).subscribe(d => {
      this.city = d.City;
      //get forecast for four days
      this.fcDaily4 = d.Forecasts.slice(0, 4);
      this.fcDaily4.forEach(element => {
        const dateformat = new Date(element.ValidFrom);
        this.fullDayName.push(this.dayFullString[dateformat.getDay()]);
      });

      //get forecast for three days
      this.fcDaily3 = d.Forecasts.slice(0, 3);
      // get Data to build the Charts
      this.fcDaily3.forEach(element => {
        this.dateformatsec = new Date(element.ValidFrom);
        this.newHour.push(element.SunshineDurationInMinutes / 60);

        this.dayItemsec = this.daysofTheWeek[this.dateformatsec.getDay()];
        this.dateStringsec.push(this.dayItemsec);
        this.monthNumbersec = ((this.dateformatsec.getMonth() + 1) < 10 ? '0' : '') + (this.dateformatsec.getMonth() + 1);
        this.daysformatsec = ((this.dateformatsec.getDate()) < 10 ? '0' : '') + (this.dateformatsec.getDate());
        this.newDayNumber.push([this.daysformatsec + this.point + this.monthNumbersec]);
        this.minTempsec.push(element.MinAirTemperatureInCelsius);
        this.maxTempsec.push(element.MaxAirTemperatureInCelsius);
        this.rainsec.push(element.PrecipitationAmountInMillimeter);
      });

      /** forecast for the week */
      this.fcDaily = d.Forecasts;
      this.fcDaily.forEach(element => {
        this.dateformat = new Date(element.ValidFrom);
        this.hour.push(element.SunshineDurationInMinutes / 60);
        this.dayItem = this.daysofTheWeek[this.dateformat.getDay()];

        this.yearItem = this.dateformat.getFullYear();
        this.dateString.push(this.dayItem);
        this.monthNumber = ((this.dateformat.getMonth() + 1) < 10 ? '0' : '') + (this.dateformat.getMonth() + 1);
        this.daysformat = ((this.dateformat.getDate()) < 10 ? '0' : '') + (this.dateformat.getDate());
        this.eightdays.push([this.daysformat + this.point + this.monthNumber]);
        this.minTemp.push(element.MinAirTemperatureInCelsius);
        this.maxTemp.push(element.MaxAirTemperatureInCelsius);
        this.rain.push(element.PrecipitationAmountInMillimeter);
      });
      this.renderChart8DaysTrend(this.eightdays, this.hour, this.dateString, this.minTemp, this.maxTemp, this.rain);
      this.rendertChart3DaysTrend(this.dateStringsec, this.minTempsec, this.maxTempsec, this.rainsec, this.newDayNumber, this.newHour);
    }));
  }
  // get the wind Origin
  setWindOrigin(windDirection: any) {
    if (windDirection >= 0 && windDirection <= 45) {
      this.windOrigin = 'N';
    } else if (windDirection > 45 && windDirection <= 90) {
      this.windOrigin = 'O';
    } else if (windDirection > 90 && windDirection <= 135) {
      this.windOrigin = 'SO';
    } else if (windDirection > 135 && windDirection <= 180) {
      this.windOrigin = 'S';
    } else if (windDirection > 180 && windDirection <= 225) {
      this.windOrigin = 'SW';
    } else if (windDirection > 225 && windDirection <= 270) {
      this.windOrigin = 'W';
    } else if (windDirection > 270 && windDirection <= 315) {
      this.windOrigin = 'NW';
    } else if (windDirection > 315 && windDirection <= 360) {
      this.windOrigin = 'N';
    }
    return this.windOrigin;
  }

  //get the windStrength
  setWindStrength(windSpeedInBeaufort: any) {
    if (windSpeedInBeaufort == 0) {
      this.windStrength = 'Windstille';
    } else if (windSpeedInBeaufort == 1) {
      this.windStrength = 'Leiser Zug';
    } else if (windSpeedInBeaufort == 2) {
      this.windStrength = 'leichte Brise';
    } else if (windSpeedInBeaufort == 3) {
      this.windStrength = 'Schwache Brise';
    } else if (windSpeedInBeaufort == 4) {
      this.windStrength = 'Mäßige Brise';
    } else if (windSpeedInBeaufort == 5) {
      this.windStrength = 'Frische Brise';
    } else if (windSpeedInBeaufort == 6) {
      this.windStrength = 'Starker Wind';
    } else if (windSpeedInBeaufort == 7) {
      this.windStrength = 'Steifer Wind';
    } else if (windSpeedInBeaufort == 8) {
      this.windStrength = 'Stürmischer Wind';
    } else if (windSpeedInBeaufort == 9) {
      this.windStrength = 'Sturm';
    } else if (windSpeedInBeaufort == 10) {
      this.windStrength = 'Schwerer Sturm';
    } else if (windSpeedInBeaufort == 11) {
      this.windStrength = 'Orkanartiger Sturm';
    } else if (windSpeedInBeaufort == 12) {
      this.windStrength = 'Orkan';
    }
    return this.windStrength;
  }

  // get today's forecast for morning ,day, afternoon,evening
  showFcOfMorning() {
    this.subs.add(this.util.getOnlyTodayForecast(this.longitude, this.latitude).subscribe(d => {
      if (this.currentTime.getHours() >= 6 && this.currentTime.getHours() < 12) {
        this.dayRanges = [
          { fc: d.Forecasts.filter(d => 5 <= new Date(d.ValidFrom).getHours() && new Date(d.ValidFrom).getHours() < 10) }, // morning
          { fc: d.Forecasts.filter(d => 10 <= new Date(d.ValidFrom).getHours() && new Date(d.ValidFrom).getHours() < 15) }, // noon
          { fc: d.Forecasts.filter(d => 17 <= new Date(d.ValidFrom).getHours() && new Date(d.ValidFrom).getHours() < 21) }, // evening
        ];
        this.morningFc = d.Forecasts.filter(d => new Date(d.ValidFrom).getHours() == 9);
        this.middayFc = d.Forecasts.filter(d => new Date(d.ValidFrom).getHours() == 14);
        this.eveningFc = d.Forecasts.filter(d => new Date(d.ValidFrom).getHours() == 20);
        this.fourFcinDay.push(this.morningFc, this.middayFc, this.eveningFc);
        this.array = ['Morgens', 'Mittags', 'Abends', 'Morgens'];
      } else {
        this.dayRanges = [
          { fc: d.Forecasts.filter(d => 10 <= new Date(d.ValidFrom).getHours() && new Date(d.ValidFrom).getHours() < 15) }, // noon
          { fc: d.Forecasts.filter(d => 17 <= new Date(d.ValidFrom).getHours() && new Date(d.ValidFrom).getHours() < 21) }, // evening
          { fc: d.Forecasts.filter(d => 21 <= new Date(d.ValidFrom).getHours() || new Date(d.ValidFrom).getHours() < 5) }  // night
        ];
        this.middayFc = d.Forecasts.filter(d => new Date(d.ValidFrom).getHours() == 14);
        this.eveningFc = d.Forecasts.filter(d => new Date(d.ValidFrom).getHours() == 20);
        this.fourFcinDay.push(this.middayFc, this.eveningFc);

        this.subs.add(this.util.getOnlyTomorrowForecast(this.longitude, this.latitude).subscribe(t => {
          this.nightFc = t.Forecasts.filter(t => new Date(t.ValidFrom).getHours() == 3);
          this.fourFcinDay.push(this.nightFc);
          this.array = ['Mittags', 'Abends', 'Nachts', 'Morgens'];
        }));
      }
      this.subs.add(this.util.getOnlyTomorrowForecast(this.longitude, this.latitude).subscribe(d => {
        this.tomorrowFc = d.Forecasts.filter(d => new Date(d.ValidFrom).getHours() == 9);

        this.tomorrowDay = this.daysofTheWeek[new Date(this.tomorrowFc[1].ValidFrom).getDay()];
        this.tomorrowMonth = this.monthsOfTheYear[new Date(this.tomorrowFc[0].ValidFrom).getMonth()];
        this.loading = false;
      }));
    }));
  }

  toggleCharts(el: HTMLElement) {
    this.isViewable = !this.isViewable;
    this.isdisabled = !this.isdisabled;
    this.hour.length = 0, this.dateString.length = 0, this.minTemp.length = 0, this.maxTemp.length = 0, this.rain.length = 0, this.eightdays.length = 0;
    this.minTempsec.length = 0, this.maxTempsec.length = 0, this.rainsec.length = 0, this.newDayNumber.length = 0, this.newHour.length = 0, this.dateStringsec.length = 0;
    this.getDailyForecast();
    this.scroll(el);
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }

  //build Charts
  renderChart8DaysTrend(dayNumber, hours, dateString, minTemp, maxTemp, rain) {
    Chart.defaults.global.defaultFontSize = 14;
    Chart.defaults.global.responsive = true;
    if (this.mode === 'light') {
      Chart.defaults.global.defaultFontColor = 'black';
    } else {
      Chart.defaults.global.defaultFontColor = 'white';
    }
    this.chart = new Chart('canvas', {
      type: 'bar',
      data: {
        labels: dateString,
        datasets: [
          {
            type: 'line',
            label: 'Max. Temp. (°C)',
            data: maxTemp,
            borderColor: 'rgba(181, 146, 19, 0.5)',
            backgroundColor: 'rgba(181, 146, 19, 0.5)',
            fill: false,
            yAxisID: 'temperature',
          },
          {
            type: 'line',
            label: 'Min. Temp. (°C)',
            data: minTemp,
            borderColor: 'rgba(29, 165, 228, 0.5)',
            backgroundColor: 'rgba(29, 165, 228, 0.5)',
            fill: false,
            yAxisID: 'temperature',
          },
          {
            label: 'Niederschlagsmenge (l/m2)',
            data: rain,
            borderColor: 'rgba(0, 0, 0, 0)',
            backgroundColor: 'rgba(157, 203, 241, 1)',
            yAxisID: 'rain',
          }
        ]
      },
      options: {
        legend: {
          display: false
        },
        scales: {

          xAxes: [{
            position: 'top',
            display: true,
            gridLines: {
              display: false
            },
          }],
          yAxes: [
            {
              id: 'temperature',
              ticks: {
                beginAtZero: true,
                callback: function (value, index, values) {
                  return value + '°C';
                }
              },
              gridLines: {
                color: 'rgba(211,211,211 ,1)',
              },
              scaleLabel: {
                display: false,
                labelString: 'temperature'
              },
            },
            {
              id: 'rain',
              position: 'right',
              ticks: {
                beginAtZero: true,
                callback: function (value, index, values) {
                  return value + ' ' + 'l/m2';
                }
              },
              gridLines: {
                display: false
              },
              scaleLabel: {
                display: false,
                labelString: 'rain'
              }
            },
          ]
        },
      }
    });

    this.chart2 = new Chart('canvas2', {
      type: 'bar',
      data: {
        labels: dayNumber,

        datasets: [{
          label: 'Sonnenschein (h)',
          data: hours,
          borderColor: 'rgba(0, 0, 0, 0)',
          backgroundColor: 'rgba(235, 203, 19, 1)',
        }]
      },
      options: {
        legend: {
          display: false
        },
        scales: {
          yAxes: [{
            ticks: {
              beginAtZero: true,
              callback: function (value, index, values) {
                return value + ' ' + 'h';
              }
            },
            gridLines: { color: 'rgba(211,211,211 ,1)', }
          }],
          xAxes: [
            {
              gridLines: {
                display: false
              },
              ticks: {
                callback: function (value, index, values) {
                  return value + '.';
                }
              }
            }
          ]
        }
      }
    });

    this.cdRef.detectChanges();
  }
  rendertChart3DaysTrend(dateStringsec, minTempsec, maxTempsec, rainsec, newDayNumber, newHour) {
    Chart.defaults.global.defaultFontSize = 14;
    Chart.defaults.global.responsive = true;
    if (this.mode === 'light') {
      Chart.defaults.global.defaultFontColor = 'black';
    } else {
      Chart.defaults.global.defaultFontColor = 'white';
    }
    this.chart3 = new Chart('canvas3', {
      type: 'bar',
      data: {
        labels: dateStringsec,
        datasets: [
          {
            type: 'line',
            label: 'Max. Temp. (°C)',
            data: maxTempsec,
            borderColor: 'rgba(181, 146, 19, 0.5)',
            backgroundColor: 'rgba(181, 146, 19, 0.5)',
            yAxisID: 'temperature',
            fill: false
          },
          {
            type: 'line',
            label: 'Min. Temp. (°C)',
            data: minTempsec,
            borderColor: 'rgba(29, 165, 228, 0.5)',
            backgroundColor: 'rgba(29, 165, 228, 0.5)',
            yAxisID: 'temperature',
            fill: false
          },
          {
            label: 'Niederschlagsmenge (l/m2)',
            data: rainsec,
            borderColor: 'rgba(0, 0, 0, 0)',
            backgroundColor: 'rgba(157, 203, 241, 1)',
            yAxisID: 'rain',
          }
        ]
      },
      options: {
        legend: {
          display: false
        },
        scales: {
          xAxes: [{
            position: 'top',
            display: true,
            gridLines: {
              display: false
            },
          }],
          yAxes: [
            {
              id: 'temperature',
              ticks: {
                beginAtZero: true,
                callback: function (value, index, values) {
                  return value + '°C';
                }
              },
              gridLines: {
                color: 'rgba(211,211,211 ,1)',
              },
              scaleLabel: {
                display: false,
                labelString: 'temperature'
              },
            },
            {
              id: 'rain',
              position: 'right',
              ticks: {
                beginAtZero: true,
                callback: function (value, index, values) {
                  return value + ' ' + 'l/m2';
                }
              },
              gridLines: {
                display: false
              },
              scaleLabel: {
                display: false,
                labelString: 'rain'
              }
            },
          ]
        },
      }
    });
    this.chart4 = new Chart('canvas4', {
      type: 'bar',
      data: {
        labels: newDayNumber,
        datasets: [{
          label: 'Sonnenschein (h)',
          data: newHour,
          borderColor: 'rgba(0, 0, 0, 0)',
          backgroundColor: 'rgba(235, 203, 19, 1)',
        }]
      },
      options: {
        legend: {
          display: false
        },
        scales: {
          yAxes: [{
            ticks: {
              beginAtZero: true,
              callback: function (value, index, values) {
                return value + ' ' + 'h';
              }
            },
            gridLines: { color: 'rgba(211,211,211 ,1)', }
          }],
          xAxes: [
            {
              gridLines: {
                display: false
              },
              ticks: {
                callback: function (value, index, values) {
                  return value + '.';
                }
              }
            }
          ]
        }
      }
    });
    this.cdRef.detectChanges();

  }
  public getWeatherIcon(data, percMM, range) {
    return this.weatherService.getWeatherIcon(data, percMM, range);
  }
}
