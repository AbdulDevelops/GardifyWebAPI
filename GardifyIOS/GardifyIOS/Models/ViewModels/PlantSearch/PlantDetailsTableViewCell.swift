//
//  PlantDetailsTableViewCell.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 18.11.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class PlantDetailsTableViewCell: UITableViewCell {
    
    
    @IBOutlet weak var categoryNameLabel: UILabel!
    @IBOutlet weak var categoryValueLabel: UILabel!
    
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}
