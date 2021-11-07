//
//  EcoCategoryCollectionViewCell.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 19.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class EcoCategoryCollectionViewCell: UICollectionViewCell {
    
    @IBOutlet weak var categoryImage: UIImageView!
    
    func onConfigure(image: UIImage){
        DispatchQueue.main.async {
            self.categoryImage.image = image
        }
    }
}
