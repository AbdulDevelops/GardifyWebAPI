//
//  MyGardenPlantCountChangeViewController.swift
//  GardifyIOS
//
//  Created by Netzlab on 10.11.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class MyGardenPlantCountChangeViewController: UIViewController {

    @IBOutlet weak var countLabel: UILabel!
    
    @IBOutlet weak var countStepper: UIStepper!
    
    @IBOutlet weak var saveButton: UIButton!
    
    @IBOutlet weak var backButton: UIButton!
    
    @IBOutlet weak var gardenListTableView: UITableView!
    
    var userGardenList: [UserPlantModel] = []
    var selectedGarden: MyGardenModel?
    var gardenDelegate: MyGardenOptionDelegate?

    var currentCount: Int = 0
    
    override func viewDidLoad() {
        super.viewDidLoad()

        self.configurePadding()
        pageConfiguration()
        loadContent()
        // Do any additional setup after loading the view.
    }
    
    func loadContent(){
        currentCount = selectedGarden?.UserPlant.Count ?? 0
        countStepper.value = Double(currentCount)
        updateCountLabel()
        print("selected index is", self.gardenListTableView.indexPathForSelectedRow)
        
    }
    
    
    @IBAction func onCountChange(_ sender: Any) {
        currentCount = Int(countStepper.value)
        updateCountLabel()
    }
    
    func updateCountLabel(){
        countLabel.text = "Anzahl: \(currentCount)"
    }
    
    func pageConfiguration(){
        self.saveButton.setGreenButton()
        self.backButton.setGrayButton()
        
        self.gardenListTableView.addBorderWidth()
        self.gardenListTableView.addBorderRadius()
    }
    
    @IBAction func onSave(_ sender: Any) {
        if gardenListTableView.indexPathForSelectedRow == nil{
            self.ShowAlert(message: "Bitte Ausfüllen")
            
            return
        }
        
        if userGardenList.count == 0{
            return
        }
        
        var gardenPlant = selectedGarden?.UserPlant
//        gardenPlant?.Count = currentCount
        
        let selectedIndex = (gardenListTableView.indexPathForSelectedRow?.row)!
        let selectedUserListId = userGardenList[selectedIndex].Id
        
        let params: [String: Any?] = [
            "Id": gardenPlant?.Id,
            "Name": gardenPlant?.Name,
            "Count": currentCount,
            "Description": gardenPlant?.Description,
            "IsInPot": gardenPlant?.IsInPot,
            "Notes": gardenPlant?.Notes,
            "UserListId": selectedUserListId
        ]
        
        NetworkManager().requestDataAsync(type: GardenUserPlantEditModel.self, APP_URL.MY_GARDEN_USER_PLANTS + "true", params, method: .put, printRequest: true){response in
            
            if !response.success{
                self.ShowBackAlert(message: response.result as! String)
                return
            }
            
            self.ShowBackAlert(message: "Deine Änderungen wurden gespeichert")
            let responsePlant = response.result as! GardenUserPlantEditModel
            
            self.gardenDelegate?.updateSingleUserGardenFromExternal(data: responsePlant)
            print(responsePlant)
            
        }
        
        print("params is", params)
    }
    
    @IBAction func onBack(_ sender: Any) {
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

extension MyGardenPlantCountChangeViewController: UITableViewDelegate, UITableViewDataSource{
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return self.userGardenList.count
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: "addGardenCell", for: indexPath) as! checkListTableViewCell
        
        cell.plantPositionLabel.text = self.userGardenList[indexPath.row].Name
        
        return cell
    }
    
    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
 
    }
    
}
