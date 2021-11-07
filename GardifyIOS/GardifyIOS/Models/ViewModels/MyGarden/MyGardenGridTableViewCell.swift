//
//  MyGardenGridTableViewCell.swift
//  GardifyIOS
//
//  Created by Netzlab on 09.11.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class MyGardenGridTableViewCell: UITableViewCell {

    @IBOutlet weak var gridImage1: UIImageView!
    
    @IBOutlet weak var gridImage2: UIImageView!
    @IBOutlet weak var gridImage3: UIImageView!
    
    @IBOutlet weak var button1: UIButton!
    
    @IBOutlet weak var button2: UIButton!
    
    @IBOutlet weak var button3: UIButton!
    
    var viewController: MyGardenViewController?
    var plantIdList: [Int] = []
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }
    
    @IBAction func onButton1Click(_ sender: Any) {
        if plantIdList.count < 1{
            return
        }
        
        self.viewController?.goToPlantDetails(plantId: self.plantIdList[0])
    }
    
    @IBAction func onButton2Click(_ sender: Any) {
        if plantIdList.count < 2{
            return
        }
        
        self.viewController?.goToPlantDetails(plantId: self.plantIdList[1])
    }
    
    @IBAction func onButton3Click(_ sender: Any) {
        if plantIdList.count < 3{
            return
        }
        
        self.viewController?.goToPlantDetails(plantId: self.plantIdList[2])
    }
    
    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}
