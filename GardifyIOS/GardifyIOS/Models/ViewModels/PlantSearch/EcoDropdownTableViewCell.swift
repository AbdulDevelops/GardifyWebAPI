//
//  EcoDropdownTableViewCell.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 24.09.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class EcoDropdownTableViewCell: UITableViewCell {

    @IBOutlet weak var ecoFilterImage: UIImageView!
    @IBOutlet weak var selectedEcoImage: UIImageView!
    @IBOutlet weak var ecoLabel: UILabel!
    
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }
    func onConfigure(text: String){
        pageConfiguration(text)
        
    }
    
    func pageConfiguration(_ text: String){
        
        self.ecoLabel.text = text
        let bgColor = UIView()
        bgColor.backgroundColor = .clear
        self.selectedBackgroundView = bgColor
        self.tintColor = .gray
        

        if let image = UIImage(named: text.replacingOccurrences(of: " ", with: "_")) {
            ecoFilterImage.image = image
        }
   
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        if selected{

            selectedEcoImage.image = UIImage(systemName:  "checkmark")
            
        }
        else{
      
            selectedEcoImage.image = UIImage(systemName: "circle")
        }
        // Configure the view for the selected state
    }

}
