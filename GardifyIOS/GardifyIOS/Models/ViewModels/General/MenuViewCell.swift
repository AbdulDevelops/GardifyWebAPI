//
//  MenuViewCell.swift
//  GardifyText
//
//  Created by Netzlab on 30.07.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class MenuViewCell: UITableViewCell {

    @IBOutlet weak var menuLabel: UILabel!
    @IBOutlet weak var secondaryLabel: UILabel!
    
    var isMainMenu: Bool = true
    
    var isFrontBold: Bool = true
    
    var isBotBorderOn: Bool = true
    
    @IBOutlet weak var grayView: UIView!
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }
    
    func onConfigure(title: String){
        
        pageConfiguration(title: title)
    }
    
    func pageConfiguration(title: String){
        
        var splitWord = title.components(separatedBy: " ")
        self.menuLabel.text = String(splitWord[0]).uppercased()
        self.secondaryLabel.text = ""
        if splitWord.count > 1{
            self.secondaryLabel.text = String(splitWord[1]).uppercased()
        }
        
        self.menuLabel.font = isFrontBold ? UIFont.systemFont(ofSize: 17, weight: .semibold) :  UIFont.systemFont(ofSize: 17, weight: .light)
        
        self.secondaryLabel.font = !isFrontBold ? UIFont.systemFont(ofSize: 16, weight: .semibold) :  UIFont.systemFont(ofSize: 16, weight: .light)
        
        self.grayView.alpha = isBotBorderOn ? 1 : 0
        
        self.contentView.backgroundColor = isMainMenu ? .systemBackground : .systemGray5
        
        
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }
    
    

}
