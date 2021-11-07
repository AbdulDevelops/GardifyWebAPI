//
//  UISwitch.swift
//  GardifyIOS
//
//  Created by Netzlab on 23.12.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation
import UIKit

extension UISwitch{
    
    func set(width: CGFloat, height: CGFloat){
        let standardHeight: CGFloat = 31
        
        let standardWidth: CGFloat = 51
        
        let heightRatio = height / standardHeight
        
        let widthRatio = width / standardWidth
        
        self.transform = CGAffineTransform(scaleX: widthRatio, y: heightRatio)
    }
}
