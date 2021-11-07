//
//  UIViewController.swift
//  GardifyText
//
//  Created by Netzlab on 27.07.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit
import Foundation

var loadingSpinner : UIView?

extension UIViewController{
    
    func applyTheme(){
        view.backgroundColor = .Background
    }
    
    func configurePadding(){
        
        let width = view.frame.size.width
        let sidePadding = width / 340.0 * 28
        let verticalPadding = width / 340.0 * 28
        
        view.layoutMargins = UIEdgeInsets(top: verticalPadding + 30, left: sidePadding, bottom: verticalPadding / 2, right: sidePadding)
    }
    
    func configureSpecialPadding(){
        
        let width = view.frame.size.width
        let sidePadding = 0
        let verticalPadding = width / 340.0 * 28
        
        view.layoutMargins = UIEdgeInsets(top: 0, left: 0, bottom: 0, right: 0)
    }
    
    func configureSinglePadding(){
        
        let width = view.frame.size.width
        let sidePadding = width / 340.0 * 28
        let verticalPadding = width / 340.0 * 28
        
        view.layoutMargins = UIEdgeInsets(top: verticalPadding + 70, left: sidePadding, bottom: verticalPadding / 2, right: sidePadding)
    }
    
    
    
    func updateNavigationBackColor(color: UIColor){
        let tabController = self.tabBarController as! MainTabBarController

        tabController.navButtonView?.backgroundColor = color
    }
    
    
    func updateNavigationBar(isMain: Bool, _ firstTitle: String = "", _ secondTitle: String = "", _ image: String = ""){
        

        
        guard let tabController = self.tabBarController as? MainTabBarController else{
            self.navigationController?.popViewController(animated: true)
            return
        }
        
        if isMain{
            tabController.setMainNav()
        }
        else{
            tabController.setOtherNav()
            tabController.setTitle(first: firstTitle, second: secondTitle)
            
            if let imageView = UIImage(named: image) {
                tabController.navBackButtonImage!.image = imageView
            }
            
        }
//        self.tabBarController?.navigationItem.addRightButton(width: width!)
        tabController.updateNav()
        DispatchQueue.main.async {
            updatePlantCount(){result in
                if !result{
                    return
                }
                tabController.updateNav()


            }
            
            updateTodoCount(){result in
                if !result{
                    return
                }
                tabController.updateNav()


            }
            
            updateWarningCount(){result in
                if !result{
                    return
                }
                tabController.updateNav()

            }
        }
        
        
        
    }
    
    func updateNavigationBarExternal(tabController: MainTabBarController, isMain: Bool, _ firstTitle: String = "", _ secondTitle: String = ""){
        
        
        
        
        if isMain{
            tabController.setMainNav()
        }
        else{
            tabController.setOtherNav()
            tabController.setTitle(first: firstTitle, second: secondTitle)
        }
        //        self.tabBarController?.navigationItem.addRightButton(width: width!)
        tabController.updateNav()
        
        updatePlantCount(){result in
            if !result{
                return
            }
            tabController.updateNav()
            
            //            self.tabBarController?.navigationItem.addRightButton(width: width!)
        }
        
        updateTodoCount(){result in
            if !result{
                return
            }
            tabController.updateNav()
            
            //            self.tabBarController?.navigationItem.addRightButton(width: width!)
        }
        
    }
    
    func updateHomeNavigation(){
        
    }
    
    func ShowTitleAlert(title: String, message: String){
        let alert = UIAlertController(title : title , message: message, preferredStyle: .alert)
        let closeAction = UIAlertAction(title: "Schließen", style: UIAlertAction.Style.cancel)
        alert.addAction(closeAction)
        DispatchQueue.main.async {
            self.present(alert, animated: true, completion: nil)

        }
    }
    
    func ShowAlert(message: String){
        let alert = UIAlertController(title :"", message: message, preferredStyle: .alert)
        let closeAction = UIAlertAction(title: "Schließen", style: UIAlertAction.Style.cancel)
        alert.addAction(closeAction)
        
        DispatchQueue.main.async {
            self.present(alert, animated: true, completion: nil)

        }
    }
    
    func ShowMessageAlert(title: String, message: String){
        let alert = UIAlertController(title : title, message: message, preferredStyle: .alert)
        let closeAction = UIAlertAction(title: "Schließen", style: UIAlertAction.Style.cancel)
        alert.addAction(closeAction)
        DispatchQueue.main.async {
            self.present(alert, animated: true, completion: nil)

        }
    }
    

    
    func ShowBackAlert(message: String){
        let alert = UIAlertController(title :"", message: message, preferredStyle: .alert)
        let closeAction = UIAlertAction(title: "Schließen", style: UIAlertAction.Style.cancel, handler: {(alert: UIAlertAction!) in
            DispatchQueue.main.async( execute: {
                self.navigationController?.popViewController(animated: true)
            })
            
        })
        alert.addAction(closeAction)
        DispatchQueue.main.async {
            self.present(alert, animated: true, completion: nil)

        }
    }
    
