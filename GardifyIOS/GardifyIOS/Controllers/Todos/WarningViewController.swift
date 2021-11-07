//
//  WarningViewController.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 28.10.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class WarningViewController: UIViewController {

    
    @IBOutlet weak var frostWarningButton: UIButton!
    
    @IBOutlet weak var stormWarningButton: UIButton!
    
    @IBOutlet weak var frostWarningTable: FullTableView!
    
    
    @IBOutlet weak var infoButton: UIButton!
    
    @IBOutlet weak var infoPopUpView: UIView!
    
    @IBOutlet weak var settingsButton: UIButton!
    
    var warningData: [WarningModel] = []
    var plantWarningData: [WarningModel] = []
    var deviceWarningData: [WarningModel] = []
    var isFrostWarning: Bool = true
    var isStormWarning: Bool = false
    var isInfoExtended: Bool = false
    
    @IBOutlet weak var warningSwitchOffLabel: UILabel!
    
    @IBOutlet weak var warningSwitchOffSwitch: UISwitch!
    
    @IBOutlet weak var infoLabel1: UILabel!
    @IBOutlet weak var infoSwitch1: UISwitch!
    
    @IBOutlet weak var infoLabel2: UILabel!
    @IBOutlet weak var infoSwitch2: UISwitch!
    
    override func viewDidLoad() {
        super.viewDidLoad()

        pageConfiguration()
        self.applyTheme()
        self.configurePadding()
        self.showSpinner(onView: self.view)
        
        DispatchQueue.main.async {
            self.loadWarning()
        }
        // Do any additional setup after loading the view.
        
        
    }
    
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
        
        self.updateNavigationBar(isMain: false, "", "WARNUNGEN", "main_todoCalender")

    }
    
    
    @IBAction func onFrostButtonClick(_ sender: Any) {
        isFrostWarning = true
        isStormWarning = false
        
        UpdateStormMode()
    }
    
    
    @IBAction func onStormButtonClick(_ sender: Any) {
        isFrostWarning = false
        isStormWarning = true
        
        UpdateStormMode()
    }
    
    func UpdateStormMode() {
        if isFrostWarning {
            self.frostWarningButton.setGreenButton()
            self.stormWarningButton.setWhiteButton()
        }
        else {
            self.frostWarningButton.setWhiteButton()
            self.stormWarningButton.setGreenButton()
        }
        //Manually overwrite border raduis
        frostWarningButton.layer.cornerRadius = 5
        stormWarningButton.layer.cornerRadius = 5
        
        self.frostWarningTable.reloadData()
    }
    
    
    @IBAction func onDropdownButtonClick(_ sender: Any) {
        
    }
    
    func pageConfiguration(){
        
        frostWarningTable.dataSource = self
        frostWarningTable.delegate = self
        
        self.frostWarningTable.backgroundColor = .clear
        
        UpdateStormMode()
        
        updateInfoPopUpModal()
        
        setInfoLabelData()
        
    }
    
    func setInfoLabelData() {
        infoPopUpView.addBorderRadius()
        infoSwitch1.transform = CGAffineTransform(scaleX: 0.5, y: 0.5)
        infoSwitch2.transform = CGAffineTransform(scaleX: 0.5, y: 0.5)
        infoSwitch1.isEnabled = false
        infoSwitch2.isEnabled = false
        infoLabel1.attributedText = getBoldText(firstText: "             Warnung abschalten.", secondText: " Für laufende Winterperiode. Am 21.06 wird die Warnung automatisch wieder aktiviert")
        infoLabel2.attributedText = getBoldText(firstText: "             Warnung aktivieren.", secondText:" Die weiteren Parameter können unter Einstellung individuell angepasst werden")
        self.infoPopUpView.backgroundColor = .systemBackground
    }
    
    
    @IBAction func onInfoClick(_ sender: Any) {
        isInfoExtended = !isInfoExtended
        
        updateInfoPopUpModal()
    }
    
    @IBAction func onSettingsClick(_ sender: Any) {
        
        let tabBar = self.tabBarController as! MainTabBarController
        
        tabBar.navigateToLoggedPage(page: .settingPage)
    }
    
    
    func updateInfoPopUpModal(){
        var alphaValue = 0
        if isInfoExtended{
            alphaValue = 1
        }
        
        UIView.animate(withDuration: 0.5, animations: {
            self.view.layoutIfNeeded()
            
            self.infoPopUpView.alpha = CGFloat(alphaValue)
        })
    }
    

    func loadWarning(){
        
        print("is logged in", UserDefaultKeys.IsLoggedIn.bool())
        
        if !UserDefaultKeys.IsLoggedIn.bool(){
            self.ShowBackAlert(message: "Bitte einloggen")
            
            return
        }
        
        NetworkManager().requestDataAsync(type: [WarningModel].self, APP_URL.USER_WARNING_COUNT, printRequest : true){response in
            if response.success{
                let result = response.result as! [WarningModel]
                self.removeSpinner()
                //self.warningData = result
                self.plantWarningData = result.filter{item in (item.ObjectType == 4)}
                self.deviceWarningData = result.filter{item in (item.ObjectType == 19)}
                
                self.frostWarningTable.reloadData()
            }
            self.removeSpinner()
            return
        }
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


extension WarningViewController: UITableViewDelegate, UITableViewDataSource{
    
    
    func numberOfSections(in tableView: UITableView) -> Int {
        return 2
    }
    
    func tableView(_ tableView: UITableView, titleForHeaderInSection section: Int) -> String? {
        
        if section == 0 {
            return "Pflanzen"
        }
        else if section == 1 {
            return "Geräte"
        }
        return ""
    }
    
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        if section == 0 { //4:Plant type
            
            return plantWarningData.count
        }
        else if section == 1 { //19:Device type
            
            return deviceWarningData.count
        }
        return 0
        
        
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: "warningCell", for: indexPath) as! WarningTableViewCell
        
        if indexPath.section == 0 {
            self.warningData = self.plantWarningData
            cell.isPlantData = true
        }
        else if indexPath.section == 1 {
            self.warningData = self.deviceWarningData
            cell.isPlantData = false
        }
        
        cell.outerView.backgroundColor = .systemBackground
        print("warning is", warningData[indexPath.row].Dismissed)
        if !warningData[indexPath.row].Dismissed{
            cell.outerView.backgroundColor = rgba(255, 150, 150, 0.5)
        }
        cell.onConfigure()
        
        //cell.outerView.backgroundColor = .Background
        
        cell.parent = self.frostWarningTable
        
        cell.controllerData = self
        
        cell.isFrostWarning = self.isFrostWarning
        
        if isFrostWarning {
            cell.warningSwitch.isOn = warningData[indexPath.row].NotifyForFrost
            
        }
        else if isStormWarning {
            cell.warningSwitch.isOn = warningData[indexPath.row].NotifyForWind
        }
        
        cell.relatedObjectId = warningData[indexPath.row].RelatedObjectId
        
        cell.warningData = warningData[indexPath.row]
        
        
        //cell.titleLabel.text = warningData[indexPath.row].Title
        cell.tempDescription = warningData[indexPath.row].Text ?? ""
        cell.warningLabel.text = warningData[indexPath.row].RelatedObjectName
        cell.updateDropdownHeight()
//        cell.descriptionLabel.text = warningData[indexPath.row].Text
        cell.alertConditionLabel.text = "≤" + "\(warningData[indexPath.row].AlertConditionValue)" + "º"
        
        if indexPath.section == 0 { //Plant section
            if warningData[indexPath.row].IsInPot {
                cell.isInPotImage.image = UIImage(named: "Warnungen_Topfpflanzen")
                
            }
            else {
                cell.isInPotImage.image = UIImage(named: "Warnungen_Beetpflanzen")
            }
        }
        else //Device section
        {
            cell.isInPotImage.image = nil
        }
        
        return cell
    }
    
    
}
