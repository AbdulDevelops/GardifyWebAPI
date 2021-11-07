//
//  MyGardenListenEditGardenViewController.swift
//  GardifyIOS
//
//  Created by Netzlab on 09.09.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class MyGardenListenEditGardenViewController: UIViewController {

    var userGarden: UserPlantModel?
    var gardenDelegate: MyGardenOptionDelegate?
    
    
    @IBOutlet weak var gardenTitleLabel: UILabel!
    @IBOutlet weak var newNameField: UITextField!
    @IBOutlet weak var saveButton: UIButton!
    @IBOutlet weak var closeButton: UIButton!
    @IBOutlet weak var descriptionTextView: UITextView!
    
    
    
    override func viewDidLoad() {
        super.viewDidLoad()

        self.configurePadding()
        pageConfiguration()
        // Do any additional setup after loading the view.
    }
    
    
    func pageConfiguration(){
        
        gardenTitleLabel.text = "\(userGarden?.Name ?? "") bearbeiten"
        newNameField.text = userGarden?.Name
        descriptionTextView.text = userGarden?.Description?.replacingOccurrences(of: "No Description", with: "Noch keine Beschreibung")
        
        descriptionTextView.addBorderWidth()
        descriptionTextView.addBorderRadius()
        saveButton.setGreenButton()
        closeButton.setWhiteButton()
        
    }
    
    override func touchesBegan(_ touches: Set<UITouch>, with event: UIEvent?) {
        print("touched")
        view.endEditing(true    )
    }
    
    @IBAction func onSave(_ sender: Any) {
        
        view.endEditing(true    )

        let params: [String: Any?] = [
            "Id": userGarden?.Id,
            "Name": newNameField.text,
            "Description": descriptionTextView.text,
            "GardenId": userGarden?.GardenId,
            "ListSelected": userGarden?.ListSelected
            
        ]
        
        
        self.showSpinner(onView: self.view)
        NetworkManager().requestDataAsync(type: String.self, APP_URL.USER_LIST_UPDATE, params, method: .put){response in
            self.removeSpinner()
            self.gardenDelegate?.updateGardenUserList()

            self.ShowBackAlert(message: "Deine Änderungen wurden gespeichert")
//            self.navigationController?.popViewController(animated: true)
            
        }
    }
    
    @IBAction func onClose(_ sender: Any) {
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
