//
//  searchTagCollectionViewCell.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 31.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class searchTagCollectionViewCell: UICollectionViewCell {
    
    @IBOutlet weak var outerView: UIView!
    @IBOutlet weak var tagLabel: UILabel!
    func onConfigure(title: String){
        
        tagLabel.textColor = .white
        tagLabel.text = title
    }
    
    
}
