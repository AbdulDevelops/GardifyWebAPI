//
//  ColorCollectionViewCell.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 19.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class ColorCollectionViewCell: UICollectionViewCell {
    
    @IBOutlet weak var outerView: UIView!
    
    func onConfigure(color: UIColor){
        
        self.outerView.addBorderRadiusSmall()
        self.outerView.layer.borderWidth = 1
        self.outerView.layer.borderColor = CGColor(srgbRed: 0.7, green: 0.7, blue: 0.7, alpha: 0.5)
        self.outerView.backgroundColor = color
    }
}
