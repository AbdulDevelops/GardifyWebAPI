//
//  reusableCollectionView.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 19.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation
import UIKit

class ColorCollectionList: NSObject, UICollectionViewDelegate, UICollectionViewDataSource{
    
    
    var colorData: [String] = []
    
    func collectionView(_ collectionView: UICollectionView, numberOfItemsInSection section: Int) -> Int {

        return colorData.count
    }
    
    func collectionView(_ collectionView: UICollectionView, cellForItemAt indexPath: IndexPath) -> UICollectionViewCell {
        
        let collectionCell = collectionView.dequeueReusableCell(withReuseIdentifier: "colorCell", for: indexPath) as! ColorCollectionViewCell
        collectionCell.onConfigure(color: ColorsKey[colorData[indexPath.row]]!)
        
        return collectionCell
    }
}



class EcoCategoryCollectionList: NSObject, UICollectionViewDelegate, UICollectionViewDataSource{
    
    var badgesId: [String] = []
    func collectionView(_ collectionView: UICollectionView, numberOfItemsInSection section: Int) -> Int {
        
        return badgesId.count
    }
    
    func collectionView(_ collectionView: UICollectionView, cellForItemAt indexPath: IndexPath) -> UICollectionViewCell {
        
        let cell = collectionView.dequeueReusableCell(withReuseIdentifier: "ecoCategoryCell", for: indexPath) as! EcoCategoryCollectionViewCell
        
//        cell.onConfigure(image: getEcoImage(key: 447))
                cell.onConfigure(image: getEcoImageString(key: badgesId[indexPath.row]))
        
        return cell
    }
    
    
}
