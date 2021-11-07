//
//  NewRegisterViewController.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 14.12.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class NewRegisterViewController: UIViewController {
    
    @IBOutlet weak var emailText: UITextField!
    @IBOutlet weak var usernameText: UITextField!
    @IBOutlet weak var countryLabel: UILabel!
    @IBOutlet weak var countryButton: UIButton!
    @IBOutlet weak var postalcodeText: UITextField!
    @IBOutlet weak var passwordText: UITextField!
    @IBOutlet weak var repeatPasswordText: UITextField!
    @IBOutlet weak var countryTable: UITableView!
    @IBOutlet weak var countryTableHeight: NSLayoutConstraint!
    @IBOutlet weak var registerButton: UIButton!
    @IBOutlet weak var subscribeNewsletterSwitch: UISwitch!
    
    
    var isCountryExtended : Bool = false
    
    var countryList: [String] = ["Deutschland","Österreich","Schweiz"]
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        pageConfiguration()
        
        self.countryTable.reloadData()
        
        // Do any additional setup after loading the view.
    }
    
    
    
    func pageConfiguration() {
        self.passwordText.isSecureTextEntry = true
        self.repeatPasswordText.isSecureTextEntry = true
        self.registerButton.addBorderRadius()
    }
    
    @IBAction func onCountryButtonClick(_ sender: Any) {
        self.updateCountryTable()
    }
    
    func updateCountryTable() {
        self.isCountryExtended = !self.isCountryExtended
        if self.isCountryExtended {
            self.countryTableHeight.constant = CGFloat(40*countryList.count)
        }
        else {
            self.countryTableHeight.constant = CGFloat(0)
        }
    }
    
    @IBAction func onClickOutside(_ sender: Any) {
        print(sender)
        view.endEditing(true)
    }
    
    
    @IBAction func onRegisterClick(_ sender: Any) {
        //check for mandatory data entry
        if (self.emailText.text == "" || self.usernameText.text == "" || self.countryLabel.text == "" || self.postalcodeText.text == "" || self.passwordText.text == "" || self.repeatPasswordText.text == "") {
            self.ShowAlert(message: "Bitte geben Sie alle Pflichtfelder ein")
            return
        }
        
        if self.passwordText.text != self.repeatPasswordText.text {
            ShowAlert(message: "Passwörter stimmen nicht überein")
            return
        }
        
//        if self.emailText.text == "" {
//            self.emailText.addBorder(toSide: .bottom, withColor: .init(red: 256, green: 0, blue: 0, alpha: 1), andThickness: 1)
//            return
//        }
//        else {
//            self.emailText.clearBorderWidth()
//        }
//
//        if self.usernameText.text == "" {
//            self.usernameText.addBorder(toSide: .bottom, withColor: .init(red: 256, green: 0, blue: 0, alpha: 1), andThickness: 1)
//            return
//        }
//        else {
//            self.usernameText.clearBorderWidth()
//        }
//
//        if self.countryLabel.text == "Land auswählen*" {
//            self.countryLabel.text = "Deutschland" //for testing : remove later
////            self.countryLabel.addBorder(toSide: .bottom, withColor: .init(red: 256, green: 0, blue: 0, alpha: 1), andThickness: 1)
////            return
//        }
//        else {
//            self.countryLabel.clearBorderWidth()
//        }
//
//        if self.postalcodeText.text == "" {
//            self.postalcodeText.addBorder(toSide: .bottom, withColor: .init(red: 256, green: 0, blue: 0, alpha: 1), andThickness: 1)
//            return
//        }
//        else {
//            self.postalcodeText.clearBorderWidth()
//        }
//
//        if self.postalcodeText.text == "" {
//            self.postalcodeText.addBorder(toSide: .bottom, withColor: .init(red: 256, green: 0, blue: 0, alpha: 1), andThickness: 1)
//            return
//        }
//        else {
//            self.postalcodeText.clearBorderWidth()
//        }
//
//        if self.passwordText.text == "" {
//            self.passwordText.addBorder(toSide: .bottom, withColor: .init(red: 256, green: 0, blue: 0, alpha: 1), andThickness: 1)
//            return
//        }
//        else {
//            self.passwordText.clearBorderWidth()
//        }
//
//        if self.repeatPasswordText.text == "" {
//            self.repeatPasswordText.addBorder(toSide: .bottom, withColor: .init(red: 256, green: 0, blue: 0, alpha: 1), andThickness: 1)
//            return
//        }
//        else {
//            self.repeatPasswordText.clearBorderWidth()
//        }
        
        
        //check for matching password
        
        
        self.registerNewUser()
    }
    
    func registerNewUser(){
        let params: [String: Any?] = [
            "Firstname": "",
            "Lastname": "",
            "Street": "",
            "HouseNr": "",
            "PLZ": Int(self.postalcodeText.text!),
            "City": "",
            "Country":self.countryLabel.text,
            "UserName":self.usernameText.text,
            "Gender": "",
            "Email":self.emailText.text,
            "Password":self.passwordText.text,
            "ConfirmPassword":self.repeatPasswordText.text
        ]
        print(params)
        let sendURL = APP_URL.USER_REGISTER_NEW_USER + "\(self.subscribeNewsletterSwitch.isOn)"
        NetworkManager().requestDataAsync(type: UserSettingInfo.self, sendURL, params, method: .post, ignoreContent: true){response in
            //Handle success or failure
            print("response: ", response.success)
            print("response result: ",response.result!)
            if response.success {
                //self.ShowAlert(message: "Vielen dank.....")
                self.ShowBackAlert(message: "Vielen Dank für Deine Registrierung! \nUm Deine Registrierung abzuschließen erhältst Du von uns eine Bestätigungsmail mit einem Bestätigungslink, der Deine Registrierung aktiviert. \nKeine Bestätigungsmail erhalten? Bitte überprüfe Deinen Spam- bzw. Junkmail-Ordner.")
            }
            else {
                self.ShowAlert(message: response.result as! String)
                
            }
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

extension NewRegisterViewController: UITableViewDelegate, UITableViewDataSource{
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return self.countryList.count
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = UITableViewCell()
        
        cell.textLabel?.text = countryList[indexPath.row]
        
        return cell
    }
    
//    func tableView(_ tableView: UITableView, willSelectRowAt indexPath: IndexPath) -> IndexPath? {
//
//        return indexPath
//    }
    
    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        self.countryLabel.text = self.countryList[indexPath.row]
        self.updateCountryTable()
    }
    
    func tableView(_ tableView: UITableView, didDeselectRowAt indexPath: IndexPath) {
        self.countryLabel.text = self.countryList[indexPath.row]
        //self.updateCountryTable()
    }
    
}
