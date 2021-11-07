//
//  DetailFilterFixedItemTableViewCell.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 28.09.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class DetailFilterFixedItemTableViewCell: UITableViewCell {

    
    @IBOutlet weak var filterLabel: UILabel!
    
    var detailDelegate: DetailTableSelectDelegate?
    
    @IBOutlet weak var onSelected: UIButton!
    @IBOutlet weak var labelImage: UIImageView!
    var labelId: Int?
    var filterSelected: Bool = false
    var filterType: FilterType?
    
    func filterSelection(isSelected: Bool){
        
        filterSelected = isSelected
        selectionUpdate()
    }
    
    func selectionUpdate(){
        print("filter type is", filterType)
        if filterType == FilterType.colorsTag{
            labelImage.addBorderRadiusSmall()
            labelImage.addBorderWidth()
            print("color key is", self.filterLabel.text)
            let color = ColorsKey[self.filterLabel.text ?? ""] ?? UIColor.black
            print("color value is", color)
            labelImage.backgroundColor = color

            labelImage.tintColor = .gray
            labelImage.image =  nil

            if filterSelected{
//                labelImage.image = UIImage(systemName:  "checkmark")
                self.backgroundColor = .gray

//                labelImage.image = UIImage(systemName:  "checkmark")

            }
            else{
                self.backgroundColor = .clear

                labelImage.image = nil
            }

            return
        }
//        else{
            labelImage.backgroundColor = .clear
            labelImage.tintColor = .gray
            labelImage.clearBorderWidth()
            if filterSelected{
                labelImage.image = UIImage(systemName:  "checkmark")
            }
            else{
                labelImage.image = UIImage(systemName:  "circle")
            }
//        }
        
    }
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }
    
    @IBAction func selectButtonPressed(_ sender: Any) {
        self.detailDelegate?.isSelected(id: labelId!, name: filterLabel.text ?? "")
    }
    

    override func setSelected(_ selected: Bool, animated: Bool) {
//        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}
