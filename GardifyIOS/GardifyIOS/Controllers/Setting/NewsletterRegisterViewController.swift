//
//  NewsletterRegisterViewController.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 14.09.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class NewsletterRegisterViewController: UIViewController {

    @IBOutlet weak var newsletterRegisterButton: UIButton!
    @IBOutlet weak var firstNameField: UITextField!
    @IBOutlet weak var lastNameField: UITextField!
    @IBOutlet weak var emailField: UITextField!
    
    override func viewDidLoad() {
        super.viewDidLoad()

        pageConfiguration()
        // Do any additional setup after loading the view.
    }
    

    func pageConfiguration(){
        
        self.emailField.text = UserDefaultKeys.Email.string()
        
        self.newsletterRegisterButton.setGreenButton()
    }
    
    @IBAction func onSubmit(_ sender: Any) {
        
        if firstNameField.text == nil || lastNameField.text == nil || emailField.text == nil {
            self.ShowBackAlert(message: "Bitte füllen Sie alle Informationen aus")
            return
        }
        
        let params: [String: Any?] = [
            "FirstName" : self.firstNameField.text!,
            "LastName" : self.lastNameField.text!,
            "Email" : self.emailField.text!
        ]
        self.showSpinner(onView: self.view)
        print("newsletter params is", params)
        NetworkManager().requestDataAsync(type: String.self, APP_URL.NEWSLETTER_BASE, params, method: .post){response in
            self.removeSpinner()
            self.ShowBackAlert(message: "Vielen Dank für Ihre Anmeldung")
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
