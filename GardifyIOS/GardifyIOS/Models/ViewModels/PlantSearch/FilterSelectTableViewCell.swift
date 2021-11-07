//
//  FilterSelectTableViewCell.swift
//  GardifyIOS
//
//  Created by Netzlab on 18.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class FilterSelectTableViewCell: UITableViewCell {

    @IBOutlet weak var filterNameLabel: UILabel!
    
    @IBOutlet weak var selectedStatusImage: UIImageView!
    
    @IBOutlet weak var filterImage: UIImageView!
    @IBOutlet weak var filterImageWidth: NSLayoutConstraint!
    
    var isColor: Bool = false
    var isEcoCat:Bool = false
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }
    
    func onConfigure(text: String){
        pageConfiguration(text)

    }
    
    func pageConfiguration(_ text: String){
        
        self.filterNameLabel.text = text
        let bgColor = UIView()
        bgColor.backgroundColor = .clear
        self.selectedBackgroundView = bgColor
        self.tintColor = .gray
        
        if isEcoCat{
            filterImageWidth.constant = 30
            if let image = UIImage(named: text.replacingOccurrences(of: " ", with: "_")) {
                filterImage.image = image
            }
        }
        else{
            filterImageWidth.constant = 0
        }
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        if isColor{
            selectedStatusImage.addBorderRadiusSmall()
            selectedStatusImage.addBorderWidth()
            let color = ColorsKey[self.filterNameLabel.text ?? ""] ?? UIColor.black
            selectedStatusImage.backgroundColor = color

            selectedStatusImage.tintColor = color

            
        }
        
        if selected{
            if isColor{
                self.backgroundColor = .gray
                
            }
            else{
                selectedStatusImage.image = UIImage(systemName:  "checkmark")
            }
            
        }
        else{
            if isColor{
                selectedStatusImage.image =  nil
                self.backgroundColor = .clear

                
            }
            else{
                selectedStatusImage.image = UIImage(systemName: "circle")
            }
        }
        // Configure the view for the selected state
    }
    
    

}
