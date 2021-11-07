//
//  UIImageView.swift
//  GardifyText
//
//  Created by Netzlab on 27.07.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit
import Alamofire

extension UIImageView{
    public func imageFromUrl(urlString: String){
        Alamofire.request(urlString).response{ response in
            guard let data = response.data else{
                return
            }
            self.image = UIImage(data: data, scale: 1)
        }
    }
    
    func flipXAxis(){
        self.transform = CGAffineTransform(scaleX: 1, y: -1)
    }
    
    func flipYAxis(){
        self.transform = CGAffineTransform(scaleX: -1, y: 1)
    }
    
    func rotateCW(){
        self.transform = CGAffineTransform(rotationAngle: 3.14 * 0.5)
    }
    
    func rotateCCW(){
        self.transform = CGAffineTransform(rotationAngle: 3.14 * 1.5)
    }
    
    func revertFlip(){
        self.transform = CGAffineTransform(scaleX: 1, y: 1)
        
        self.transform = CGAffineTransform(rotationAngle: 0)
    }
}

@IBDesignable class CircularImageView: UIImageView {
    
    override var image: UIImage? {
        didSet {
            super.image = image?.circularImage(size: nil)
        }
    }
    
}
