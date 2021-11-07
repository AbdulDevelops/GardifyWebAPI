//
//  MinimalTextField.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 17.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation
import UIKit



@IBDesignable
class MinimalTextField: UITextField {
    
    @IBInspectable var horizontalInset: CGFloat = 0
    @IBInspectable var verticalInset: CGFloat = 0
    
    
    override func textRect(forBounds bounds: CGRect) -> CGRect {
        return bounds.insetBy(dx: horizontalInset, dy: verticalInset)
    }
    
    override func editingRect(forBounds bounds: CGRect) -> CGRect {
        return textRect(forBounds: bounds)
    }
    
}
