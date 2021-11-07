//
//  PlantSearchFilterSliderViewController.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 11.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit
import MultiSlider


class PlantSearchFilterSliderViewController: UIViewController {

    @IBOutlet weak var sliderView: UIView!
    @IBOutlet weak var multiSlider: MultiSlider!
    @IBOutlet weak var finishButton: UIButton!
    @IBOutlet weak var mainFinishButton: UIButton!
    
    @IBOutlet weak var greenSliderView: UIView!
    @IBOutlet weak var outerSliderView: UIView!
    
    @IBOutlet weak var greenSliderTrailing: NSLayoutConstraint!
    @IBOutlet weak var greenSliderLeading: NSLayoutConstraint!
    
    
    var isItMonth: Bool = true
    var rangeData: [Int] = []
    var sliderType: SliderType?
    var filterDelegate: FilterUpdateDelegate?
    
    @IBOutlet weak var rangeLabel: UILabel!
    
    override func viewDidLoad() {
        super.viewDidLoad()
        self.sliderConfiguration()
        self.configurePadding()
        
        self.mainFinishButton.setGreenButton()

        switch sliderType {
        case .month:
            self.setMonthSlider()
        case .height:
            self.setHeightSlider()

        case .freezing:
            self.setFreezingSlider()
        default:
            return
        }

       
        // Do any additional setup after loading the view.
    }
    

    
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(false)
        adjustGreenSliderPosition()

    }
    

    
    func sliderConfiguration(){
//        multiSlider.valueLabelPosition = .top
        multiSlider.isContinuous = true
        print(self.rangeData.map{CGFloat($0)})

        
        multiSlider.addTarget(self, action: #selector(sliderChanged(_:)), for: .valueChanged) // continuous changes
        multiSlider.addTarget(self, action: #selector(sliderDragEnded(_:)), for: . touchUpInside) // sent when drag ends
    }
    
    func setMonthSlider(){
        multiSlider.minimumValue = 1
        multiSlider.maximumValue = 12
        multiSlider.value = self.rangeData.map{CGFloat($0)}
        updateRangeLabel()

    }
    
    func setHeightSlider(){
        multiSlider.minimumValue = 0
        multiSlider.maximumValue = 800
        multiSlider.value = self.rangeData.map{CGFloat($0)}
        updateRangeLabel()
        

    }
    
    func setFreezingSlider(){
        multiSlider.hasRoundTrackEnds = false
        multiSlider.tintColor = .clear
        multiSlider.valueLabelPosition = .top
        let sliderLabel = multiSlider.valueLabels[0]
        
        sliderLabel.textColor = .clear
        multiSlider.valueLabelFormatter.positiveSuffix = "°C"
        multiSlider.valueLabelFormatter.negativeSuffix = "°C"
        multiSlider.minimumValue = -45
        multiSlider.maximumValue = 10
        multiSlider.snapStepSize = 5
        
        var freezingValue: CGFloat = CGFloat(10 - (12 - self.rangeData[0] ) * 5 )
        print("freezing value is", freezingValue)
        multiSlider.value = [freezingValue]
        updateRangeLabel()
        
        multiSlider.thumbImage = UIImage(systemName: "minus")
        let gesture = UITapGestureRecognizer(target: self, action: #selector(sliderTapped(gestureRecognizer:)))
        outerSliderView.alpha = 1
        self.multiSlider.addGestureRecognizer(gesture)
        

    }
    
    @objc func sliderTapped(gestureRecognizer: UIGestureRecognizer){
        print("clicked")
        if sliderType == SliderType.freezing{
            
            let pointTapped: CGPoint = gestureRecognizer.location(in: self.outerSliderView)
            print("tapped in", pointTapped)
            //        let positionOfSlider: CGPoint = multiSlider.frame.origin
            let widthOfSlider: CGFloat = outerSliderView.frame.size.width
            let newSliderValue = (((pointTapped.x / widthOfSlider) * CGFloat(55) - 45) / CGFloat(5) ).rounded() * 5
            print("width in", widthOfSlider, newSliderValue)

            multiSlider.value = [CGFloat(newSliderValue)]
            self.adjustGreenSliderPosition()

        }
    }
    
    func adjustGreenSliderPosition(){
        
        if sliderType == SliderType.freezing{
            let fullWidth = self.outerSliderView.frame.width
            let sliderValue = Int(multiSlider.value[0])
            let offsetWidth = (CGFloat((sliderValue + 45)) / CGFloat(55)) * fullWidth
            print("offset width is", offsetWidth, sliderValue, fullWidth)
            UIView.animate(withDuration: 0.25, animations: {
//                self.greenSliderTrailing.constant = offsetWidth
                self.greenSliderLeading.constant = offsetWidth
                self.view.layoutIfNeeded()
            })
        }
    }
    
    func updateRangeLabel(){
       
        if sliderType == SliderType.freezing{
            rangeLabel.text = "Frosthärte"
            return
        }
        let minValue = multiSlider.value[0]
        let maxValue = multiSlider.value[1]
        if isItMonth{
            rangeLabel.text = "\(String.getMonthString(month: Int(minValue))) - \(String.getMonthString(month: Int(maxValue)))"
            return
        }
        rangeLabel.text = "\( Int(minValue)) - \(Int(maxValue))"

    }
    
    func updateRangeDataValue(){
        self.filterDelegate?.updateSliderValue(value: self.getSliderRangeValue(), sender: self, type: sliderType!)

    }
    
    func getSliderRangeValue() -> [Int]{
        if sliderType == SliderType.freezing{
            let value = Int( multiSlider.value[0])
            
            return [(value + 50)/5]
        }
        let minValue = multiSlider.value[0]
        let maxValue = multiSlider.value[1]
        
        return [Int(minValue), Int(maxValue)]
    }
    
    @objc  func sliderChanged(_ sender: MultiSlider){
        self.adjustGreenSliderPosition()
        self.updateRangeLabel()
    }
    
    @objc 	func sliderDragEnded(_ sender: MultiSlider){
    }
    
    @IBAction func onFinish(_ sender: Any) {
         updateRangeDataValue()
        self.navigationController?.popViewController(animated: true)

        
    }
    
    @IBAction func onFinishMain(_ sender: Any) {
        updateRangeDataValue()
        self.navigationController?.popViewController(animated: true)
    }
    /*
    // MARK: - Navigation

    // In a storyboard-based application, you will often want to do a little preparation before navigation
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        // Get the new view controller using segue.destination.
        // Pass the selected object to the new view controller.
    }
    */

}
