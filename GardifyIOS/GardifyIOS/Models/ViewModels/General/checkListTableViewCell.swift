//
//  AddGardenPlantListTableViewCell.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 19.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class checkListTableViewCell: UITableViewCell {

    @IBOutlet weak var checkImage: UIImageView!
    @IBOutlet weak var plantPositionLabel: UILabel!
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }
    
    func onConfigure(text: String){
        self.plantPositionLabel.text = text
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated) 

        if selected{
            self.checkImage.image = UIImage(systemName: "checkmark.square.fill")
        }
        else{
            self.checkImage.image = UIImage(systemName: "square")
        }
        // Configure the view for the selected state
    }

}
