//
//  HomeViewCell.swift
//  GardifyText
//
//  Created by Netzlab on 27.07.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class HomeViewCell: UICollectionViewCell {
    
    @IBOutlet weak var outerView: UIView!
    @IBOutlet weak var cellFirstLabel: UILabel!
    @IBOutlet weak var cellSecondLabel: UILabel!
    @IBOutlet weak var iconImage: UIImageView!
    @IBOutlet weak var imageHeightConstraint: NSLayoutConstraint!
    @IBOutlet weak var imageWidthConstraint: NSLayoutConstraint!
    
    @IBOutlet var numberCounterLabel: UILabel!
    @IBOutlet var numberImage: UIImageView!
    @IBOutlet weak var iconBottomConstraint: NSLayoutConstraint!
    
    func setCounterNumber(number: Int){
        numberImage.alpha = 0
        numberCounterLabel.text = ""
        if number > 0{
            numberImage.alpha = 1
            numberCounterLabel.text = "\(number)"
        }
    }
    
    func onConfigure(image: String,
                     imageSize: CGFloat,
                     titleTextFirst: String,
                     titleTextSecond: String,
                     cellColor: UIColor,
                     isFirstBold: Bool){
        
        iconImage.image = UIImage(named: image)
        imageHeightConstraint.constant = -imageSize
        iconBottomConstraint.constant = 10 + imageSize / 2
     
        styleConfiguration(backgroundColor: cellColor, textColor: .white)
        // Content Modifier
        cellFirstLabel.text = titleTextFirst
        cellSecondLabel.text = titleTextSecond
        
        if isFirstBold{
            cellFirstLabel.font = UIFont.systemFont(ofSize: 16, weight: .black)
            cellFirstLabel.addCharacterSpacing(kernValue: 1.8)

            cellSecondLabel.addCharacterSpacing(kernValue: 1.5)
            cellSecondLabel.font = UIFont.systemFont(ofSize: 16, weight: .regular)

        }
        else{
            cellFirstLabel.font = UIFont.systemFont(ofSize: 16, weight: .regular)
            cellFirstLabel.addCharacterSpacing(kernValue: 1.5)

            cellSecondLabel.font = UIFont.systemFont(ofSize: 16, weight: .black)
            cellSecondLabel.addCharacterSpacing(kernValue: 1.8)

        }
    }
    
    func styleConfiguration(backgroundColor: UIColor,
                            textColor: UIColor){
        self.backgroundColor = .clear
        self.outerView.backgroundColor = backgroundColor
        self.cellFirstLabel.textColor = textColor
        self.cellSecondLabel.textColor = textColor

        self.outerView.addBorderRadius()
    }
}
