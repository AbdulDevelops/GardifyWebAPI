//
//  WeatherUpdateViewCell.swift
//  GardifyIOS
//
//  Created by Rifat Hussain on 17.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit
class WeatherUpdateViewCell: UITableViewCell {
    
    var delegate: WeatherUpdateViewController?
    
    @IBOutlet weak var headerView: UIView!
    @IBOutlet weak var timeHeaderLabel: UILabel!
    @IBOutlet weak var timeLabel: UILabel!
    @IBOutlet weak var temperatureImageView: UIImageView!
    @IBOutlet weak var temperatureLabel: UILabel!
    @IBOutlet weak var precipitationImageView: UIImageView!
    @IBOutlet weak var precipitationLabel: UILabel!
    @IBOutlet weak var windDirectionImageView: UIImageView!
    @IBOutlet weak var windDirectionLabel: UILabel!
    @IBOutlet weak var airPressureImageView: UIImageView!
    @IBOutlet weak var airPressureLabel: UILabel!
    @IBOutlet weak var horizontalSeparatorView: UIView!
    @IBOutlet weak var arrowImageView: UIImageView!
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
        
        
    }
    
    func configureHeader(headerName: String){
        if delegate == nil {
            
            return
        }
        headerView.addBorderRadius()
        timeHeaderLabel.text = "\(headerName)"
    }
    

    func isLast(){
        self.addBorderRadius()
        horizontalSeparatorView.alpha = 0
    }
    
    func ConfigureCell(todayWeather: Weather){
        if delegate == nil {
            
            return
        }
        let rawDate = todayWeather.ValidFrom?.toDate("yyyy-MM-dd'T'HH:mm:ssZ")
        
        let hour = rawDate?.addingTimeInterval(-3600).toString( output: "HH:00")
        
      
        timeLabel.text = "\(hour!)"
        
        var floatTemp = todayWeather.AirTemperatureInCelsius
        temperatureLabel.text = "\(Int(floatTemp!.rounded()))°C"
        precipitationLabel.text = "\(todayWeather.PrecipitationProbabilityInPercent!)%"
        windDirectionLabel.text = "\(delegate!.setWindStrength(windSpeedInBeaufort: todayWeather.WindSpeedInBeaufort!)) aus \(delegate!.setWindOrigin(windDirection: todayWeather.WindDirectionInDegree!)) (\(todayWeather.WindSpeedInKilometerPerHour!) km/h)"
        airPressureLabel.text = "Luftdruck \(todayWeather.AirPressureAtSeaLevelInHectoPascal!) hPa"
    horizontalSeparatorView.backgroundColor = UIColor.gray
    }
    
}

