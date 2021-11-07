//
//  EcoScanViewController.swift
//  GardifyIOS
//
//  Created by Rifat Hussain on 10.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//
import Foundation
import UIKit
import Charts
import MessageUI

struct CalenderData: Encodable {
    var gardenRating: Int
    var areaRating: Int
    var plantsRating: Int
    var totalArea: String
    var gardenArea: String
    var graph: [Int]
    var label: [String]
    var userInfo: Userdata
    var date: String
}

struct Userdata: Encodable {
    var id: String
    var HouseNr: String
    var FirstName: String
    var LastName: String
    var UserName: String
    var City: String
    var Street: String
    var Zip: String
    var Country: String
    
    enum CodingKeys: String, CodingKey {
        case id = "$id"
        case HouseNr = "HouseNr"
        case FirstName = "FirstName"
        case LastName = "LastName"
        case UserName = "UserName"
        case City = "City"
        case Street = "Street"
        case Zip = "Zip"
        case Country = "Country"
    }
    
}

class EcoScanViewController : UIViewController, MFMailComposeViewControllerDelegate, ChartViewDelegate, IAxisValueFormatter {
    
    @IBOutlet weak var ecoScanScrollView: UIScrollView!
    @IBOutlet weak var durationPlantRatingChartView: CombinedChartView!
    

    @IBOutlet weak var ecoContainerView: UIView!
    @IBOutlet weak var TitleLabel: UILabel!
    @IBOutlet weak var userInfoLabel: UILabel!
    @IBOutlet weak var totalAreaCalculateView: UIView!
    @IBOutlet weak var greanAreaCalculateView: UIView!
    @IBOutlet weak var dateLabel: UILabel!
    @IBOutlet weak var ecoScanDetailsLabel: UILabel!
    
    
    @IBOutlet weak var landAreaOuterView: UIView!
    @IBOutlet weak var landAreaGradientView: UIView!
    @IBOutlet weak var surfaceLabel: UILabel!
    @IBOutlet weak var greenAreaLabel: UILabel!
    @IBOutlet weak var surfaceAreaInput: UITextField!
    @IBOutlet weak var greenAreaInput: UITextField!
    @IBOutlet weak var EvaluationTitle: UILabel!
    @IBOutlet weak var landUseLabel: UILabel!
    @IBOutlet weak var ecoCriteriaLabel: UILabel!
    
    @IBOutlet weak var landUseValueLabel: UILabel!
    @IBOutlet weak var landAreaMeterSquareLabel: UILabel!
    @IBOutlet weak var landAreaLeadingWidth: NSLayoutConstraint!
    
    @IBOutlet weak var ecoCriteriaOuterView: UIView!
    @IBOutlet weak var ecoCriteriaLeadingWidth: NSLayoutConstraint!
    
    @IBOutlet weak var plantDiversityLeadingWidth: NSLayoutConstraint!
    @IBOutlet weak var plantDiversityOuterView: UIView!
    
    @IBOutlet weak var ecoCriteriaValueLabel: UILabel!
    @IBOutlet weak var floweringTimeTitleLabel: UILabel!
    @IBOutlet weak var saveToCalenderButton: UIButton!
    @IBOutlet weak var plantDiversityValueLabel: UILabel!
    @IBOutlet weak var greenedAreaMeterSquareLabel: UILabel!
    @IBOutlet weak var ecoCriteriaGradientView: UIView!
    @IBOutlet weak var plantDiversityLabel: UILabel!
    @IBOutlet weak var plantDiverityGradientView: UIView!
    @IBOutlet weak var chartsLabelDescription: UIView!
    @IBOutlet weak var linechartdescriptionColor: UIView!
    @IBOutlet weak var lineChartDescriptionLabel: UILabel!
    @IBOutlet weak var barChartDescriptionColor: UIView!
    @IBOutlet weak var barChartDescriptionLabel: UILabel!
    
    @IBOutlet weak var ecoBarIndicatorView: UIView!
    @IBOutlet weak var ecoCriteriaIndicatorView: UIView!
    @IBOutlet weak var plantDivIndicatorView: UIView!
    
