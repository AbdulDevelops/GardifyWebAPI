//
//  PlantSearchTableViewCell.swift
//  GardifyIOS
//
//  Created by Netzlab on 05.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class PlantSearchTableViewCell: UITableViewCell {

    @IBOutlet weak var nameGermanLabel: UILabel!
    @IBOutlet weak var nameLatinLabel: UILabel!
    
    var plantData: PlantDetailModel?
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }
    
    func onConfigure(plant: PlantDetailModel){
        
        plantData = plant
        pageConfiguration()
    }
    
    private func pageConfiguration(){
        print("plant is", (self.plantData?.NameLatin))
        nameLatinLabel.text =  "aaa"
        nameGermanLabel.text =  "bbb"
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }
    
}
