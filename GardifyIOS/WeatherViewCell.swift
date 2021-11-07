//
//  WeatherViewCell.swift
//  GardifyIOS
//
//  Created by Rifat Hussain on 05.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//



import UIKit

class WeatherViewCell: UITableViewCell {
    var delegate: WeatherViewController?
    public let dateFormatter = DateFormatter()
    @IBOutlet weak var hourLabel: UILabel!
    @IBOutlet weak var weatherImage: UIImageView!
    @IBOutlet weak var temperatureLabel: UILabel!
    @IBOutlet weak var precipitationLabel: UILabel!
    @IBOutlet weak var windDirectionLabel: UILabel!
    @IBOutlet weak var weekDayLabel: UILabel!
    @IBOutlet weak var dailyDateLabel: UILabel!
    @IBOutlet weak var minimunTempratureLabel: UILabel!
    @IBOutlet weak var maximumTemperatureLabel: UILabel!
    var today: String?
    var tomorrow: String?
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }
    
    func ConfigureCell(todayWeather: Weather){
        if delegate == nil {
            
            return
        }
        let hour = todayWeather.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:00:00'+'02:00", output: "HH:00")
        hourLabel.text = "\(hour)"
        temperatureLabel.text = "\(todayWeather.AirTemperatureInCelsius!)%"
        windDirectionLabel.text = "\(delegate!.setWindStrength(windSpeedInBeaufort: todayWeather.WindSpeedInBeaufort!)) aus \(delegate!.setWindOrigin(windDirection: todayWeather.WindDirectionInDegree!))"

    }
    
    func ConfigureCellDailyWeather(dailyWeather: DailyWeather){
        if delegate == nil {
            
            return
        }
        
        let todayDate = Date()
        let now = Calendar.current.dateComponents(in: .current, from: Date())

        let tomorrow = DateComponents(year: now.year, month: now.month, day: now.day! + 1)
        let dateTomorrow = Calendar.current.date(from: tomorrow)!

        
        dateFormatter.dateFormat = "yyyy-MM-dd"
        self.today = dateFormatter.string(from: todayDate)
        self.tomorrow = dateFormatter.string(from: dateTomorrow)
        
        dateFormatter.setLocalizedDateFormatFromTemplate("EEEE")
        
        if dailyWeather.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:00:00'+'02:00", output: "yyyy-MM-dd") ==
            self.today{
            weekDayLabel.text = "Today"
        }
        else if dailyWeather.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:00:00'+'02:00", output: "yyyy-MM-dd") == self.tomorrow{
            weekDayLabel.text = "Tomorrow"
        }else{
            weekDayLabel.text =  "\(dailyWeather.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:00:00'+'02:00", output: "EEEE"))"
        }
        

        dailyDateLabel.text = "\(dailyWeather.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:00:00'+'02:00", output: "MM-dd"))"
        minimunTempratureLabel.text = "\(dailyWeather.MinAirTemperatureInCelsius!) °C"
        maximumTemperatureLabel.text = "\(dailyWeather.MaxAirTemperatureInCelsius!) °C"

    }
    

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}
