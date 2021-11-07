//
//  ListenListTableViewCell.swift
//  GardifyIOS
//
//  Created by Netzlab on 07.09.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class ListenListTableViewCell: UITableViewCell {

    @IBOutlet weak var editButton: UIButton!
    @IBOutlet weak var titleLabel: UILabel!
    @IBOutlet weak var filterButton: UIButton!
    
    var gardenId: Int?
    var gardenData: UserPlantModel?
    var viewController: UIViewController?
    var selectDelegate: MyGardenOptionDelegate?

    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
        self.backgroundColor = .clear
    }
    
    
    @IBAction func onEdit(_ sender: Any) {
        
        viewController?.dismiss(animated: true, completion: {
            self.selectDelegate?.goToEditGarden(gardenItem: self.gardenData!)
        })
    }
    
    @IBAction func onFilterButtonClicked(_ sender: Any) {
        print("filter button clicked")
        viewController?.dismiss(animated: true, completion: {
            self.selectDelegate?.updateGardenPlantFilter(listId: self.gardenData?.Id ?? -1)
        })
    }
    
    @IBAction func onDelete(_ sender: Any) {
        viewController?.dismiss(animated: true, completion: {
            self.selectDelegate?.goToDeleteGarden(gardenItem: self.gardenData!)
        })
    }
    
    func onConfigure(){
        
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}
