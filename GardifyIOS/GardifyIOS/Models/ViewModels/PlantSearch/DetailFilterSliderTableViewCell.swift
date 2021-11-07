//
//  DetailFilterSliderTableViewCell.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 01.10.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit
import MultiSlider

class DetailFilterSliderTableViewCell: UITableViewCell {

    @IBOutlet weak var filterTitleLabel: UILabel!
    @IBOutlet weak var sliderInfoLabel: UILabel!
    
    @IBOutlet weak var filterSlider: MultiSlider!
    
    var isItMonth: Bool = true

    var rangeData: [Int] = []
    var sliderType: SliderType?
    var filterDelegate: FilterUpdateDelegate?
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }
    
    
    func sliderConfiguration(){
        //        multiSlider.valueLabelPosition = .top
        filterSlider.isContinuous = true
//        filterSlider.minimumValue = 1
//        filterSlider.maximumValue = 12
//        filterSlider.value = [1,5]
        switch sliderType {
        case .month:
            self.setMonthSlider()
        case .height:
            self.setHeightSlider()

        default:
            return
        }

        
        filterSlider.addTarget(self, action: #selector(sliderChanged(_:)), for: .valueChanged) // continuous changes

    }
    
    func getSliderRangeValue() -> [Int]{

        let minValue = filterSlider.value[0]
        let maxValue = filterSlider.value[1]
        
        return [Int(minValue), Int(maxValue)]
    }
    
    func updateRangeDataValue(){
        self.filterDelegate?.updateSliderValue(value: self.getSliderRangeValue(), sender: nil, type: sliderType!)
        
    }
    
    func setMonthSlider(){
        filterSlider.minimumValue = 1
        filterSlider.maximumValue = 12
        filterSlider.value = self.rangeData.map{CGFloat($0)}
        updateRangeLabel()
        
    }
    
    func setHeightSlider(){
        filterSlider.minimumValue = 0
        filterSlider.maximumValue = 800
        filterSlider.value = self.rangeData.map{CGFloat($0)}
        updateRangeLabel()
        
        
    }
    
    
    func updateRangeLabel(){
        
        if sliderType == SliderType.freezing{
            sliderInfoLabel.text = "Frosthärte"
            return
        }
        let minValue = filterSlider.value[0]
        let maxValue = filterSlider.value[1]
        if isItMonth{
            sliderInfoLabel.text = "\(String.getMonthString(month: Int(minValue))) - \(String.getMonthString(month: Int(maxValue)))"
            updateRangeDataValue()

            return
        }
        sliderInfoLabel.text = "\( Int(minValue)) - \(Int(maxValue))"
        
        updateRangeDataValue()
    }
    
    @objc  func sliderChanged(_ sender: MultiSlider){
        self.updateRangeLabel()
    }
    
//    func setMonthSlider(){
//        multiSlider.minimumValue = 1
//        multiSlider.maximumValue = 12
////        multiSlider.value = self.rangeData.map{CGFloat($0)}
////        updateRangeLabel()
//
//    }
//
//    func setHeightSlider(){
//        multiSlider.minimumValue = 0
//        multiSlider.maximumValue = 800
////        multiSlider.value = self.rangeData.map{CGFloat($0)}
////        updateRangeLabel()
//
//
//    }
    

}
