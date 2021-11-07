//
//  WeatherUpdateViewController.swift
//  GardifyIOS
//
//  Created by Rifat Hussain on 17.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation
import UIKit
import Charts
import CoreLocation

struct hourlyForecastData {
    var opened = Bool()
    var title = String()
    var hourlyWeatherDetails: [Weather]?
    
}

struct dayRangeForecastData {
    var forcast: [Weather]?
}

class WeatherUpdateViewController: UIViewController, UITableViewDelegate, UITableViewDataSource, UICollectionViewDelegate, UICollectionViewDataSource, IAxisValueFormatter {
    
    @IBOutlet weak var weatherUpdateScrollView: UIScrollView!
    @IBOutlet weak var weatherUpdateView: UIView!
    @IBOutlet weak var currentWeatherContainer: UIView!
    @IBOutlet weak var WeatherStatusImageView: UIImageView!
    @IBOutlet weak var cityLabel: UILabel!
    @IBOutlet weak var temperatureLabel: UILabel!
    @IBOutlet weak var weatherClarityLabel: UILabel!
    @IBOutlet weak var dateLabel: UILabel!
    @IBOutlet weak var horizontalSplitView: UIView!
    @IBOutlet weak var windStatusImageView: UIImageView!
    @IBOutlet weak var windStatusLabel: UILabel!
    @IBOutlet weak var windDirectionImageView: UIImageView!
    @IBOutlet weak var windDirectionLabel: UILabel!
    @IBOutlet weak var airPressureImageView: UIImageView!
    @IBOutlet weak var airPressureLabel: UILabel!
    @IBOutlet weak var humidityImageView: UIImageView!
    @IBOutlet weak var humidityLabel: UILabel!
    
    @IBOutlet weak var dailyForecastLabel: UILabel!
    @IBOutlet weak var hourlyForecastLabel: UILabel!
    
    @IBOutlet weak var dailyWeatherCollectionView: UICollectionView!
    @IBOutlet weak var hourlyWeatherTableView: FullTableView!
    
    @IBOutlet weak var chartsContainer: UIView!
    @IBOutlet weak var dayTrendChartView: CombinedChartView!
    @IBOutlet weak var dailyTemperatureChartView: BarChartView!
    
    @IBOutlet weak var maxColorView: UIView!
    @IBOutlet weak var maxTempLabel: UILabel!
    @IBOutlet weak var minTempColorView: UIView!
    @IBOutlet weak var minTempLabel: UILabel!
    @IBOutlet weak var niederschlagsmengeColorView: UIView!
    @IBOutlet weak var niederschlagsmengeLabel: UILabel!
    @IBOutlet weak var sonnenscheinColorView: UIView!
    @IBOutlet weak var sonnenscheinLabel: UILabel!
    
    
    var days = [String]()
    var dates = [String]()
    weak var axisFormatDelegate: IAxisValueFormatter?
    let geocoder = CLGeocoder()
    var hourlyWeatherTableViewData = [hourlyForecastData]()
    var dayRanges = [dayRangeForecastData]()
    var todayWeatherDetails: [Weather]?
    var precipitationWeatherDetails: [Weather]?
    var currentWeatherDetails: [Weather]?
    var morningWeatherDetails: [Weather]?
    var noonWeatherDetails: [Weather]?
    var eveningWeatherDetails: [Weather]?
    var nightWeatherDetails: [Weather]?
    var tomorrowWeatherDetails: ForecastModel?
    var dailyForecastDetails: DailyForecastModel?
    
    public let dateTime = Date()
    public var currentTime: String?
    public let dateFormatter = DateFormatter()
    
    override func viewWillAppear(_ animated: Bool) {
        super.viewWillAppear(animated)
        self.updateNavigationBar(isMain: false, "GARTEN", "WETTER", "main_weather")

    }
    
    override func viewDidAppear(_ animated: Bool) {
       super.viewWillAppear(false)

        self.configurePage()
        
    }
    
