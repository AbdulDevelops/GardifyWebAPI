//
//  Weather.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 03.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation

struct ForecastModel: Codable {
    let Forecasts: [Weather]
}

struct DailyForecastModel: Codable {
    let City: String
    let Forecasts: [DailyWeather]
}


struct Weather: Codable{
    let LocatedAt: [Float]?
    let ValidFrom: String?
    let ValidUntil: String?
    let AirTemperatureInCelsius: Float?
    let AirPressureAtSeaLevelInHectoPascal: Float?
    let WindSpeedInKilometerPerHour: Float?
    let WindDirectionInDegree: Int?
    let PrecipitationProbabilityInPercent: Float?
    let WeatherCode: Int?
    let ValidPeriod: String?
    let PrecipitationAmountInMillimeter: Float??
    let WindSpeedInBeaufort: Int?
    let RelativeHumidityInPercent: Float?
    let CloudCoverLowerThan2000MeterInOcta: Float?
    
}

struct DailyWeather: Codable{
    let LocatedAt: [Float]?
    let ValidFrom: String?
    let ValidUntil: String?
    let MinAirTemperatureInCelsius: Float?
    let MaxAirTemperatureInCelsius: Float?
    let maxWindSpeedInKilometerPerHour: Float?
    let SunshineDurationInMinutes: Int?
    let PrecipitationAmountInMillimeter: Float?
    let ValidPeriod: String?
    let CloudCoverLowerThan2000MeterInOcta: Float?
    
}
