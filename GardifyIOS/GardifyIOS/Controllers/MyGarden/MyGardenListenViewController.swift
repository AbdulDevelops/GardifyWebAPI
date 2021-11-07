//
//  MyGardenListenViewController.swift
//  GardifyIOS
//
//  Created by Netzlab on 07.09.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class MyGardenListenViewController: UIViewController {

    @IBOutlet weak var outerView: UIView!
    
    @IBOutlet weak var userListGardenTableView: FullTableView!
    @IBOutlet weak var backButton: UIButton!
    
    var mainTabBar: MainTabBarController?
    var selectDelegate: MyGardenOptionDelegate?
    
    var userListData: [UserPlantModel] = []
    
    override func viewDidLoad() {
        super.viewDidLoad()

        
        // Do any additional setup after loading the view.
    }
    
    func loadUserList(){
        
        NetworkManager().requestDataAsync(type: [UserPlantModel].self, APP_URL.USER_LIST){response in
            
            if !response.success{
                
                self.ShowAlert(message: response.result as! String)
//                self.removeSpinner()
                return
            }
//            self.removeSpinner()
            self.userListData = response.result as! [UserPlantModel]
            self.userListGardenTableView.reloadData()
            
        }
    }
    
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
        
//        self.showSpinner(onView: self.view)
        DispatchQueue.main.async {
            self.loadUserList()
        }
        
        self.pageConfiguration()
        mainTabBar?.setNavigationButtonStatus(status: false)
        
    }
    
    override func viewWillDisappear(_ animated: Bool) {
        mainTabBar?.setNavigationButtonStatus(status: true)
    }
    
    func pageConfiguration(){
        
        self.userListGardenTableView.backgroundColor = .clear
        self.outerView.addBorderRadius()
        self.backButton.setClearWhiteButton()
    }
    

    @IBAction func onBack(_ sender: Any) {
         dismiss(animated: true, completion: nil)
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

extension MyGardenListenViewController: UITableViewDelegate, UITableViewDataSource{
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return self.userListData.count
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: "listenCell", for: indexPath) as! ListenListTableViewCell
        
        cell.selectDelegate = self.selectDelegate
        cell.gardenId = self.userListData[indexPath.row].GardenId
        cell.gardenData = self.userListData[indexPath.row]
        cell.viewController = self
        cell.titleLabel.text = self.userListData[indexPath.row].Name
        
        return cell
    }
    
    
    
}
