import Foundation
import UIKit

class WeatherViewController: UIViewController, UITableViewDelegate, UITableViewDataSource {
    
    @IBOutlet weak var weatherScrollView: UIScrollView!
   // @IBOutlet weak var dailyWeatherTableView: UITableView!
    @IBOutlet weak var hourlyWeatherDataTableView: UITableView!
    @IBOutlet weak var cityLabel: UILabel!
    @IBOutlet weak var currentTemperatureLabel: UILabel!
    @IBOutlet weak var weatherStatusLabel: UILabel!
    @IBOutlet weak var windDirectionLabel: UILabel!
    @IBOutlet weak var windSpeedLabel: UILabel!
    @IBOutlet weak var humidityLabel: UILabel!
    @IBOutlet weak var airPressureLabel: UILabel!
    @IBOutlet weak var noonWeatherButton: UIButton!
    @IBOutlet weak var afterNoonWeatherButton: UIButton!
    @IBOutlet weak var nightWeatherButton: UIButton!
    @IBOutlet weak var tomorrowsWeatherButton: UIButton!
    @IBOutlet weak var hourlyForecastLabel: UILabel!
    @IBOutlet weak var dailyForecastButton: UIButton!
    
    var todayWeatherDetails: [Weather]?
    var currentWeatherDetails: [Weather]?
    var tomorrowWeatherDetails: ForecastModel?
    var dailyForecastDetails: DailyForecastModel?
    var isDaily : Bool = false
    public var currentDateTime: String?
    public let dateFormatter = DateFormatter()
    
    override func viewDidAppear(_ animated: Bool) {
        hourlyForecastLabel.text = "Hourly Forecast"
        noonWeatherButton.backgroundColor = UIColor.gray
        noonWeatherButton.setTitle("Mittag", for: .normal)
        noonWeatherButton.setTitleColor(UIColor.white, for: .normal)
        afterNoonWeatherButton.setTitle("Abend", for: .normal)
        afterNoonWeatherButton.setTitleColor(UIColor.white, for: .normal)
        afterNoonWeatherButton.backgroundColor = UIColor.gray
        nightWeatherButton.setTitle("Nacht", for: .normal)
        nightWeatherButton.setTitleColor(UIColor.white, for: .normal)
        nightWeatherButton.backgroundColor = UIColor.gray
        tomorrowsWeatherButton.setTitle("Morgen", for: .normal)
        tomorrowsWeatherButton.backgroundColor = UIColor.gray
        tomorrowsWeatherButton.setTitleColor(UIColor.white, for: .normal)
        dailyForecastButton.setTitle("Morgens", for: .normal)
        dailyForecastButton.backgroundColor = UIColor.gray
        dailyForecastButton.setTitleColor(UIColor.white, for: .normal)
        
        let date = Date()
        
        dateFormatter.dateFormat = "HH"
        self.currentDateTime = dateFormatter.string(from: date)
        
        self.applyTheme()
        
    }
    override func viewDidLoad() {
        
        self.loadForcast()
        super.viewDidLoad()
        
      


        hourlyWeatherDataTableView.delegate = self
        hourlyWeatherDataTableView.dataSource = self

    }
    