    func configurePage(){
        self.applyTheme()
        self.currentWeatherContainer.addBorderRadius()
        self.chartsContainer.backgroundColor = UIColor.white
        self.chartsContainer.addBorderRadius()
        self.chartsContainer.addShadow()
        self.currentWeatherContainer.addShadow()
     
        self.dayTrendChartView.addBorderRadius()
        // self.chartsContainer.backgroundColor = UIColor.gray
        dateFormatter.dateFormat = "HH"
        self.currentTime = dateFormatter.string(from: dateTime)
        
        self.currentTime = "\((Int(self.currentTime!)! + 1) % 24)"
        self.hourlyForecastLabel.text = "Stündliche Vorhersage"
        self.dailyForecastLabel.text = "Aussichten"
        windStatusImageView.image = UIImage(named: "Regenwahrscheinlichkeit_Niederschlag")
        windDirectionImageView.image = UIImage(named: "Wind")
        airPressureImageView.image = UIImage(named: "Luftdruck")
        humidityImageView.image = UIImage(named: "Luftfeuchte")
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()

        self.showSpinner(onView: self.view)
        
        self.configurePadding()
        axisFormatDelegate = self
        // days = ["Mo", "Di", "Mi", "Do", "Fr", "Sa", "So", "Mo"]
        
        dateFormatter.dateFormat = "dd.MM.yyyy"
        
        self.hourlyWeatherTableView.backgroundColor = .clear
        self.dateLabel.text = "aktuell, \(dateFormatter.string(from: dateTime))"
        self.hourlyWeatherTableView.delegate = self
        self.hourlyWeatherTableView.dataSource = self
        self.dailyWeatherCollectionView.delegate = self
        self.dailyWeatherCollectionView.dataSource = self
        
        DispatchQueue.main.async {
            self.loadForcast()
            
        }
        
       
        
    }
    
    
    
    func loadForcast(){
        
        NetworkManager().requestDataAsync(type: DailyForecastModel.self, APP_URL.Forecast_URL + "daily") {data in
            
            if !data.success{
                self.ShowAlert(message: "Cannot load daily data")
                self.removeSpinner()
                return
            }
            
            self.dailyForecastDetails = data.result as? DailyForecastModel
            self.dailyForecastDetails?.Forecasts.forEach{
                self.days.append($0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "EE"))
                self.dates.append($0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "dd.MM"))
            }
            self.cityLabel.text = self.dailyForecastDetails?.City
            
//            self.getCityNameFromZip(zipCode: self.dailyForecastDetails?.City ?? "")
            self.configureCombinedChart()
            self.configureBarChart()
            self.dailyWeatherCollectionView.reloadData()
            
