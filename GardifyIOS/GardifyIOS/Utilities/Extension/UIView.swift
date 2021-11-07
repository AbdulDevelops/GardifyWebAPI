//
//  UIView.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 17.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation
import UIKit

extension UIView{
    
    @objc     func addBorderRadius(){
        self.layer.cornerRadius = 10
    }
    
    func clearBorderRadius(){
        self.layer.cornerRadius = 0
    }
    
    func addCustomBorderRadius(topLeft : Bool, topRight: Bool, botLeft: Bool, botRight: Bool){
        
        self.clipsToBounds = true
        self.layer.cornerRadius = 10
        
        var maskList: CACornerMask = []
        
        
        if topLeft{
            maskList.insert(.layerMinXMinYCorner)
        }
        if topRight{
            maskList.insert(.layerMaxXMinYCorner)
        }
        if botLeft{
            maskList.insert(.layerMinXMaxYCorner)
        }
        if botRight{
            maskList.insert(.layerMaxXMaxYCorner)
        }
//
        self.layer.maskedCorners = maskList
        
    }
    
    func addBorderWidth(){
        self.layer.borderWidth = 1
        self.layer.borderColor = CGColor(srgbRed: 0.1, green: 0.1, blue: 0.1, alpha: 0.2)
    }
    
    func clearBorderWidth(){
        self.layer.borderWidth = 0
    }
    
    func addBorderWidthThick(){
        self.layer.borderWidth = 3
        self.layer.borderColor = CGColor(srgbRed: 0.1, green: 0.1, blue: 0.1, alpha: 1)
    }
    
    func addMaskBound(){
        self.layer.masksToBounds = true
    }
    
    func addBorderRadiusSmall(){
        self.layer.cornerRadius = 5
        self.layer.masksToBounds = true
    }
    
    enum ViewSide {
        case left, right, top, bottom
    }
    
    func applyTheme(){
        self.backgroundColor = .Background

    }
    
    func addBorder(toSide side: ViewSide, withColor color: CGColor, andThickness thickness: CGFloat) {
        
        let border = CALayer()
        border.backgroundColor = color
        
        switch side {
        case .left: border.frame = CGRect(x: self.frame.origin.x, y: self.frame.origin.y, width: thickness, height: self.frame.size.height)
        case .right: border.frame = CGRect(x: self.frame.size.width - thickness, y: self.frame.origin.y, width: thickness, height: self.frame.size.height)
        case .top: border.frame = CGRect(x: self.frame.origin.x, y: self.frame.origin.y, width: self.frame.size.width, height: thickness)
        case .bottom: border.frame = CGRect(x: self.frame.origin.x, y: self.frame.size.height - thickness, width: self.frame.size.width, height: thickness)
        }
        self.layer.addSublayer(border)
    }
    
    func addShadow(){
 
        self.layer.shadowColor = UIColor.black.cgColor
        self.layer.shadowOpacity = 0.2
        self.layer.shadowOffset = .init(width: 0, height: 2)
        self.layer.shadowRadius = 2

    }
    
    func setGreenButtonView(){
        self.addBorderRadius()
        self.backgroundColor = UIColor(displayP3Red: 122/255, green: 157/255, blue: 52/255, alpha: 1)
    }
    
    func setWhiteNoShadowView(){
        self.addBorderRadius()
        self.backgroundColor = .systemBackground
        self.clearBorderWidth()
    }
    
    func setWhiteButtonView(){
        self.addBorderRadius()
        self.backgroundColor = .systemBackground
        self.addShadow()
    }
    
    func asImage() -> UIImage {
        if #available(iOS 10.0, *) {
            let renderer = UIGraphicsImageRenderer(bounds: bounds)
            return renderer.image { rendererContext in
                layer.render(in: rendererContext.cgContext)
            }
        } else {
            UIGraphicsBeginImageContext(self.frame.size)
            self.layer.render(in:UIGraphicsGetCurrentContext()!)
            let image = UIGraphicsGetImageFromCurrentImageContext()
            UIGraphicsEndImageContext()
            return UIImage(cgImage: image!.cgImage!)
        }
    }
}

@IBDesignable class RoundView : UIView{
    
    override init(frame: CGRect) {
        super.init(frame: frame)
        sharedInit()
    }
    
    required init?(coder aDecoder: NSCoder) {
        super.init(coder: aDecoder)
        sharedInit()
    }
    
    override func prepareForInterfaceBuilder() {
        sharedInit()
    }
    
    func sharedInit() {
        self.addBorderRadius()
    }
    

}
