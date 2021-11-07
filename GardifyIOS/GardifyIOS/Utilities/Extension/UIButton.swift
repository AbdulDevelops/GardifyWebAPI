//
//  UIButton.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 17.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation
import UIKit

extension UIButton{
    
    func setCustomSearchButton(){
//        self.layer.addBorder(toSide: .bottom, withColor: CGColor(srgbRed: 0, green: 0, blue: 0, alpha: 1), andThickness: 1)
        self.contentHorizontalAlignment = .left
        self.setTitleColor(UIColor.gray, for: .normal)
    }
    
    func setCustomColorButton(color: UIColor){
        self.addBorderRadius()
        self.setTitleColor(.white, for: .normal)
        self.backgroundColor = color
    }
    
    func setGreenButton(){
        self.addBorderRadius()
        self.setTitleColor(.white, for: .normal)
        self.backgroundColor = UIColor(displayP3Red: 122/255, green: 157/255, blue: 52/255, alpha: 1)
    }
    

    
    func setRedButton(){
        self.addBorderRadius()
        self.setTitleColor(.white, for: .normal)
        self.backgroundColor = rgb(220, 53, 69)
    }
    
    func setGrayButton(){
        self.addBorderRadius()
        self.setTitleColor(.white, for: .normal)
        self.backgroundColor = .gray
    }
    
    
    func setWhiteButton(){
        self.addBorderRadius()
        self.setTitleColor(.label, for: .normal)
        self.backgroundColor = .systemBackground
        self.addShadow()
    }
    
    func setClearWhiteButton(){
        self.addBorderRadius()
        self.setTitleColor(.label, for: .normal)
        self.backgroundColor = .systemBackground
    }
    

    
}

@IBDesignable class RoundButton : UIButton{

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
        refreshCorners(value: cornerRadius)
    }

    func refreshCorners(value: CGFloat) {
        layer.cornerRadius = value
    }

    @IBInspectable var cornerRadius: CGFloat = 15 {
        didSet {
            refreshCorners(value: cornerRadius)
        }
    }
}
