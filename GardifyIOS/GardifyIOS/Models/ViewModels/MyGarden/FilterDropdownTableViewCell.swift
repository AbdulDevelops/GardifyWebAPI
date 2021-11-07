//
//  FilterDropdownTableViewCell.swift
//  GardifyIOS
//
//  Created by Netzlab on 11.09.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class FilterDropdownTableViewCell: UITableViewCell {

    @IBOutlet weak var filterImage: UIImageView!
    
    @IBOutlet weak var filterLabel: UILabel!
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
        self.contentView.backgroundColor = .clear
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}
