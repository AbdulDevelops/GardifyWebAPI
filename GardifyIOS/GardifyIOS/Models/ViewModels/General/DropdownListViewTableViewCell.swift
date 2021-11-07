//
//  DropdownListViewTableViewCell.swift
//  GardifyIOS
//
//  Created by Netzlab on 09.12.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class DropdownListViewTableViewCell: UITableViewCell {

    var parent: DropdownListView?
    
    @IBOutlet weak var cellLabel: UILabel!
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }

    @IBAction func onClick(_ sender: Any) {
        parent?.selectedIndex = self.tag
        parent?.updateTable()
    }
    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }
    
}
