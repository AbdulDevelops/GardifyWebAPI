//
//  MyGardenDeviceSelectViewController.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 03.09.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class MyGardenDeviceSelectViewController: UIViewController {

    @IBOutlet weak var deviceListTableView: UITableView!
    @IBOutlet weak var outerTableView: UIView!
    @IBOutlet weak var deviceAddButton: UIButton!
    
    var deviceList: [AdminDeviceListModel] = []
    var viewController: MyGardenViewController?
    var selectedDevice: [Int] = []
    override func viewDidLoad() {
        super.viewDidLoad()

        self.pageConfiguration()
        self.configurePadding()
         self.showSpinner(onView: self.view)
        DispatchQueue.main.async {
            self.loadDeviceList()

        }
        // Do any additional setup after loading the view.
    }
    
    func pageConfiguration(){
        
        self.outerTableView.addBorderRadius()
        self.outerTableView.addBorderWidth()
        
        self.deviceAddButton.setGreenButton()
    }
    
    func loadDeviceList(){
        
       
        
        NetworkManager().requestDataAsync(type: [AdminDeviceListModel].self, APP_URL.DEVICE_ADMIN_LIST, printRequest: true){response in
            if !response.success{
                
                self.ShowAlert(message: response.result as! String)
                self.removeSpinner()
                return
            }
            self.removeSpinner()
            self.deviceList = response.result as! [AdminDeviceListModel]
            print("device list is", self.deviceList)

            self.deviceListTableView.reloadData()
            
        }
    }

    @IBAction func addDevice(_ sender: Any) {
        
        NetworkManager().requestRawAsync(type: String.self, APP_URL.DEVICE_ADMIN_ADD, self.selectedDevice, method: "POST", printRequest: true){response in
            self.viewController?.getDeviceList()
            self.ShowBackAlert(message: "Dein Gerät wurde hinzugefügt")
        }
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

extension MyGardenDeviceSelectViewController: UITableViewDelegate, UITableViewDataSource{
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return deviceList.count
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: "addGardenCell", for: indexPath) as! checkListTableViewCell
        
        cell.plantPositionLabel.text = self.deviceList[indexPath.row].Name
        
        return cell
        
    }
    
    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        self.selectedDevice.append(self.deviceList[indexPath.row].Id)
        
        print("device list is", self.selectedDevice)
    }
    
    func tableView(_ tableView: UITableView, didDeselectRowAt indexPath: IndexPath) {
       let selectedChar = self.deviceList[indexPath.row].Id
        self.selectedDevice = self.selectedDevice.filter({$0 != selectedChar})
        print("device list is", self.selectedDevice)

    }
    
    
    
    
}
