//
//  ToDoTableViewCell.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 19.11.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class ToDoTableViewCell: UITableViewCell {
    
    @IBOutlet weak var todoTitleLabel: UILabel!
    @IBOutlet weak var todoDescriptionLabel: UILabel!

    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}
