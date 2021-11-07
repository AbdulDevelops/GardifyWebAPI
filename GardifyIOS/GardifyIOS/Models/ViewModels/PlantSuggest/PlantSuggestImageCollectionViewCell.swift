//
//  PlantSuggestImageCollectionViewCell.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 03.12.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class PlantSuggestImageCollectionViewCell: UICollectionViewCell {
    
    @IBOutlet weak var plantSuggestImage: UIImageView!
    
    
    @IBOutlet weak var imageButton: UIButton!
    var parent : UICollectionView?
    var controllerData : PlantSuggestViewController?
    var heightForCollection: NSLayoutConstraint!
    
    @IBAction func onImageClick(_ sender: Any) {
        print("entry")
        
        let refreshAlert = UIAlertController(title: "Lösche Bild", message: "Möchten Sie dieses Bild löschen?", preferredStyle: UIAlertController.Style.alert)
        
        refreshAlert.addAction(UIAlertAction(title: "Ok", style: .default, handler: { (action: UIAlertAction!) in
            print("Handle Ok logic here")
            let index = (self.controllerData?.imageFiles.firstIndex(of: self.plantSuggestImage.image!))! as Int
            self.controllerData?.imageFiles.remove(at: index)
            if self.controllerData?.imageFiles.count == 0 {
                self.heightForCollection.constant = 0
            }
            self.parent?.reloadData()
        }))
        
        refreshAlert.addAction(UIAlertAction(title: "Cancel", style: .cancel, handler: { (action: UIAlertAction!) in
            print("Handle Cancel Logic here")
            return
        }))
        
        self.controllerData?.present(refreshAlert, animated: true, completion: nil)
        
        
    }
    
}


