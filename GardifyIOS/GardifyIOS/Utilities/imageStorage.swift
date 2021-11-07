//
//  imageStorage.swift
//  GardifyIOS
//
//  Created by Netzlab on 13.10.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation
import UIKit


 func store(image: UIImage, forKey key: String) {
    if let pngRepresentation = image.pngData() {
        UserDefaults.standard.set(pngRepresentation, forKey: key)
    }
}

func removeImageFromStorage(forKey key: String){
    UserDefaults.standard.removeObject(forKey: key)
}

 func retrieveImage(forKey key: String) -> UIImage? {
    if let imageData = UserDefaults.standard.object(forKey: key) as? Data,
                let image = UIImage(data: imageData) {
                
                return image
            }
    return nil
}
