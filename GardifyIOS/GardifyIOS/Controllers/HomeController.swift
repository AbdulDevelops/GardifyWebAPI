//
//  HomeController.swift
//  GardifyText
//
//  Created by Netzlab on 27.07.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit
import Alamofire

private let reuseIdentifier = "Cell"

protocol HomeDelegate {
    func goToHomeNavigation(page: Int)
}

class HomeController: UICollectionViewController {
    
    
    @IBOutlet var homeCollectionView: UICollectionView!
    
    @IBOutlet weak var navView: UIView!
    @IBOutlet weak var navViewWidth: NSLayoutConstraint!
    
    @IBOutlet weak var backButton: UIButton!
    @IBOutlet weak var topInfoView: UIView!
    @IBOutlet weak var plantCountLabel: UILabel!
    @IBOutlet weak var todoCountLabel: UILabel!
    @IBOutlet weak var menuButton: UIButton!
    @IBOutlet weak var mainLogoImage: UIImageView!
    @IBOutlet weak var firstTitleLabel: UILabel!
    @IBOutlet weak var secondTitleLabel: UILabel!
    @IBOutlet weak var titleView: UIView!
    @IBOutlet weak var navHeight: NSLayoutConstraint!
    @IBOutlet weak var navTopHeight: NSLayoutConstraint!
    @IBOutlet weak var backButtonView: UIView!
    @IBOutlet weak var backButtonImage: UIImageView!
    
    @IBOutlet weak var navPlantButton: UIButton!
    @IBOutlet weak var navTodoButton: UIButton!
    @IBOutlet weak var navWarningButton: UIButton!
    @IBOutlet weak var navWarningBadge: UIImageView!
    
    
    @IBOutlet weak var warningLabel: UILabel!
    @IBOutlet weak var navBackgroundWidth: NSLayoutConstraint!
    
    var homeKeyIndex: [Int] = []
    var homeCellTextFirst: [String]?
    var homeCellTextSecond: [String]?
    var homeCellImageNames: [String] = []
    
    var cellRatio: [CGFloat] = []
    var homeCellColor: [UIColor] = []
    var homeBoldTitle: [Bool] = []
    var cellAccess: [Bool] = []
    var homeCellCount: [Int] = []
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        loginCheck()
        PageConfiguration()
        styleConfiguration()
        navConfiguration()
        // Register cell classes
        self.collectionView!.register(UICollectionViewCell.self, forCellWithReuseIdentifier: reuseIdentifier)
        homeCellConfiguration()
        configureGesture()
        //               let width = self.navigationController?.navigationBar.frame.width
        //               self.navigationItem.addHomeNavigation(width: width!)
        
        
        
