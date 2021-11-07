//
//  DailyWeatherViewCell.swift
//  GardifyIOS
//
//  Created by Rifat Hussain on 19.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class DailyWeatherViewCell: UICollectionViewCell {
    var delegate: WeatherUpdateViewController?
    
    @IBOutlet weak var dailyWeatherContainer: UIView!
    @IBOutlet weak var dayLabel: UILabel!
    @IBOutlet weak var timeLabel: UILabel!
    @IBOutlet weak var minTemperatureLabel: UILabel!
    @IBOutlet weak var horizontalSeparatorView: UIView!
    @IBOutlet weak var maxTemperatureLabel: UILabel!
    
    public let dateFormatter = DateFormatter()
    var today: String?
    var tomorrow: String?
    
    override func awakeFromNib() {
        super.awakeFromNib()
    }
    
    func ConfigureCellDailyWeather(dailyWeather: DailyWeather){
        
        if delegate == nil {
            
            return
        }
        dailyWeatherContainer.addBorderRadius()
        horizontalSeparatorView.backgroundColor = UIColor.gray

        let todayDate = Date()
        let now = Calendar.current.dateComponents(in: .current, from: Date())

        let tomorrow = DateComponents(year: now.year, month: now.month, day: now.day! + 1)
        let dateTomorrow = Calendar.current.date(from: tomorrow)!

        
        dateFormatter.dateFormat = "yyyy-MM-dd"
        self.today = dateFormatter.string(from: todayDate)
        self.tomorrow = dateFormatter.string(from: dateTomorrow)
        
        dateFormatter.setLocalizedDateFormatFromTemplate("EEEE")
        
        if dailyWeather.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "yyyy-MM-dd") ==
            self.today{
            dayLabel.text = "Heute"
        }
        else if dailyWeather.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "yyyy-MM-dd") == self.tomorrow{
            dayLabel.text = "Morgen"
        }else{
            var dateValue = dailyWeather.ValidFrom!.toDate("yyyy-MM-dd'T'HH:mm:ssZ")
            dayLabel.text = String.getDayStringFull(day: dateValue?.dayNumberOfWeek() ?? 0)
        }
        
        timeLabel.text = "\(dailyWeather.ValidFrom!.toDateString("yyyy-MM-dd'T'HH:mm:ssZ", output: "dd.MM"))"
        minTemperatureLabel.text = "\(dailyWeather.MinAirTemperatureInCelsius!) °C"
        maxTemperatureLabel.text = "\(dailyWeather.MaxAirTemperatureInCelsius!) °C"
        

    }
    

    
}
