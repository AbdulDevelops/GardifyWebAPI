//
//  MyGardenDeviceDropdownTableViewCell.swift
//  GardifyIOS
//
//  Created by Netzlab on 05.11.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit
import WebKit

class MyGardenDeviceDropdownTableViewCell: UITableViewCell {

    @IBOutlet weak var deviceImage: UIImageView!
    @IBOutlet weak var deviceName: UILabel!
    
    @IBOutlet weak var deviceCountLabel: UILabel!
    @IBOutlet weak var freezeSwitch: UISwitch!
    @IBOutlet weak var stormSwitch: UISwitch!
    
    @IBOutlet weak var deviceWebView: WKWebView!
    var data: MyGardenDeviceListModel?
    var viewController: MyGardenViewController?
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }
    
    func onConfigure(data: MyGardenDeviceListModel){
        self.deviceName.text = data.Name
//        self.deviceCountLabel.text = "\(data.Count)"
  
        self.data = data
        updateDataContent(data: data)
//        self.stormSwitch.isOn = data.notifyForWind
//        self.freezeSwitch.isOn = data.notifyForFrost
        var imageURL = data.EntryImages![0].SrcAttr
//        var processedURL = (imageURL?.toURL(false, false, true))!
//        let request = URLRequest(url: URL(string: processedURL)!)
        var imageName = imageURL?.slice(from: "DeviceImages/", to: ".svg")
        
        deviceImage.image = UIImage(named: imageName ?? "")
        
        
//        NetworkManager().getImageFromUrl(urlString: (processedURL)!){image in
//            self.deviceImage.image = image
//        }
        
    }
    
    func updateDataContent(data: MyGardenDeviceListModel){
//        self.deviceName.text = data.Name
        self.deviceCountLabel.text = "\(data.Count)"
  
//        self.data = data
        self.stormSwitch.isOn = data.notifyForWind
        self.freezeSwitch.isOn = data.notifyForFrost
    }

    @IBAction func onAddDevice(_ sender: Any) {
        var currentCount = data?.Count
        self.data?.Count = currentCount! + 1
        updateDataContent(data: self.data!)
        updateDeviceValue()

    }
    
    @IBAction func onSubtractDevice(_ sender: Any) {
        var currentCount = data?.Count
        if currentCount! <= 1{
            return
        }
        self.data?.Count = currentCount! - 1
        updateDataContent(data: self.data!)
        updateDeviceValue()
    }
    
    func updateDeviceValue(){
        let params : [String: Any? ] = [
            "Id" : data?.Id,
            "Name": data?.Name,
            "isActive": data?.isActive,
            "notifyForWind": data?.notifyForWind,
            "notifyForFrost": data?.notifyForFrost,
            "Note": data?.Note,
            "Gardenid": data?.Gardenid,
            "AdminDevId": data?.AdminDevId,
            "CreatedBy": data?.CreatedBy,
            "EditedBy": data?.EditedBy,
            "UserDevListId": data?.UserDevListId,
            "Date": data?.Date,
            "Count": data?.Count,
            "Todos": data?.Todos,
            "showMenu": true
        ]
        NetworkManager().requestDataAsync(type: MyGardenDeviceListModel.self, APP_URL.DEVICE_COUNT_UPDATE, params, method: .put){response in
            print("response")
            
        }
    }
    
    @IBAction func onOptionClicked(_ sender: Any) {
        deleteDevice()
    }
    
    func deleteDevice(){
        NetworkManager().requestDataAsync(type: MyGardenDeviceListModel.self, APP_URL.DEVICE_USER_LIST + "/\(self.data?.Id ?? 0)", method: .delete){response in
            self.viewController?.getDeviceList()
            self.viewController?.ShowAlert(message: "Gerät wurde erfolgreich gelöscht")
            
        }
    }
    
    @IBAction func onFreezeSwitch(_ sender: Any) {
        var currentState = data?.notifyForFrost
        
        data?.notifyForFrost = !currentState!
        updateDeviceValue()
    }
    @IBAction func onStormSwitch(_ sender: Any) {
        var currentState = data?.notifyForWind
        
        data?.notifyForWind = !currentState!
        updateDeviceValue()
        
    }
    
    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}
