//
//  UINavigationItem.swift
//  GardifyIOS
//
//  Created by Netzlab on 24.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation
import UIKit

extension UINavigationItem{
    @objc
    func addRightButton(width: CGFloat){
        
        let darkGreenColor = UIColor(displayP3Red: 106/255, green: 133/255, blue: 52/255, alpha: 1)
        
        let boxWidth = width * 8 / 9
        let height = CGFloat(40)
        let viewFN = UIView(frame: CGRect(x: -(width * 2 / 9), y: 0, width: boxWidth, height: height))
//        viewFN.backgroundColor = darkGreenColor
        
        let outerView = UIView(frame: CGRect.init(x: (width * 1 / 20), y:  -100, width: boxWidth + 100, height: height + 100))
        
        outerView.backgroundColor = darkGreenColor
        outerView.addBorderRadius()
        
        let plantLabel = setTitleLabel(x: width / 15, y: 0, width: width / 4, height: height / 4, text: "PFLANZEN", fontWeight: .light)
        
        
        let todoLabel = setTitleLabel(x: width / 3, y: 0, width: width / 4, height: height / 4, text: "TO-DO", fontWeight: .light)
        
        
        var plantCount = "0/0"
        var todoCount = "0/0"
        if UserDefaultKeys.IsLoggedIn.bool(){
            plantCount = "\(UserDefaultKeys.plantSort.int())/\(UserDefaultKeys.plantTotal.int())"
            
            todoCount = "\(UserDefaultKeys.todoMonth.int())/\(UserDefaultKeys.todoYear.int())"
        }
        
        let plantCountLabel = setTitleLabel(x: width / 15, y: height / 2, width: width / 4, height: height / 4, text: plantCount, fontWeight: .semibold)
        
        let todoCountLabel = setTitleLabel(x: width / 3, y: height / 2, width: width / 4, height: height / 4, text: todoCount, fontWeight: .semibold)


 

        let button3 = UIButton(frame: CGRect.init(x: boxWidth - 60, y: -20, width: 60, height: height))
        
        button3.addBorderRadius()
        
        button3.backgroundColor = darkGreenColor
        
        viewFN.addSubview(outerView)
        viewFN.addSubview(plantLabel)
      
        viewFN.addSubview(plantCountLabel)
        viewFN.addSubview(todoLabel)
        viewFN.addSubview(todoCountLabel)

        
        viewFN.addSubview(button3)
        
        viewFN.addBorderRadius()
        
        let rightBarButton = UIBarButtonItem(customView: viewFN)
        
        
        //
        //        self.navigationItem.rightBarButtonItems = [rightBarButton, blankButton]
        
        self.titleView = viewFN
        //        self.navigationItem.rightBarButtonItem = negativeButton
        
        
    }
    
    private func setTitleLabel(x: CGFloat, y: CGFloat, width: CGFloat, height: CGFloat, text: String, fontWeight: UIFont.Weight) -> UILabel{
        let label = UILabel(frame: CGRect.init(x: x, y:y, width: width, height: height))
        label.text = text
        label.textColor = .white
        label.font = UIFont.systemFont(ofSize: 15, weight: fontWeight)
        label.textAlignment = .center
        
        return label
    }
    
    func addHomeNavigation(width: CGFloat){
        
        let boxWidth = width * 8 / 9
        
        let height = CGFloat(40)
        let viewFN = UIView(frame: CGRect(x: 0, y: 0, width: boxWidth, height: height))
        
        let outerView = UIView(frame: CGRect(x: -100, y: 0, width: width + 100, height: 60))
//        outerView.backgroundColor = .green
        
//        let imageView = UIImageView(frame: CGRect.init(x: 0, y: 0, width: width * 2 / 3, height: height))
        let label = UILabel(frame: CGRect.init(x: 0, y: 0, width: width * 2 / 3, height: height))
        label.text = "GARDIFY"
        
//        viewFN.addSubview(label)
        
        let logo = UIImageView(frame: CGRect.init(x: -(width * 1 / 8), y: 0, width: width * 2 / 3, height: height))
        
        logo.image = UIImage(named: "gardify_logo_horizontal")
        logo.contentMode = .scaleAspectFit
        viewFN.addSubview(outerView)
        viewFN.addSubview(logo)
        //
        //        self.navigationItem.rightBarButtonItems = [rightBarButton, blankButton]
        
        self.titleView = viewFN
        
    }
    
    @objc func tapCustom(){
    }
    
    @objc func tapCustom2(){
    }
}