            NetworkManager().requestDataAsync(type: ForecastModel.self, APP_URL.Forecast_URL + "todayForecast") {data in
                if !data.success{
                    self.ShowAlert(message: "Cannot load today data")
                    self.removeSpinner()
                    return
                }
                let todayData = (data.result as! ForecastModel)
                self.todayWeatherDetails = todayData.Forecasts.filter({$0.ValidPeriod=="PT0S"})
                self.precipitationWeatherDetails = todayData.Forecasts.filter({$0.ValidPeriod=="PT1H"})
                
                self.todayWeatherDetails?.forEach{
                    var psoWeather = $0
                    var currentHour = $0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH")
                    print("hour is", currentHour, self.currentTime)
                    if(currentHour == self.currentTime){
                        self.temperatureLabel.text = "\($0.AirTemperatureInCelsius!) °C"
                        self.windStatusLabel.text = "\($0.PrecipitationProbabilityInPercent!)% / \($0.PrecipitationAmountInMillimeter! ?? 0.0) l/m2"
                        self.airPressureLabel.text = "Luftdruck \($0.AirPressureAtSeaLevelInHectoPascal!) hPa"
                        self.humidityLabel.text = "\($0.RelativeHumidityInPercent!)% Luftfeuchte"
                        self.windDirectionLabel.text = "\(self.setWindStrength(windSpeedInBeaufort: $0.WindSpeedInBeaufort!)) aus \(self.setWindOrigin(windDirection: $0.WindDirectionInDegree!)) (\($0.WindSpeedInKilometerPerHour!)km/h)"
                        
                        
                        if (self.currentTime! as NSString).integerValue >= 6 && (self.currentTime! as NSString).integerValue < 12{
                            self.dayRanges = [
                                dayRangeForecastData(forcast: self.todayWeatherDetails!.filter({$0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH") >=  "05" &&  $0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH") < "10"})),
                                dayRangeForecastData(forcast: self.todayWeatherDetails!.filter({$0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH") >=  "10" &&  $0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH") < "15"})),
                                dayRangeForecastData(forcast: self.todayWeatherDetails!.filter({$0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH") >=  "17" &&  $0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH") < "21"}))
                            ]
                        }else{
                            self.dayRanges = [
                                dayRangeForecastData(forcast: self.todayWeatherDetails!.filter({$0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH") >=  "10" &&  $0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH") < "15"})),
                                dayRangeForecastData(forcast: self.todayWeatherDetails!.filter({$0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH") >=  "15" &&  $0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH") < "21"})),
                                dayRangeForecastData(forcast: self.todayWeatherDetails!.filter({$0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH") >=  "21" ||  $0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH") < "5"}))
                            ]
                        }
                        
                        
                        self.precipitationWeatherDetails?.forEach{data in
                            if(data.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH") == self.currentTime){
                                
                                let weatherOutputData = self.getWeatherclarity(psoWeather: psoWeather, pshPrecipitation: (data.PrecipitationAmountInMillimeter ?? 0.0)!, range: self.dayRanges)
                                
                                self.WeatherStatusImageView.image = weatherOutputData.1
                                
                                self.weatherClarityLabel.text = "\(weatherOutputData.0)"
                            }
                            
                        }
                        
                    }
                }
                
                
                
                self.morningWeatherDetails = self.todayWeatherDetails!.filter({$0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH") >= "06" && $0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH") <= "10"})
                
                self.noonWeatherDetails = self.todayWeatherDetails!.filter({$0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH") >= "11" && $0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH") <= "15"})
                
                self.eveningWeatherDetails = self.todayWeatherDetails!.filter({$0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH") >= "16" && $0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH") <= "20"})
                
                self.nightWeatherDetails = self.todayWeatherDetails!.filter({($0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH") >= "21" && $0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH") <= "23") || ($0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH") == "00")})
                
                self.hourlyWeatherTableViewData = [
                    hourlyForecastData(opened: false, title: "MORGENS", hourlyWeatherDetails: self.morningWeatherDetails),
                    hourlyForecastData(opened: false, title: "MITTAGS", hourlyWeatherDetails: self.noonWeatherDetails),
                    hourlyForecastData(opened: false, title: "ABENDS", hourlyWeatherDetails: self.eveningWeatherDetails),
                    hourlyForecastData(opened: false, title: "NACHTS", hourlyWeatherDetails: self.nightWeatherDetails)]
                
                self.hourlyWeatherTableView.reloadData()
                
            }
            self.removeSpinner()
            
        }
        
        NetworkManager().requestDataAsync(type: ForecastModel.self, APP_URL.Forecast_URL + "tomorrowForecast") {data in
            if !data.success{
                self.ShowAlert(message: "Cannot load tomorrow data")
                self.removeSpinner()
                return
            }
            self.tomorrowWeatherDetails = (data.result as! ForecastModel)
            self.removeSpinner()
        }
        
        
    }
    
    func configureCombinedChart(){
        maxColorView.backgroundColor = UIColor.init(displayP3Red: 221/255, green: 184/255, blue: 99/255, alpha: 1)
        maxTempLabel.text = "Max. Temp. (°C)"
        minTempColorView.backgroundColor = UIColor.init(displayP3Red: 77/255, green: 130/255, blue: 137/255, alpha: 1)
        minTempLabel.text = "Min. Temp. (°C)"
        niederschlagsmengeColorView.backgroundColor = UIColor.init(displayP3Red: 176/255, green: 225/255, blue: 255/255, alpha: 1)
        niederschlagsmengeLabel.text = "Niederschlagsmenge"
        dayTrendChartView.isUserInteractionEnabled = false
        let legend = dayTrendChartView.legend
        legend.enabled = false
        legend.horizontalAlignment = .right
        legend.verticalAlignment = .top
        legend.orientation = .vertical
        legend.drawInside = true
        legend.yOffset = 10.0;
        legend.xOffset = 10.0;
        legend.yEntrySpace = 0.0;
        
        
        let xaxis = dayTrendChartView.xAxis
        //  xaxis.valueFormatter = axisFormatDelegate
        xaxis.spaceMin = 0.5
        xaxis.spaceMax = 0.5
        xaxis.drawGridLinesEnabled = false
        xaxis.labelPosition = .top
        xaxis.drawLabelsEnabled = true
        xaxis.valueFormatter = IndexAxisValueFormatter(values:self.days)
        xaxis.granularity = 1
        xaxis.granularityEnabled = true
        xaxis.labelCount = 8
        
        
        let leftAxisFormatter = NumberFormatter()
        leftAxisFormatter.maximumFractionDigits = 10
        
        let rightAxisFormatter = NumberFormatter()
        rightAxisFormatter.maximumFractionDigits = 10
        
        let yaxis = dayTrendChartView.leftAxis
        yaxis.spaceTop = 0.35
        yaxis.axisMinimum = 0
        yaxis.axisMaximum = 30
        yaxis.labelCount = 6
        yaxis.drawGridLinesEnabled = true
        yaxis.drawLabelsEnabled = true
        yaxis.valueFormatter = YAxisValueFormatter()
        
        
        let rightAxis = dayTrendChartView.rightAxis
        rightAxis.spaceTop = 0.35
        rightAxis.axisMinimum = 0
        rightAxis.axisMaximum = 15
        rightAxis.labelCount = 3
        rightAxis.drawGridLinesEnabled = false
        rightAxis.drawLabelsEnabled = true
        rightAxis.valueFormatter = RightAxisValueFormatter()
        
        var barChartEntries = [BarChartDataEntry]()
        var minTempLineChartEntries = [ChartDataEntry]()
        var maxTempLineChartEntries = [ChartDataEntry]()
        
        
        var i = 0
        for x in self.dailyForecastDetails!.Forecasts{
            barChartEntries.append(BarChartDataEntry(x: Double(i), y: Double(x.PrecipitationAmountInMillimeter!)))
            minTempLineChartEntries.append(ChartDataEntry(x: Double(i), y: Double(x.MinAirTemperatureInCelsius!)))
            maxTempLineChartEntries.append(ChartDataEntry(x: Double(i), y: Double(x.MaxAirTemperatureInCelsius!)))
            i+=1
        }
        
        let barChartSet = BarChartDataSet(entries: barChartEntries, label: "Niederschlagsmenge")
        
        barChartSet.colors = [UIColor.init(displayP3Red: 176/255, green: 225/255, blue: 255/255, alpha: 1)]
        barChartSet.drawValuesEnabled = false
        
        let minTempLineChartSet = LineChartDataSet(entries: minTempLineChartEntries, label: "Min. Temp. (°C)")
        minTempLineChartSet.colors = [UIColor.init(displayP3Red: 77/255, green: 130/255, blue: 137/255, alpha: 1)]
        minTempLineChartSet.drawValuesEnabled = false
        minTempLineChartSet.drawCirclesEnabled = false
        minTempLineChartSet.mode = .cubicBezier
        
        let maxTempLineChartSet = LineChartDataSet(entries: maxTempLineChartEntries, label: "Max. Temp. (°C)")
        maxTempLineChartSet.colors = [UIColor.init(displayP3Red: 221/255, green: 184/255, blue: 99/255, alpha: 1)]
        maxTempLineChartSet.drawValuesEnabled = false
        maxTempLineChartSet.drawCirclesEnabled = false
        maxTempLineChartSet.mode = .cubicBezier
        
        
        let combinedData = CombinedChartData()
        combinedData.barData = BarChartData(dataSet: barChartSet)
        combinedData.lineData = LineChartData(dataSets: [maxTempLineChartSet, minTempLineChartSet])
        dayTrendChartView.data = combinedData
        
    }
    
    func configureBarChart(){
        sonnenscheinColorView.backgroundColor = UIColor.init(displayP3Red: 253/255, green: 192/255, blue: 0/255, alpha: 1)
        sonnenscheinLabel.text = "Sonnenschein (h)"
        dailyTemperatureChartView.isUserInteractionEnabled = false
        let legend = dailyTemperatureChartView.legend
        legend.enabled = false
        legend.horizontalAlignment = .right
        legend.verticalAlignment = .top
        legend.orientation = .vertical
        legend.drawInside = true
        legend.yOffset = 10.0;
        legend.xOffset = 10.0;
        legend.yEntrySpace = 0.0;
        
        
        let xaxis = dailyTemperatureChartView.xAxis
        //xaxis.valueFormatter = axisFormatDelegate
        xaxis.drawGridLinesEnabled = false
        xaxis.labelPosition = .bottom
        xaxis.drawLabelsEnabled = true
        xaxis.valueFormatter = IndexAxisValueFormatter(values:self.dates)
        xaxis.granularity = 1
        xaxis.labelCount = dates.count
        
        let leftAxisFormatter = NumberFormatter()
        leftAxisFormatter.maximumFractionDigits = 1
        
        let yaxis = dailyTemperatureChartView.leftAxis
        yaxis.spaceTop = 0.35
        yaxis.axisMinimum = 0
        yaxis.axisMaximum = 16
        yaxis.labelCount = 7
        yaxis.drawGridLinesEnabled = true
        yaxis.drawLabelsEnabled = true
        dailyTemperatureChartView.rightAxis.enabled = false
        yaxis.valueFormatter = YAxisValueFormatterForSunshine()
        
        var barChartEntries = [BarChartDataEntry]()
        
        var i = 0
        for x in self.dailyForecastDetails!.Forecasts{
            barChartEntries.append(BarChartDataEntry(x: Double(i), y: Double(x.SunshineDurationInMinutes!/60)))
            
            i+=1
        }
        
        let barChartSet = BarChartDataSet(entries: barChartEntries, label: "Sonnenschein (h)")
        
        barChartSet.colors = [UIColor.init(displayP3Red: 253/255, green: 192/255, blue: 0/255, alpha: 1)]
        barChartSet.drawValuesEnabled = false
        
        
        let data = BarChartData(dataSet: barChartSet)
        
        dailyTemperatureChartView.data = data
        
        
    }
    
    func stringForValue(_ value: Double, axis: AxisBase?) -> String {
        return self.days[Int(value)]
    }
    
    func getCityNameFromZip(zipCode: String){
        
        geocoder.geocodeAddressString(String(zipCode.suffix(5))) {
            (placemarks, error) -> Void in
            
            if let error = error{
                self.cityLabel.text = "\(zipCode)"
            }
            
            if let placemark = placemarks?[0] {
                
                self.cityLabel.text = placemark.locality!
                
            }else{
                self.cityLabel.text = "\(zipCode)"
            }
            
        }
        
    }
    
    func isDayLight(time: String)-> Bool{
        return time > "06" && time <= "19"
    }
    
    func  setWindStrength(windSpeedInBeaufort: Int)-> String {
        var windStrength = " "
        if (windSpeedInBeaufort == 0) {
            windStrength = "Windstille"
        } else if (windSpeedInBeaufort == 1) {
            windStrength = "Leiser Zug"
        } else if (windSpeedInBeaufort == 2) {
            windStrength = "leichte Brise"
        } else if (windSpeedInBeaufort == 3) {
            windStrength = "Schwache Brise"
        } else if (windSpeedInBeaufort == 4) {
            windStrength = "Mäßige Brise"
        } else if (windSpeedInBeaufort == 5) {
            windStrength = "Frische Brise"
        } else if (windSpeedInBeaufort == 6) {
            windStrength = "Starker Wind"
        } else if (windSpeedInBeaufort == 7) {
            windStrength = "Steifer Wind"
        } else if (windSpeedInBeaufort == 8) {
            windStrength = "Stürmischer Wind"
        } else if (windSpeedInBeaufort == 9) {
            windStrength = "Sturm"
        } else if (windSpeedInBeaufort == 10) {
            windStrength = "Schwerer Sturm"
        } else if (windSpeedInBeaufort == 11) {
            windStrength = "Orkanartiger Sturm"
        } else if (windSpeedInBeaufort == 12) {
            windStrength = "Orkan"
        }
        return windStrength
    }
    
    func  setWindOrigin(windDirection: Int) -> String {
        
        var windOrigin = ""
        if (windDirection >= 0 && windDirection <= 45) {
            windOrigin = "N"
        } else if (windDirection > 45 && windDirection <= 90) {
            windOrigin = "O"
        } else if (windDirection > 90 && windDirection <= 135) {
            windOrigin = "SO"
        } else if (windDirection > 135 && windDirection <= 180) {
            windOrigin = "S"
        } else if (windDirection > 180 && windDirection <= 225) {
            windOrigin = "SW"
        } else if (windDirection > 225 && windDirection <= 270) {
            windOrigin = "W"
        } else if (windDirection > 270 && windDirection <= 315) {
            windOrigin = "NW"
        } else if (windDirection > 315 && windDirection <= 360) {
            windOrigin = "N"
        }
        return windOrigin
    }
    
    func getWeatherclarity(psoWeather: Weather, pshPrecipitation: Float, range: [dayRangeForecastData])-> (String, UIImage){
        var weatherClarity = ""
        var weatherImage = UIImage()
        var precipitationAmount = getMax(a: pshPrecipitation, b: psoWeather.PrecipitationAmountInMillimeter! ?? 0.0)
        var rangeMax = range.reduce(into: Float()){output, input in
            
            var precipitationValues = input.forcast?.map({$0.PrecipitationAmountInMillimeter!})
            var sum = 0.0
            precipitationValues?.forEach{data in
                sum += Double(data!)
            }
            output += Float(sum)
            
        }
        if(self.getPrecipitationLevel(precipitationAmount: precipitationAmount, rangeMax: rangeMax)==1){
            switch self.windLevel(value: psoWeather.WindSpeedInBeaufort!) {
            case 0:
                weatherClarity = "leichter Regen"
                if isDayLight(time: psoWeather.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH")){
//                    self.WeatherStatusImageView.image = UIImage(named: "Sonne_mit_Regen_Stufe_1")
                    weatherImage = UIImage(named: "Sonne_mit_Regen_Stufe_1")!
                }else{
//                    self.WeatherStatusImageView.image = UIImage(named: "Mond_mit_Regen_Stufe_1")
                    weatherImage = UIImage(named: "Mond_mit_Regen_Stufe_1")!

                }
                
            case 1:
                weatherClarity = "leichter Regen, Wind"
//                self.WeatherStatusImageView.image = UIImage(named: "Wind_mit_Regen_Stufe_1")
                weatherImage = UIImage(named: "Wind_mit_Regen_Stufe_1")!
                
            case 2:
                weatherClarity = "starker Wind, Regen"
                
//                self.WeatherStatusImageView.image = UIImage(named: "Wind_mit_Regen_Stufe_2")
                weatherImage = UIImage(named: "Wind_mit_Regen_Stufe_2")!

            default:
                break
            }
        }else if(self.getPrecipitationLevel(precipitationAmount: precipitationAmount, rangeMax: rangeMax)==2){
            switch self.windLevel(value: psoWeather.WindSpeedInBeaufort!) {
            case 0:
                weatherClarity = "starker Regen"
                if isDayLight(time: psoWeather.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH")){
//                    self.WeatherStatusImageView.image = UIImage(named: "Sonne_mit_Regen_Stufe_2")
                    weatherImage = UIImage(named: "Sonne_mit_Regen_Stufe_2")!

                }else{
//                    self.WeatherStatusImageView.image = UIImage(named: "Mond_mit_Regen_Stufe_2")
                    weatherImage = UIImage(named: "Mond_mit_Regen_Stufe_2")!

                }
                
            case 1:
                weatherClarity = "starker Regen, Wind"
//                self.WeatherStatusImageView.image = UIImage(named: "Wind_mit_Regen_Stufe_1")
                weatherImage = UIImage(named: "Wind_mit_Regen_Stufe_1")!

                
            case 2:
                weatherClarity = "starker Wind, Regen"
                
//                self.WeatherStatusImageView.image = UIImage(named: "Wind_mit_Regen_Stufe_2")
                weatherImage = UIImage(named: "Wind_mit_Regen_Stufe_2")!

            default:
                break
            }
        }else if self.getCloudCoverLevel(cloudCoverLevel: psoWeather.CloudCoverLowerThan2000MeterInOcta!) == 0{
            switch self.windLevel(value: psoWeather.WindSpeedInBeaufort!) {
            case 0:
                weatherClarity = "klar"
                if isDayLight(time: psoWeather.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH")){
//                    self.WeatherStatusImageView.image = UIImage(named: "Sonne")
                    weatherImage = UIImage(named: "Sonne")!

                }else{
//                    self.WeatherStatusImageView.image = UIImage(named: "Mond")
                    weatherImage = UIImage(named: "Mond")!

                }
                
            case 1:
                weatherClarity = "leichter Wind"
                if isDayLight(time: psoWeather.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH")){
//                    self.WeatherStatusImageView.image = UIImage(named: "Wind_mit_Sonne_Stufe_1")
                    weatherImage = UIImage(named: "Wind_mit_Sonne_Stufe_1")!

                }else{
//                    self.WeatherStatusImageView.image = UIImage(named: "Wind_mit_Mond_Stufe_1")
                    weatherImage = UIImage(named: "Wind_mit_Mond_Stufe_1")!

                }
                
            case 2:
                weatherClarity = "starker Wind"
                if isDayLight(time: psoWeather.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH")){
//                    self.WeatherStatusImageView.image = UIImage(named: "Wind_mit_Sonne_Stufe_1")
                    weatherImage = UIImage(named: "Wind_mit_Sonne_Stufe_1")!

                }else{
//                    self.WeatherStatusImageView.image = UIImage(named: "Wind_mit_Mond_Stufe_1")
                    weatherImage = UIImage(named: "Wind_mit_Mond_Stufe_1")!

                }
            default:
                break
            }
            
        }else if self.getCloudCoverLevel(cloudCoverLevel: psoWeather.CloudCoverLowerThan2000MeterInOcta!) == 1{
            if(self.getPrecipitationLevel(precipitationAmount: precipitationAmount, rangeMax: rangeMax)==0){
                switch self.windLevel(value: psoWeather.WindSpeedInBeaufort!) {
                case 0:
                    weatherClarity = "leicht bewölkt"
                    if isDayLight(time: psoWeather.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH")){
//                        self.WeatherStatusImageView.image = UIImage(named: "Wolke_mit_Sonne")
                        weatherImage = UIImage(named: "Wolke_mit_Sonne")!

                    }else{
//                        self.WeatherStatusImageView.image = UIImage(named: "Wolke_mit_Mond")
                        weatherImage = UIImage(named: "Wolke_mit_Mond")!

                    }
                    
                case 1:
                    weatherClarity = "leicht bewölkt"
                    if isDayLight(time: psoWeather.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH")){
//                        self.WeatherStatusImageView.image = UIImage(named: "Wind_mit_Sonne_Stufe_1")
                        weatherImage = UIImage(named: "Wind_mit_Sonne_Stufe_1")!

                    }else{
//                        self.WeatherStatusImageView.image = UIImage(named: "Wind_mit_Mond_Stufe_1")
                        weatherImage = UIImage(named: "Wind_mit_Mond_Stufe_1")!

                    }
                    
                case 2:
                    weatherClarity = "starker Wind"
                    if isDayLight(time: psoWeather.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH")){
//                        self.WeatherStatusImageView.image = UIImage(named: "Wind_mit_Sonne_Stufe_2")
                        weatherImage = UIImage(named: "Wind_mit_Sonne_Stufe_2")!

                    }else{
//                        self.WeatherStatusImageView.image = UIImage(named: "Wind_mit_Mond_Stufe_2")
                        weatherImage = UIImage(named: "Wind_mit_Mond_Stufe_2")!

                    }
                default:
                    break
                }
            }
            
        }else if self.getCloudCoverLevel(cloudCoverLevel: psoWeather.CloudCoverLowerThan2000MeterInOcta!) == 2{
            
            if(self.getPrecipitationLevel(precipitationAmount: precipitationAmount, rangeMax: rangeMax)==0){
                switch self.windLevel(value: psoWeather.WindSpeedInBeaufort!) {
                case 0:
                    weatherClarity = "stark bewölkt"
//                    self.WeatherStatusImageView.image = UIImage(named: "Wolken")
                    weatherImage = UIImage(named: "Wolken")!

                    
                case 1:
                    weatherClarity = "stark bewölkt, Wind"
//                    self.WeatherStatusImageView.image = UIImage(named: "Wolken")
                    weatherImage = UIImage(named: "Wolken")!

                    
                case 2:
                    weatherClarity = "stark bewölkt & Wind"
                    
//                    self.WeatherStatusImageView.image = UIImage(named: "Wind_mit_Regen_Stufe_2")
                    weatherImage = UIImage(named: "Wind_mit_Regen_Stufe_2")!

                default:
                    break
                }
                
            }
            else if(self.getPrecipitationLevel(precipitationAmount: precipitationAmount, rangeMax: rangeMax)==1){
                switch self.windLevel(value: psoWeather.WindSpeedInBeaufort!) {
                case 0:
                    weatherClarity = "Regen"
//                    self.WeatherStatusImageView.image = UIImage(named: "Regen_Stufe_1")
                    weatherImage = UIImage(named: "Regen_Stufe_1")!

                    
                case 1:
                    weatherClarity = "Regen & Wind"
//                    self.WeatherStatusImageView.image = UIImage(named: "Wind_mit_Regen_Stufe_1")
                    weatherImage = UIImage(named: "Wind_mit_Regen_Stufe_1")!

                    
                case 2:
                    weatherClarity = "starker Wind, Regen"
                    
//                    self.WeatherStatusImageView.image = UIImage(named: "Wind_mit_Regen_Stufe_2")
                    weatherImage = UIImage(named: "Wind_mit_Regen_Stufe_2")!

                default:
                    break
                }
                
            }
            else if(self.getPrecipitationLevel(precipitationAmount: precipitationAmount, rangeMax: rangeMax)==2){
                switch self.windLevel(value: psoWeather.WindSpeedInBeaufort!) {
                case 0:
                    weatherClarity = "starker Regen"
//                    self.WeatherStatusImageView.image = UIImage(named: "Regen_Stufe_2")
                    weatherImage = UIImage(named: "Regen_Stufe_2")!

                    
                case 1:
                    weatherClarity = "heftiger Regen"
//                    self.WeatherStatusImageView.image = UIImage(named: "Wind_mit_Gewitter_mit_Regen_Stufe_1")
                    weatherImage = UIImage(named: "Wind_mit_Gewitter_mit_Regen_Stufe_1")!

                    
                case 2:
                    weatherClarity = "stürmisch Regen"
                    
//                    self.WeatherStatusImageView.image = UIImage(named: "Wind_mit_Gewitter_mit_Regen_Stufe_2")
                    weatherImage = UIImage(named: "Wind_mit_Gewitter_mit_Regen_Stufe_2")!

                default:
                    break
                }
                
            }
            
            
            
        }
        
        return (weatherClarity, weatherImage)
    }
    
    func getPrecipitationLevel(precipitationAmount: Float, rangeMax: Float) -> Int {
        
        if (rangeMax > 0) {
            var maxPrecipitationAmount = Double(self.getMax(a: precipitationAmount, b: rangeMax))
            
            if (maxPrecipitationAmount <= 1) { return 0; }
            if (1 < maxPrecipitationAmount && maxPrecipitationAmount <= 5) { return 1; }
        }
        if precipitationAmount <= 1{
            return 0
        }
        if (1 < precipitationAmount && precipitationAmount <= 5){
            return 1
        }
        return 2;
    }
    
    func getMax(a: Float, b: Float)-> Float{
        if(a>b){
            return a
        }else{
            return b
        }
    }
    
    func getCloudCoverLevel(cloudCoverLevel: Float)-> Int{
        if (cloudCoverLevel < 1.5){
            return 0
        }else if(cloudCoverLevel >= 1.5 && cloudCoverLevel <= 5.0){
            return 1
        }else{
            return 2
        }
    }
    
    func windLevel(value: Int)-> Int{
        if(value < 5){
            return 0
        }else if (value >= 5 && value <= 6){
            return 1
        }else{
            return 2
        }
    }
    
    func numberOfSections(in tableView: UITableView) -> Int {
        return self.hourlyWeatherTableViewData.count
    }
    
    func tableView(_ tableView: UITableView, heightForHeaderInSection section: Int) -> CGFloat {
        return 0
    }
    
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        if( self.hourlyWeatherTableViewData[section].opened == true){
            return self.hourlyWeatherTableViewData[section].hourlyWeatherDetails!.count+1
        }else {
            return 1
        }
    }
    
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        
        if(indexPath.row == 0){
            let cell = tableView.dequeueReusableCell(withIdentifier: "hourlyForecastHeaderCell", for: indexPath) as! WeatherUpdateViewCell
            cell.delegate = self
            cell.headerView.backgroundColor = .systemBackground
            cell.configureHeader(headerName: self.hourlyWeatherTableViewData[indexPath.section].title)
            cell.separatorInset = .zero
            if self.hourlyWeatherTableViewData[indexPath.section].opened == false{
                cell.arrowImageView.revertFlip()
                cell.headerView.addCustomBorderRadius(topLeft: true, topRight: true, botLeft: true, botRight: true)
            }else{
                cell.headerView.addCustomBorderRadius(topLeft: true, topRight: true, botLeft: false, botRight: false)
                cell.arrowImageView.flipXAxis()
            }
            return cell
        }
        else{
            let cell = tableView.dequeueReusableCell(withIdentifier: "weatherUpdateTableViewCell", for: indexPath) as! WeatherUpdateViewCell
            cell.delegate = self
            
            cell.clearBackground()
            cell.addBorderRadius()
            cell.temperatureImageView.image = UIImage(named: "Luftfeuchte")
            cell.precipitationImageView.image = UIImage(named: "Regenwahrscheinlichkeit_Niederschlag")
            cell.windDirectionImageView.image = UIImage(named: "Wind")
            cell.airPressureImageView.image = UIImage(named: "Luftdruck")
            cell.ConfigureCell(todayWeather: (self.hourlyWeatherTableViewData[indexPath.section].hourlyWeatherDetails?[indexPath.row-1])!)
            cell.backgroundColor = .systemBackground
            cell.addCustomBorderRadius(topLeft: false, topRight: false, botLeft: true, botRight: true)
            cell.clearBorderRadius()
            
            let currentData = self.hourlyWeatherTableViewData[indexPath.section].hourlyWeatherDetails?[indexPath.row-1]
            
            self.precipitationWeatherDetails?.forEach{data in
                if(data.ValidUntil!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH") == currentData?.ValidUntil!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "HH")){
                    
                    let weatherOutputData = self.getWeatherclarity(psoWeather: currentData!, pshPrecipitation: (data.PrecipitationAmountInMillimeter ?? 0.0)!, range: self.dayRanges)
                    
                    cell.temperatureImageView.image = weatherOutputData.1
                    print("success", currentData?.ValidUntil, data.ValidUntil)

                }
                else{
                    print("failed", currentData?.ValidUntil, data.ValidUntil)
                }
                
            }
            

            
            cell.horizontalSeparatorView.alpha = 1
            if self.hourlyWeatherTableViewData[indexPath.section].hourlyWeatherDetails!.count <= indexPath.row{
                print("index is,", indexPath.row, self.hourlyWeatherTableViewData[indexPath.section].hourlyWeatherDetails!.count)

                cell.isLast()

            }
            return cell
        }
        return UITableViewCell()
    }
    
    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        if(indexPath.row == 0){
            if self.hourlyWeatherTableViewData[indexPath.section].opened == true{
                self.hourlyWeatherTableViewData[indexPath.section].opened = false
                let sections = IndexSet.init(integer: indexPath.section)
                self.hourlyWeatherTableView.reloadSections(sections, with: .none)
            }else{
                self.hourlyWeatherTableViewData[indexPath.section].opened = true
                let sections = IndexSet.init(integer: indexPath.section)
                self.hourlyWeatherTableView.reloadSections(sections, with: .none)
            }
        }
    }
    
    func collectionView(_ collectionView: UICollectionView, numberOfItemsInSection section: Int) -> Int {
        return self.dailyForecastDetails?.Forecasts.count ?? 0
    }
    func collectionView(_ collectionView: UICollectionView, layout collectionViewLayout: UICollectionViewLayout,minimumLineSpacingForSectionAt section: Int) -> CGFloat {
        return 15
        
    }
    
    func collectionView(_ collectionView: UICollectionView, cellForItemAt indexPath: IndexPath) -> UICollectionViewCell {
        let cell = collectionView.dequeueReusableCell(withReuseIdentifier: "dailyWeather", for: indexPath) as! DailyWeatherViewCell
        cell.delegate = self
        cell.ConfigureCellDailyWeather(dailyWeather: (self.dailyForecastDetails?.Forecasts[indexPath.row])!)
        return cell
    }
}

class YAxisValueFormatter: IAxisValueFormatter {
    
    func stringForValue(_ value: Double, axis: AxisBase?) -> String {
        return String(Int(value)) + " °C"
    }
}
class RightAxisValueFormatter: IAxisValueFormatter {
    func stringForValue(_ value: Double, axis: AxisBase?) -> String {
        return String(Int(value)) + "  l/m2"
    }
}

class YAxisValueFormatterForSunshine: IAxisValueFormatter {
    
    func stringForValue(_ value: Double, axis: AxisBase?) -> String {
        return String(Int(value)) + "H"
    }
}
