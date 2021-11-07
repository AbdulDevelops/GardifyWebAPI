//
//  PlantSuggestViewController.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 03.12.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit
import Alamofire

class PlantSuggestViewController: UIViewController, UIImagePickerControllerDelegate & UINavigationControllerDelegate {
    
    @IBOutlet weak var cameraScanButton: UIButton!
    @IBOutlet weak var scrollView: UIScrollView!
    
    @IBOutlet weak var albumScanButton: UIButton!
    
    @IBOutlet weak var sendButton: UIButton!
    
    @IBOutlet weak var uploadedImagesCollection: UICollectionView!
    
    @IBOutlet weak var imageFileName: UITextField!
    
    @IBOutlet weak var heightForCollection: NSLayoutConstraint!
    
    var userData : UserInfo?
    var imagePickerController : UIImagePickerController!
    //var imageFile : UIImage?
    var imageFiles : [UIImage] = []
    
    override func viewDidLoad() {
        super.viewDidLoad()

        configurePadding()
        configureStyle()
        
        scrollView.isExclusiveTouch = false
        // Do any additional setup after loading the view.
    }
    
    func configureStyle() {
        cameraScanButton.addBorderRadius()
        albumScanButton.addBorderRadius()
        sendButton.addBorderRadius()
    }
    
    override func viewWillAppear(_ animated: Bool) {
        super.viewWillAppear(animated)
        self.updateNavigationBar(isMain: false, "PFLANZEN", "ERGÄNZEN", "main_suggest")
        
    }
    
    
    @IBAction func onClickOutside(_ sender: Any) {
        print("touched")
        view.endEditing(true)
    }
    
    
    @IBAction func onSendClick(_ sender: Any) {
        print("send count ", self.imageFiles.count)
        uploadImages(imageFiles: self.imageFiles, imageFileName: self.imageFileName.text ?? "plantSuggestImage")
    }
    
    
    func uploadImages(imageFiles: [UIImage], imageFileName: String){
        
        
        if imageFiles.count == 0{
            self.ShowAlert(message: "Bitte laden Sie ein Bild hoch")
            
            return
        }
        
        self.showSpinner(onView: self.view, "Bild wird hochgeladen...")
        DispatchQueue.main.async {

            let authHeader = NetworkManager()._getTokenHeaders()
            let manager = Alamofire.SessionManager.default
            let headers: HTTPHeaders = ["Content-type": "multipart/form-data", "Content-Disposition": "form-data", "Authorization": authHeader["Authorization"]!]

            do{
                var count = 0
                manager.upload(multipartFormData: { (formData) in
                    for item in imageFiles {
                    if let fileData = (item.resizedTo2MB()?.pngData()) { // File data
                        formData.append(fileData, withName: "imageFile"+"\(count)", fileName: imageFileName+"\(count)"+".png", mimeType: "image/png")
                        formData.append(Data((imageFileName+"\(count)"+".img").utf8), withName: "imageTitle"+"\(count)")
                    }
                        count+=1
                    }
                    formData.append(Data((imageFileName+".png").utf8), withName: "name")
                    formData.append(Data("false".utf8), withName: "ignoreMatches")

                }, to: APP_URL.UPLOAD_PLANT_SUGGEST_IMAGES, method: HTTPMethod.post, headers: headers){encoding in
                    //print("response is",encoding)
                    switch encoding{
                    case .success(let req, _, _):
                        //self.getUserInfo()
                        print("response is ", encoding )

                        self.clearData()
                        self.removeSpinner()
                        let userName = String(UserDefaultKeys.Username.string() ?? "")
                        
                        self.ShowAlert(message: """
                            Hallo
                            """ + " \(userName), \n"
                                        + """
                            die Ergänzung der von Dir gewünschten Pflanzen mit all ihren To-Dos und Eigenschaften wird auf Grund des hohen Aufkommens einige Zeit in Anspruch nehmen. \n\n
                            Wir bitten um etwas Geduld. Weiterhin viel Spaß mit Gardify.
                            """)
                        //self.fromCamera = false

                    case .failure:
                        self.ShowAlert(message: "es gibt einen Fehler")
                        self.removeSpinner()
                        //self.fromCamera = false

                        break
                    }
                }
            }
            catch {
                self.ShowAlert( message: "es gibt einen Fehler")
                //self.fromCamera = false

                self.removeSpinner()
            }
        }
    }
    
    func clearData(){
        self.imageFileName.text = ""
        self.imageFiles.removeAll()
        self.heightForCollection.constant = 0
        self.uploadedImagesCollection.reloadData()
    }
    
    @IBAction func onPhotoUpload(_ sender: Any) {
        imagePickerController = UIImagePickerController()
        imagePickerController.delegate = self
        imagePickerController.sourceType = .camera
        present(imagePickerController, animated: true, completion: nil)
    }
    
    @IBAction func onImageUpload(_ sender: Any) {
        imagePickerController = UIImagePickerController()
        imagePickerController.delegate = self
        imagePickerController.sourceType = .photoLibrary
        present(imagePickerController, animated: true, completion: nil)
    }
    
    func imagePickerController(_ picker: UIImagePickerController, didFinishPickingMediaWithInfo info: [UIImagePickerController.InfoKey : Any]) {
        
        self.showSpinner(onView: self.view, "Pflanze wird bestimmt..")
        
        imagePickerController.dismiss(animated: true, completion: {
            var imageFile = info[UIImagePickerController.InfoKey(rawValue: UIImagePickerController.InfoKey.originalImage.rawValue)] as? UIImage
            
            print("count ", self.imageFiles.count)
            self.imageFiles.append(imageFile!)
            print("count new ", self.imageFiles.count)
            
            //self.uploadedImage.image = self.imageFile
            //self.uploadedImagesCollection
            self.heightForCollection.constant = 100
            self.uploadedImagesCollection.reloadData()
            self.removeSpinner()
        })
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

extension PlantSuggestViewController: UICollectionViewDelegate, UICollectionViewDataSource{
    func collectionView(_ collectionView: UICollectionView, numberOfItemsInSection section: Int) -> Int {
        print("im count ", self.imageFiles.count)
        return self.imageFiles.count
    }
    
    func collectionView(_ collectionView: UICollectionView, cellForItemAt indexPath: IndexPath) -> UICollectionViewCell {
        print("enter cell")
        let cell = collectionView.dequeueReusableCell(withReuseIdentifier: "plantSuggestImage", for: indexPath) as! PlantSuggestImageCollectionViewCell
        
        cell.plantSuggestImage.image = self.imageFiles[indexPath.row]
        cell.plantSuggestImage.contentMode = .scaleAspectFill
        
        cell.parent = self.uploadedImagesCollection
        cell.controllerData = self
        cell.heightForCollection = self.heightForCollection
        
        
        return cell
    }
    
    
}
