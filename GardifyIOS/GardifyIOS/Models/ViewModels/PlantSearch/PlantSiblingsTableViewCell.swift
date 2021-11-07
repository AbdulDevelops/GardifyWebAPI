//
//  PlantSiblingsTableViewCell.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 19.11.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class PlantSiblingsTableViewCell: UITableViewCell {
    
    var viewController: PlantDetailViewController?
    var plantId : Int = 0
    
    @IBOutlet weak var image1: UIImageView!
    
    
    @IBAction func onSiblingClick(_ sender: Any) {
        
//        self.viewController?.plantId = self.plantId
//        
//        self.viewController?.viewDidLoad()
        
        self.viewController?.goToPlantDetails(plantId : self.plantId)
    }
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}
