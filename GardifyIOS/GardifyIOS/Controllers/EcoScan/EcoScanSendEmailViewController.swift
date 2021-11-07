//
//  EcoScanSendEmailViewController.swift
//  GardifyIOS
//
//  Created by Netzlab on 08.12.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit
import Alamofire

class EcoScanSendEmailViewController: UIViewController {

    
    @IBOutlet weak var emailTextField: UITextField!
    
    @IBOutlet weak var senderEmailTextField: UITextField!
    
    @IBOutlet weak var fromNameTextField: UITextField!
    
    @IBOutlet weak var toNameTextField: UITextField!
    
    @IBOutlet weak var messageTextArea: UITextView!
    
    @IBOutlet weak var outerView: UIView!
    
    @IBOutlet weak var sendButton: UIButton!
    
    var ecoScanImage: UIImage?
    
    
    override func viewDidLoad() {
        super.viewDidLoad()

        self.configurePadding()
        self.applyTheme()
        configurePage()
        
        // Do any additional setup after loading the view.
    }
    
    func configurePage(){
        
        outerView.addBorderRadius()
        messageTextArea.addBorderWidth()
        messageTextArea.addBorderRadiusSmall()
        messageTextArea.text = "es ist jetzt ganz einfach, selbst aktiv etwas gegen das Insekten- und Bienensterben und für mehr Biodiversität zu tun. Privaten Gärten und Balkonen kommt dabei eine sehr wichtige Aufgabe zu, z. B. durch die richtige Auswahl der Pflanzen. Im Anhang siehst du den Ökoscan von meinem Garten, der auf www.gardify.de erstellt wurde. Hier kannst Du ganz einfach in der Pflanzensuche insektenfreundliche Pflanzen finden und Pflanzen per Foto bestimmen. Die Anwendung ist übrigens kostenlos.\nVielleicht hast du ja Lust, deinen eigenen Garten auch mal zu checken."
        
        sendButton.setGreenButton()
        
    }
    
    @IBAction func onSendEmail(_ sender: Any) {
        
        print("message send is",self.messageTextArea.text?.utf8, self.messageTextArea.text)
        
        let imageData = self.ecoScanImage?.resizedTo200kb()!.pngData()
        let image64 = imageData?.base64EncodedString()
        print("image base is", image64!)
        let params: [String: Any?] = [
            "email": self.emailTextField.text,
            "fromMail": self.senderEmailTextField.text,
            "fromName": self.fromNameTextField.text,
            "toName": self.toNameTextField.text,
            "emailText": self.messageTextArea.text,
            "image": "data:image/png;base64,\(image64!)",
        ]
        
        let emailText = self.emailTextField.text
        let fromMailText = self.senderEmailTextField.text

        let fromNameText = self.fromNameTextField.text

        let toNameText = self.toNameTextField.text

        let emailFullText = self.messageTextArea.text

//
//        print("params is", params)
//
//        NetworkManager().requestDataAsync(type: String.self, APP_URL.ACCOUNT_SEND_ECOSCAN_MAIL, params, method: .post, printRequest: true){response in
//
//        }
        
        
        DispatchQueue.main.async {

            let authHeader = NetworkManager()._getTokenHeaders()
            let manager = Alamofire.SessionManager.default
            let headers: HTTPHeaders = ["Content-type": "multipart/form-data", "Content-Disposition": "form-data", "Authorization": authHeader["Authorization"]!]

            do{
                manager.upload(multipartFormData: { (formData) in
                    formData.append(Data(emailText?.utf8 ?? "".utf8), withName: "email")
                    formData.append(Data(fromMailText?.utf8 ?? "".utf8), withName: "fromMail")
                    formData.append(Data(fromNameText?.utf8 ?? "".utf8), withName: "fromName")
                    formData.append(Data(toNameText?.utf8 ?? "".utf8), withName: "toName")
                    formData.append(Data(emailFullText?.utf8 ?? "".utf8), withName: "emailText")

                    let imageData = self.ecoScanImage?.pngData()
                    let image64 = imageData?.base64EncodedString(options: .lineLength64Characters)

                    print("base image is", image64)
                    formData.append(Data("data:image/png;base64,\(image64!)".utf8), withName: "image")

                }, to: APP_URL.ACCOUNT_SEND_ECOSCAN_MAIL, method: HTTPMethod.post, headers: headers){encoding in
                    print("response is",encoding)
                    switch encoding{
                    case .success(let req, _, _):
//                        self.getUserInfo()
                        self.ShowBackAlert(message: "Deine Ökoscan-Ergebnis wurde gesendet")
                        self.removeSpinner()
//                        self.fromCamera = false

                    case .failure:
                        self.ShowAlert(message: "es gibt einen Fehler")
                        self.removeSpinner()
//                        self.fromCamera = false

                        break
                    }
                }
            }catch{
                self.ShowAlert( message: "es gibt einen Fehler")
            }
        }
    }
    
    

    override func touchesBegan(_ touches: Set<UITouch>, with event: UIEvent?) {
        print("touched")
        view.endEditing(true )
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

extension EcoScanSendEmailViewController: UIScrollViewDelegate{
    
    func scrollViewDidScroll(_ scrollView: UIScrollView) {
        view.endEditing(true )

    }
}
