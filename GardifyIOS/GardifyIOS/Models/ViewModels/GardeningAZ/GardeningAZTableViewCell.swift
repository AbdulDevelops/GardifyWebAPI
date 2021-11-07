//
//  GardeningAZTableViewCell.swift
//  GardifyIOS
//
//  Created by Netzlab on 04.09.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class GardeningAZTableViewCell: UITableViewCell {

    @IBOutlet weak var titleLabel: UILabel!
   
    @IBOutlet weak var gardeningImageView: UIImageView!

    @IBOutlet weak var outerView: UIView!
    @IBOutlet weak var gardeningDescription: UILabel!
    
    @IBOutlet weak var arrowDownImage: UIImageView!
    
    var isInfoExtended: Bool = false
    var parentTable: UITableView?
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }
    
    func onConfigure(){
        configureDropdown()
        self.outerView.addBorderRadius()
        self.backgroundColor = .clear
        self.contentView.backgroundColor = .clear
    }
    
    func configureDropdown(){
        
        if isInfoExtended{
            gardeningDescription.numberOfLines = 0
            arrowDownImage.flipXAxis()
        }
        else{
            gardeningDescription.numberOfLines = 3
            arrowDownImage.revertFlip()
        }
        
      
    }
    
    
    @IBAction func onDropdownToggle(_ sender: Any) {
        isInfoExtended = !isInfoExtended
        
        configureDropdown()
        
        DispatchQueue.main.async {
            self.parentTable?.reloadData()
        }
    }
    
    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}
