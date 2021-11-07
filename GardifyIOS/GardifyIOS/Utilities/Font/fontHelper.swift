//
//  fontHelper.swift
//  GardifyIOS
//
//  Created by Netzlab on 19.11.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation
import UIKit

struct AppFontName {
  static let regular = "SourceSansPro-Regular"
  static let bold = "SourceSansPro-Bold"
  static let lightAlt = "SourceSansPro-Regular-Light"
    static let thin = "SourceSansPro-Light"
    static let semiBold = "SourceSansPro-SemiBold"
    
}

//customise font
extension UIFontDescriptor.AttributeName {
  static let nsctFontUIUsage = UIFontDescriptor.AttributeName(rawValue: "NSCTFontUIUsageAttribute")
}

extension UIFont {

  @objc class func mySystemFont(ofSize size: CGFloat) -> UIFont {
        return UIFont(name: AppFontName.regular, size: size)!
  }

  @objc class func myBoldSystemFont(ofSize size: CGFloat) -> UIFont {
        return UIFont(name: AppFontName.bold, size: size)!
  }
    
    @objc class func mySemiBoldSystemFont(ofSize size: CGFloat) -> UIFont {
          return UIFont(name: AppFontName.semiBold, size: size)!
    }

  @objc class func myItalicSystemFont(ofSize size: CGFloat) -> UIFont {
        return UIFont(name: AppFontName.lightAlt, size: size)!
  }
    
    

  @objc convenience init(myCoder aDecoder: NSCoder) {
    guard
        let fontDescriptor = aDecoder.decodeObject(forKey: "UIFontDescriptor") as? UIFontDescriptor,
        
        
        let fontAttribute = fontDescriptor.fontAttributes[.nsctFontUIUsage] as? String else {
        self.init(myCoder: aDecoder)
        return
    }
    
    var fontName = ""
    
    print("type is", fontAttribute)
    switch fontAttribute {
    case "CTFontRegularUsage":
        fontName = AppFontName.regular
    case "CTFontMediumUsage":
        fontName = AppFontName.semiBold
    case "CTFontEmphasizedUsage", "CTFontBoldUsage":
        fontName = AppFontName.bold
    case  "CTFontDemiUsage":
        fontName = AppFontName.semiBold
    case "CTFontObliqueUsage":
        fontName = AppFontName.lightAlt

    case "CTFontThinUsage":
        fontName = AppFontName.thin
    default:
        fontName = AppFontName.regular
    }
    
    self.init(name: fontName, size: fontDescriptor.pointSize)!
  }

  class func overrideInitialize() {
    guard self == UIFont.self else { return }

    if let systemFontMethod = class_getClassMethod(self, #selector(systemFont(ofSize:))),
        let mySystemFontMethod = class_getClassMethod(self, #selector(mySystemFont(ofSize:))) {
        method_exchangeImplementations(systemFontMethod, mySystemFontMethod)
    }

    if let boldSystemFontMethod = class_getClassMethod(self, #selector(boldSystemFont(ofSize:))),
        let myBoldSystemFontMethod = class_getClassMethod(self, #selector(myBoldSystemFont(ofSize:))) {
        method_exchangeImplementations(boldSystemFontMethod, myBoldSystemFontMethod)
    }

    if let italicSystemFontMethod = class_getClassMethod(self, #selector(italicSystemFont(ofSize:))),
        let myItalicSystemFontMethod = class_getClassMethod(self, #selector(myItalicSystemFont(ofSize:))) {
        method_exchangeImplementations(italicSystemFontMethod, myItalicSystemFontMethod)
    }

    if let initCoderMethod = class_getInstanceMethod(self, #selector(UIFontDescriptor.init(coder:))), // Trick to get over the lack of UIFont.init(coder:))
        let myInitCoderMethod = class_getInstanceMethod(self, #selector(UIFont.init(myCoder:))) {
        method_exchangeImplementations(initCoderMethod, myInitCoderMethod)
    }
  }
}
