//
//  FilterCategoryListViewCell.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 27.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class FilterCategoryListViewCell: UITableViewCell {

    @IBOutlet weak var categoryImage: UIImageView!
    @IBOutlet weak var categoryLabel: UILabel!
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }
    
    func onConfigure(title: String, imageName: String){
        self.categoryLabel.text = title
        
        if let image = UIImage(named: "cat_"+imageName) {
            self.categoryImage.image = image
        }
        else{
            self.categoryImage.image = nil
        }
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state vc
    }

}
