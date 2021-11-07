//
//  MainTabBarController.swift
//  GardifyText
//
//  Created by Netzlab on 24.07.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit
import Alamofire

enum menuPage: String, CaseIterable {
    
//    case myGarden = "Mein Garten"
//    case toDoCalender = "To-Do kalender"
    case pflanzenSuche = "Pflanzen suche"
    case pflanzenDoc = "Pflanzen Doc"
    case pflanzenScan = "Pflanzen Scan"
//    case pflanzenSuggest = "Pflanzen Ergänzen"
//    case okoscan = "Garten Ökoscan"
    case video = "Garten Videos"
//    case weather = "Garten Wetter"
    case gartenWissen = "Garten Glossar"
    case news = "Gardify News"

    case community = "Gardify Community"
    case shop = "gardify Shop"
    case space1 = ""
    case agb = "AGB"
//    case shopcart = "warenkorb"
//    case wishlist = "Wunschliste"
//    case order = "bestellungen"
    case loginPage = "Anmelden"
//    case settingPage = "Einstellung"
//    case newsletter = "newsletter"
//    case contact = "kontakt"
    case data = "datenschutz"
    case policy = "impressum"
    
}

func getSecondaryMenuIndex() -> Int{
    return menuPage.allCases.firstIndex(where: {$0.rawValue == ""})!
}

enum LoggedInMenuPage: String, CaseIterable {
    
    case myGarden = "Mein Garten"
    case toDoCalender = "To-Do kalender"
    case pflanzenSuche = "Pflanzen suche"
    case pflanzenDoc = "Pflanzen Doc"
    case pflanzenScan = "Pflanzen Scan"
    case pflanzenSuggest = "Pflanzen Ergänzen"
    case okoscan = "Garten Ökoscan"
    case video = "Garten Videos"
    case weather = "Garten Wetter"
    case gartenWissen = "Garten Glossar"
    case news = "Gardify News"

    case community = "Gardify Community"
    case shop = "gardify Shop"

    case space1 = ""
    
    case agb = "AGB"
    case shopcart = "warenkorb"
    case wishlist = "Wunschliste"
    case order = "bestellungen"
    case logoutPage = "Abmelden"
    case settingPage = "Einstellung"
    case newsletter = "newsletter"
    case contact = "kontakt"
    case data = "datenschutz"
    case policy = "impressum"
}

func getLoggedInSecondaryMenuIndex() -> Int{
    return LoggedInMenuPage.allCases.firstIndex(where: {$0.rawValue == ""})!
}

enum OtherPage: Int, CaseIterable{
    case warningPage = 20
}

protocol MenuNavigationDelegate {
    
    func navigateToPage(page: menuPage)
    func navigateToLoggedPage(page: LoggedInMenuPage)
}


class MainTabBarController: UITabBarController {
    
    let transition = SlideInTransition()
    
    var isMenuEnabled : Bool = true
    
    var navView: UIView?
    var navButtonView: UIButton?
    var navBackButtonView: UIView?
    var navBackButtonImage: UIImageView?
    var navMenuButton: UIButton?
    var navPlantCount: UILabel?
    var navTodoCount: UILabel?
    var navWarningCount: UILabel?
    var navFirstTitle: UILabel?
    var navSecondTitle: UILabel?
    var navTitleView: UIView?
    var navMainLogo: UIImageView?
    var navWarningBadge: UIImageView?
    
    var homeDelegate: HomeDelegate?
    
