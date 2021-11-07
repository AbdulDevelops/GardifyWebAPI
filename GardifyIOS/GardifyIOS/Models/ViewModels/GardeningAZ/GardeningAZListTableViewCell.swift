//
//  GardeningAZListTableViewCell.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 29.10.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class GardeningAZListTableViewCell: UITableViewCell {

    
    @IBOutlet weak var outerView: UIView!
    @IBOutlet weak var titleLabel: UILabel!
    @IBOutlet weak var contentLabel: UILabel!
    
    var isExtended = false
    var parentTable: UITableView?

    var descText: String = ""
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }
    
    func onConfigure(){
        
        self.backgroundColor = .clear
        self.contentView.backgroundColor = .clear
        self.outerView.addBorderRadius()
    }
    
    func updateDropdown(){
        if isExtended{
            self.contentLabel.text =  descText
        }
        else{
            self.contentLabel.text = nil
        }
        
        self.parentTable?.reloadData()
    }

    @IBAction func onDropdownClicked(_ sender: Any) {
        isExtended = !isExtended
        updateDropdown()
    }
    
    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}
