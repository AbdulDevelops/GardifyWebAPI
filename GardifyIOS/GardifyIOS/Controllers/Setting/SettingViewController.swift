//
//  SecondViewController.swift
//  GardifyText
//
//  Created by Netzlab on 23.07.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit
import Alamofire

class SettingViewController: UIViewController, UIImagePickerControllerDelegate, UINavigationControllerDelegate {
    
    @IBOutlet weak var outerView: UIView!
    
    var userData: UserInfo?
    var settingData: UserSettingInfo?
    
    
    @IBOutlet weak var usernameLabel: UILabel!
    @IBOutlet weak var emailLabel: UILabel!
    @IBOutlet weak var nameLabel: UILabel!
    @IBOutlet weak var addressLabel: UILabel!
    @IBOutlet weak var roundOuterView: UIView!
    @IBOutlet weak var tempUpButton: UIButton!
    @IBOutlet weak var tempDownButton: UIButton!
    @IBOutlet weak var freezingTempLabel: UILabel!
    
    @IBOutlet weak var frostWarningSwitch: UISwitch!
    @IBOutlet weak var stormWarningSwitch: UISwitch!
    @IBOutlet weak var pushNotifSwitch: UISwitch!
    @IBOutlet weak var emailNotifSwitch: UISwitch!
    @IBOutlet weak var colorModeSwitch: UISwitch!
    @IBOutlet weak var newsletterButton: UIButton!
    @IBOutlet weak var deleteAccountButton: UIButton!
    @IBOutlet weak var userProfileView: UIView!
    @IBOutlet weak var userProfileImage: CircularImageView!
    
    @IBOutlet weak var personalDetailsView: UIView!
    @IBOutlet weak var warningDetailsView: UIView!
    @IBOutlet weak var contactView: UIView!
    
    @IBOutlet weak var minusButton: UIButton!
    @IBOutlet weak var plusButton: UIButton!
    
    @IBOutlet weak var countLabel: UILabel!
    
    var imagePickerController : UIImagePickerController!
    var imagePath : String?
    var imageFile : UIImage?
    
    var fromCamera = false
    
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        if !self.isLoggedIn(){
            return
        }
        
        self.showSpinner(onView: self.view)
        DispatchQueue.main.async {
            self.getUserInfo()
            if !self.fromCamera{
                self.getUserImageProfile()
                
            }
        }

        
        configureGesture()
        //        getUserInfo()
        print("setting page")
        // Do any additional setup after loading the view.
        let tapGestureRecognizer = UITapGestureRecognizer(target: self, action: #selector(imageTapped(tapGestureRecognizer:)))
        userProfileImage.isUserInteractionEnabled = true
        userProfileImage.addGestureRecognizer(tapGestureRecognizer)
        
        
    }
    
    func configureGesture(){
        let tabBar = tabBarController as! MainTabBarController
        self.view.addGestureRecognizer(tabBar.swipeGesture)
        
    }
    
    func pageConfiguration(){
        
        self.userProfileView.layer.cornerRadius = 500
        //        self.userProfileImage.image = UIImage(named: "PlantPlaceholder")
        self.newsletterButton.setGreenButton()
        //self.deleteAccountButton.setRedButton()
        self.deleteAccountButton.addBorderRadius()
        
        self.personalDetailsView.addBorderRadius()
        self.warningDetailsView.addBorderRadius()
        self.contactView.addBorderRadius()
        
        self.stormWarningSwitch.set(width: 40, height: 25)
        self.frostWarningSwitch.set(width: 40, height: 25)
    }
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        if segue.identifier == "newsletter"
        {
            return
        }
        
        var controller = segue.destination as! SettingEditViewController
        
