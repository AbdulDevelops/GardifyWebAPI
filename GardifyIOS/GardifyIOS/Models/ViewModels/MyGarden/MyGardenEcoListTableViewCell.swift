//
//  MyGardenEcoListTableViewCell.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 02.09.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class MyGardenEcoListTableViewCell: UITableViewCell {

    @IBOutlet weak var ecoTitle: UILabel!
    @IBOutlet weak var ecoDetails: UILabel!
    @IBOutlet weak var outerView: UIView!
    @IBOutlet weak var ecoSwitch: UISwitch!
    
    @IBOutlet weak var detailButton: UIButton!
    
    var isUsed: Bool = false
    var ecoData: MyGardenEcoListModel?
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
        
        
    }
    
    func onConfigure(data: MyGardenEcoListModel, id: Int){
        ecoData = data
        self.detailButton.tag = id
        self.ecoTitle.text = data.Name
        self.ecoDetails.text = data.Description
        self.isUsed = data.Checked
        self.setSwitchState(state: self.isUsed)
        
        
        self.pageConfiguration()
    }
    
    func pageConfiguration(){
        self.backgroundColor = .clear
        self.contentView.backgroundColor = .clear
        self.outerView.addBorderRadius()
    }
    
    func setSwitchState(state: Bool){
        self.ecoSwitch.isOn = state
    }
    
    @IBAction func onSwitch(_ sender: Any) {
        self.isUsed = self.ecoSwitch.isOn
        let params: [String: Any?] = [
            "Id": self.ecoData?.Id,
            "Checked": self.isUsed
        ]
        
        NetworkManager().requestDataAsync(type: String.self, APP_URL.USER_UPDATE_ECO_ELEMENT, params, method: .put){response in
            
        }
    }
    

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        
        // Configure the view for the selected state
    }

}