        // Do any additional setup after loading the view.
    }
    
    func waitFunction(){
        
    }
    
    func loginCheck(){
        
        if !UserDefaultKeys.IsLoggedIn.bool(){
            return
        }
        
        let expiryDate = UserDefaultKeys.ExpireDate.string()?.toDate("yyyy-MM-dd'T'HH:mm:ss.SSSSSSSZ")
        
        if expiryDate == nil{
            userLogOut()
            self.ShowAlert(message: "Bitte Anmelden")
            return
        }
        
        if expiryDate! >= Date(){
            return
        }
        userLogOut()
        self.ShowAlert(message: "Bitte Anmelden")

    }
    
    func configureGesture(){
        let tabBar = tabBarController as! MainTabBarController
        self.view.addGestureRecognizer(tabBar.swipeGesture)
        
    }
    
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
        
        DispatchQueue.main.async {
            self.homeCellConfiguration()
            self.menuReadNumberUpdate()
            self.homeCellCountConfiguration()
        }
      
    }
    
    override func viewWillAppear(_ animated: Bool) {
        super.viewWillAppear(false)
        
   
        
        self.updateNavigationBar(isMain: true)
        
        //
        //        let width = self.navigationController?.navigationBar.frame.width
        //     self.tabBarController?.navigationItem.addRightButton(width: width!)
        
        
    }
    
    
    func menuReadNumberUpdate(){
        updateLastVideoTime(){result in
            print("number of video is", result)
        }
    }
    
    func homeCellConfiguration(){
        
        print("col width is", self.homeCollectionView.frame.width, self.homeCollectionView.frame.height)
        let width = (self.homeCollectionView.frame.width - 80) / 2
        
        let layout = self.homeCollectionView.collectionViewLayout as! UICollectionViewFlowLayout
        
        layout.itemSize = CGSize(width: width, height: width)
        //        self.homeCollectionView.cellsi
    }
    
    func homeCellCountConfiguration(){
        
        DispatchQueue.main.async {
            updateLastVideoTime(){result in
                self.homeCellCount[6] = result
                self.homeCollectionView.reloadData()
                updateLastNewsTime(){result in
                    self.homeCellCount[1] = result
                    self.homeCollectionView.reloadData()
                }
                
            }
            
        }

       
    }
    
    func navConfiguration(){
        //        UIApplication.shared.keyWindow?.addSubview(self.navView)
        //
        //        UIApplication.shared.keyWindow!.bringSubviewToFront(self.navView)
        
        
        topInfoView.addBorderRadius()
        
        self.backButton.addBorderRadiusSmall()
        self.backButton.addMaskBound()
        self.backButtonView.addBorderRadiusSmall()
        print()
        if !Device.IS_IPHONE_XS && !Device.IS_IPHONE_XS_MAX && !Device.IS_IPHONE_12 && !Device.IS_IPHONE_12_MAX {
            navHeight.constant = 145
            print("is not notch")
            navTopHeight.constant = 85
        }
        
        navBackgroundWidth.constant = self.homeCollectionView.frame.width
        
        let tabController = self.tabBarController as! MainTabBarController
        
        tabController.homeDelegate = self
        
        tabController.navBackButtonView = self.backButtonView
        tabController.navView = self.navView
        tabController.navButtonView = self.backButton
        tabController.navMenuButton = self.menuButton
        tabController.navBackButtonImage = self.backButtonImage
        tabController.navPlantCount = self.plantCountLabel
        tabController.navWarningBadge = self.navWarningBadge
        
        tabController.navTodoCount = self.todoCountLabel
        tabController.navWarningCount = self.warningLabel
        tabController.navTitleView = self.titleView
        tabController.navFirstTitle = self.firstTitleLabel
        tabController.navSecondTitle = self.secondTitleLabel
        tabController.navMainLogo = self.mainLogoImage
        
        tabController.navConfiguration()
        
        tabController.updateNav()
        //        tabController.navButtonView?.alpha = 0
        //        tabController.navButtonView?.isEnabled = false
        //        tabController.navButtonView?.backgroundColor = .red
        
    }
    
    
    @IBAction func onTapBack(_ sender: Any) {
//        Alamofire.SessionManager.default.session.invalidateAndCancel()
        
            print("back tapped")
            self.navigationController?.popViewController(animated: true)

    }
    
    
    @IBAction func onTapMenu(_ sender: Any) {
        
        let tabController = self.tabBarController as! MainTabBarController
        
        tabController.openMenu()
    }
    
    @IBAction func onPlantNavClick(_ sender: Any) {
        let tabController = self.tabBarController as! MainTabBarController
        
        tabController.openPlantList()
    }
    
    @IBAction func onTodoNavClick(_ sender: Any) {
        let tabController = self.tabBarController as! MainTabBarController
        
        tabController.openTodoList()
    }
    
    @IBAction func onWarningNavClick(_ sender: Any) {
        let tabController = self.tabBarController as! MainTabBarController
        
        tabController.openWarningPage()
    }
    
    func PageConfiguration(){
        
        homeKeyIndex = [
            0,3,13,2,4,5,12,7,6,9,11, 1,8,10
        ]
        
        homeCellTextFirst = ["MEIN",
                             "GARDIFY",
                            
                             "PFLANZEN",
                             "TO-DO",
                             "PFLANZEN",
                             "PFLANZEN",
                             
                             "GARTEN"
                             ,
                             "GARTEN",
                             
                             "GARDIFY",
                             "GARTEN",
                             
                             "GARDIFY",
                             "GARTEN",
                             "PFLANZEN",
                             "GUIDED"
            
        ]
        
        homeCellTextSecond = ["GARTEN",
                              "NEWS",
                              
                              "SUCHE",
                              "KALENDER",
                              
                              "DOC",
                              "SCAN",
                              
                              "VIDEO",
                              "ÖKOSCAN",
                              
                              "COMMUNITY",
                              "WETTER",
                              
                              "SHOP",
                              "GLOSSAR",
                              "ERGÄNZEN",
                              "TOUR"
            
        ]
        
        cellRatio = [30,30,30,30,30,30,30,30,30,30,30,30, 30,30]
        
        homeCellImageNames = ["main_myGardenNormal",
                              "main_News",
                              
                              "main_plantSearch",
                              "main_todoCalender",
                              
                              "main_plantDoc",
                              "main_plantScan",
                              
                              "main_video"
                              ,
                              "main_ecoScan",
                              
                              "main_community",
                              "main_weather",
                              
                              "main_shop",
                              "main_knowhow",
                              "main_suggest",
                              "main_tour"
                              ]
        
        homeCellCount = [0,0,0,0,0,0,0,0,0,0,0,0,0,0]
        
        homeCellColor = [
            rgb(70, 118,110),
            rgb(99, 131, 104),
            
            rgb(145,169,94),
            rgb(105,140,53),
            
            rgb(92,162,139),
            rgb(89 ,148,139),
            
            rgb(70, 118,110),
            rgb(129,161,58),
            
            rgb(88,138,80),
            rgb(80,125,133),
           
            rgb(105,128,54),
            rgb(138,143,44),
            rgb(132,172,138),
            rgb(96,118,116)
        ]
        
        cellAccess = [true, true, true, true, true, true, true, true, true, true, true, true, true, true]
        homeBoldTitle = [true, false, false, true, false, false, false, false, false, false, false, false, false, false]
        
    }
    
    override func collectionView(_ collectionView: UICollectionView, didSelectItemAt indexPath: IndexPath) {
        
        self.goToHomeNavigation(page: homeKeyIndex[indexPath.row])
        
        
    }
    
    
    func styleConfiguration(){
        
        self.homeCollectionView.applyTheme()
        //       navigationConfiguration()
    }
    
    //    func navigationConfiguration(){
    //        let greyColor = UIColor(displayP3Red: 240/255, green: 240/255, blue: 240/255, alpha: 1)
    //
    //        let greenColor = UIColor(displayP3Red: 122/255, green: 157/255, blue: 52/255, alpha: 1)
    //        self.navigationController?.navigationBar.barTintColor = greenColor
    //        self.navigationController?.navigationBar.titleTextAttributes = [NSAttributedString.Key.foregroundColor : greyColor]
    //        let yourBackImage = UIImage(systemName: "arrow.left")
    //        self.navigationController?.navigationBar.backIndicatorImage = yourBackImage
    //        self.navigationController?.navigationBar.backIndicatorTransitionMaskImage = yourBackImage
    //    }
    //
    /*
     // MARK: - Navigation
     
     // In a storyboard-based application, you will often want to do a little preparation before navigation
     override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
     // Get the new view controller using [segue destinationViewController].
     // Pass the selected object to the new view controller.
     }
     */
    
    // MARK: UICollectionViewDataSource
    
    override func numberOfSections(in collectionView: UICollectionView) -> Int {
        // #warning Incomplete implementation, return the number of sections
        return 1
    }
    
    
    override func collectionView(_ collectionView: UICollectionView, numberOfItemsInSection section: Int) -> Int {
        // #warning Incomplete implementation, return the number of items
        return homeCellTextFirst?.count ?? 0
    }
    
    override func collectionView(_ collectionView: UICollectionView, cellForItemAt indexPath: IndexPath) -> UICollectionViewCell {
        let cell = collectionView.dequeueReusableCell(withReuseIdentifier: "homeCell", for: indexPath) as! HomeViewCell
        
        let index = homeKeyIndex[indexPath.row]
        // Configure the cell
        cell.setCounterNumber(number: self.homeCellCount[index])
        cell.onConfigure(image: homeCellImageNames[index], imageSize: cellRatio[index],
                         titleTextFirst: (homeCellTextFirst![index]),
                         titleTextSecond: homeCellTextSecond![index],
                         cellColor: homeCellColor[index],
                         isFirstBold: (indexPath.row < 2 ))
        
        if !cellAccess[index]{
            cell.contentView.alpha = 0.2
        }
        else{
            cell.contentView.alpha = 1
        }
        
        return cell
    }
    
    
    
    
    // MARK: UICollectionViewDelegate
    
    /*
     // Uncomment this method to specify if the specified item should be highlighted during tracking
     override func collectionView(_ collectionView: UICollectionView, shouldHighlightItemAt indexPath: IndexPath) -> Bool {
     return true
     }
     */
    
    /*
     // Uncomment this method to specify if the specified item should be selected
     override func collectionView(_ collectionView: UICollectionView, shouldSelectItemAt indexPath: IndexPath) -> Bool {
     return true
     }
     */
    
    /*
     // Uncomment these methods to specify if an action menu should be displayed for the specified item, and react to actions performed on the item
     override func collectionView(_ collectionView: UICollectionView, shouldShowMenuForItemAt indexPath: IndexPath) -> Bool {
     return false
     }
     
     override func collectionView(_ collectionView: UICollectionView, canPerformAction action: Selector, forItemAt indexPath: IndexPath, withSender sender: Any?) -> Bool {
     return false
     }
     
     override func collectionView(_ collectionView: UICollectionView, performAction action: Selector, forItemAt indexPath: IndexPath, withSender sender: Any?) {
     
     }
     */
    
}

