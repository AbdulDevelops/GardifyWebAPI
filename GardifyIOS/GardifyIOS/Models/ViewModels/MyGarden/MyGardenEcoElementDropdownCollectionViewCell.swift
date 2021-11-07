//
//  MyGardenEcoElementDropdownCollectionViewCell.swift
//  GardifyIOS
//
//  Created by Netzlab on 06.11.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class MyGardenEcoElementDropdownCollectionViewCell: UICollectionViewCell {
    
    @IBOutlet weak var ecoTitleLabel: UILabel!
    
    @IBOutlet weak var ecoDescLabel: UILabel!
    @IBOutlet weak var ecoSwitch: UISwitch!
    
    @IBOutlet weak var ecoCounterButton: UIButton!
    
    @IBOutlet weak var ecoMinusButton: UIButton!
    
    @IBOutlet weak var ecoPlusButton: UIButton!
    
    
    var parent: MyGardenViewController?
    var isUsed: Bool = false
    var currentCount: Int = 0
    var ecoData: MyGardenEcoListModel?
    
    func onConfigure(data: MyGardenEcoListModel, id: Int){
        ecoData = data
//        self.detailButton.tag = id
        self.ecoTitleLabel.text = data.Name
        self.ecoDescLabel.text = data.Description
        ecoCounterButton.addBorderRadius()
        ecoMinusButton.addBorderRadius()
        ecoPlusButton.addBorderRadius()
        
        currentCount = data.EcoCount
        ecoCounterButton.setTitle("\(data.EcoCount)", for: .normal)
//        self.ecoDescLabel.sizeToFit()
        self.isUsed = data.Checked
        self.setSwitchState(state: self.isUsed)
        
        self.pageConfiguration()
    }
    
    func pageConfiguration(){
        self.backgroundColor = .clear
        self.contentView.backgroundColor = .systemBackground
        self.contentView.addBorderRadius()
    }
    
    
    @IBAction func onGoToDetail(_ sender: Any) {
        self.parent?.goToEcoElementDetailPage(index: self.tag)
    }
    
    @IBAction func onCountMinus(_ sender: Any) {
        
        self.currentCount = max(0, self.currentCount - 1)
        ecoCounterButton.setTitle("\(self.currentCount)", for: .normal)
        let params: [String: Any?] = [
            "Id": self.ecoData?.Id,
            "Checked": self.isUsed,
            "EcoCount": self.currentCount
        ]
        
        DispatchQueue.main.async {
            NetworkManager().requestDataAsync(type: String.self, APP_URL.USER_UPDATE_ECO_ELEMENT_COUNT, params, method: .put){response in
                
            }
        }
    }
    
    @IBAction func onCountPlus(_ sender: Any) {
        
        self.currentCount = self.currentCount + 1
        ecoCounterButton.setTitle("\(self.currentCount)", for: .normal)
        let params: [String: Any?] = [
            "Id": self.ecoData?.Id,
            "Checked": self.isUsed,
            "EcoCount": self.currentCount
        ]
        
        DispatchQueue.main.async {
            NetworkManager().requestDataAsync(type: String.self, APP_URL.USER_UPDATE_ECO_ELEMENT_COUNT, params, method: .put){response in
                
            }
        }
        
    }
    
    @IBAction func onEcoSwitch(_ sender: Any) {
        self.isUsed = self.ecoSwitch.isOn
        let params: [String: Any?] = [
            "Id": self.ecoData?.Id,
            "Checked": self.isUsed
        ]
        DispatchQueue.main.async {
            NetworkManager().requestDataAsync(type: String.self, APP_URL.USER_UPDATE_ECO_ELEMENT, params, method: .put){response in
                
            }
        }
       
    }
    
    func setSwitchState(state: Bool){
        self.ecoSwitch.isOn = state
    }
}
