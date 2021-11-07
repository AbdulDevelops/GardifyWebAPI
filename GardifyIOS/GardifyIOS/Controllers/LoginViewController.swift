//
//  LoginViewController.swift
//  GardifyText
//
//  Created by Netzlab on 30.07.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit
import Alamofire

class LoginViewController: UIViewController, UITextFieldDelegate{
    
    
    @IBOutlet weak var usernameTextField: UITextField!
    @IBOutlet weak var passwordTextField: UITextField!
    @IBOutlet weak var loginButton: UIButton!
    @IBOutlet weak var backButton: UIButton!
    @IBOutlet weak var outerView: UIView!
    @IBOutlet weak var showPasswordButton: UIButton!
    @IBOutlet weak var registerButton: UIButton!
    
    
    var tabBar: MainTabBarController?
    
    var isPasswordShown: Bool = false
    
    override func viewDidLoad() {
        super.viewDidLoad()
    
        pageConfiguration()
        
        

        // Do any additional setup after loading the view.
    }
    
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
        
        self.updateNavigationBarExternal(tabController: tabBar!, isMain: true, "EINLOGGEN")

    }
    
    func pageConfiguration(){
        self.applyTheme()
        self.outerView.addBorderRadius()
        self.outerView.addShadow()
        self.loginButton.setGreenButton()
        self.backButton.setWhiteButton()
        
        self.showPasswordButton.alpha = 0
    }
    
    @IBAction func onLogin(_ sender: Any) {
        requestLogin()
    }
    
    @IBAction func onBack(_ sender: Any) {
        self.navigationController?.popViewController(animated: true)

    }
    
    func requestLogin(){
        var username = usernameTextField.text
        var password = passwordTextField.text
        
        let parameters = ["Email": username!, "Password": password!, "RememberMe": false] as [String : Any]
        
//        let networkManager = NetworkManager()
        
        NetworkManager().requestDataAsync(type: LoginModel.self, APP_URL.USER_LOGIN, parameters, method: .post, printRequest: true){ response in
            
            if !response.success{
                self.ShowAlert(message: response.result as! String)
                return
            }
            
            UserDefaultKeys.IsLoggedIn.set(true)
            UserDefaultKeys.UserId.set((response.result as! LoginModel).UserId)
            UserDefaultKeys.Username.set((response.result as! LoginModel).Name)
            UserDefaultKeys.ExpireDate.set((response.result as! LoginModel).ExpiresUtc)
            
            print("expired in", UserDefaultKeys.ExpireDate.string())
            UserDefaultKeys.JWT.set((response.result as! LoginModel).Token)
            UserDefaultKeys.Email.set((response.result as! LoginModel).Email)
            self.navigationController?.popViewController(animated: true)
            
        }
    }
    
    func textFieldDidBeginEditing(_ textField: UITextField) {
        if textField != passwordTextField{
            return
        }
        
        self.showPasswordButton.alpha = 1
    }
    
    func textFieldDidEndEditing(_ textField: UITextField) {
        if textField != passwordTextField{
            return
        }
        isPasswordShown = false
        updateShowPasswordButton()
        self.showPasswordButton.alpha = 0
    }
    
    func updateShowPasswordButton(){
        if isPasswordShown{
            self.showPasswordButton.setTitle("ausblenden", for: .normal)
            self.passwordTextField.isSecureTextEntry = false
        }
        else{
            self.showPasswordButton.setTitle("anzeigen", for: .normal)
            self.passwordTextField.isSecureTextEntry = true

        }
    }
    
    @IBAction func onPasswordShowToggle(_ sender: Any) {
        isPasswordShown = !isPasswordShown
        updateShowPasswordButton()
    }
    
    @IBAction func onRegisterClick(_ sender: Any) {
        
    }
    
    
    func textFieldShouldReturn(_ textField: UITextField) -> Bool {
        print(textField)
        if textField == usernameTextField{
            textField.resignFirstResponder()
            passwordTextField.becomeFirstResponder()
        }
        else{
            passwordTextField.resignFirstResponder()
            requestLogin()
        }
        return true
    }
    
    override func touchesBegan(_ touches: Set<UITouch>, with event: UIEvent?) {
        print("touched")
        view.endEditing(true    )
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
