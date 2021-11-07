//
//  SettingEditViewController.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 14.09.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

enum SettingType {
    case email
    case password
    case name
    case location
}


class SettingEditViewController: UIViewController {

    @IBOutlet weak var settingEditTitleLabel: UILabel!
    @IBOutlet weak var emailContainerView: UIView!
    @IBOutlet weak var currentPasswordContainerView: UIView!
    @IBOutlet weak var newPasswordContainerView: UIView!
    @IBOutlet weak var newPasswordConfirmContainerView: UIView!
    @IBOutlet weak var nameContainerView: UIView!
    @IBOutlet weak var newUsernameContainerView: UIView!
    @IBOutlet weak var newAddressContainerView: UIView!
    @IBOutlet weak var newPostCodeContainerView: UIView!
    @IBOutlet weak var cityContainerView: UIView!
    @IBOutlet weak var countryContainerView: UIView!
    @IBOutlet weak var saveButton: UIButton!
    
    @IBOutlet weak var backButton: UIButton!
    @IBOutlet weak var emailAddressTextField: UITextField!
    @IBOutlet weak var currentPasseordTextField: UITextField!
    @IBOutlet weak var newPasswordTextField: UITextField!
    @IBOutlet weak var newPasswordConfimTextField: UITextField!
    @IBOutlet weak var firstNameTextfield: UITextField!
    @IBOutlet weak var lastNameTextField: UITextField!
    @IBOutlet weak var usernameTextField: UITextField!
    @IBOutlet weak var streetNameTextField: UITextField!
    @IBOutlet weak var streetNumberTextField: UITextField!
    @IBOutlet weak var postNumberTextField: UITextField!
    @IBOutlet weak var cityTextField: UITextField!
    @IBOutlet weak var countryTextField: UITextField!
    
    @IBOutlet weak var currentPasswordConstraints: NSLayoutConstraint!
    @IBOutlet weak var nameContainerConstraints: NSLayoutConstraint!
    @IBOutlet weak var streetContainerConstraints: NSLayoutConstraint!
    @IBOutlet weak var buttonToEmailConstraints: NSLayoutConstraint!
    
    @IBOutlet weak var buttonToPasswordConstraints: NSLayoutConstraint!
    @IBOutlet weak var buttonToNameConstraints: NSLayoutConstraint!
    @IBOutlet weak var buttonToCountryContainer: NSLayoutConstraint!
    @IBOutlet weak var addressToUserNameContainer: NSLayoutConstraint!
    @IBOutlet weak var LocaltionStreetConstraints: NSLayoutConstraint!
    @IBOutlet weak var StreetNumberLabel: UILabel!
    
    var pageMode: SettingType?
    var userData: UserInfo?
    var locationData: LocationInfo?
    var offsetY:CGFloat = 0

    
    override func viewDidLoad() {
        super.viewDidLoad()

//        self.configurePadding()
        // Do any additional setup after loading the view.
        self.pageConfiguration()
        self.getUserInfo()
    }
    
    override func viewDidAppear(_ animated: Bool) {
        loadData()
    }
    
    func pageConfiguration(){
        if pageMode != .name{
            emailContainerView.isHidden = true
            currentPasswordContainerView.isHidden = true
            newPasswordContainerView.isHidden = true
            newPasswordConfirmContainerView.isHidden = true
            nameContainerView.isHidden = true
            newUsernameContainerView.isHidden = true
            addressToUserNameContainer.isActive = false
            newAddressContainerView.isHidden = true
            newPostCodeContainerView.isHidden = true
            cityContainerView.isHidden = true
            countryContainerView.isHidden = true
            saveButton.isHidden = true
        }
        else{
            self.configurePadding()
        }
        
        
        saveButton.setTitle("Speichern", for: .normal)
        saveButton.setGreenButton()
        
        backButton.setGrayButton()
        
    }
    
    @IBAction func onBack(_ sender: Any) {
        
        dismiss(animated: true, completion: nil)

    }
    override func viewWillDisappear(_ animated: Bool) {
        super.viewWillDisappear(animated)
        unsubscribeFromAllNotifications()
    }
    
