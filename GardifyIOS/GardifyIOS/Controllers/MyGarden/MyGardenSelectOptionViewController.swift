//
//  MyGardenSelectOptionViewController.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 03.09.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class MyGardenSelectOptionViewController: UIViewController {

    @IBOutlet weak var backButton: UIButton!
    @IBOutlet weak var outerView: UIView!
    @IBOutlet weak var goToPlantScanButton: UIButton!
    @IBOutlet weak var goToPlantSearch: UIButton!
    @IBOutlet weak var addNewDeviceButton: UIButton!
    
    @IBOutlet weak var plantSearchHeight: NSLayoutConstraint!
    @IBOutlet weak var plantScanHeight: NSLayoutConstraint!
    @IBOutlet weak var addDeviceHeight: NSLayoutConstraint!
    
    var mainTabBar: MainTabBarController?
    var selectDelegate: MyGardenOptionDelegate?
    
    var isPlants : Bool = true
    
    override func viewDidLoad() {
        super.viewDidLoad()

        self.PageConfiguration()
        mainTabBar?.setNavigationButtonStatus(status: false)
        // Do any additional setup after loading the view.
    }
    
    func PageConfiguration(){
        
        configureBackgroundBlur()
        self.outerView.addBorderRadius()
        goToPlantScanButton.setClearWhiteButton()
        goToPlantSearch.setClearWhiteButton()
        addNewDeviceButton.setClearWhiteButton()
        backButton.setClearWhiteButton()
        
        if isPlants{
            addDeviceHeight.constant = 0
            addNewDeviceButton.setTitle("", for: .normal)
        }
        else{
                    plantSearchHeight.constant = 0
            goToPlantSearch.setTitle("", for: .normal)
                    plantScanHeight.constant = 0
            goToPlantScanButton.setTitle("", for: .normal)
        }
    }
    
    func configureBackgroundBlur(){
//        if !UIAccessibility.isReduceTransparencyEnabled {
//            view.backgroundColor = .clear
//
//            let blurEffect = UIBlurEffect(style: .dark)
//            let blurEffectView = UIVisualEffectView(effect: blurEffect)
//            //always fill the view
//            blurEffectView.frame = self.view.bounds
//            blurEffectView.autoresizingMask = [.flexibleWidth, .flexibleHeight]
//
//            view.insertSubview(blurEffectView, at: 0)//if you have more UIViews, use an insertSubview API to place it where needed
//        }
    }
    
    func hideDeviceOption(){
//        addDeviceHeight.constant = 0
//        addDeviceButton.isUserInteractionEnabled = false
//        addDeviceButton.setTitle("", for: .normal)
    }
    
    func hidePlantsOption(){
        
//        plantSearchHeight.constant = 0
        goToPlantSearch.isUserInteractionEnabled = false
//        plantScanHeight.constant = 0
        goToPlantScanButton.isUserInteractionEnabled = false
    }
    
    override func viewWillDisappear(_ animated: Bool) {
        super.viewWillDisappear(false)
        
        mainTabBar?.setNavigationButtonStatus(status: true)
    }
    
    
    override func viewWillAppear(_ animated: Bool) {
        super.viewWillAppear(false)
    }
    
    @IBAction func onBack(_ sender: Any) {
        dismiss(animated: true, completion: nil)
    }
    
    @IBAction func goToPlantSearch(_ sender: Any) {
        dismiss(animated: true, completion: {
            self.selectDelegate?.goToPlantSearch()
 
        })

    }
    
    @IBAction func goToPlantScan(_ sender: Any) {
        dismiss(animated: true, completion: {
            self.selectDelegate?.goToPlantScan()
        })

    }
    
    @IBAction func onAddDevice(_ sender: Any) {
        dismiss(animated: true, completion: {
            self.selectDelegate?.goToDeviceList()
        })
    }
    /*
    // MARK: - Navigation

    // In a storyboard-based application, you will often want to do a little preparation before navigation
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        // Get the new view controller using segue.destination.
        // Pass the selected object to the new view controller.
    }
    */

}
