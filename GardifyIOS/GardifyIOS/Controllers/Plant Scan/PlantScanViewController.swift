//
//  PlantScanViewController.swift
//  GardifyText
//
//  Created by Netzlab on 27.07.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit
import Alamofire

class PlantScanViewController: UIViewController, UINavigationControllerDelegate, UIImagePickerControllerDelegate {
    
    
//    @IBOutlet weak var scanImageView: UIImageView!
    
    @IBOutlet weak var albumScanButton: UIButton!
    @IBOutlet weak var cameraScanButton: UIButton!
    @IBOutlet weak var outerView: UIView!
    
    @IBOutlet weak var lastScanView: UIView!
    @IBOutlet weak var infoView: UIView!
    
    @IBOutlet weak var lastImageCollectionView: UICollectionView!
    @IBOutlet weak var lastImageToggle: UIButton!
    @IBOutlet weak var lastImageHeight: NSLayoutConstraint!
    
    @IBOutlet weak var infoViewHeight: NSLayoutConstraint!
    @IBOutlet weak var scrollView: UIScrollView!
    @IBOutlet weak var infoTextView: UIView!
    
    
    var lastImageArray: [UIImageView] = []
    var currentImageStorage: [UIImage?] = []
    var imagePickerController : UIImagePickerController!
    var imagePath : String?
    var imageFile : UIImage?
    var scanResult : ScanResult?
    var imageCount: Int = 0
    
    var selectedLastImageIndex: Int = -1
    
    var isLastImageExtended: Bool = false
    var isInfoExtended = false
    
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
//        scanImageView.image = UIImage(named: "PlantPlaceholder")

        
        styleConfiguration()
        configurePadding()
        imageConfiguration()
//        updateDropdown()

        // Do any additional setup after loading the view.
    }
    
    override func viewWillAppear(_ animated: Bool) {
        super.viewWillAppear(animated)
        self.updateNavigationBar(isMain: false, "PFLANZEN", "SCAN", "main_plantScan")

        imageCellConfiguration()
    }
    
    func imageConfiguration(){
//        imageCellConfiguration()
        refreshImage()
        
    }
    
    func updateDropdown(){
        if isLastImageExtended{
            lastImageHeight.constant = 60 + self.lastImageCollectionView.frame.width
            self.lastImageCollectionView.scrollToItem(at: IndexPath(row: 0, section: 0), at: .top, animated: false)
//            lastImageCollectionView.reloadData()
        }
        else{
            lastImageHeight.constant = 40
        }
        
        var infoAlpha = 0
        
        if isInfoExtended{
            self.infoViewHeight.constant = 50 + self.infoTextView.frame.height
            infoAlpha = 1
        }else{
            self.infoViewHeight.constant = 40
        }
        
        
        if infoAlpha == 0{
            UIView.animate(withDuration: 0.25, animations: {
                self.infoTextView.alpha = CGFloat(infoAlpha)
                
            })
        }
        UIView.animate(withDuration: 0.5, animations: {
            self.scrollView.layoutIfNeeded()
            
        },completion: {_ in
            if infoAlpha == 1{
                UIView.animate(withDuration: 0.25, animations: {
                    self.infoTextView.alpha = CGFloat(infoAlpha)
                    
                })
            }
            
        })
    }
    
    func imageCellConfiguration(){

        lastImageCollectionView.reloadData()
//        let width = (self.lastImageCollectionView.frame.width - 20) / 3
//
//        let layout = self.lastImageCollectionView.collectionViewLayout as! UICollectionViewFlowLayout
//
//        layout.itemSize = CGSize(width: width, height: width)
        //        self.homeCollectionView.cellsi
    }
    
    @IBAction func onLastImageToggle(_ sender: Any) {
        isLastImageExtended = !isLastImageExtended
        
        updateDropdown()
    }
    
    @IBAction func onInfoToggle(_ sender: Any) {
            isInfoExtended = !isInfoExtended
        updateDropdown()

    }
    
    func refreshImage(){
        var currentImage = getScanImage()
        currentImageStorage = currentImage
//        var cnt = 0
//        for image in currentImage.reversed()
//        {
//            lastImageArray[cnt].image = image
//            cnt += 1
//        }
        imageCount = currentImage.count
        lastImageCollectionView.reloadData()
    }
    
    
    
    
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
        

    }
    
    func styleConfiguration(){
        
        self.outerView.addBorderRadius()
//        self.cameraScanButton.setGreenButton()
//        self.albumScanButton.setGreenButton()
        self.cameraScanButton.addBorderRadius()
        self.albumScanButton.addBorderRadius()
        self.cameraScanButton.backgroundColor = rgb(65, 119, 111)
        self.albumScanButton.backgroundColor = rgb(83, 149, 141)
        self.lastScanView.setWhiteButtonView()
        self.infoView.setWhiteButtonView()
//        self.scanButton.setGreenButton()
        self.applyTheme()
    }
    
    func scanUploadedImage(){
        if imageFile == nil{
            self.ShowAlert(message: "Pflanze wird mit PlantNet bestimmt ...")
            self.removeSpinner()
            
            return
        }
        
        
        DispatchQueue.main.async {
            
            let manager = Alamofire.SessionManager.default
            let headers: HTTPHeaders = ["Content-type": "multipart/form-data", "Content-Disposition": "form-data"]
            
            do{
                manager.upload(multipartFormData: { (formData) in
                    if let fileData = (self.imageFile?.resizedTo2MB()!.pngData()) { // File data
                        formData.append(fileData, withName: "img", fileName: "file.png", mimeType: "image/png")
                    }
                }, to: APP_URL.PLANT_SCAN_API, method: HTTPMethod.post, headers: headers){encoding in
                    print("response is",encoding)
                    switch encoding{
                    case .success(let req, _, _):
                        req.responseJSON { (resObj) in
                            switch resObj.result{
                            case .success:
                                if let resData = resObj.data{
                                    do {
                                        //                                        let res = try JSONSerialization.jsonObject(with: resData)
                                        let scanData = try JSONDecoder().decode(ScanResult.self, from: resData)
                                        self.scanResult = scanData
                                        
                                        self.performSegue(withIdentifier: "scanResult", sender: nil)
                                        self.removeSpinner()
                                        
                                        
                                    } catch{
                                        self.ShowAlert(message: "Es tut uns Leid, es gibt ein Problem mit PlantNet. Versuche es bitte später noch einmal")
                                        self.removeSpinner()
                                        
                                    }
                                }
                                break
                            case .failure (let err):
                                self.ShowAlert(message: "Es tut uns Leid, es gibt ein Problem mit PlantNet. Versuche es bitte später noch einmal")
                                self.removeSpinner()
                                
                                break
                            }
                        }
                        break
                    case .failure:
                        self.ShowAlert(message: "Es tut uns Leid, es gibt ein Problem mit PlantNet. Versuche es bitte später noch einmal")
                        self.removeSpinner()
                        
                        break
                    }
                }
            }catch{
                self.ShowAlert(message: "Es tut uns Leid, es gibt ein Problem mit PlantNet. Versuche es bitte später noch einmal")
                self.removeSpinner()
            }
            
        }
    }
    
    @IBAction func onImageScan(_ sender: Any) {
        
        
        scanUploadedImage()
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
        
        self.showSpinner(onView: self.view, "Pflanze wird mit PlantNet bestimmt..")

        imagePickerController.dismiss(animated: true, completion: {
            self.imageFile = info[UIImagePickerController.InfoKey(rawValue: UIImagePickerController.InfoKey.originalImage.rawValue)] as? UIImage
            
            
            self.scanUploadedImage()
            storeScanImage(image: self.imageFile!)

            self.refreshImage()
        })
//        imagePath = (info[UIImagePickerController.InfoKey(rawValue: UIImagePickerController.InfoKey.imageURL.rawValue)] as! NSURL).absoluteString

       
        
    }
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        if segue.identifier == "scanResult"{
            let scanDetailView = segue.destination as! PlantScanResultViewController
            scanDetailView.scanResultData = self.scanResult
        }
    }
    
    func setImageFromHistory(pos: Int){
        if pos > imageCount{
            return
        }
        
        var currentImage = getScanImage()
        
        imageFile = currentImage.reversed()[pos-1]
        scanUploadedImage()

        
    }
    

    
}


