//
//  ContainerViewController.swift
//  GardifyText
//
//  Created by Netzlab on 24.07.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class ContainerViewController: UIViewController{
    override func viewDidLoad(){
        super.viewDidLoad()
        configureHomeController()
    }
    
    func configureHomeController(){
        let homeController = HomeController()
        let controller = UINavigationController(rootViewController: homeController)
        
        view.addSubview(controller.view)
        addChild(controller)
        controller.didMove(toParent: self)
    }
}
