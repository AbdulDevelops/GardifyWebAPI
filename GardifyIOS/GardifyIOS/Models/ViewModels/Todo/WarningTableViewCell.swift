//
//  WarningTableViewCell.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 28.10.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class WarningTableViewCell: UITableViewCell {

    
    @IBOutlet weak var outerView: UIView!
    
    
    @IBOutlet weak var warningSwitch: UISwitch!
    
    @IBOutlet weak var warningLabel: UILabel!
    
    @IBOutlet weak var isInPotImage: UIImageView!
    
    @IBOutlet weak var dropdownButton: UIButton!
    @IBOutlet weak var dropdownImage: UIImageView!
    
    @IBOutlet weak var alertConditionLabel: UILabel!
    
    var tempDescription: String = ""
    
    var isDropdown:Bool = false
    var parent: FullTableView?
    
    var relatedObjectId:Int = 0
    
    var isFrostWarning:Bool = true
    
    var warningData: WarningModel?
    
    var controllerData : WarningViewController?
    
    var isPlantData : Bool = false
    
    
    @IBOutlet weak var descriptionLabel: UILabel!
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }
    
    
    @IBAction func onDropdownClick(_ sender: Any) {
        isDropdown = !isDropdown
        updateDropdownHeight()
        parent?.reloadData()
    }
    
    @IBAction func onWarningSwitchToggle(_ sender: Any) {
        
        
        let sendURL = (self.isPlantData == true ? APP_URL.UPDATE_PLANT_WARNING_NOTIFICATION : APP_URL.UPDATE_DEVICE_WARNING_NOTIFICATION) + "\(self.relatedObjectId)" + "/" + "\(self.isFrostWarning)"
        
        NetworkManager().requestDataAsync(type: String?.self, sendURL, printRequest : true){response in
            if response.success{
                //let result = response.result as! String?
                //Response not taken into consideration since it is of no use after API result comes
            }
            
            self.controllerData?.loadWarning()
            self.parent?.reloadData()
            return
                
        }
    }
    
    
    func updateDropdownHeight() {
        if isDropdown{
            
            descriptionLabel.text = tempDescription
            dropdownImage.flipXAxis()
            
        }
        else{
           
            self.descriptionLabel.text = ""
            dropdownImage.revertFlip()
            
        }
    }
    
    func onConfigure(){
        self.backgroundColor = .clear
        self.contentView.backgroundColor = .clear
        
        self.outerView.addBorderRadius()
        
        self.updateDropdownHeight()
    }
    
    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}
