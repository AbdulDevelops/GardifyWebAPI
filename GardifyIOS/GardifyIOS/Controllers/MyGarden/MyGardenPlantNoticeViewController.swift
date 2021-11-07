//
//  MyGardenPlantNoticeViewController.swift
//  GardifyIOS
//
//  Created by Netzlab on 10.11.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class MyGardenPlantNoticeViewController: UIViewController {

    @IBOutlet weak var noticeTextField: UITextView!
    @IBOutlet weak var saveButton: UIButton!
    
    @IBOutlet weak var backButton: UIButton!
    
    var gardenData: MyGardenModel?
    var gardenDelegate: MyGardenOptionDelegate?

    
    override func viewDidLoad() {
        super.viewDidLoad()

        self.configurePadding()
        
        loadContent()
        pageConfiguration()
        // Do any additional setup after loading the view.
    }
    
    func loadContent(){
        self.noticeTextField.text = nil
    }
    
    func pageConfiguration(){
        self.saveButton.setGreenButton()
        self.backButton.setGrayButton()
        
        noticeTextField.addBorderWidth()
        noticeTextField.addBorderRadius()
    }
    
    @IBAction func onSave(_ sender: Any) {
        
        if gardenData == nil{
            self.ShowBackAlert(message: "Es gibt einen fehler")
            return
        }
        
        var gardenPlant = gardenData?.UserPlant
        gardenPlant?.Notes = noticeTextField.text
        
        let params: [String: Any?] = [
            "Id": gardenPlant?.Id,
            "Name": gardenPlant?.Name,
            "Count": gardenPlant?.Count,
            "Description": gardenPlant?.Description,
            "IsInPot": gardenPlant?.IsInPot,
            "Notes": gardenPlant?.Notes,
        ]
        
        NetworkManager().requestDataAsync(type: GardenUserPlantEditModel.self, APP_URL.MY_GARDEN_USER_PLANTS + "false", params, method: .put, printRequest: true){response in
            
            if !response.success{
                self.ShowBackAlert(message: response.result as! String)
                return
            }
            
            self.ShowBackAlert(message: "Deine Notiz wurde gespeichert")

            let responsePlant = response.result as! GardenUserPlantEditModel
            self.gardenDelegate?.updateSingleUserGardenFromExternal(data: responsePlant)
            print(responsePlant)
            
        }
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
