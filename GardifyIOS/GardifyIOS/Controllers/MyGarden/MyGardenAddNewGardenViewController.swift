//
//  MyGardenAddNewGardenViewController.swift
//  GardifyIOS
//
//  Created by Netzlab on 04.11.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class MyGardenAddNewGardenViewController: UIViewController {

    @IBOutlet weak var submitButton: UIButton!
    
    @IBOutlet weak var cancelButton: UIButton!
    
    @IBOutlet weak var listNameField: UITextField!
    
    @IBOutlet weak var descriptionField: UITextField!
    
    var gardenDelegate: MyGardenOptionDelegate?
    var gardenId: Int?
    
    override func viewDidLoad() {
        super.viewDidLoad()

        self.configurePadding()
        pageConfiguration()
        // Do any additional setup after loading the view.
    }
    

    func pageConfiguration(){
        
        submitButton.setGreenButton()
        cancelButton.setGrayButton()
    }
    
    @IBAction func onSubmit(_ sender: Any) {
        submitGarden()
    }
    
    @IBAction func onCancel(_ sender: Any) {
        self.navigationController?.popViewController(animated: true)
    }
    
    
    func submitGarden(){
        if listNameField.text == nil{
            self.ShowAlert(message: "Bitte Ausfüllen")
        }
        
        if listNameField.text == ""{
            self.ShowAlert(message: "Bitte Ausfüllen")
        }
        
        let params: [String: Any?] = [
            "Name": listNameField.text,
            "Description": descriptionField.text,
            "GardenId": gardenId
        ]
        
        NetworkManager().requestDataAsync(type: [UserPlantModel].self, APP_URL.ADD_USER_GARDEN, params,  method: .post){response in
            
            if !response.success{
                
                self.ShowAlert(message: response.result as! String)
                self.removeSpinner()
                return
            }
            self.removeSpinner()
            let result = response.result as! [UserPlantModel]
            
            self.gardenDelegate?.updateUserGardenFromExternal(data: result)
            
            self.ShowBackAlert(message: "Deine Liste wurde gespeichert")
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