    @IBOutlet weak var flachennutzungButton: RoundButton!
    @IBOutlet weak var okoElementeButton: RoundButton!
    @IBOutlet weak var pflanzenvielfaltButton: RoundButton!
    @IBOutlet weak var bluhdauerBurron: RoundButton!
    
    
    @IBOutlet weak var ecoDetailDropdownArrowImage: UIImageView!
    @IBOutlet weak var ecoScanDetailExtraLabel: UILabel!
    @IBOutlet weak var sendEmailPageButton: UIButton!
    
    @IBOutlet weak var topOuterView: UIView!
    @IBOutlet weak var mainOuterView: UIView!
    
    
    let jsonEncoder = JSONEncoder()
    
    var gardenRating: Int?
    var areaRating: Int?
    var plantsRating: Int?
    
    var isDetailExtended = false
    
    var userInfo: UserInfo?
    var myGardenInfo: MyGardenLightModel?
    
    var months = ["", "Jan.", "Feb.", "März", "April", "Mai", "Juni", "Juli", "Aug.", "Sept.", "Okt.", "Nov.", "Dez.", ""]
    var durationRatingPlant = [DurationRatingPlant]()
    public var dateFormatter = DateFormatter()
    weak var axisFormatDelegate: IAxisValueFormatter?
    let date = Date()
    var areaGradientLayer:  CAGradientLayer?
    
    var offsetY:CGFloat = 0
    
    var lineChartData = [ 2, 2, 4, 6, 7, 15, 20, 20, 15, 10, 6, 4, 2, 2]
    
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
        
