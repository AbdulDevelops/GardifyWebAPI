//
//  DetailFilterFixedDoubleTableViewCell.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 28.09.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class DetailFilterFixedDoubleTableViewCell: UITableViewCell {

    @IBOutlet weak var filterLabel1: UILabel!
    
    @IBOutlet weak var filterLabel2: UILabel!
    var detailDelegate: DetailTableSelectDelegate?
    @IBOutlet weak var labelImage1: UIImageView!
    @IBOutlet weak var labelImage2: UIImageView!
    
    @IBOutlet weak var selectButton1: UIButton!
    @IBOutlet weak var selectButton2: UIButton!
    var filterType: FilterType?

    
    var labelId1: Int?
    var labelId2: Int?
    
    var filterSelected1: Bool = false
    var isSingle: Bool = false
    
    func filterSelection1(isSelected: Bool){
        
        filterSelected1 = isSelected
        selectionUpdate()
    }
    
    var filterSelected2: Bool = false
    
    func filterSelection2(isSelected: Bool){
        
        filterSelected2 = isSelected
        selectionUpdate()
    }
    
    func selectionUpdate(){
        
        if filterType == FilterType.colorsTag{
            labelImage1.addBorderRadiusSmall()
            labelImage1.addBorderWidth()
            let color = ColorsKey[self.filterLabel1.text ?? ""] ?? UIColor.black
            labelImage1.backgroundColor = color
            
            labelImage1.tintColor = .gray
            
            labelImage2.addBorderRadiusSmall()
            labelImage2.addBorderWidth()
            let color2 = ColorsKey[self.filterLabel2.text ?? ""] ?? UIColor.black
            labelImage2.backgroundColor = color2
            
            labelImage2.tintColor = .gray
//            labelImage1.image =  nil
            
            if filterSelected1{
                labelImage1.image = UIImage(systemName:  "checkmark")!.resizedSize(to: CGSize(width: 25, height: 25)).withTintColor(.gray)
            }
            else{
                labelImage1.image = nil
            }
            
            if filterSelected2{
                labelImage2.image = UIImage(systemName:  "checkmark")!.resizedSize(to: CGSize(width: 25, height: 25)).withTintColor(.gray)
            }
            else{
                labelImage2.image = nil
            }
            return
        }
        labelImage1.backgroundColor = .clear
        labelImage1.tintColor = .gray
        labelImage1.clearBorderWidth()
        
        labelImage2.backgroundColor = .clear
        labelImage2.tintColor = .gray
        labelImage2.clearBorderWidth()

        if filterSelected1{
            labelImage1.image = UIImage(systemName:  "checkmark")!
       
        }
        else{
            labelImage1.image = UIImage(systemName:  "circle")
        }
        
        if filterSelected2{
            labelImage2.image = UIImage(systemName:  "checkmark")
      
        }
        else{
            labelImage2.image = UIImage(systemName:  "circle")
        }
    }
    @IBAction func onSelect1(_ sender: Any) {
        self.detailDelegate?.isSelected(id: labelId1!, name: filterLabel1.text ?? "")
    }
    @IBAction func onSelect2(_ sender: Any) {
        if !isSingle{
            self.detailDelegate?.isSelected(id: labelId2!, name: filterLabel2.text ?? "")

        }

    }
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
//        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}