        switch segue.identifier {
        case "emailEdit":
            controller.pageMode = .email
            break
        case "passwordEdit":
            controller.pageMode = .password
            break
        case "nameEdit":
            controller.pageMode = .name
            break
        case "locationEdit":
            controller.pageMode = .location
            break
        default:
            break
        }
        
        
    }
    
    @objc func imageTapped(tapGestureRecognizer: UITapGestureRecognizer) {
        _ = tapGestureRecognizer.view as! CircularImageView
        let alertController = UIAlertController()

        let openPhotoAction = UIAlertAction(title: "Neues Profilbild hochladen", style: .default) {
            (action: UIAlertAction!) in
            self.openProfilePhotoChooseOption()
        }

        let removePhotoAction = UIAlertAction(title: "Profilbild entfernen", style: .default) {
            (action: UIAlertAction!) in
            self.openPhotoGallery()
            self.dismiss(animated: true, completion: nil)
            self.ShowAlert(message: "Profilfoto entfernt")
        }
        
        let OKAction = UIAlertAction(title: "Schließen", style: .cancel) {
            (action: UIAlertAction!) in
            self.dismiss(animated: true, completion: nil)
        }

        alertController.addAction(openPhotoAction)
        alertController.addAction(removePhotoAction)
        alertController.addAction(OKAction)

        self.present(alertController, animated: true, completion: nil)
        
    }
    
    func openProfilePhotoChooseOption(){
        let alertController = UIAlertController(title: "Wählen", message: "", preferredStyle: .alert)
    
        let openPhotoAction = UIAlertAction(title: "Kamera", style: .default) {
            (action: UIAlertAction!) in
            self.openCamera()
        }

        let removePhotoAction = UIAlertAction(title: "Album", style: .default) {
            (action: UIAlertAction!) in
            self.openPhotoGallery()
        }
        
        let OKAction = UIAlertAction(title: "Schließen", style: .cancel) {
            (action: UIAlertAction!) in
            self.dismiss(animated: true, completion: nil)
        }

        alertController.addAction(openPhotoAction)
        alertController.addAction(removePhotoAction)
        alertController.addAction(OKAction)

        self.present(alertController, animated: true, completion: nil)
    }
    
    func updatePageInfo(){
        
        
        
        self.usernameLabel.text = self.userData?.UserName
        //        self.emailLabel.text = self.userData?.
        
        self.emailLabel.text = "\(UserDefaultKeys.Email.string()!)"
        self.nameLabel.text = "\(self.userData?.FirstName ?? "") \(self.userData?.LastName ?? "")"
        
        self.addressLabel.text = "\(self.userData!.Zip), \(self.userData!.Country)"
        
    }
    
    func updateProfileImage(image: UIImage){
        self.userProfileImage.image = image
    }
    
    func updateSettingInfo(){
        if let data = self.settingData{
            //self.freezingTempLabel.text = "\(data.FrostDegreeBuffer)"
            self.frostWarningSwitch.isOn = data.ActiveFrostAlert
            self.stormWarningSwitch.isOn = data.ActiveStormAlert
            //self.pushNotifSwitch.isOn = data.AlertByPush
            //self.emailNotifSwitch.isOn = data.AlertByEmail
            
            //self.colorModeSwitch.isOn = UserDefaultKeys.darkmode.bool()
        }
        
    }
    
    @IBAction func onFrostWarningChange(_ sender: Any) {
        self.settingData?.ActiveFrostAlert = self.frostWarningSwitch.isOn
        self.updateSettingInfo()
        updateSettingUserData()
    }
    
    @IBAction func onStormWarningChange(_ sender: Any) {
        self.settingData?.ActiveStormAlert = self.stormWarningSwitch.isOn
        self.updateSettingInfo()
        updateSettingUserData()
        
        
    }
    
    @IBAction func onPushNotifChange(_ sender: Any) {
        self.settingData?.AlertByPush = self.pushNotifSwitch.isOn
        self.updateSettingInfo()
        updateSettingUserData()
        
    }
    
    @IBAction func onEmailNotifChange(_ sender: Any) {
        self.settingData?.AlertByEmail = self.emailNotifSwitch.isOn
        self.updateSettingInfo()
        updateSettingUserData()
        
    }
    
    @IBAction func onThemeChanges(_ sender: Any) {
        UserDefaultKeys.darkmode.set(self.colorModeSwitch.isOn)
        
        guard let windowScene = UIApplication.shared.connectedScenes.first as? UIWindowScene,
              let sceneDelegate = windowScene.delegate as? SceneDelegate
        else {
            return
        }
        let window = sceneDelegate.window
        
        window!.setTheme()
    }
    
    @IBAction func onDeleteAccount(_ sender: Any) {
        
        let alertController = UIAlertController(title: "Sind Sie sicher?", message: "", preferredStyle: .alert)
        
        let deleteAccount = UIAlertAction(title: "Konto Löschen", style: .default) {
            (action: UIAlertAction!) in
            self.dismiss(animated: true, completion: nil)
            self.doDeleteAccount()
//            self.openProfilePhotoChooseOption()
        }
        
        let OKAction = UIAlertAction(title: "Abbrechen", style: .cancel) {
            (action: UIAlertAction!) in
            self.dismiss(animated: true, completion: nil)
        }
        

        alertController.addAction(deleteAccount)
        alertController.addAction(OKAction)
        self.present(alertController, animated: true, completion: nil)

    }
    
    
    func doDeleteAccount(){
   
        NetworkManager().requestDataAsync(type: String.self, APP_URL.ACCOUNT_USER_DELETE + UserDefaultKeys.UserId.string()!, nil, method: .delete ){response in
            self.tabBarController?.selectedIndex = 0
            userLogOut()
            self.ShowAlert(message: "Sie sind abgemeldet")

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
            
            self.updatePageInfo()
        }
        
        
        
        NetworkManager().requestDataAsync(type: UserSettingInfo.self, APP_URL.ACCOUNT_USER_SETTING){response in
            if !response.success{
                
                self.ShowAlert(message: response.result as! String)
                return
            }
            
            self.settingData = response.result as! UserSettingInfo
            self.updateSettingInfo()
        }
        
  
    }
    
    func getUserImageProfile(){
        
//        self.showSpinner(onView: self.view)
        
        NetworkManager().requestDataAsync(type: UserImageInfo.self, APP_URL.ACCOUNT_USER_IMAGES){response in
              if !response.success{
                  
                  self.ShowAlert(message: response.result as! String)
                self.removeSpinner()

                  return
              }
            self.removeSpinner()

              if (response.result as! UserImageInfo).Images.count > 0{
                  var rawImage = (response.result as! UserImageInfo).Images[0].SrcAttr!
                  var imageUrl = rawImage.toURL(false, false, false)
                  getImageFiles(files: [imageUrl]){images in
                      
                      self.updateProfileImage(image: images[0])
                  }
              }
              
              
          }
    }
    
    func openCamera(){
        imagePickerController = UIImagePickerController()
        imagePickerController.delegate = self
        imagePickerController.sourceType = .camera
        present(imagePickerController, animated: true, completion: nil)
    }
 
    
    func openPhotoGallery(){
        imagePickerController = UIImagePickerController()
        imagePickerController.delegate = self
        imagePickerController.sourceType = .photoLibrary
        present(imagePickerController, animated: true, completion: nil)
    }
    

    
    @IBAction func onTempUp(_ sender: Any) {
        self.settingData?.FrostDegreeBuffer += 1
        self.updateSettingInfo()
        updateSettingUserData()
    }
    
    @IBAction func onTempDown(_ sender: Any) {
        self.settingData?.FrostDegreeBuffer -= 1
        self.updateSettingInfo()
        updateSettingUserData()
    }
    
    @IBAction func onMinusClick(_ sender: Any) {
        //Todo : not working
        let val: Int = Int(self.countLabel.text!)!
        print(val)
        self.countLabel.text = "\(val - 1)"
        
        self.settingData?.FrostDegreeBuffer = val - 1
        updateSettingUserData()
//        print((val ?? 0 - 1) as? String)
//        print(self.countLabel.text)
        
    }
    
    @IBAction func onPlusClick(_ sender: Any) {
        let val: Int = Int(self.countLabel.text!)!
        print(val)
        self.countLabel.text = "\(val + 1)"
        self.settingData?.FrostDegreeBuffer = val + 1
        updateSettingUserData()
        //Todo : implement same logic as minus
    }
    
    
    
     func imagePickerController(_ picker: UIImagePickerController, didFinishPickingMediaWithInfo info: [UIImagePickerController.InfoKey : Any]) {
            imagePickerController.dismiss(animated: true, completion: nil)
        fromCamera = true
    //        imagePath = (info[UIImagePickerController.InfoKey(rawValue: UIImagePickerController.InfoKey.imageURL.rawValue)] as! NSURL).absoluteString

            imageFile = info[UIImagePickerController.InfoKey(rawValue: UIImagePickerController.InfoKey.originalImage.rawValue)] as? UIImage
            self.updateProfileImage(image: imageFile!)
            userProfileImage.image = imageFile
            
            
            uploadedImage(imageFile: imageFile!)
            
        }
    
    func uploadedImage(imageFile: UIImage){
        if imageFile == nil{
            self.ShowAlert(message: "Bitte laden Sie ein Bild hoch")
            
            return
        }
        
        
        DispatchQueue.main.async {
            
            let authHeader = NetworkManager()._getTokenHeaders()
            let manager = Alamofire.SessionManager.default
            let headers: HTTPHeaders = ["Content-type": "multipart/form-data", "Content-Disposition": "form-data", "Authorization": authHeader["Authorization"]!]
           
            do{
                manager.upload(multipartFormData: { (formData) in
                    if let fileData = (self.imageFile?.resizedTo2MB()!.pngData()) { // File data
                        formData.append(fileData, withName: "imageFile", fileName: "file.png", mimeType: "image/png")
                        formData.append(Data("profile.img".utf8), withName: "imageTitle")
                    }
                }, to: APP_URL.PROFILE_PHOTO_UPLOAD, method: HTTPMethod.post, headers: headers){encoding in
                    print("response is",encoding)
                    switch encoding{
                    case .success(let req, _, _):
                        self.getUserInfo()
                        self.removeSpinner()
                        self.fromCamera = false

                    case .failure:
                        self.ShowAlert(message: "es gibt einen Fehler")
                        self.removeSpinner()
                        self.fromCamera = false

                        break
                    }
                }
            }catch{
                self.ShowAlert( message: "es gibt einen Fehler")
                self.fromCamera = false

                self.removeSpinner()
            }
            
        }
    }
    
    func updateSettingUserData(){
        let params: [String: Any?] = [
            "UserId": self.settingData?.UserId,
            "ActiveFrostAlert": self.settingData?.ActiveFrostAlert,
            "ActiveStormAlert": self.settingData?.ActiveStormAlert,
            "AlertByPush": self.settingData?.AlertByPush,
            "AlertByEmail": self.settingData?.AlertByEmail,
            "FrostDegreeBuffer": self.settingData?.FrostDegreeBuffer
        ]
        NetworkManager().requestDataAsync(type: UserSettingInfo.self, APP_URL.ACCOUNT_USER_SETTING_UPDATE, params, method: .put){response in
            
        }
    }
    
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
        
        self.updateNavigationBar(isMain: true)
        self.applyTheme()
        self.configureSinglePadding()
        pageConfiguration()
//
        if UserDefaultKeys.Email.string() == nil{
            self.ShowBackAlert(message: "Bitte melden Sie sich erneut an, um alle Informationen anzuzeigen.")
            self.tabBarController?.selectedIndex = 0
            return
        }
        
        
        if !self.isLoggedIn(){
            self.tabBarController?.selectedIndex = 0

            return
        }
//
//        self.showSpinner(onView: self.view)
        DispatchQueue.main.async {
            self.getUserInfo()
            if !self.fromCamera{
                self.getUserImageProfile()

            }
        }
        
    }
    
    
}

