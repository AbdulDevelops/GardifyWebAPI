//
//  UIWindow.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 21.09.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation
import UIKit

extension UIWindow{
    
    func setTheme(){
        if UserDefaultKeys.darkmode.bool(){
            self.overrideUserInterfaceStyle = .dark
            
        }
        else{
            self.overrideUserInterfaceStyle = .light
            
        }
    }
}