    func loadForcast(){
        NetworkManager().requestDataAsync(type: ForecastModel.self, APP_URL.Forecast_URL + "todayForecast") {data in
            if !data.success{
                self.ShowAlert(message: "Cannot load today data")
                return
            }
            self.loadTomorrowForecast()
            self.loadDailyForecast()
            let todayData = (data.result as! ForecastModel)
            self.todayWeatherDetails = todayData.Forecasts.filter({$0.ValidPeriod=="PT0S"})
            
            self.todayWeatherDetails?.forEach{
                if($0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:00:00'+'02:00", output: "HH") == self.currentDateTime){
                    self.currentTemperatureLabel.text = "\($0.AirTemperatureInCelsius!) °C"
                    self.airPressureLabel.text = "Luftdruck \($0.AirPressureAtSeaLevelInHectoPascal!) hPa"
                    self.humidityLabel.text = "\($0.RelativeHumidityInPercent!)% Luftefeuchte"
                    self.windDirectionLabel.text = "\(self.setWindStrength(windSpeedInBeaufort: $0.WindSpeedInBeaufort!)) aus \(self.setWindOrigin(windDirection: $0.WindDirectionInDegree!))"
                    self.windSpeedLabel.text = "(\($0.WindSpeedInKilometerPerHour!)km/h)"
                }
            }
            
            self.currentWeatherDetails = self.todayWeatherDetails!.filter({$0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:00:00'+'02:00", output: "HH") >= self.currentDateTime ?? "" &&  $0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:00:00'+'02:00", output: "HH") <= "23"})
            self.hourlyWeatherDataTableView.reloadData()
            
        }
        
        
    }
    
    func loadTomorrowForecast(){
        NetworkManager().requestDataAsync(type: ForecastModel.self, APP_URL.Forecast_URL + "tomorrowForecast") {data in
            if !data.success{
                self.ShowAlert(message: "Cannot load tomorrow data")
                return
            }
            self.tomorrowWeatherDetails = (data.result as! ForecastModel)
            //self.weatherTableView.reloadData()
        }
    }
    
    func loadDailyForecast(){
        NetworkManager().requestDataAsync(type: DailyForecastModel.self, APP_URL.Forecast_URL + "daily") {data in
            
            if !data.success{
                self.ShowAlert(message: "Cannot load daily data")
                return
            }
            
            self.dailyForecastDetails = data.result as? DailyForecastModel
            self.cityLabel.text = self.dailyForecastDetails?.City
            self.hourlyWeatherDataTableView.reloadData()
        }

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
    
    @IBAction func clickMittagButton(_ sender: UIButton) {
        isDaily = false
        self.currentWeatherDetails = self.todayWeatherDetails!.filter({$0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:00:00'+'02:00", output: "HH") >= "11" && $0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:00:00'+'02:00", output: "HH") <= "15"})
      //  self.dailyWeatherTableView.isHidden = true
        self.hourlyWeatherDataTableView.isHidden = false
        self.hourlyWeatherDataTableView.reloadData()
    }
    
    @IBAction func clickAbendButton(_ sender: UIButton) {
        isDaily = false

        self.currentWeatherDetails = self.todayWeatherDetails!.filter({$0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:00:00'+'02:00", output: "HH") >= "16" && $0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:00:00'+'02:00", output: "HH") <= "19"})
       // self.dailyWeatherTableView.isHidden = true
        self.hourlyWeatherDataTableView.isHidden = false
        self.hourlyWeatherDataTableView.reloadData()
    }
    @IBAction func clickNightButton(_ sender: UIButton) {
        isDaily = false
        self.currentWeatherDetails = self.todayWeatherDetails!.filter({$0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:00:00'+'02:00", output: "HH") >= "20" && $0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:00:00'+'02:00", output: "HH") <= "23"})
       // self.dailyWeatherTableView.isHidden = true
        self.hourlyWeatherDataTableView.isHidden = false
        self.hourlyWeatherDataTableView.reloadData()
    }
    
    @IBAction func clickMorgenButton(_ sender: UIButton) {
        isDaily = false
        self.currentWeatherDetails = self.todayWeatherDetails!.filter({$0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:00:00'+'02:00", output: "HH") >= "06" && $0.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:00:00'+'02:00", output: "HH") <= "10"})
      //  self.dailyWeatherTableView.isHidden = true
        self.hourlyWeatherDataTableView.isHidden = false
        self.hourlyWeatherDataTableView.reloadData()
        
    }
    
    @IBAction func clickDailyForecastButton(_ sender: UIButton) {
       isDaily = true
        self.hourlyWeatherDataTableView.reloadData()
    }
    
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        if !isDaily{
            return self.currentWeatherDetails?.count ?? 0
        }
        if isDaily{
            return self.dailyForecastDetails?.Forecasts.count ?? 0
        }
        
         return 0
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        
        if !isDaily{
            let cell = tableView.dequeueReusableCell(withIdentifier: "hourlyWeatherViewCell", for: indexPath) as! WeatherViewCell
            cell.delegate = self
            cell.ConfigureCell(todayWeather: (self.currentWeatherDetails?[indexPath.row])!)
            return cell
        }
        if isDaily{
            let cell = tableView.dequeueReusableCell(withIdentifier: "dailyWeatherViewCell", for: indexPath) as! WeatherViewCell
            cell.delegate = self
            cell.ConfigureCellDailyWeather(dailyWeather: (self.dailyForecastDetails?.Forecasts[indexPath.row])!)
            return cell
        }
        return UITableViewCell()
    }
}
