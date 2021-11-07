//
//  PlantDocAskViewController.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 19.10.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit
import Alamofire


class PlantDocAskViewController: UIViewController {

    @IBOutlet weak var cameraButton: UIButton!
    @IBOutlet weak var albumButton: UIButton!
    @IBOutlet weak var imageCollectionView: UICollectionView!
    @IBOutlet weak var submitAskButton: UIButton!
    @IBOutlet weak var imageChecklist: UIImageView!
    
    @IBOutlet weak var answerTextField: UITextView!
    @IBOutlet weak var themeTextField: UITextField!
    
    var imagePickerController : UIImagePickerController!
    var imageFile: [UIImage] = []
    var isLoading: Bool = false
    var isOwnImage: Bool = false
    
    override func viewDidLoad() {
        super.viewDidLoad()

        
        configurePadding()
        pageConfiguration()
        // Do any additional setup after loading the view.
    }
    
    func pageConfiguration(){
        self.applyTheme()
        self.submitAskButton.setGrayButton()
        self.submitAskButton.alpha = 0.5
        self.cameraButton.addBorderRadius()
        self.albumButton.addBorderRadius()
        self.cameraButton.backgroundColor = rgb(65, 119, 111)
        self.albumButton.backgroundColor = rgb(83, 149, 141)
        self.imageCollectionView.backgroundColor = .clear
        self.answerTextField.addBorderWidth()
        self.answerTextField.addBorderRadius()
    }
    
    @IBAction func onImageSelfToggle(_ sender: Any) {
        isOwnImage = !isOwnImage
        updateCheck()
    }
    func updateCheck(){
        if imageFile.count > 0 {
            self.submitAskButton.setGreenButton()
            self.submitAskButton.alpha = 1
        }
        else{
            self.submitAskButton.setGrayButton()
            self.submitAskButton.alpha = 0.5

        }
        
        if isOwnImage{
            imageChecklist.image = UIImage(systemName: "checkmark.square")
        }else{
            imageChecklist.image = UIImage(systemName: "square")

        }
    }
    

    @IBAction func onCameraClick(_ sender: Any) {
        if isLoading {
            return
        }
        imagePickerController = UIImagePickerController()
        imagePickerController.delegate = self
        imagePickerController.sourceType = .camera
        present(imagePickerController, animated: true, completion: nil)
    }
    @IBAction func onAlbumClick(_ sender: Any) {
        print("album clicked")
        if isLoading {
            return
        }
        imagePickerController = UIImagePickerController()
        imagePickerController.delegate = self
        imagePickerController.sourceType = .photoLibrary
        present(imagePickerController, animated: true, completion: nil)
    }

    @IBAction func onSubmitDoc(_ sender: Any) {
        
        let params: [String: Any?] = [
            "Description" : nil,
            "Isownfoto" : isOwnImage,
            "QuestionText" : answerTextField.text,
            "Thema" : themeTextField.text
        ]
        
        NetworkManager().requestDataAsync(type: Int.self, APP_URL.PLANT_DOC_ASK_NEW_ENTRIES, params, method: .post, printRequest: true){response in
            if !response.success{
                self.ShowAlert(message: response.result as! String)
                return
            }
            self.uploadImage(entryId: response.result as! Int)
        }
    }
    
    func uploadImage(entryId: Int){
        if imageFile.count < 1{
            return
        }
        self.showSpinner(onView: view.self)
        DispatchQueue.main.async {
            let authHeader = NetworkManager()._getTokenHeaders()
            let configuration = URLSessionConfiguration.default
            
            configuration.urlCredentialStorage = nil
            let manager = Alamofire.SessionManager(configuration: configuration)
            let headers: HTTPHeaders = ["Content-type": "multipart/form-data",  "Authorization": authHeader["Authorization"]!]
            
            do {
                manager.upload(multipartFormData: {(formData) in
                    var cnt = 0
                    for image in self.imageFile{
                        if let fileData = (image.resizedTo2MB()!.pngData()) {
                            let fileName = "doc_" + (Date().toString(output: "yyyyMMdd_hhmmss")) + ".png"
                            formData.append(fileData, withName: "imageFile\(cnt)", fileName: fileName, mimeType: "image/png")
                            formData.append(Data(fileName.utf8), withName: "imageTitle\(cnt)")
                           
                        }
                        cnt += 1
                    }
                    formData.append(Data("\(entryId)".utf8), withName: "id")
                    print("form is", formData)
                }, to: APP_URL.PLANT_DOC_UPLOAD_IMAGE, method: .post, headers: headers){encoding in
                    print(encoding)
                    switch encoding{
                    case .success(let req, _, _):
                        self.removeSpinner()
                        self.ShowBackAlert( message: "Vielen Dank für deine Frage. In Kürze erhälst du eine Antwort. Wenn do die Veröffentlichung erlaubt hast, wird deine Frage hier veröffentlicht")
                    case .failure:
                        self.ShowAlert( message: "es gibt einen Fehler")
                        self.removeSpinner()
                        break
                    }
                }
                
            }
            catch{
                
                self.ShowAlert( message: "es gibt einen Fehler")
                
                self.removeSpinner()
            }
        }
    }
    
}

extension PlantDocAskViewController: UINavigationControllerDelegate, UIImagePickerControllerDelegate {
    
    func imagePickerController(_ picker: UIImagePickerController, didFinishPickingMediaWithInfo info: [UIImagePickerController.InfoKey : Any]) {
        self.isLoading = true
        self.showSpinner(onView: self.view, "Pflanze wird bestimmt..")
        
        imagePickerController.dismiss(animated: true, completion: {
            
            guard let uploadedImage = info[UIImagePickerController.InfoKey(rawValue: UIImagePickerController.InfoKey.originalImage.rawValue)] as? UIImage else{
                self.removeSpinner()
                self.isLoading = false
                return
            }
            self.imageFile.append(uploadedImage)
            self.imageCollectionView.reloadData()
            self.removeSpinner()
            self.updateCheck()
            self.isLoading = false


        })
        //        imagePath = (info[UIImagePickerController.InfoKey(rawValue: UIImagePickerController.InfoKey.imageURL.rawValue)] as! NSURL).absoluteString
        
        
        
    }
}

extension PlantDocAskViewController: UICollectionViewDelegate, UICollectionViewDataSource{
    func collectionView(_ collectionView: UICollectionView, numberOfItemsInSection section: Int) -> Int {
        
        return self.imageFile.count
    }
    
    func collectionView(_ collectionView: UICollectionView, cellForItemAt indexPath: IndexPath) -> UICollectionViewCell {
        let cell = collectionView.dequeueReusableCell(withReuseIdentifier: "docImageCell", for: indexPath) as! PlantDocAskImageCollectionViewCell
        print("cell for index of", indexPath)
        cell.backgroundColor = .clear
        cell.contentView.backgroundColor = .clear
        cell.docImage.image = self.imageFile[indexPath.row]
        return cell
        
    }
    
    func collectionView(_ collectionView: UICollectionView, didSelectItemAt indexPath: IndexPath) {
        self.imageFile.remove(at: indexPath.row)
        self.imageCollectionView.reloadData()
        updateCheck()
    }
    
    
}
