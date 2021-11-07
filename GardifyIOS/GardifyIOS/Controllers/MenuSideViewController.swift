//
//  MenuSideViewController.swift
//  GardifyIOS
//
//  Created by Netzlab on 17.11.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class MenuSideViewController: UIViewController , UITableViewDelegate, UITableViewDataSource{
    

    var menuNavigationDelegate: MenuNavigationDelegate?

    @IBOutlet weak var menuTableView: UITableView!
    
    override func viewDidLoad() {
        super.viewDidLoad()
        self.menuTableView.reloadData()
        menuTableView.backgroundColor = .systemGray5
        // Do any additional setup after loading the view.
    }
    
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
        
        self.menuTableView.reloadData()
    }
    
    @IBAction func onBack(_ sender: Any) {
        dismiss(animated: true, completion: nil)
    }
    
    func tableView(_ tableView: UITableView, willSelectRowAt indexPath: IndexPath) -> IndexPath? {
        if UserDefaultKeys.IsLoggedIn.bool(){
            return ((LoggedInMenuPage.allCases[indexPath.row].rawValue != "") ? indexPath : nil)
        }
        else{
            return ((menuPage.allCases[indexPath.row].rawValue != "") ? indexPath : nil)
        }
    }
    
    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        dismiss(animated: true, completion: {
            if UserDefaultKeys.IsLoggedIn.bool(){
                self.menuNavigationDelegate?.navigateToLoggedPage(page: LoggedInMenuPage.allCases[indexPath.row])
            }
            else{
                self.menuNavigationDelegate?.navigateToPage(page: menuPage.allCases[indexPath.row])
            }
            
        })
    }
    
    
    
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        if UserDefaultKeys.IsLoggedIn.bool(){

            return LoggedInMenuPage.allCases.count
        }
        else{
            

            return menuPage.allCases.count
        }
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: "menuCell", for: indexPath) as! MenuViewCell
         
        if UserDefaultKeys.IsLoggedIn.bool(){
            cell.isMainMenu = indexPath.row <= getLoggedInSecondaryMenuIndex()
            cell.isFrontBold = indexPath.row <= 1
            cell.isBotBorderOn = indexPath.row != (getLoggedInSecondaryMenuIndex() - 1)
            
            cell.onConfigure(title: LoggedInMenuPage.allCases[indexPath.row].rawValue)
        }
        else{
            cell.isMainMenu = indexPath.row <= getSecondaryMenuIndex()
            cell.isFrontBold = false
            cell.isBotBorderOn = indexPath.row != (getSecondaryMenuIndex() - 1)
            cell.onConfigure(title: menuPage.allCases[indexPath.row].rawValue)
        }
        
        return cell
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
