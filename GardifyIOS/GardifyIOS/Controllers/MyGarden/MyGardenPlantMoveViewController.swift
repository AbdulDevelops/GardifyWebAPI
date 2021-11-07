//
//  MyGardenPlantMoveViewController.swift
//  GardifyIOS
//
//  Created by Netzlab on 10.11.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class MyGardenPlantMoveViewController: UIViewController {

    @IBOutlet weak var titleLabel: UILabel!
    @IBOutlet weak var gardenListTableView: UITableView!
    @IBOutlet weak var saveButton: UIButton!
    @IBOutlet weak var backButton: UIButton!
    
    var userGardenList: [UserPlantModel] = []
    var selectedGarden: MyGardenModel?
    
    var gardenDelegate: MyGardenOptionDelegate?
    
    override func viewDidLoad() {
        super.viewDidLoad()

        self.configurePadding()
//        self.applyTheme()
        pageConfiguration()
        loadContent()
        // Do any additional setup after loading the view.
    }
    
    func pageConfiguration(){
        
        self.saveButton.setGreenButton()
        self.backButton.setGrayButton()
        
        self.gardenListTableView.addBorderWidth()
        self.gardenListTableView.addBorderRadius()
    }

    func loadContent(){
        if selectedGarden == nil{
            return
        }
        
        titleLabel.text = (self.selectedGarden?.UserPlant.Name)! + " in eine andere Liste verschieben"
    }
    
    @IBAction func onSave(_ sender: Any) {
        
        if gardenListTableView.indexPathForSelectedRow == nil{
            self.ShowAlert(message: "Bitte Ausfüllen")
            
            return
        }
        
        let params: [String: Any?] = ["UserPlantId": self.selectedGarden?.UserPlant.Id ,
                                      "NewListId": self.userGardenList[gardenListTableView.indexPathForSelectedRow!.row].Id]
        
        
        
        DispatchQueue.main.async {
            NetworkManager().requestDataAsync(type: [MyGardenModel].self, APP_URL.MOVE_PLANT, params, method: .post){response in
                if !response.success{
                    self.ShowAlert(message: "Es gibt ein fehler")

                    return
                }
                
                let dataResult = response.result as! [MyGardenModel]
                
                self.gardenDelegate?.updateGardenDataFromExternal(data: dataResult)
                
                self.ShowBackAlert(message: "Deine Pflanze wurde erfolgreich verschoben")
            }
        }
    }
    
    @IBAction func onBack(_ sender: Any) {
        self.navigationController?.popViewController(animated: true)
    }


}

extension MyGardenPlantMoveViewController: UITableViewDelegate, UITableViewDataSource{
    
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return self.userGardenList.count
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        
        let cell = tableView.dequeueReusableCell(withIdentifier: "addGardenCell", for: indexPath) as! checkListTableViewCell
        
        cell.plantPositionLabel.text = self.userGardenList[indexPath.row].Name
        
        return cell
    }
    
    
}
