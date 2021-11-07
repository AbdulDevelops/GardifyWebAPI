//
//  PlantGardenAddViewController.swift
//  GardifyIOS
//
//  Created by Netzlab on 03.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit
import Alamofire

class propResponse: Codable{
    let Id: Int
}

class addRequestModel: Encodable{
    var Count = 0
    var InitialAgeInDays  = 0
    var PlantId = 0
    var IsInPot = false
    var Todos: TodoModel? = nil
    var ArrayOfUserList: [UserPlantListModel] = []
    
}

class PlantGardenAddViewController: UIViewController, UIPickerViewDelegate, UIPickerViewDataSource {

    
    var userPlantList: [UserPlantModel] = []
    var plantId: Int?
    var plantCount: Int = 1
    var plantName: String?
    var gardenDetail: MyGardenLightModel?
    @IBOutlet weak var addPlantToGardenTitle: UILabel!
    @IBOutlet weak var gardenNameLabel: UILabel!
    @IBOutlet weak var plantListTableView: UITableView!
    @IBOutlet weak var submitButton: UIButton!
    @IBOutlet weak var countPickerView: UIPickerView!
    @IBOutlet weak var topPlantSwitch: UISwitch!
    
    override func viewDidLoad() {
        super.viewDidLoad()
        getGarden()
        
        self.configurePickerView()
        self.configurePadding()
        self.pageConfiguration()

        // Do any additional setup after loading the view.
    }
    
    func configurePickerView(){
      
    }
    
    func getGarden(){
        self.showSpinner(onView: self.view)
        NetworkManager().requestDataAsync(type: MyGardenLightModel.self, APP_URL.USER_GARDEN_MAIN){response in
            if !response.success{
                self.ShowAlert(message: response.result as! String)
                self.removeSpinner()
                self.navigationController?.popViewController(animated: true)
                return
            }
            
            self.removeSpinner()
            self.gardenDetail = (response.result as! MyGardenLightModel)
            self.pageConfiguration()

        }
        
       
    }
    
    func pageConfiguration(){
        
        plantListTableView.addBorderWidth()
        plantListTableView.addBorderRadius()
        print("garden list", userPlantList)
        addPlantToGardenTitle.text = (plantName)! + " zu meinem Gartenbereich hinzufügen"
        
        gardenNameLabel.text = "Gartenbereich: \(gardenDetail?.Name ?? "")"
        self.submitButton.setGreenButton()
        
    }
    

    
    @IBAction func onAddToGarden(_ sender: Any) {
        
        let selectedPlant = self.userPlantList.filter({$0.ListSelected == true})
        
        if selectedPlant.count < 1{
            self.ShowAlert(message: "Bitte Liste auswählen!")
            return
        }
        var userList: [UserPlantListModel] = []
        selectedPlant.forEach{ plant in
            let plant: UserPlantListModel = UserPlantListModel(UserPlantId: 1, UserListId: plant.Id)
            userList.append(plant)
  
            
        }
        

        
        var requestData = addRequestModel()
        
        requestData.Count = self.plantCount
        requestData.InitialAgeInDays = 1
        requestData.IsInPot = self.topPlantSwitch.isOn
        requestData.PlantId = plantId!
        requestData.ArrayOfUserList = userList
        
        self.showSpinner(onView: self.view)
        print("add garden params is", requestData.Count)
        
        NetworkManager().requestJsonAsync(type: addRequestModel.self, APP_URL.USER_PLANT_PROP_ADD ,requestData , method: "POST"){response in
            print("response is", response)
            self.removeSpinner()
            self.ShowBackAlert(message: "Pflanze wurde dem Garten hinzugefügt")
        }
        return
 
        
    }
    
    func numberOfComponents(in pickerView: UIPickerView) -> Int {
        return 1
    }
    
    func pickerView(_ pickerView: UIPickerView, numberOfRowsInComponent component: Int) -> Int {
        return 98
    }
    
    func pickerView(_ pickerView: UIPickerView, titleForRow row: Int, forComponent component: Int) -> String? {
        return "\(row + 1)"
    }
    
    func pickerView(_ pickerView: UIPickerView, didSelectRow row: Int, inComponent component: Int) {
        plantCount = row + 1
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

extension PlantGardenAddViewController: UITableViewDelegate, UITableViewDataSource{
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return userPlantList.count
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: "addGardenCell", for: indexPath) as! checkListTableViewCell
        
        cell.onConfigure(text: userPlantList[indexPath.row].Name)
        
        if userPlantList[indexPath.row].ListSelected{
            tableView.selectRow(at: indexPath, animated: false, scrollPosition: .none)
        }
        
        return cell
    }
    
    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        userPlantList[indexPath.row].ListSelected = true
    }
    
    func tableView(_ tableView: UITableView, didDeselectRowAt indexPath: IndexPath) {
        userPlantList[indexPath.row].ListSelected = false

    }
    
    
}
