import { Injectable } from '@angular/core';
import { WeatherIcons } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class WeatherService {

  constructor() { }
  public getWeatherIcon(data, percInMili = 0, range = []) {
    const WData = {
      icon: '',
      desc: ''
    };
    const code = data.WeatherCode;
    const precType = data.PrecipitationType;
    const cloudCover = data.CloudCoverLowerThan2000MeterInOcta;
    const windSpeed = data.WindSpeedInBeaufort;
    const percAmount = Math.max(percInMili, data.PrecipitationAmountInMillimeter);
    const rangeMax = range.reduce((acc, v) =>  acc + v.PrecipitationAmountInMillimeter, 0);
   
    if (this.percLevel(percAmount, rangeMax) === 1) {
      switch(this.precType(precType)) {
        case 0: switch(this.windLevel(windSpeed)) {
                  case 0: WData.icon = this.isDaylight(data) ? WeatherIcons.SunRain1 : WeatherIcons.MoonRain1;
                          WData.desc = 'leichter Regen'; break;
                  case 1: WData.icon = WeatherIcons.RainWind1;
                          WData.desc = 'leichter Regen, Wind'; break;
                  case 2: WData.icon = WeatherIcons.RainWind2;
                          WData.desc = 'starker Wind, Regen'; break;
                } break;
        case 1: switch(this.windLevel(windSpeed)) {
                  case 0: WData.icon = this.isDaylight(data) ? WeatherIcons.SunHail1 : WeatherIcons.MoonHail1;
                          WData.desc = 'leichter Hagel'; break;
                  case 1: WData.icon = WeatherIcons.HailWind1;
                          WData.desc = 'leichter Hagel, Wind'; break;
                  case 2: WData.icon = WeatherIcons.HailWind2;
                          WData.desc = 'starker Wind, Hagel'; break;
                } break;
        case 2: switch(this.windLevel(windSpeed)) {
                  case 0: WData.icon = this.isDaylight(data) ? WeatherIcons.SunSnow1 : WeatherIcons.MoonSnow1;
                          WData.desc = 'leichter Schnee'; break;
                  case 1: WData.icon = WeatherIcons.SnowWind1;
                          WData.desc = 'leichter Schnee, Wind'; break;
                  case 2: WData.icon = WeatherIcons.SnowWind2;
                          WData.desc = 'starker Wind, Schnee'; break;
                } break;
      }
    } else if (this.percLevel(percAmount, rangeMax) === 2) {
      switch(this.windLevel(windSpeed)) {
        case 0: WData.icon = this.isDaylight(data) ? WeatherIcons.SunRain2 : WeatherIcons.MoonRain2;
                WData.desc = 'starker Regen'; break;
        case 1: WData.icon = WeatherIcons.RainWind1;
                WData.desc = 'starker Regen, Wind'; break;
        case 2: WData.icon = WeatherIcons.RainWind2;
                WData.desc = 'starker Regen & Wind'; break;
      }
    } else if (this.cloudCoverLevel(cloudCover) === 0) {
      switch(this.windLevel(windSpeed)) {
        case 0: WData.icon = this.isDaylight(data) ? WeatherIcons.Sun : WeatherIcons.Moon;
                WData.desc = 'klar'; break;
        case 1: WData.icon = this.isDaylight(data) ? WeatherIcons.SunWind1 : WeatherIcons.MoonWind1;
                WData.desc = 'leichter Wind'; break;
        case 2: WData.icon = this.isDaylight(data) ? WeatherIcons.SunWind2 : WeatherIcons.MoonWind2;
                WData.desc = 'starker Wind'; break;
      }
    } else if (this.cloudCoverLevel(cloudCover) === 1) {
      if (this.percLevel(percAmount, rangeMax) === 0) {
        switch(this.windLevel(windSpeed)) {
          case 0: WData.icon = this.isDaylight(data) ? WeatherIcons.SunClouds : WeatherIcons.MoonClouds;
                  WData.desc = 'leicht bewölkt'; break;
          case 1: WData.icon = this.isDaylight(data) ? WeatherIcons.SunWind1 : WeatherIcons.MoonWind1;
                  WData.desc = 'leicht bewölkt'; break;
          case 2: WData.icon = this.isDaylight(data) ? WeatherIcons.SunWind2 : WeatherIcons.MoonWind2;
                  WData.desc = 'starker Wind'; break;
        }
      }
    } else if (this.cloudCoverLevel(cloudCover) === 2) {
      if (this.percLevel(percAmount, rangeMax) === 0) {
        switch(this.windLevel(windSpeed)) {
          case 0: WData.icon = WeatherIcons.Clouds;
                  WData.desc = 'stark bewölkt'; break;
          case 1: WData.icon = WeatherIcons.Clouds;
                  WData.desc = 'stark bewölkt, Wind'; break;
          case 2: WData.icon = WeatherIcons.RainWind2;
                  WData.desc = 'stark bewölkt & Wind'; break;
        }
      } else if (this.percLevel(percAmount, rangeMax) === 1) {
        switch(this.windLevel(windSpeed)) {
          case 0: WData.icon = WeatherIcons.Rain1;
                  WData.desc = 'Regen'; break;
          case 1: WData.icon = WeatherIcons.RainWind1;
                  WData.desc = 'Regen & Wind'; break;
          case 2: WData.icon = WeatherIcons.RainWind2;
                  WData.desc = 'starker Wind, Regen'; break;
        }
      } else if (this.percLevel(percAmount, rangeMax) === 2) {
        switch(this.windLevel(windSpeed)) {
          case 0: WData.icon = WeatherIcons.Rain2;
                  WData.desc = 'starker Regen'; break;
          case 1: WData.icon = WeatherIcons.StormyWind1;
                  WData.desc = 'heftiger Regen'; break;
          case 2: WData.icon = WeatherIcons.StormyWind2;
                  WData.desc = 'stürmisch Regen'; break;
        }
      }
    } else if ([-4,45,46,47,48,49].includes(code)) {
      WData.icon = WeatherIcons.Fog;
      WData.desc = 'Nebel';
    }
    WData.icon = WeatherIcons.base + WData.icon;
    return WData;
  }
  // returns true if time is between 6:30 and 19:00
  private isDaylight(data): boolean {
    const current = new Date(data.ValidFrom).getTime();
    const eve = new Date(data.ValidFrom);
    const morning = new Date(data.ValidFrom);
    eve.setHours(19,0,0,0);
    morning.setHours(6,30,0,0);
    return morning.getTime() <= current && current <= eve.getTime();
  }

  private inRange(x, min, max) {
    return x >= min && x <= max;
  }

  private percLevel(data, rangeMax = 0) { 
    if (rangeMax > 0) {
      data = Math.max(data, rangeMax);
    }
    if (data <= 1) { return 0; }
    if (1 < data && data <= 5) { return 1; }
    return 2;
  }

  private precType(data) {
    if ([22,23,26,36,37,38,39,68,69,93,94,95,97].includes(data) 
        || this.inRange(data,70,79)
        || this.inRange(data,83,88)) { return 2; } // snow
    if ([27,89,90,96,99].includes(data)) { return 1; }  // hail
    return 0; // rain or nothing
  }

  private cloudCoverLevel(data) {
    if (data < 1.5) { return 0; }
    if (1.5 <= data && data <= 5) { return 1; }
    return 2;
  }

  private windLevel(data) {
    if (data < 5) { return 0; }
    if (5 <= data && data <= 6) { return 1; }
    return 2;
  }

}

