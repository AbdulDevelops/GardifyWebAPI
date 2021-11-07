//
//  NewsLetterViewController.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 15.12.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class NewsLetterViewController: UIViewController {
    
    @IBOutlet weak var emailText: UITextField!
    @IBOutlet weak var submitButton: UIButton!
    @IBOutlet weak var emailView: UIView!
    @IBOutlet weak var lastLabel: UILabel!
    @IBOutlet weak var cancelButton: UIButton!
    
    @IBOutlet weak var frontNameField: UITextField!
    
    @IBOutlet weak var nameField: UITextField!
    @IBOutlet var onScrollTap: UITapGestureRecognizer!
    override func viewDidLoad() {
        super.viewDidLoad()
        
        self.pageConfiguration()
        self.configurePadding()
        // Do any additional setup after loading the view.
    }
    
    
    @IBAction func onTapscreen(_ sender: Any) {
        
        self.view.endEditing(true)
    }
    
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
        
        self.updateNavigationBar(isMain: false, "", "NEWSLETTER", "main_logo")
        
    }
    
    func pageConfiguration() {
        self.emailView.addBorderRadius()
        self.submitButton.setGreenButton()
        self.cancelButton.setGrayButton()
        self.emailView.backgroundColor = .clear
        
        self.lastLabel.attributedText = self.buildCustomeLabel()
    }
    
    func buildCustomeLabel() -> NSMutableAttributedString {
        
        let text1 = "Hast du Fragen zum Umgang mit Deinen Daten? Informiere dich über unsere "
        let text2 = "Datenschutzgrundsätze "
        let text3 = "und unsere "
        let text4 = "Allgemeinen Geschäftsbedingungen."
        
        let attrs = [NSAttributedString.Key.font : UIFont.boldSystemFont(ofSize: 17)]
        
        let normalText1 = NSMutableAttributedString(string:text1)
        let boldText2 = NSMutableAttributedString(string:text2, attributes:attrs)
        let normalText3 = NSMutableAttributedString(string:text3)
        let boldText4 = NSMutableAttributedString(string:text4, attributes:attrs)
        
        
        normalText1.append(boldText2)
        normalText1.append(normalText3)
        normalText1.append(boldText4)
        
        return normalText1
    }
    
    @IBAction func onSubscribe(_ sender: Any) {
        
        
        let params: [String: Any?] = [
            "LastName": self.nameField.text,
            "FirstName": self.frontNameField.text,
            "Email": self.emailText.text
        ]
        
        NetworkManager().requestDataAsync(type: String.self, APP_URL.NEWSLETTER_BASE, params, method: .post){
            response in
            
            if !response.success{
                
                self.ShowAlert(message: "Sie haben sich bereits für den Newsletter angemeldet")
                return
            }
            
            self.ShowAlert(message: "Der Newsletter wurde erfolgreich bestellt.")
        }
        
        
    }
    
    
    @IBAction func onUnsubscribe(_ sender: Any) {
        
        NetworkManager().requestDataAsync(type: String.self, APP_URL.NEWSLETTER_BASE + "/unsubscribe", nil, method: .post){
            response in
            
            if !response.success{
                
                self.ShowAlert(message: "Ein Fehler ist aufgetreten: Sie haben sich bereits für den Newsletter abgemeldet")
                return
            }
            
            self.ShowAlert(message: "Der Newsletter wurde erfolgreich abbestellt.")
        }
    }
    
    override func touchesBegan(_ touches: Set<UITouch>, with event: UIEvent?) {
        self.view.endEditing(true)
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
