
//
//  FullTableView.swift
//  GardifyIOS
//
//  Created by Netzlab on 17.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation
import UIKit

class FullTableView: UITableView{
    override var intrinsicContentSize: CGSize{
        self.layoutIfNeeded()
        return self.contentSize
    }
    
    override var contentSize: CGSize{
        didSet{
            self.invalidateIntrinsicContentSize()
        }
    }
    
    override func reloadData() {
        super.reloadData()
        self.invalidateIntrinsicContentSize()
    }
    
    
}

class FullCollectionView: UICollectionView{
    
    override var intrinsicContentSize: CGSize{
        self.layoutIfNeeded()
        return self.contentSize
    }
    
    override var contentSize: CGSize{
        didSet{
            self.invalidateIntrinsicContentSize()
        }
    }
    
    override func reloadData() {
        super.reloadData()
        
  
        self.invalidateIntrinsicContentSize()
    }
}