    func showPartialSpinner(onView: UIView, _ loadingText: String = ""){
        let spinnerView = UIView.init(frame: onView.bounds)
        spinnerView.backgroundColor = UIColor.init(red: 0.5, green: 0.5, blue: 0.5, alpha: 0.7)
        let ai = UIActivityIndicatorView.init(style: .large)
        
        let text = UILabel(frame: CGRect(x: 0, y: 0, width: onView.bounds.width, height: 100))
        text.text = loadingText
        text.textAlignment = .center
        text.font = UIFont.systemFont(ofSize: 17, weight: .bold)
//        text.backgroundColor = .red
        text.textColor = .white
        text.center = CGPoint(x: spinnerView.bounds.midX, y: spinnerView.bounds.minY)
        
        spinnerView.addSubview(text)
        ai.transform = CGAffineTransform(scaleX: 2, y: 2)

        ai.startAnimating()
        ai.center = CGPoint(x: spinnerView.center.x, y: 100)
        
//        DispatchQueue.main.async {
            spinnerView.addSubview(ai)
            onView.addSubview(spinnerView)
//        }
        
        loadingSpinner = spinnerView
    }
    
    func showSpinner(onView: UIView, _ loadingText: String = ""){
        let tabbar =  self.tabBarController as! MainTabBarController

//        tabbar.setNavigationButtonStatus(status: false)
        
        let spinnerView = UIView.init(frame: onView.bounds)
        spinnerView.backgroundColor = UIColor.init(red: 0.5, green: 0.5, blue: 0.5, alpha: 0.7)
        let ai = UIActivityIndicatorView.init(style: .large)
        
        let text = UILabel(frame: CGRect(x: 0, y: 0, width: onView.bounds.width, height: 100))
        text.text = loadingText
        text.textAlignment = .center
        text.font = UIFont.systemFont(ofSize: 17, weight: .bold)
//        text.backgroundColor = .red
        text.textColor = .white
        text.center = CGPoint(x: spinnerView.bounds.midX, y: spinnerView.bounds.midY + 50)
        
        spinnerView.addSubview(text)
        
        ai.transform = CGAffineTransform(scaleX: 2, y: 2)
        ai.startAnimating()
        
        ai.center = spinnerView.center
        
//        DispatchQueue.main.async {
            spinnerView.addSubview(ai)
            onView.addSubview(spinnerView)
//        }
        
        loadingSpinner = spinnerView
    }
    
    func removeSpinner(){
        

        DispatchQueue.main.async {
            loadingSpinner?.removeFromSuperview()
            if let tabbar =  self.tabBarController as? MainTabBarController{
                tabbar.setNavigationButtonStatus(status: true)

            }

            loadingSpinner = nil
        }
    }
    
    func isLoggedIn() -> Bool{
        if !UserDefaultKeys.IsLoggedIn.bool(){
            let storyBoard = UIStoryboard(name: "Login", bundle: nil)
            let controller = storyBoard.instantiateViewController(withIdentifier: "LoginView") as! LoginViewController
            
            guard let currentTabBar = self.tabBarController as? MainTabBarController else{
                controller.tabBar = self as! MainTabBarController
                self.navigationController?.pushViewController(controller, animated: true)
                return false
            }
            controller.tabBar = currentTabBar
            self.navigationController?.pushViewController(controller, animated: true)
            return false
        }
        return true
    }
    
    func goToPlantDetails(plantId : Int){
        
        let storyBoard = UIStoryboard(name: "PlantDetail", bundle: nil)
        let controller = storyBoard.instantiateViewController(withIdentifier: "PlantDetailView") as! PlantDetailViewController
        
        controller.plantId = plantId
        self.navigationController?.pushViewController(controller, animated: true)
    }
    
    func goToAddGarden(plantId: Int, plantModel: PlantDetailModel?, _ plantName: String = ""){
        
        
        
        NetworkManager().requestDataAsync(type: [UserPlantModel].self, APP_URL.USER_PLANT_BY_ID + "\(plantId)"){response in
            
            let storyBoard = UIStoryboard(name: "PlantGarden", bundle: nil)
            let controller = storyBoard.instantiateViewController(withIdentifier: "PlantGardenAddView") as! PlantGardenAddViewController
            
            controller.userPlantList = (response.result as! [UserPlantModel])
//                controller.plantDetail = plantModel
            if plantModel != nil{
                controller.plantId = plantModel?.Id
                controller.plantName = plantModel?.NameGerman
            }
            else{
                controller.plantId = plantId
                controller.plantName = plantName
            }
                
                self.navigationController?.pushViewController(controller, animated: true)
        
            
            
        }
    }
    
    func goToSuggestNewPlant(){
        
    }
    
    func goToTodoDetails(todoId: Int){
        
        let storyBoard = UIStoryboard(name: "Todo", bundle: nil)
        let controller = storyBoard.instantiateViewController(withIdentifier: "TodoDetailView") as! TodoDetailViewController
        controller.todoId = todoId
        self.navigationController?.pushViewController(controller as! UIViewController, animated: true)
    }
    
    func goToController<T>(storyBoard: String, controllerName: String, type: T.Type){
        let storyBoard = UIStoryboard(name: storyBoard, bundle: nil)
        let controller = storyBoard.instantiateViewController(withIdentifier: controllerName) as! T
        self.navigationController?.pushViewController(controller as! UIViewController, animated: true)


    }
    
    func getBoldText(firstText: String, secondText: String) -> NSMutableAttributedString{
        
        let boldText = firstText
        let attrs = [NSAttributedString.Key.font : UIFont.boldSystemFont(ofSize: 14)]
        let attributedString = NSMutableAttributedString(string:boldText, attributes:attrs)
        
        let normalText = secondText
        let normalString = NSMutableAttributedString(string:normalText)
        
        attributedString.append(normalString)
        
        return attributedString
    }
}