        self.updateNavigationBar(isMain: false, "GARTEN", "ÖKOSCAN", "main_ecoScan")
        
    }
    
    override func viewWillDisappear(_ animated: Bool) {
        super.viewWillDisappear(animated)
        unsubscribeFromAllNotifications()
    }
    
    override func viewDidLoad() {
        self.loadPageContent()
        self.loadUserInfo()
        
        self.configurePadding()
        self.applyTheme()
        axisFormatDelegate = self
        // months = []
        initializeInitialValue()

        durationPlantRatingChartView.delegate = self
        self.surfaceAreaInput.addTarget(self, action: #selector(self.LandAreaTextFieldDidChange(_:)), for: .editingChanged)
        self.greenAreaInput.addTarget(self, action: #selector(self.GreenAreaTextFieldDidChange(_:)), for: .editingChanged)
        if !MFMailComposeViewController.canSendMail() {
            print("Mail services are not available")
            return
        }
    }
    
    func initializeInitialValue(){
        self.greenAreaInput.text = UserDefaultKeys.totalUsedArea.string()
        self.surfaceAreaInput.text = UserDefaultKeys.totalGardenArea.string()
        
        adjustLandArea()
        adjustGreenArea()
    }
    
    @IBAction func onExtendUpdate(_ sender: Any) {
        
        isDetailExtended = !isDetailExtended
        
        if isDetailExtended{
            ecoScanDetailsLabel.numberOfLines = 0
            ecoScanDetailExtraLabel.text = "Hinweis: Pflanzen, die über den Pflanzenscan in deinen Garten gespeichert wurden, beeinflussen auch die Pflanzenvielfalt und die Menge der insektenfreundlichen, blühenden Pflanzen nach Monaten. Der Ökoscan passt sich automatisch an, sobald wir die Pflanzen vollständig in die Datenbank aufgenommen haben."
            ecoDetailDropdownArrowImage.flipXAxis()
        }
        else{
            ecoScanDetailsLabel.numberOfLines = 3
            ecoScanDetailExtraLabel.text = ""
            ecoDetailDropdownArrowImage.revertFlip()
        }
    }
    
    
    func adjustLandValueBarPosition(value: Int){
        
        let width = landAreaOuterView.bounds.width
        landAreaLeadingWidth.constant = (width - 24.0) / 100.0 * CGFloat(max(min(value, 100),0)) + 2
        
    }
    
    func adjustEcoCriteriaBarPosition(value: Int){
        
        let width = ecoCriteriaOuterView.bounds.width
        ecoCriteriaLeadingWidth.constant = (width - 24.0) / 100.0 * CGFloat(max(min(value, 100),0))
        
    }
    
    func adjustPlantDiversityBarPosition(value: Int){
        
        let width = plantDiversityOuterView.bounds.width
        plantDiversityLeadingWidth.constant = (width - 24.0) / 100.0 * CGFloat(max(min(value, 100),0))
        
    }
    
    
    func configureChart(){
        durationPlantRatingChartView.isUserInteractionEnabled = false
        let legend = durationPlantRatingChartView.legend
        legend.enabled = false
        legend.horizontalAlignment = .right
        legend.verticalAlignment = .top
        legend.orientation = .vertical
        legend.drawInside = true
        legend.yOffset = 10.0;
        legend.xOffset = 10.0;
        legend.yEntrySpace = 0.0;
        
        
        let xaxis = durationPlantRatingChartView.xAxis
        xaxis.drawGridLinesEnabled = false
        xaxis.labelPosition = .bottom
        xaxis.drawLabelsEnabled = true
        xaxis.valueFormatter = IndexAxisValueFormatter(values: months)
        xaxis.granularity = 1
        xaxis.granularityEnabled = true
        xaxis.spaceMin = -0.5
        xaxis.spaceMax = -0.5
        xaxis.labelCount = months.count
        
        
        let leftAxisFormatter = NumberFormatter()
        leftAxisFormatter.maximumFractionDigits = 10
        
        
        
        let yaxis = durationPlantRatingChartView.leftAxis
        yaxis.spaceTop = 0.35
        yaxis.axisMinimum = 0
        yaxis.axisMaximum = 60
        yaxis.labelCount = 5
        yaxis.drawGridLinesEnabled = false
        yaxis.drawLabelsEnabled = false
        durationPlantRatingChartView.rightAxis.enabled = false
        
        
        
        var barChartEntries = [BarChartDataEntry]()
        var barChartCappedEntries = [BarChartDataEntry]()
        var lineChartEntries = [ChartDataEntry]()
        
        // var plantCount = self.durationRatingPlant.map({$0.PlantCount})
        for x in 0..<self.durationRatingPlant.count{            
            barChartEntries.append(BarChartDataEntry(x: Double(x+1), y: Double(self.durationRatingPlant[x].PlantCount!)))
            
            print("cap is \(self.lineChartData[x]), value is \(self.durationRatingPlant[x].PlantCount!), final is \(min(self.durationRatingPlant[x].PlantCount!, self.lineChartData[x]))")
            
            barChartCappedEntries.append(BarChartDataEntry(x: Double(x+1), y: Double(min(self.durationRatingPlant[x].PlantCount!, self.lineChartData[x+1]) )))
        }
        
        for i in 0..<self.lineChartData.count{
            lineChartEntries.append(ChartDataEntry(x: Double(i), y: Double(self.lineChartData[i])))
            
        }
        
        
        
        let barChartSet = BarChartDataSet(entries: barChartEntries, label: "Anzahl meiner blühenden Arten/Sorten")
        barChartSet.colors = [UIColor(displayP3Red: 224/255, green: 231/255, blue: 200/255, alpha: 1)]
        barChartSet.barBorderColor = UIColor.green
        barChartSet.drawValuesEnabled = false
        
        let barChartSetCap = BarChartDataSet(entries: barChartCappedEntries, label: "Optimum erreicht")
        barChartSetCap.colors = [UIColor(displayP3Red: 141/255, green: 173/255, blue: 48/255, alpha: 1)]
        barChartSetCap.barBorderColor = UIColor.green
        barChartSetCap.drawValuesEnabled = false
        
        let lineChartSet = LineChartDataSet(entries: lineChartEntries, label: "Optimale Kurve der Blühdauer")
        lineChartSet.colors = [UIColor(displayP3Red: 0/255, green: 0/255, blue: 0/255 ,alpha: 1)]
        lineChartSet.drawValuesEnabled = false
        lineChartSet.drawCirclesEnabled = false
        lineChartSet.fillColor = UIColor(displayP3Red: 178/255, green: 204/255, blue: 142/255, alpha: 0)
//        lineChartSet.fillColor = .label
        lineChartSet.drawFilledEnabled = true
        lineChartSet.mode = .cubicBezier
        
        let combinedData = CombinedChartData()
        
//        let singleData = CombinedChartData()
        
//        singleData.barData = BarChartData(data)
//        combinedData.barData = BarChartData(dataSet: barChartSetCap)
        combinedData.barData = BarChartData(dataSets: [barChartSet, barChartSetCap])
        combinedData.lineData = LineChartData(dataSet: lineChartSet)
        durationPlantRatingChartView.data = combinedData
        
        
    }
    
    func stringForValue(_ value: Double, axis: AxisBase?) -> String {
        return self.months[Int(value)]
    }
    
    func loadPageContent(){
        ecoContainerView.backgroundColor = UIColor(displayP3Red: 240/255, green: 240/255, blue: 240/255, alpha: 1)
   
   

        
        totalAreaCalculateView.addBorderRadius()
        greanAreaCalculateView.addBorderRadius()
        topOuterView.addBorderRadius()
        mainOuterView.addBorderRadius()
//        totalAreaCalculateView.backgroundColor = UIColor(displayP3Red: 178/255, green: 204/255, blue: 143/255, alpha: 1)
//        greanAreaCalculateView.backgroundColor = UIColor(displayP3Red: 178/255, green: 204/255, blue: 143/255, alpha: 1)
        flachennutzungButton.backgroundColor = UIColor(displayP3Red: 178/255, green: 204/255, blue: 143/255, alpha: 1)
        okoElementeButton.backgroundColor = UIColor(displayP3Red: 178/255, green: 204/255, blue: 143/255, alpha: 1)
        pflanzenvielfaltButton.backgroundColor = UIColor(displayP3Red: 178/255, green: 204/255, blue: 143/255, alpha: 1)
        bluhdauerBurron.backgroundColor = UIColor(displayP3Red: 178/255, green: 204/255, blue: 143/255, alpha: 1)
   
        TitleLabel.text = "Mein persönlicher Ökoscan"
        ecoScanDetailsLabel.text = "Mit deinem persönlichen Ökoscan bekommst du ein Gefühl dafür, wie ökologisch dein Garten ist und in welchem Bereich du noch etwas für die Natur tun kannst. Hierbei spielen die Verwendung deiner Außenfläche, die Auswahl und Blühdauer der Pflanzen sowie die Anzahl der Öko-Elemente eine Rolle."
        surfaceLabel.text = "Außenfläche = Bepflanzte + versiegelte Außenfläche"
        greenAreaLabel.text = "Begrünte Fläche"
    
        
        dateFormatter.dateFormat = "dd.MM.yyyy"
        dateLabel.text = "\(dateFormatter.string(from: date))"

        saveToCalenderButton.setTitle("Ergebnis im Kalender speichern", for: .normal)
        saveToCalenderButton.setGreenButton()
        sendEmailPageButton.setGrayButton()
//        saveToCalenderButton.setTitleColor(UIColor.white, for: .normal)
        landUseValueLabel.clearBorderWidth()
        ecoCriteriaValueLabel.clearBorderWidth()
        plantDiversityValueLabel.clearBorderWidth()
//        landUseValueLabel.addBorderRadiusSmall()
    
        ecoBarIndicatorView.setWhiteButtonView()
        ecoBarIndicatorView.layer.cornerRadius = 3
        ecoBarIndicatorView.alpha = 0.7
        ecoCriteriaIndicatorView.setWhiteButtonView()
        ecoCriteriaIndicatorView.layer.cornerRadius = 3
        ecoCriteriaIndicatorView.alpha = 0.7
        plantDivIndicatorView.setWhiteButtonView()
        plantDivIndicatorView.layer.cornerRadius = 3
        plantDivIndicatorView.alpha = 0.7
        landUseValueLabel.setWhiteButtonView()
        landUseValueLabel.layer.cornerRadius = 3
        landUseValueLabel.layer.masksToBounds = true
//        landUseValueLabel.layer.borderWidth = 2
//        landUseValueLabel.layer.borderColor = UIColor.black.cgColor
        landUseValueLabel.backgroundColor = .systemBackground
        landUseValueLabel.alpha = 0.7
        
        ecoCriteriaValueLabel.layer.borderWidth = 2
        ecoCriteriaValueLabel.layer.borderColor = UIColor.black.cgColor
        ecoCriteriaValueLabel.backgroundColor = UIColor(white: 1, alpha: 0.5)
        plantDiversityValueLabel.layer.borderWidth = 2
        plantDiversityValueLabel.layer.borderColor = UIColor.black.cgColor
        plantDiversityValueLabel.backgroundColor = UIColor(white: 1, alpha: 0.5)
        EvaluationTitle.text = "Auswertung"
        landUseLabel.text = "Flächennutzung"
        plantDiversityLabel.text = "Pflanzenvielfalt"
        ecoCriteriaLabel.text = "Öko-Elemente"
        landUseValueLabel.text = ""
        ecoCriteriaValueLabel.text = ""
        plantDiversityValueLabel.text = ""
        landAreaMeterSquareLabel.text = "m2"
        greenedAreaMeterSquareLabel.text = "m2"
        //durationPlantRatingChartView.backgroundColor = UIColor.white
        
        self.landAreaGradientView.addBorderRadiusSmall()
        
        self.ecoCriteriaGradientView.addBorderRadiusSmall()
        
        self.plantDiverityGradientView.addBorderRadiusSmall()
        self.configureDynamicLandAreaGradient(areaGradientView: landAreaGradientView, categoryNumber: 1)
        self.configureLandAreaViewGradient(areaGradientView: ecoCriteriaGradientView, categoryNumber: 1)
        self.configureLandAreaViewGradient(areaGradientView: plantDiverityGradientView, categoryNumber: 3)
        
        subscribeToNotification(UIResponder.keyboardWillShowNotification, selector: #selector(keyboardWillShowOrHide))
        subscribeToNotification(UIResponder.keyboardWillHideNotification, selector: #selector(keyboardWillShowOrHide))
        initializeHideKeyboard()
        
    }
    
    func configureDynamicLandAreaGradient(areaGradientView: UIView, categoryNumber: Int){
        areaGradientView.setNeedsDisplay()
        
        if areaGradientLayer == nil{
            let gradientLayer: CAGradientLayer = CAGradientLayer()
            
            // Set frame of gradient layer.
            gradientLayer.frame = areaGradientView.bounds
            
            // Color at the top of the gradient.
            let topColor: CGColor = UIColor(displayP3Red: 255/255, green: 1/255, blue: 1/255, alpha: 1).cgColor
            
            // Color at the middle of the gradient.
            let middleColor: CGColor = UIColor.yellow.cgColor
            
            // Color at the bottom of the gradient.
            let bottomColor: CGColor = UIColor(displayP3Red: 4/255, green: 128/255, blue: 4/255, alpha: 1).cgColor
            
            // Set colors.
            gradientLayer.colors = [topColor, middleColor, bottomColor]
            
            areaGradientLayer = gradientLayer
            
            areaGradientLayer!.startPoint = CGPoint(x: 0.0, y: 0.5)
            
            // Set end point.
            areaGradientLayer!.endPoint = CGPoint(x: 1.0, y: 0.5)
            
            // Insert gradient layer into view's layer heirarchy.
            
            areaGradientView.layer.insertSublayer(areaGradientLayer!, at: 0)
            
        }
        
        switch categoryNumber {
        case 1:
            areaGradientLayer!.locations = [0.0, 0.09, 1.0]
            lineChartData = [2, 2, 4, 6, 7, 15,20, 20, 15, 10, 6, 4, 2, 2]
            break
        case 2:
            areaGradientLayer!.locations = [0.0, 0.33, 1.0]
            lineChartData = [2, 4, 6, 10, 13, 20, 30, 30, 20, 15, 8, 5, 4, 2]
            break
        case 3:
            lineChartData = [2,  5, 7, 12, 16, 27, 40, 40, 27, 20, 12, 7, 5, 2]
            areaGradientLayer!.locations = [0.0, 0.5, 1.0]
            break
        case 4:
            areaGradientLayer!.locations = [0.0, 0.6, 1.0]
            lineChartData = [2, 6, 8, 15, 25, 35, 50, 50, 35, 25, 14, 8, 6, 2]
            break
        default:
            break
        }
        
        configureChart()
        
        
    }
    
    func  configureLandAreaViewGradient(areaGradientView: UIView, categoryNumber: Int){
        areaGradientView.setNeedsDisplay()
        // Initialize gradient layer.
        let gradientLayer: CAGradientLayer = CAGradientLayer()
        
        // Set frame of gradient layer.
        gradientLayer.frame = areaGradientView.bounds
        
        // Color at the top of the gradient.
        let topColor: CGColor = UIColor(displayP3Red: 255/255, green: 1/255, blue: 1/255, alpha: 1).cgColor
        
        // Color at the middle of the gradient.
        let middleColor: CGColor = UIColor.yellow.cgColor
        
        // Color at the bottom of the gradient.
        let bottomColor: CGColor = UIColor(displayP3Red: 4/255, green: 128/255, blue: 4/255, alpha: 1).cgColor
        
        // Set colors.
        gradientLayer.colors = [topColor, middleColor, bottomColor]
        
        switch categoryNumber {
        case 1:
            gradientLayer.locations = [0.0, 0.09, 1.0]
            break
        case 2:
            gradientLayer.locations = [0.0, 0.33, 1.0]
            break
        case 3:
            gradientLayer.locations = [0.0, 0.5, 1.0]
            break
        case 4:
            gradientLayer.locations = [0.0, 0.6, 1.0]
            break
        default:
            break
        }
        
        
        // Set start point.
        gradientLayer.startPoint = CGPoint(x: 0.0, y: 0.5)
        
        // Set end point.
        gradientLayer.endPoint = CGPoint(x: 1.0, y: 0.5)
        
        // Insert gradient layer into view's layer heirarchy.
        
        areaGradientView.layer.insertSublayer(gradientLayer, at: 0)
    }
    
    func loadUserInfo(){
        DispatchQueue.global(qos: .background).async {
            NetworkManager().requestDataAsync(type: UserInfo.self, APP_URL.ACCOUNT_USER_INFO) {data in
                if !data.success{
                    self.ShowAlert(message: "Cannot load user data")
                    return
                }
                self.userInfo = (data.result as! UserInfo)
                DispatchQueue.main.async {
                    self.userInfoLabel.text = "Garten von \(self.userInfo!.UserName)"

                }
            }
            
            NetworkManager().requestDataAsync(type: MyGardenLightModel.self, APP_URL.USER_GARDEN_MAIN) {data in
                if !data.success{
                    self.ShowAlert(message: "Cannot load user data")
                    return
                }
                self.myGardenInfo = data.result as! MyGardenLightModel
            }
            
            NetworkManager().requestDataAsync(type: Int.self, APP_URL.RATING_PLANT) {data in
                if !data.success{
                    self.ShowAlert(message: "Cannot load user data")
                    return
                }
                self.plantsRating = data.result as! Int
                DispatchQueue.main.async{
                    
                    self.adjustPlantDiversityBarPosition(value: data.result as! Int)
                }
            }
            NetworkManager().requestDataAsync(type: Int.self, APP_URL.RATING_TOTAL_ECO) {data in
                if !data.success{
                    self.ShowAlert(message: "Cannot load user data")
                    return
                }
                self.gardenRating = data.result as! Int
                
                DispatchQueue.main.async{
                    self.adjustEcoCriteriaBarPosition(value: data.result as! Int)
                }
            }
            
            NetworkManager().requestDataAsync(type: [DurationRatingPlant].self, APP_URL.DURATION_RATING_PLANT) {data in
                if !data.success{
                    self.ShowAlert(message: "Cannot load duration plant rating data")
                    return
                }
                self.durationRatingPlant = (data.result as! [DurationRatingPlant])
                
                DispatchQueue.main.async{
                    self.configureChart()
                }
            }
        }
        
        
    }
    
    
    
    @objc func LandAreaTextFieldDidChange(_ textField: UITextField) {
        adjustLandArea()
    }
    
    func adjustLandArea(){
        if self.surfaceAreaInput.text != nil {
            let landArea = (self.surfaceAreaInput.text as! NSString).integerValue
            UserDefaultKeys.totalGardenArea.set(landArea)
            if landArea >= 0 && landArea < 100{
                self.configureDynamicLandAreaGradient(areaGradientView: landAreaGradientView, categoryNumber: 1)
            }else if landArea >= 100 && landArea <= 499{
                self.configureDynamicLandAreaGradient(areaGradientView: landAreaGradientView, categoryNumber: 2)
            }else if landArea >= 500 && landArea < 999{
                self.configureDynamicLandAreaGradient(areaGradientView: landAreaGradientView, categoryNumber: 3)
            }else if landArea >= 1000{
                self.configureDynamicLandAreaGradient(areaGradientView: landAreaGradientView, categoryNumber: 4)
            }else{
                self.configureDynamicLandAreaGradient(areaGradientView: landAreaGradientView, categoryNumber: 1)
            }
            if  self.greenAreaInput.text != nil {
                let landArea = (self.surfaceAreaInput.text as! NSString).integerValue
                let greenArea = (self.greenAreaInput.text as! NSString).integerValue
                if greenArea > 0 && landArea > 0{
                    var landUseValue : Float = (Float(greenArea)/Float(landArea))*100
                    self.areaRating = Int((landArea/landArea)*100)
                    adjustLandValueBarPosition(value: Int(landUseValue))
                }else{
                    self.areaRating = 0
                    adjustLandValueBarPosition(value: 0)
                }
                
            }
            
        }
    }
    
    @objc func GreenAreaTextFieldDidChange(_ textField: UITextField) {
        
        adjustGreenArea()
    }
    
    func adjustGreenArea(){
        if self.surfaceAreaInput.text != nil && self.greenAreaInput.text != nil {
            let landArea = (self.surfaceAreaInput.text as! NSString).integerValue
            let greenArea = (self.greenAreaInput.text as! NSString).integerValue
            UserDefaultKeys.totalUsedArea.set(greenArea)

            if greenArea > 0 && landArea > 0{
                var landUseValue: Float = (Float(greenArea)/Float(landArea))*100
                adjustLandValueBarPosition(value: Int(landUseValue))
            }else{
                adjustLandValueBarPosition(value: 0)
            }
            
        }
    }
    
    @IBAction func clickFlachennutzung(_ sender: RoundButton) {
        self.ShowAlert(message: "Je weniger Flächen versiegelt werden desto besser. Optimal ist es, wenn so viel Außenflächen, wie möglich aus offenem Boden mit Pflanzenbewuchs bestehen.")
        
    }
    @IBAction func clickOkoKriterien(_ sender: RoundButton) {
        self.ShowAlert(message: "Insektenhotel, Nistkästen oder Komposthaufen - je mehr Ökokriterien im Garten realisiert werden, desto ökologischer ist dein Garten.")
    }
    @IBAction func clickPflanzenvielfalt(_ sender: RoundButton) {
        self.ShowAlert(message: "Artenvielfalt ist das A und O für ein stabiles ökologisches Gleichgewicht im Garten. Je mehr verschiedene sinnvolle Pflanzen kultiviert werden, desto besser.")
    }
    @IBAction func clickBluhdauer(_ sender: Any) {
        self.ShowAlert(message: "Menge der insektenfreundlichen, blühenden Pflanzen nach Monaten")
    }
    
    
    @IBAction func clickSaveToCalender(_ sender: UIButton) {
        var calenderData = CalenderData(gardenRating: self.gardenRating ?? 0, areaRating: self.areaRating ?? 0, plantsRating: self.plantsRating ?? 0, totalArea: self.surfaceAreaInput.text ?? "0", gardenArea: self.greenAreaInput.text ?? "0", graph: lineChartData, label: self.months, userInfo: Userdata(id: "1", HouseNr: self.userInfo!.HouseNr , FirstName: self.userInfo!.FirstName, LastName: self.userInfo!.LastName, UserName: self.userInfo!.UserName, City: self.userInfo!.City, Street: self.userInfo!.Street, Zip: self.userInfo!.Zip, Country: self.userInfo!.Country), date: self.date.toString(output: "YYYY-mm-dd'T'HH:mm:ss'Z'"))
        
        let jsonData = try! jsonEncoder.encode(calenderData)
        let json = String(data: jsonData, encoding: .utf8)
        let base64Data = self.convertJsonToBase64(json: json!)
        
        if isLoggedIn(){
            let userId = UserDefaultKeys.UserId.string()
            let gardenId = self.myGardenInfo?.Id
            let params: [String: Any] = [
                "Title" : "Mein Ökoscan",
                "Description" : "\(base64Data)",
                "Date" : date.toString(output: "YYYY-MM-dd"),
                "UserId" : "\(userId!)",
                "EntryObjectId": gardenId!,
                "EntryOf": 20
            ]
            
           // guard params !=
        
            
            self.showSpinner(onView: self.view)
            NetworkManager().requestDataAsync(type: String.self, APP_URL.DIARY_ROUTE, params, method: .post, printRequest: true){response in
                print(params)
                self.removeSpinner()
                self.ShowAlert(message: "Dein Ergebnis wurde im Kalender gespeichert")
            }
            
        }
        
    }
    
    func convertJsonToBase64(json: String) -> String{
        do {
            //let jsonData = try JSONSerialization.data(withJSONObject: json, options: .prettyPrinted)
            return
                Data(json.utf8).base64EncodedString()
        }
        catch {
            return ""
        }
        
    }
    
    @IBAction func clickSendEmail(_ sender: UIButton) {
//        let mailComposeViewController = configureMailController()
//        if MFMailComposeViewController.canSendMail() {
//            self.present(mailComposeViewController, animated: true, completion: nil)
//        } else {
//            showMailError()
//        }
    }
    
//    func configureMailController() -> MFMailComposeViewController {
//
//        let mailComposerVC = MFMailComposeViewController()
//        mailComposerVC.mailComposeDelegate = self
//
//        mailComposerVC.setToRecipients([""])
//        mailComposerVC.setSubject("")
//        mailComposerVC.setMessageBody("\(emailMessageBodyTextView.text!)", isHTML: false)
//
//        return mailComposerVC
//    }
//
    func showMailError() {
        let sendMailErrorAlert = UIAlertController(title: "Could not send email", message: "Your device could not send email", preferredStyle: .alert)
        let dismiss = UIAlertAction(title: "Ok", style: .default, handler: nil)
        sendMailErrorAlert.addAction(dismiss)
        self.present(sendMailErrorAlert, animated: true, completion: nil)
    }
    
    func mailComposeController(_ controller: MFMailComposeViewController, didFinishWith result: MFMailComposeResult, error: Error?) {
        controller.dismiss(animated: true, completion: nil)
    }
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        if segue.identifier == "sendEmail"{
            let controller = segue.destination as! EcoScanSendEmailViewController
            
            print("current view size is", mainOuterView.bounds.size)
            
            print("current bound is", mainOuterView.bounds)
            
            print("current bound is", mainOuterView.frame)
            let image = ecoContainerView.asImage()
//            let renderer = UIGraphicsImageRenderer(size: mainOuterView.bounds.size)
//            let image = renderer.image { ctx in
//                view.drawHierarchy(in: mainOuterView.bounds, afterScreenUpdates: true)
//            }
//
//            print("image size is", image.size)
            
            controller.ecoScanImage = image
        }
    }
}

extension EcoScanViewController {
    func initializeHideKeyboard(){
        let tap: UITapGestureRecognizer = UITapGestureRecognizer(
            target: self,
            action: #selector(dismissMyKeyboard))
        view.addGestureRecognizer(tap)
    }
    
    @objc func dismissMyKeyboard(){
        view.endEditing(true)
    }
    
}
extension EcoScanViewController {
    func subscribeToNotification(_ notification: NSNotification.Name, selector: Selector) {
        NotificationCenter.default.addObserver(self, selector: selector, name: notification, object: nil)
    }
    func unsubscribeFromAllNotifications() {
        NotificationCenter.default.removeObserver(self)
    }
    @objc func keyboardWillShowOrHide(notification: NSNotification) {
        
        if let userInfo = notification.userInfo {
            let endFrame = userInfo[UIResponder.keyboardFrameEndUserInfoKey] as? CGRect
            let animationDuration = userInfo[UIResponder.keyboardAnimationDurationUserInfoKey] as? Double ?? 0
            let animationCurveRawValue = (userInfo[UIResponder.keyboardAnimationCurveUserInfoKey] as? Int) ?? Int(UIView.AnimationOptions.curveEaseInOut.rawValue)
            let animationCurve = UIView.AnimationOptions(rawValue: UInt(animationCurveRawValue))
            if let _ = endFrame, endFrame!.intersects(self.view.frame) {
                self.offsetY = self.view.frame.maxY - endFrame!.minY
                UIView.animate(withDuration: animationDuration, delay: TimeInterval(0), options: animationCurve, animations: {
                    self.view.frame.origin.y = self.view.frame.origin.y - self.offsetY
                }, completion: nil)
            } else {
                if self.offsetY != 0 {
                    UIView.animate(withDuration: animationDuration, delay: TimeInterval(0), options: animationCurve, animations: {
                        self.view.frame.origin.y = self.view.frame.origin.y + self.offsetY
                        self.offsetY = 0
                    }, completion: nil)
                }
            }
        }
    }
}