    @IBOutlet var swipeGesture: UISwipeGestureRecognizer!
    var currentMenu: UIViewController = UIViewController()
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        self.pageConfiguration()
        // Do any additional setup after loading the view.
    }
    
    func navConfiguration(){
        
        
        UIApplication.shared.keyWindow?.addSubview(self.navView!)
        
        UIApplication.shared.keyWindow!.bringSubviewToFront(self.navView!)
        
        print("nav configured")
        
    }
    
    func setMainNav(){
        //        self.navButtonView?.alpha = 0
        self.navButtonView?.isEnabled = false
        //        self.navMainLogo?.alpha = 1
        //        self.navTitleView?.alpha = 0
        UIView.animate(withDuration: 0.3, animations: {
            self.navButtonView?.alpha = 0
            self.navMainLogo?.alpha = 1
            self.navBackButtonView?.alpha = 0
            self.navTitleView?.alpha = 0
        })
    }
    
    func setOtherNav(){
        self.navButtonView?.isEnabled = true
        
        UIView.animate(withDuration: 0.3, animations: {
            self.navButtonView?.alpha = 1
            self.navMainLogo?.alpha = 0
            self.navBackButtonView?.alpha = 1
            
            self.navTitleView?.alpha = 1
        })
        
        
    }
    
    func setTitle(first: String, second: String){
        self.navFirstTitle?.text = first
        self.navSecondTitle?.text = second
    }
    
    func updateNav(){
        
        var plantCount = "0/0"
        var todoCount = "0/0"
        
        var warningCount = "0"
        
        self.navWarningBadge?.alpha = 0
        if UserDefaultKeys.IsLoggedIn.bool(){
            plantCount = "\(UserDefaultKeys.plantSort.int())/\(UserDefaultKeys.plantTotal.int())"
            
            todoCount = "\(UserDefaultKeys.todoMonth.int())/\(UserDefaultKeys.todoYear.int())"
            
            warningCount = "\(UserDefaultKeys.warningCount.int())"
            
            if UserDefaultKeys.warningNotif.bool(){
                self.navWarningBadge?.alpha = 1
            }
        }
        
        self.navPlantCount?.text = plantCount
        self.navTodoCount?.text = todoCount
        self.navWarningCount?.text = warningCount
    }
    
    func pageConfiguration(){
        self.tabBar.items![0].selectedImage = UIImage(named: "tabbar_home")!.resizedSize(to: CGSize(width: 20, height: 20))
        self.tabBar.items![1].selectedImage = UIImage(named: "tabbar_cart")!.resizedSize(to: CGSize(width: 25, height: 25))
        self.tabBar.items![2].selectedImage = UIImage(named: "tabbar_setting")!.resizedSize(to: CGSize(width: 20, height: 20))
        
        self.tabBar.items![0].image = UIImage(named: "tabbar_home_off")!.resizedSize(to: CGSize(width: 20, height: 20))
        self.tabBar.items![1].image = UIImage(named: "tabbar_cart_off")!.resizedSize(to: CGSize(width: 24, height: 24))
        self.tabBar.items![2].image = UIImage(named: "tabbar_setting_off")!.resizedSize(to: CGSize(width: 20, height: 20))
        
        let view = UIView()
        
        let homeButton = UIImageView.init(image: UIImage(named: "menu_icon")?.resized(withPercentage: 0.01))
        view.addSubview(homeButton)
        
        let leftItem = UIBarButtonItem(customView: view)
        
        
//
//        let greyColor = UIColor(displayP3Red: 240/255, green: 240/255, blue: 240/255, alpha: 1)
//        let greenColor = UIColor(displayP3Red: 122/255, green: 157/255, blue: 52/255, alpha: 1)
//        let darkGreenColor = UIColor(displayP3Red: 106/255, green: 133/255, blue: 52/255, alpha: 1)
//        self.tabBar.barTintColor = greenColor
//        self.tabBar.tintColor = .white
//        self.tabBar.unselectedItemTintColor = greyColor
        
//        self.navigationController?.navigationBar.barTintColor = greenColor
//        self.navigationController?.navigationBar.setValue(true, forKey: "hidesShadow")
        
        
        
//
//        self.navigationController?.navigationBar.titleTextAttributes = [NSAttributedString.Key.foregroundColor : greyColor]
        //        addRightButton()
        
    }
    
    @IBAction func onScreenSwipe(_ sender: Any) {
        print("swiped")
        self.openMenu()
    }

    
    @IBAction func didTapMenu(_ sender: Any) {
        
        self.openMenu()
        
    }
    
    func setNavigationButtonStatus(status: Bool){
        self.isMenuEnabled = status
        if isMenuEnabled{
//            self.navView?.isUserInteractionEnabled = true
        }
        else{
//            self.navView?.isUserInteractionEnabled = false
        }
    }
    
    func openMenu(){
        
        if !isMenuEnabled{
            return
        }
        
        if transition.isOpen{
            dismiss(animated: true, completion: nil)
            return
        }
        
        let menuViewController = (storyboard?.instantiateViewController(identifier: "MenuSideController"))! as MenuSideViewController
        currentMenu = menuViewController
        menuViewController.menuNavigationDelegate = self
        menuViewController.modalPresentationStyle = .overCurrentContext
        menuViewController.transitioningDelegate = self
        present(menuViewController, animated: true)
    }
    
    func openPlantList(){
        
        if !isLoggedIn(){
            return
        }
        
        navigateToLoggedPage(page: LoggedInMenuPage.myGarden)
    }
    
    func openTodoList(){
        
        if !isLoggedIn(){
            return
        }
        
        navigateToLoggedPage(page: LoggedInMenuPage.toDoCalender)
    }
    
    func openWarningPage(){
        if !isLoggedIn(){
            return
        }
        
        navigateToOtherPage(pageId: OtherPage.warningPage)
        
    }
    /*
     // MARK: - Navigation
     
     // In a storyboard-based application, you will often want to do a little preparation before navigation
     override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
     // Get the new view controller using segue.destination.
     // Pass the selected object to the new view controller.
     }
     */
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        if segue.identifier == "login"{
            let controller = segue.destination as! LoginViewController
            
            controller.tabBar = self
        }
    }
    
}

