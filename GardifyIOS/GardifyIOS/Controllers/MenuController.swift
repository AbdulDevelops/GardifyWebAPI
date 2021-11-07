//
//  MenuController.swift
//  GardifyText
//
//  Created by Netzlab on 24.07.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class MenuController: UITableViewController{
    
    var menuNavigationDelegate: MenuNavigationDelegate?
    @IBOutlet var menuTableView: UITableView!
    
    override func viewDidLoad(){
        super.viewDidLoad()
        view.backgroundColor = .white
    }
    
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
        
        self.menuTableView.reloadData()
    }
    
    @IBAction func onSwipeLeft(_ sender: Any) {
        dismiss(animated: true, completion: nil)
    }
    
    override func tableView(_ tableView: UITableView, willSelectRowAt indexPath: IndexPath) -> IndexPath? {
        if UserDefaultKeys.IsLoggedIn.bool(){
            return ((LoggedInMenuPage.allCases[indexPath.row].rawValue != "") ? indexPath : nil)
        }
        else{
            return ((menuPage.allCases[indexPath.row].rawValue != "") ? indexPath : nil)
        }
    }
    
    override func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        
        dismiss(animated: true, completion: {
            if UserDefaultKeys.IsLoggedIn.bool(){
                self.menuNavigationDelegate?.navigateToLoggedPage(page: LoggedInMenuPage.allCases[indexPath.row])
            }
            else{
                self.menuNavigationDelegate?.navigateToPage(page: menuPage.allCases[indexPath.row])
            }
            
        })
        
        
    }
    
    override func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
      
        if UserDefaultKeys.IsLoggedIn.bool(){

            return LoggedInMenuPage.allCases.count
        }
        else{
            

            return menuPage.allCases.count
        }
    }
    
    override func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        
        let cell = tableView.dequeueReusableCell(withIdentifier: "menuCell", for: indexPath) as! MenuViewCell
         
        if UserDefaultKeys.IsLoggedIn.bool(){
            cell.onConfigure(title: LoggedInMenuPage.allCases[indexPath.row].rawValue)
        }
        else{
            cell.onConfigure(title: menuPage.allCases[indexPath.row].rawValue)
        }
        
        return cell
        
    }
    
}

