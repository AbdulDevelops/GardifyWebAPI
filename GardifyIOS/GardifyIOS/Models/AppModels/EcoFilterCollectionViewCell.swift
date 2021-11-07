//
//  EcoFilterCollectionViewCell.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 20.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class EcoFilterCollectionViewCell: UICollectionViewCell {
    
    @IBOutlet weak var ecoCountLabel: UILabel!
    @IBOutlet weak var ecoFilterImage: UIImageView!
    
    @IBOutlet weak var ecoFilterSelectedView: UIView!
    var viewController: UIViewController?
    var identifier = ""
    
    func setGesture(identifier: String){
        self.identifier = identifier
        let longGesture = UILongPressGestureRecognizer(target: self, action: #selector(long))  //Long function will call when user long press on button.
        
        self.contentView.addGestureRecognizer(longGesture)
    }
    
    @objc func long() {
        
        self.viewController?.ShowTitleAlert(title: self.identifier.replacingOccurrences(of: "_", with: " "), message: ecoInformation[self.identifier]!)
        print("Long press")
    }
    
}