extension HomeController: HomeDelegate{
    func goToHomeNavigation(page: Int) {
        self.tabBarController?.selectedIndex = 0
        self.navigationController?.popToRootViewController(animated: true)
        if page == -1{
            self.ShowAlert(message: "Demnächst verfügbar")
            return
        }
        if page == -2{
            self.ShowAlert(message: "Sie sind abgemeldet")
            return
        }
        
        if page < 14{
            let pageColor = homeCellColor[homeKeyIndex[page]]
            
            self.updateNavigationBackColor(color: pageColor)
        }
       
        
        switch page {
        case 0:
            //            ShowAlert(message: "Nicht fertig")
            //            break
            if !self.isLoggedIn(){
                return
            }
            let storyBoard = UIStoryboard(name: "MyGarden", bundle: nil)
            let controller = storyBoard.instantiateViewController(withIdentifier: "MyGardenView") as! MyGardenViewController
            
            self.navigationController?.pushViewController(controller, animated: true)
        case 1:
            let storyBoard = UIStoryboard(name: "News", bundle: nil)
            let controller = storyBoard.instantiateViewController(withIdentifier: "News") as! NewsViewController
            
            self.navigationController?.pushViewController(controller, animated: true)
        case 3:
            if !self.isLoggedIn(){
                return
            }
            let storyBoard = UIStoryboard(name: "Todo", bundle: nil)
            let controller = storyBoard.instantiateViewController(withIdentifier: "TodoView") as! TodoViewController
            self.navigationController?.pushViewController(controller, animated: true)
        case 2:
            let storyBoard = UIStoryboard(name: "PlantSearch", bundle: nil)
            //            let controller = storyBoard.instantiateViewController(withIdentifier: "PlantSearchView2") as! PlantSearchViewController
            let controller = storyBoard.instantiateViewController(withIdentifier: "PlantSearchView2")
            self.navigationController?.pushViewController(controller, animated: true)
        case 5:
            let storyBoard = UIStoryboard(name: "PlantScan", bundle: nil)
            let controller = storyBoard.instantiateViewController(withIdentifier: "PlantScanView") as! PlantScanViewController
            self.navigationController?.pushViewController(controller, animated: true)
        case 4:
            if !self.isLoggedIn(){
                return
            }
            let storyBoard = UIStoryboard(name: "PlantDoc", bundle: nil)
            let controller = storyBoard.instantiateViewController(withIdentifier: "PlantDocView") as! PlantDocViewController
            self.navigationController?.pushViewController(controller, animated: true)
        case 7:
            if !self.isLoggedIn(){
                return
            }
            let storyBoard = UIStoryboard(name: "EcoScan", bundle: nil)
            let controller = storyBoard.instantiateViewController(withIdentifier: "ecoScanView") as! EcoScanViewController
            self.navigationController?.pushViewController(controller, animated: true)
            
        case 10:
            self.ShowAlert(message: "demnächst verfügbar")
            
//            guard let url = URL(string: "https://shop.gardify.de") else { return }
//            UIApplication.shared.open(url)
            
//            let storyBoard = UIStoryboard(name: "Shop", bundle: nil)
//            let controller = storyBoard.instantiateViewController(withIdentifier: "Shop") as! ShopViewController
//            self.navigationController?.pushViewController(controller, animated: true)
        case 9:
            if !self.isLoggedIn(){
                return
            }
            let storyBoard = UIStoryboard(name: "Weather", bundle: nil)
            let controller = storyBoard.instantiateViewController(withIdentifier: "weatherUpdateView") as! WeatherUpdateViewController
            self.navigationController?.pushViewController(controller, animated: true)
            
        case 8:
            let storyBoard = UIStoryboard(name: "Community", bundle: nil)
            let controller = storyBoard.instantiateViewController(withIdentifier: "communityVC") as! CommunityViewController
            self.navigationController?.pushViewController(controller, animated: true)
            
        case 11 :
            let storyBoard = UIStoryboard(name: "GardeningAZ", bundle: nil)
            let controller = storyBoard.instantiateInitialViewController()
            print("controller exist", controller)
            self.navigationController?.pushViewController(controller!, animated: true)
            
        case 6:
            let storyBoard = UIStoryboard(name: "Video", bundle: nil)
            let controller = storyBoard.instantiateViewController(withIdentifier: "VideoList") as! VideoListViewController
            self.navigationController?.pushViewController(controller, animated: true)
            
        case 20:
            if !self.isLoggedIn(){
                return
            }
            self.updateNavigationBackColor(color: homeCellColor[3])
            let storyBoard = UIStoryboard(name: "Todo", bundle: nil)
            let controller = storyBoard.instantiateViewController(withIdentifier: "Warning") as! WarningViewController
            self.navigationController?.pushViewController(controller, animated: true)
            
        case 12:
            let storyBoard = UIStoryboard(name: "PlantSuggest", bundle: nil)
            let controller = storyBoard.instantiateViewController(withIdentifier: "plantSuggest") as! PlantSuggestViewController
            self.navigationController?.pushViewController(controller, animated: true)
          
        
        case 21:
            let storyBoard = UIStoryboard(name: "NewsLetter", bundle: nil)
            let controller = storyBoard.instantiateViewController(withIdentifier: "newsLetter") as! NewsLetterViewController
            self.navigationController?.pushViewController(controller, animated: true)
            
   
        default:
            self.ShowAlert(message: "Demnächst verfügbar")

            break
            
        }
    }
    
    
}