extension PlantScanViewController: UICollectionViewDelegate, UICollectionViewDataSource, UICollectionViewDelegateFlowLayout{
    
    func collectionView(_ collectionView: UICollectionView, layout collectionViewLayout: UICollectionViewLayout, sizeForItemAt indexPath: IndexPath) -> CGSize {
        let width = (self.lastImageCollectionView.frame.width - 20) / 3

        return CGSize(width: width, height: width)
    }
    
    func collectionView(_ collectionView: UICollectionView, numberOfItemsInSection section: Int) -> Int {
        return imageCount
    }
    
    func collectionView(_ collectionView: UICollectionView, cellForItemAt indexPath: IndexPath) -> UICollectionViewCell {
        let cell = collectionView.dequeueReusableCell(withReuseIdentifier: "lastImageCell", for: indexPath) as! PlantScanLastImageCollectionViewCell

       
        cell.lastScanImage.image = currentImageStorage.reversed()[indexPath.row]
        
        if selectedLastImageIndex != -1{
            print("something is selected", selectedLastImageIndex, indexPath.row)
            if selectedLastImageIndex != indexPath.row{
                cell.lastScanImage.alpha = 0.5
            }
            else{
                cell.lastScanImage.alpha = 1
            }
        }else{
            cell.lastScanImage.alpha = 1
        }
        return cell
    }
    
    func collectionView(_ collectionView: UICollectionView, didSelectItemAt indexPath: IndexPath) {
//        self.imageFile = currentImageStorage.reversed()[indexPath.row]
//
//        self.showSpinner(onView: self.view, "Pflanze wird bestimmt..")
//        self.scanUploadedImage()
        print("selected index is", indexPath.row, selectedLastImageIndex)
        if selectedLastImageIndex != indexPath.row{
            selectedLastImageIndex = indexPath.row
            self.lastImageCollectionView.reloadData()
            return
        }
        
        selectedLastImageIndex = -1
        self.lastImageCollectionView.reloadData()
    }
}


