//
//  CALayer.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 17.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation

import UIKit

extension CALayer{
    
    enum ViewSide {
        case left, right, top, bottom
    }
    
    func addBorder(toSide side: ViewSide, withColor color: CGColor, andThickness thickness: CGFloat) {
//        self.sublayers?[0].removeFromSuperlayer()
//        if self.sublayers != nil{
//
//            return
//        }
        self.borderWidth = 0
        self.borderColor = CGColor(srgbRed: 1, green: 1, blue: 1, alpha: 0)
        let border = CALayer()
        border.backgroundColor = color
        self.masksToBounds = true
        switch side {
        case .left: border.frame = CGRect(x: self.frame.origin.x, y: self.frame.origin.y, width: thickness, height: self.frame.size.height)
        case .right: border.frame = CGRect(x: self.frame.size.width - thickness, y: self.frame.origin.y, width: thickness, height: self.frame.size.height)
        case .top: border.frame = CGRect(x: self.frame.origin.x, y: self.frame.origin.y, width: self.frame.size.width, height: thickness)
        case .bottom: border.frame = CGRect(x: 0, y: self.frame.size.height - thickness, width: self.frame.size.width, height: thickness)
        }

        self.addSublayer(border)
        
    }
}
