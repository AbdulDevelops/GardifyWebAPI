//
//  DetailFilterDropItemTableViewCell.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 30.09.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class DetailFilterDropItemTableViewCell: UITableViewCell {

    
    @IBOutlet weak var filterSelectImage: UIImageView!
    @IBOutlet weak var filterSelectLabel: UILabel!
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        self.backgroundColor = .clear
        if selected{
            filterSelectImage.image = UIImage(systemName:  "checkmark")
        }
        else{
            filterSelectImage.image = UIImage(systemName:  "circle")
        }
        // Configure the view for the selected state
    }

}
