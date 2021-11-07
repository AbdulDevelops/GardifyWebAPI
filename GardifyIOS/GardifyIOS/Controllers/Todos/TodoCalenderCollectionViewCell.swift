//
//  TodoCalenderCollectionViewCell.swift
//  GardifyIOS
//
//  Created by Netzlab on 17.11.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class TodoCalenderCollectionViewCell: UICollectionViewCell {
    
    @IBOutlet weak var dateLabel: UILabel!
    
    @IBOutlet weak var notifImage: UIImageView!
    @IBOutlet weak var selectedView: UIView!
    var dayData: Day?
    
    func onConfigure(data: Day, isSelected: Bool, hasTodo: Bool = false){
        dateLabel.text = data.number
        dateLabel.alpha = data.isWithinDisplayedMonth ? 1 : 0.25
        
        selectedView.addBorderRadiusSmall()
        dateLabel.textColor = .label
        selectedView.alpha = 0
        notifImage.alpha = 0
        
        if hasTodo{
            notifImage.alpha = 1
        }
        
        if isSelected{
            dateLabel.textColor = .systemBackground
            selectedView.alpha = 1
        }
        
//        if !data.isWithinDisplayedMonth
    }
    
}