extension MainTabBarController: MenuNavigationDelegate{
    func navigateToPage(page: menuPage) {
        switch page {
        case .loginPage:
            let storyBoard = UIStoryboard(name: "Login", bundle: nil)
            let controller = storyBoard.instantiateViewController(withIdentifier: "LoginView") as! LoginViewController
            controller.tabBar = self
            self.navigationController?.pushViewController(controller, animated: true)
//        case .settingPage:
//            
//            self.selectedIndex = 2
//        case .myGarden: return
//        case .toDoCalender: return
        case .pflanzenSuche:
            self.homeDelegate?.goToHomeNavigation(page: 2)
        case .pflanzenScan:
            self.homeDelegate?.goToHomeNavigation(page: 5)
        case .pflanzenDoc:
            self.homeDelegate?.goToHomeNavigation(page: 4)
//        case .okoscan: return
        case .shop:
            self.homeDelegate?.goToHomeNavigation(page: 10)
//        case .weather: return
        case .community:
            self.homeDelegate?.goToHomeNavigation(page: 8)
        case .gartenWissen:
            self.homeDelegate?.goToHomeNavigation(page: 11)
        case .video:
            self.homeDelegate?.goToHomeNavigation(page: 6)
        case .news:
                self.homeDelegate?.goToHomeNavigation(page: 1)
        default:
            self.homeDelegate?.goToHomeNavigation(page: -1)
        }
    }
    
    func navigateToLoggedPage(page: LoggedInMenuPage) {
        switch page {
        case .logoutPage:
            self.homeDelegate?.goToHomeNavigation(page: -2)
            userLogOut()
        case .settingPage:
            
            self.selectedIndex = 2
        case .myGarden:
            self.homeDelegate?.goToHomeNavigation(page: 0)
        case .toDoCalender:
            self.homeDelegate?.goToHomeNavigation(page: 3)
        case .pflanzenSuche:
            self.homeDelegate?.goToHomeNavigation(page: 2)
        case .pflanzenScan:
            self.homeDelegate?.goToHomeNavigation(page: 5)
        case .pflanzenDoc:
            self.homeDelegate?.goToHomeNavigation(page: 4)
        case .okoscan:
            self.homeDelegate?.goToHomeNavigation(page: 7)
        case .shop:
            self.homeDelegate?.goToHomeNavigation(page: 10)
        case .weather:
            self.homeDelegate?.goToHomeNavigation(page: 9)
        case .community:
            self.homeDelegate?.goToHomeNavigation(page: 8)
        case .gartenWissen:
            self.homeDelegate?.goToHomeNavigation(page: 11)
        case .video:
            self.homeDelegate?.goToHomeNavigation(page: 6)
            
        case .news:
            self.homeDelegate?.goToHomeNavigation(page: 1)
        case .newsletter:
            self.homeDelegate?.goToHomeNavigation(page: 21)
        default:
            self.homeDelegate?.goToHomeNavigation(page: -1)
        }
    }
    
    func navigateToOtherPage(pageId: OtherPage){
        self.homeDelegate?.goToHomeNavigation(page: pageId.rawValue)
    }
    
    
}


extension MainTabBarController: UIViewControllerTransitioningDelegate{
    func animationController(forPresented presented: UIViewController, presenting: UIViewController, source: UIViewController) -> UIViewControllerAnimatedTransitioning? {
        transition.isOpen = true
        return transition
    }
    
    func animationController(forDismissed dismissed: UIViewController) -> UIViewControllerAnimatedTransitioning? {
        transition.isOpen = false
        return transition
    }
}
