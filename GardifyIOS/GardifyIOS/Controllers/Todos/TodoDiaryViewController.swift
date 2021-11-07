//
//  TodoDiaryViewController.swift
//  GardifyIOS
//
//  Created by Rifat Hussain on 24.09.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit
import Alamofire

class TodoDiaryViewController: UIViewController, UIImagePickerControllerDelegate, UINavigationControllerDelegate {
    
    @IBOutlet weak var titleTextField: UITextField!
    @IBOutlet weak var descriptionTextField: UITextField!
    @IBOutlet weak var datePickerView: UIDatePicker!
    @IBOutlet weak var photosLabel: UILabel!
    @IBOutlet weak var saveButton: UIButton!
    
    
    @IBOutlet var uploadImageLabel: UILabel!
    
    var imagePickerController : UIImagePickerController!
    var imageFile : UIImage?
    var date = Date()
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        self.configurePage()
        self.configurePadding()
        
        
        let dismissKeyBoardTap = UITapGestureRecognizer(target: self.view, action: #selector((UIView.endEditing)))
        view.addGestureRecognizer(dismissKeyBoardTap)
        
        let tapGestureRecognizer = UITapGestureRecognizer(target: self, action: #selector(imageTapped(tapGestureRecognizer:)))

    }
    
    @IBAction func onUploadClicked(_ sender: Any) {
        
        openPhotoGallery()
    }
    func configurePage(){
 
        saveButton.setGreenButton()
    }
    
    @objc func imageTapped(tapGestureRecognizer: UITapGestureRecognizer) {
        self.openPhotoGallery()
    }
    func openPhotoGallery(){
        imagePickerController = UIImagePickerController()
        imagePickerController.delegate = self
        imagePickerController.sourceType = .photoLibrary
        present(imagePickerController, animated: true, completion: nil)
    }
    
    func imagePickerController(_ picker: UIImagePickerController, didFinishPickingMediaWithInfo info: [UIImagePickerController.InfoKey : Any]) {
        imagePickerController.dismiss(animated: true, completion: nil)
        
        //        imagePath = (info[UIImagePickerController.InfoKey(rawValue: UIImagePickerController.InfoKey.imageURL.rawValue)] as! NSURL).absoluteString
        
        imageFile = info[UIImagePickerController.InfoKey(rawValue: UIImagePickerController.InfoKey.originalImage.rawValue)] as? UIImage
        
        uploadImageLabel.text = "1 Bild(er) ausgewählt"
//        photosLabel.text = "1 Bild(er) ausgewählt"
    }
    @IBAction func datePickerChanged(_ sender: Any) {
        self.date = datePickerView.date
    }
    
    @IBAction func clickSaveButton(_ sender: UIButton) {
        
        NetworkManager().requestDataAsync(type: MyGardenLightModel.self, APP_URL.USER_GARDEN_MAIN, printRequest: true){response in
            
            if !response.success{
                
                return
            }
            
            let params : [String: Any?] = [
                "Title":self.titleTextField.text,
                "Description": self.descriptionTextField.text,
                "Date": self.datePickerView.date.toString(output: "yyyy-MM-dd"),
                "UserId": UserDefaultKeys.UserId.string(),
                "EntryObjectId":(response.result as! MyGardenLightModel).Id,
                "EntryOf":3
            ]
            
            NetworkManager().requestDataAsync(type: Int.self, APP_URL.DIARY_ROUTE, params, method: .post){response in
                
                self.ShowBackAlert(message: "Spaichern")
                
                if self.imageFile == nil{
                    return
                }
                
                guard let idResponse = response.result as? Int else{
                    return
                }
                
                let authHeader = NetworkManager()._getTokenHeaders()
                let manager = Alamofire.SessionManager.default
                let headers: HTTPHeaders = ["Content-type": "multipart/form-data", "Content-Disposition": "form-data", "Authorization": authHeader["Authorization"]!]
                
                do{
                    manager.upload(multipartFormData: { (formData) in
                        if let fileData = (self.imageFile?.resizedTo2MB()!.pngData()) { // File data
                            formData.append(fileData, withName: "imageFile", fileName: "todoImage.png", mimeType: "image/png")
                            formData.append(Data("todoImage.png".utf8), withName: "imageTitle")
                            
                     
                            formData.append(Data("\(idResponse)".utf8), withName: "id")
    //                        formData.append("\(idResponse)".utf8, withName: "id")
                        }
                    }, to: APP_URL.TODO_IMAGE_UPLOAD, method: HTTPMethod.post, headers: headers){encoding in
                        print("response is",encoding)
                        switch encoding{
                        case .success(let req, _, _):
    //                        self.getUserInfo()
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
    //                self.fromCamera = false

                    self.removeSpinner()
                }
            }
            
        }
        
//        let params : [String: Any?] = [
//            "Title":self.titleTextField.text,"Description": descriptionTextField.text,"Date": datePickerView.date.toString(output: "yyyy-MM-dd"),"UserId": UserDefaultKeys.UserId.string(),"EntryObjectId":844,"EntryOf":3
//        ]
        
        
    }
    
}
