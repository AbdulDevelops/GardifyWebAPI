//
//  OwnTodoViewController.swift
//  GardifyIOS
//
//  Created by Rifat Hussain on 24.09.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit
import Alamofire

class OwnTodoViewController: UIViewController, UIPickerViewDelegate, UIPickerViewDataSource, UIImagePickerControllerDelegate, UINavigationControllerDelegate {
    
    
    @IBOutlet weak var titleTextField: UITextField!
    @IBOutlet weak var descriptionTextField: UITextField!
//    @IBOutlet weak var repetitionTypePickerView: UIPickerView!
    @IBOutlet weak var datePickerView: UIDatePicker!
    @IBOutlet weak var dueDatePicker: UIDatePicker!
    @IBOutlet weak var memoryPcikerView: UIPickerView!
//    @IBOutlet weak var photosLabel: UILabel!
    @IBOutlet weak var saveButton: UIButton!
    
    @IBOutlet weak var repetitionDropdownView: DropdownListView!
    
    @IBOutlet weak var memoryDropdownView: DropdownListView!
    
    @IBOutlet weak var repetitionPickerViewHeight: NSLayoutConstraint!
    @IBOutlet weak var memoryPickerViewHeight: NSLayoutConstraint!
    
    @IBOutlet var uploadImageLabel: UILabel!
    
    var imagePickerController : UIImagePickerController!
    var imageFile : UIImage?
    var selectedRepetitionType: String?
    var selectedMemoryData: String?
    var date = Date()
    var dueDate = Date()
    
    var parentView: TodoViewController?
    
    var repetitionTypeData = ["Niemals", "Täglich", "Wöchentlich", "Monatlich", "Jährlich", "2-jährig", "3-jährig", "4-jährig", "5-jährig"]
    
    var repetitionValue = [0,5,1,2,3,4,6,7,8]
    
    var selectedRepetitionIndex = 0
    
    var memoryValue = [0,1,2,3]
    var memoryData = ["Kalendereintrag", "Kalendereintrag hervorheben", "Pushnachricht", "Emailnachricht"]
    
    var selectedMemoryIndex = 0
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        self.configurePadding()
//        repetitionTypePickerView.delegate = self
//        repetitionTypePickerView.dataSource = self
//        memoryPcikerView.delegate = self
//        memoryPcikerView.dataSource = self
        self.configurePage()
        
        selectedRepetitionType = repetitionTypeData.first
        selectedMemoryData = memoryData.first
        
        configureDropdown()
        let dismissKeyBoardTap = UITapGestureRecognizer(target: self.view, action: #selector((UIView.endEditing)))
        view.addGestureRecognizer(dismissKeyBoardTap)
        
        let tapGestureRecognizer = UITapGestureRecognizer(target: self, action: #selector(imageTapped(tapGestureRecognizer:)))
//        photosLabel.isUserInteractionEnabled = true
//        photosLabel.addGestureRecognizer(tapGestureRecognizer)
//
        
    }
    
    func configureDropdown(){
        repetitionDropdownView.displayText = repetitionTypeData
        
        
        
        repetitionDropdownView.resetTable()
        
        memoryDropdownView.displayText = memoryData
        
        memoryDropdownView.resetTable()
    }
    
    func configurePage(){
//        photosLabel.layer.borderWidth = 0.2
//        photosLabel.layer.borderColor = UIColor.gray.cgColor
//        photosLabel.addBorderRadius()
//        repetitionTypePickerView.layer.borderWidth = 0.2
//        repetitionTypePickerView.layer.borderColor = UIColor.gray.cgColor
//        repetitionTypePickerView.addBorderRadius()
//        memoryPcikerView.layer.borderWidth = 0.2
//        memoryPcikerView.layer.borderColor = UIColor.gray.cgColor
//        memoryPcikerView.addBorderRadius()
        saveButton.setGreenButton()
    }
    
    func numberOfComponents(in pickerView: UIPickerView) -> Int {
        return 1
    }
    
    func pickerView(_ pickerView: UIPickerView, numberOfRowsInComponent component: Int) -> Int {
        
 
            return 1
        
    }
    
    func pickerView(_ pickerView: UIPickerView, titleForRow row: Int, forComponent component: Int) -> String? {
      
            return memoryData[row]
        
    }
    
    func pickerView(_ pickerView: UIPickerView, didSelectRow row: Int, inComponent component: Int) {
  
            selectedMemoryData = memoryData[row]
        
        
    }
    
    
    @objc func imageTapped(tapGestureRecognizer: UITapGestureRecognizer) {
        self.openPhotoGallery()
    }
    
    @IBAction func onUploadClicked(_ sender: Any) {
        openPhotoGallery()
        
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
        date = datePickerView.date
    }
    @IBAction func dueDatePickerChanged(_ sender: Any) {
        dueDate = dueDatePicker.date
    }
    
    @IBAction func clickSaveButton(_ sender: UIButton) {
        var repetitionSelectedValue = self.repetitionDropdownView.selectedIndex
        
        var noticeValue = self.memoryDropdownView.selectedIndex
        
        let currentDate = Date()
        
        let params: [String: Any?] = [
            "Title": titleTextField.text,
            "Description": descriptionTextField.text,
            "DateStart": datePickerView.date.toString(output: "yyyy-MM-dd"),
            "DateEnd": currentDate.toString(output: "yyyy-MM-dd"),
            "Notification": memoryValue[noticeValue],
            "Cycle": repetitionValue[repetitionSelectedValue],
            
            "ReferenceType":12,"ReferenceId":0,"Precision":0
        ]
        
        NetworkManager().requestDataAsync(type: Int.self, APP_URL.TO_DO_LIST, params, method: .post){response in
            self.parentView?.reloadPage()
            self.ShowBackAlert(message: "Todo wurde erstellt")
            if self.imageFile == nil{
                

                return
            }
            
            guard let idResponse = response.result as? Int else{
           
                return
            }
            
//            self.ShowBackAlert(message: "Todo wurde erstellt")
            
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
    
}
