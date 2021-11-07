//
//  MyGardenListenDeleteViewController.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 09.09.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class MyGardenListenDeleteViewController: UIViewController {

    var userGarden: UserPlantModel?
    var gardenDelegate: MyGardenOptionDelegate?
    
    var userGardenList: [UserPlantModel] = []
    
    var selectedGarden: Int = -1
    
    @IBOutlet weak var userListTableView: UITableView!
    @IBOutlet weak var outerTableView: UIView!
    @IBOutlet weak var gardenTitlelabel: UILabel!
    
    @IBOutlet weak var saveToOtherGardenButton: UIButton!
    @IBOutlet weak var backButton: UIButton!
    @IBOutlet weak var deleteAllButton: UIButton!
    
    
    override func viewDidLoad() {
        super.viewDidLoad()

        self.configurePadding()
        listFilter()
        pageConfiguration()
        // Do any additional setup after loading the view.
    }
    
    func listFilter(){
        self.userGardenList = self.userGardenList.filter({$0.Id != userGarden?.Id})
    }
    
    func pageConfiguration(){
        self.outerTableView.addBorderWidth()
        self.outerTableView.addBorderRadius()
        
        self.backButton.setWhiteButton()
        self.saveToOtherGardenButton.setRedButton()
        self.deleteAllButton.setRedButton()
        self.gardenTitlelabel.text = "\(userGarden?.Name ?? "") Löschen"
    }
    
    
    
    @IBAction func onMoveToOther(_ sender: Any) {
        
        if selectedGarden == -1{
            return
        }
        
        let params : [String: Any?] = [
            "currentListId": self.userGarden?.Id ?? 0,
            "NewListId": self.selectedGarden
        ]
        self.showSpinner(onView: self.view)
        NetworkManager().requestDataAsync(type: [MyGardenModel].self, APP_URL.MY_GARDEN_MOVE_ALL_GARDEN, params, method: .post){response in
            
            if !response.success{
                
                self.ShowAlert(message: response.result as! String)
                self.removeSpinner()
                return
            }
            self.removeSpinner()
            
            let data = response.result as! [MyGardenModel]
            
            self.ShowBackAlert(message: "löschen")
            self.gardenDelegate?.updateGardenDataFromExternal(data: data)
            
        }
    }
    
    @IBAction func onBack(_ sender: Any) {
        
       
        let alert = UIAlertController(title :"", message: "Bist du sicher?", preferredStyle: .alert)
        
        let yesAction = UIAlertAction(title: "Ja", style: .default){alert in
            self.navigationController?.popViewController(animated: true)
        }
        
        let noAction = UIAlertAction(title: "Nein", style: .default){alert in
          
        }
        
        alert.addAction(yesAction)
        alert.addAction(noAction)
        
    present(alert, animated: true)
        
    }

    @IBAction func onDeleteAll(_ sender: Any) {
        print("is will be deleted")
        
        let alert = UIAlertController(title :"", message: "Bist du sicher?", preferredStyle: .alert)
        
        let yesAction = UIAlertAction(title: "Ja", style: .default){alert in
            self.showSpinner(onView: self.view)

            NetworkManager().requestDataAsync(type: String.self, APP_URL.USER_LIST + "/\(self.userGarden?.GardenId ?? 0)/\(self.userGarden?.Id ?? 0)", method: .delete){response in
                self.removeSpinner()
                self.gardenDelegate?.updateGardenUserList()
                self.ShowBackAlert(message: "löschen")
            }
        }
        
        let noAction = UIAlertAction(title: "Nein", style: .default){alert in
          
        }
        
        alert.addAction(yesAction)
        alert.addAction(noAction)
        
        present(alert, animated: true)
        
        
        
    }
    
}

extension MyGardenListenDeleteViewController: UITableViewDelegate, UITableViewDataSource{
    
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return self.userGardenList.count
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: "addGardenCell", for: indexPath) as! checkListTableViewCell
        cell.plantPositionLabel.text = self.userGardenList[indexPath.row].Name
        return cell
        
    }
    
    func tableView(_ tableView: UITableView, willSelectRowAt indexPath: IndexPath) -> IndexPath? {
        print("will be selected", indexPath)
        if selectedGarden == self.userGardenList[indexPath.row].Id{
            selectedGarden = -1
            tableView.deselectRow(at: indexPath, animated: true)
            return indexPath
        }
        return indexPath
    }
    
    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        
        if selectedGarden == self.userGardenList[indexPath.row].Id{
            selectedGarden = -1
            return
        }
        selectedGarden = self.userGardenList[indexPath.row].Id
    }
    
    func tableView(_ tableView: UITableView, willDeselectRowAt indexPath: IndexPath) -> IndexPath? {
        print("will be deselected")
        return indexPath

    }
    
    
    func tableView(_ tableView: UITableView, didDeselectRowAt indexPath: IndexPath) {
        print("is deselected")
    }
    
}