    func loadData(){
        switch pageMode {
        case .email:
            emailContainerView.isHidden = false
            currentPasswordContainerView.isHidden = false
            buttonToEmailConstraints.isActive = true
            buttonToEmailConstraints.constant = 20
            saveButton.isHidden = false
            initializeHideKeyboard()
            break
        case .password:
            currentPasswordConstraints.constant = 20
            currentPasswordContainerView.isHidden = false
            newPasswordContainerView.isHidden = false
            newPasswordConfirmContainerView.isHidden = false
            buttonToPasswordConstraints.isActive = true
            buttonToPasswordConstraints.constant = 20
            saveButton.isHidden = false
            initializeHideKeyboard()
            break
        case .name:

            self.updatePageInfo()
            subscribeToNotification(UIResponder.keyboardWillShowNotification, selector: #selector(keyboardWillShowOrHide))
            subscribeToNotification(UIResponder.keyboardWillHideNotification, selector: #selector(keyboardWillShowOrHide))
            initializeHideKeyboard()
            break
        case .location:
            StreetNumberLabel.isHidden = true
            StreetNumberLabel.isEnabled = false
            streetNumberTextField.isEnabled = false
            streetNumberTextField.isHidden = true
            LocaltionStreetConstraints.isActive = true
            LocaltionStreetConstraints.constant = 0
            addressToUserNameContainer.isActive = false
            streetContainerConstraints.constant = 20
            newAddressContainerView.isHidden = false
            newPostCodeContainerView.isHidden = false
            cityContainerView.isHidden = false
            countryContainerView.isHidden = false
            buttonToCountryContainer.isActive = true
            buttonToCountryContainer.constant = 20
            saveButton.isHidden = false
            self.updatePageInfo()
            initializeHideKeyboard()
        default:
            break
        }
    
        
    }
    
    func updatePageInfo(){
        switch pageMode {
        case .name:
            firstNameTextfield.text = userData?.FirstName
            lastNameTextField.text = userData?.LastName
            usernameTextField.text = userData?.UserName
            streetNameTextField.text = userData?.Street
            if let houseNumber = userData?.HouseNr{
                streetNumberTextField.text = "\(houseNumber)"
            }
            postNumberTextField.text = userData?.Zip
            cityTextField.text = userData?.City
            countryTextField.text = userData?.Country
            break
        case .location:
            streetNameTextField.text = locationData?.street
            postNumberTextField.text = locationData?.zip
            cityTextField.text = locationData?.city
            countryTextField.text = locationData?.country
        default:
            break
        }
       
    }
    
    func getUserInfo(){
        NetworkManager().requestDataAsync(type: UserInfo.self, APP_URL.ACCOUNT_USER_INFO){response in
            if !response.success{
                
                self.ShowAlert(message: response.result as! String)
                self.removeSpinner()
                return
            }
            self.removeSpinner()
            self.userData = response.result as! UserInfo
            self.getLocationInfo()
        }
    }
    
    func getLocationInfo() {
        NetworkManager().requestDataAsync(type: LocationInfo.self, APP_URL.LOCATION_UPDATE, method: .get, printRequest: true){response in
            
            if !response.success{
                
                self.ShowAlert(message: response.result as! String)
                self.removeSpinner()
                return
            }
            self.removeSpinner()
            self.locationData = response.result as! LocationInfo
          
            
        }
        self.updatePageInfo()
    }
    
    
   
    @IBAction func clickSaveButton(_ sender: UIButton) {
        let userId = UserDefaultKeys.UserId.string()
        print("userId is ",userId!)
        switch pageMode {
        case .email:
            let newEmail = emailAddressTextField.text
            let password = currentPasseordTextField.text
            if newEmail != "" && password != ""{
                let params: [String: Any] = [
                    "Id" : "\(userId!)",
                    "NewEmail": newEmail!,
                    "Password": password!
                    
                ]
                NetworkManager().requestDataAsync(type: String.self, APP_URL.ACCOUNT_USER_EMAIL_UPDATE + "\(userId!)", params, method: .put, printRequest: true){response in
                    self.dismiss(animated: true, completion: .none)
                }
            }else{
                self.ShowAlert(message: "Füllen Sie Alle Informationen Aus")
            }
            self.getUserInfo()
            break
        case .password:
            let oldPassword = currentPasseordTextField.text
            let newPassword = newPasswordTextField.text
            let confirmPassword = newPasswordConfimTextField.text
            
            if oldPassword != "" && newPassword != "" && confirmPassword != ""{
                let params: [String: Any] = [
                    "Id" : "\(userId!)",
                    "OldPassword": oldPassword!,
                    "Password": newPassword!,
                    "ConfirmPassword": confirmPassword!
                    
                ]
                if newPassword != confirmPassword{
                    ShowAlert(message: "Passwort stimmt nicht überein!")
                    return
                }
                NetworkManager().requestDataAsync(type: String.self, APP_URL.ACCOUNT_USER_PASSWORD_UPDATE + "\(userId!)", params, method: .post, printRequest: true){response in
                    self.dismiss(animated: true, completion: .none)
                }
            }else{
                self.ShowAlert(message: "Füllen Sie Alle Informationen Aus")
            }
            self.getUserInfo()
            break
        case .name:
            if  let city = cityTextField.text, let country = countryTextField.text,let street = streetNameTextField.text, let houseNumber = streetNumberTextField.text, let username = usernameTextField.text, let firstName = firstNameTextfield.text, let lastName =  lastNameTextField.text, let zip = postNumberTextField.text{
                let params: [String: Any] = [
                    "City" : city,
                    "Country": country,
                    "Street": street,
                    "HouseNr": Int(houseNumber)!,
                    "UserName": username,
                    "FirstName": firstName,
                    "LastName": lastName,
                    "Zip": zip
                    
                ]
                NetworkManager().requestDataAsync(type: String.self, APP_URL.ACCOUNT_USER_DATA_UPDATE, params, method: .put, printRequest: true){response in
                    self.dismiss(animated: true, completion: .none)
                }
            }else{
                self.ShowAlert(message: "Füllen Sie Alle Informationen Aus")
            }
            self.getUserInfo()
            break
        case .location:
            if  let city = cityTextField.text, let country = countryTextField.text,let street = streetNameTextField.text, let houseNumber = streetNumberTextField.text, let zip = postNumberTextField.text{
                let params: [String: Any] = [
                    "City" : city,
                    "Country": country,
                    "Street": street + ", " + "\(houseNumber)",
                    "Zip": zip
                    
                ]
                NetworkManager().requestDataAsync(type: String.self, APP_URL.LOCATION_UPDATE, params, method: .put, printRequest: true){response in
                    self.dismiss(animated: true, completion: .none)
                    
                }
            }else{
                self.ShowAlert(message: "Füllen Sie Alle Informationen Aus")
            }
            self.getUserInfo()
            break
        default:
            self.getUserInfo()
            break
        }
    }
    

}

extension SettingEditViewController {
    func initializeHideKeyboard(){
        let tap: UITapGestureRecognizer = UITapGestureRecognizer(
            target: self,
            action: #selector(dismissMyKeyboard))
        view.addGestureRecognizer(tap)
    }
    
    @objc func dismissMyKeyboard(){
        view.endEditing(true)
    }
    
}
extension SettingEditViewController {
    func subscribeToNotification(_ notification: NSNotification.Name, selector: Selector) {
        NotificationCenter.default.addObserver(self, selector: selector, name: notification, object: nil)
    }
    func unsubscribeFromAllNotifications() {
        NotificationCenter.default.removeObserver(self)
    }
    @objc func keyboardWillShowOrHide(notification: NSNotification) {
        
        if let userInfo = notification.userInfo {
            let endFrame = userInfo[UIResponder.keyboardFrameEndUserInfoKey] as? CGRect
            let animationDuration = userInfo[UIResponder.keyboardAnimationDurationUserInfoKey] as? Double ?? 0
            let animationCurveRawValue = (userInfo[UIResponder.keyboardAnimationCurveUserInfoKey] as? Int) ?? Int(UIView.AnimationOptions.curveEaseInOut.rawValue)
            let animationCurve = UIView.AnimationOptions(rawValue: UInt(animationCurveRawValue))
            if let _ = endFrame, endFrame!.intersects(self.view.frame) {
                self.offsetY = self.saveButton.frame.maxY - endFrame!.minY
                UIView.animate(withDuration: animationDuration, delay: TimeInterval(0), options: animationCurve, animations: {
                    self.view.frame.origin.y = self.view.frame.origin.y - self.offsetY
                }, completion: nil)
            } else {
                if self.offsetY != 0 {
                    UIView.animate(withDuration: animationDuration, delay: TimeInterval(0), options: animationCurve, animations: {
                        self.view.frame.origin.y = self.view.frame.origin.y + self.offsetY
                        self.offsetY = 0
                    }, completion: nil)
                }
            }
        }
    }
}
