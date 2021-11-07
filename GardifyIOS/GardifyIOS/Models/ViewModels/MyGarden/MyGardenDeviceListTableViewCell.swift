//
//  MyGardenDeviceListTableViewCell.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 03.09.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class MyGardenDeviceListTableViewCell: UITableViewCell {

    
    @IBOutlet weak var deviceImage: UIImageView!
    @IBOutlet weak var deviceName: UILabel!
    @IBOutlet weak var outerView: UIView!
    @IBOutlet weak var deviceCount: UIButton!
    @IBOutlet weak var valueStepper: UIStepper!
    
    @IBOutlet weak var deleteButton: UIButton!
    
    var additionToggle: Bool = false
    var data: MyGardenDeviceListModel?
    var viewController: MyGardenViewController?
    
    override func awakeFromNib() {
        super.awakeFromNib()
        
        pageConfiguration()
        // Initialization code
    }
    
    func pageConfiguration(){
        self.backgroundColor = .clear
        self.outerView.addBorderRadius()
        self.deviceCount.setGreenButton()
        self.deviceCount.isUserInteractionEnabled = false
        self.deleteButton.setRedButton()
    }

    func onConfigure(data: MyGardenDeviceListModel){
        self.deviceName.text = data.Name
        self.deviceCount.setTitle("\(data.Count)", for: .normal)
        self.data = data
        self.valueStepper.value = Double(data.Count)
        
        
        
    }

    @IBAction func onStep(_ sender: Any) {
        
        if valueStepper.value < 1{
            valueStepper.value = 1
            return
        }
        self.data?.Count = Int(valueStepper.value)
        self.deviceCount.setTitle("\(data!.Count)", for: .normal)

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
        
//        NetworkManager().requestRawAsync(type: MyGardenDeviceListModel.self, APP_URL.DEVICE_COUNT_UPDATE, self.data, method: "PUT"){response in
//            print(response)
//        }
//        let params: [String: Any?] = [
//            "Id"
//        ]
        
//        NetworkManager().requestDataAsync(type: MyGardenDeviceListModel.self, APP_URL.DEVICE_COUNT_UPDATE, params, method: .put)
        
    }
    @IBAction func onDelete(_ sender: Any) {
        
        NetworkManager().requestDataAsync(type: MyGardenDeviceListModel.self, APP_URL.DEVICE_USER_LIST + "/\(self.data?.Id ?? 0)", method: .delete){response in
            self.viewController?.getDeviceList()
            self.viewController?.ShowAlert(message: "Gerät wurde erfolgreich gelöscht")
            
        }
    }
    
    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}
