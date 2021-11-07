//
//  PlantDocViewController.swift
//  GardifyText
//
//  Created by Netzlab on 27.07.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit
import Alamofire

protocol PlantDocDetailsDelegate {
    func goToPlantDocDetails(id: Int,_ cell: Any?)
}

class PlantDocViewController: UIViewController{
    
    @IBOutlet weak var docTableView: UITableView!
    @IBOutlet weak var askQuestionButton: UIButton!
    @IBOutlet weak var myPostButton: UIButton!
    @IBOutlet weak var searchView: UIView!
    @IBOutlet weak var searchTextField: UITextField!
    @IBOutlet weak var searchTextHeight: NSLayoutConstraint!
    
    @IBOutlet weak var searchTextHairline: UIView!

    var plantDocList: [PlantDoc]?
    
    var storedPlantDocList: [PlantDoc]?
    var imageList: [String: UIImage?] = [:]
    var isSearchExtended: Bool = false
    
    var plantDocAnswerDic: [String: (data: PlantDocDetailModel, status: Bool)] = [:]
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        styleConfiguration()
        configurePadding()
        self.showSpinner(onView: self.view)
        DispatchQueue.global(qos: .background).async {
            self.getAllPlantDoc()

        }
        // Do any additional setup after loading the view.
    }
    
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
        
        self.updateNavigationBar(isMain: false, "PFLANZEN", "DOC", "main_plantDoc")

    }
    
    func styleConfiguration(){
        
        self.applyTheme()
        self.askQuestionButton.setGreenButton()
        self.myPostButton.setGrayButton()
        self.docTableView.backgroundColor = .clear
        self.searchView.setWhiteButtonView()
        self.searchTextField.setWhiteNoShadowView()
        
    }
    
    func getAllPlantDoc(){
        
        NetworkManager().requestDataAsync(type: [PlantDoc].self, APP_URL.PLANT_DOC_ALL_ENTRIES){response in
            if !response.success{
                self.ShowAlert(message: response.result as! String)
                self.removeSpinner()
                return
            }
            self.storedPlantDocList = (response.result as! [PlantDoc])
            self.plantDocList = self.storedPlantDocList
            DispatchQueue.main.async {
                self.getAllImage()

            }

            
            
//            self.docTableView.reloadData()
        }
    }
    
    
    func filterResult(searchText: String){
        self.plantDocList = self.storedPlantDocList?.filter({($0.Thema!.lowercased().contains(searchText)) })
    }
    
    func showAllResult(){
        self.plantDocList = self.storedPlantDocList
    }
    
    func getAllImage(){
        
        let imageUrlList = self.plantDocList!.reduce(into: [(String,String)]()){output, input in
            var raw = input.Images![0].SrcAttr
            output.append(((raw!.toURL(false, false, false)),"\(input.QuestionId)"))
        }
        

        
        var cnt = 0
        DispatchQueue.global(qos: .background).async {

            for url in imageUrlList{
                let currentCnt = cnt
                getImageFiles(files: [url.0]){image in
                    if url.1 == imageUrlList.first?.1{
                        self.removeSpinner()

                    }

                    DispatchQueue.main.async {
                        self.imageList[url.1] = image[0]
                        self.docTableView.reloadData()
                    }
                    
                }
                cnt += 1
            }
        }
        
    }
    
    func updateSearchDropdown(){
        if isSearchExtended{
            searchTextHeight.constant = 80
            searchTextField.alpha = 1
            searchTextHairline.alpha = 1
        }
        else{
            searchTextHeight.constant = 40
            searchTextField.alpha = 0
            searchTextHairline.alpha = 0
        }
    }
    
    @IBAction func onSearchToggle(_ sender: Any) {
        isSearchExtended = !isSearchExtended
        
        updateSearchDropdown()
    }
    
    @IBAction func onSearchChange(_ sender: Any) {
        print("seach is", searchTextField.text)
        if searchTextField.text != nil && searchTextField.text != ""{
            self.filterResult(searchText: searchTextField.text!)
        }
        else{
            self.showAllResult()
        }

        docTableView.reloadData()
    }

    
    func getPlantDocAnswer(questionId: Int){
        self.showSpinner(onView: self.view)
        
        NetworkManager().requestDataAsync(type: PlantDocDetailModel.self, APP_URL.PLANT_DOC_ROUTE + "//\(questionId)/getEntry"){response in
            if !response.success{
                self.ShowAlert(message: response.result as! String)
                self.removeSpinner()
                return
            }
            self.removeSpinner()
            self.plantDocAnswerDic["\(questionId)"] = (data: response.result as! PlantDocDetailModel, status: false)
            
            self.docTableView.reloadData()
        }
    }
    
    @IBAction func onTouchView(_ sender: Any) {
        print("is touched")
        view.endEditing(true)
    }
    
    
}

extension PlantDocViewController: UIScrollViewDelegate{
    
    override func touchesBegan(_ touches: Set<UITouch>, with event: UIEvent?) {
        print("is touched")
        view.endEditing(true)

    }
    
    func scrollViewWillBeginDragging(_ scrollView: UIScrollView) {
        print("is scrolled")
        view.endEditing(true)
    }
}

extension PlantDocViewController: UITableViewDataSource{
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return plantDocList?.count ?? 0
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: "plantDocCell", for: indexPath) as! PlantDocViewCell
        cell.tag = indexPath.row
        cell.descriptionDelegate = self
        cell.parent = self
        cell.isDetailExtended = false
        if plantDocAnswerDic["\(plantDocList![indexPath.row].QuestionId)"] != nil{
            cell.plantDocAnswer = plantDocAnswerDic["\(plantDocList![indexPath.row].QuestionId)"]!.data
            cell.isDetailExtended = (plantDocAnswerDic["\(plantDocList![indexPath.row].QuestionId)"]!.status)
        }
        
        cell.onConfigure(data: plantDocList![indexPath.row])
        
        cell.docImage.image = UIImage(named: "PlantPlaceholder")
        
        
        
        //Checks if Image is loaded. If not, returns placeholder
        if self.imageList.count <= indexPath.row{
            return cell
        }
        
        guard let image = self.imageList["\(plantDocList![indexPath.row].QuestionId)"] else {
            return cell
        }
        
        cell.docImage.image = image
        return cell
    }
    
    
}

extension PlantDocViewController: PlantDocDetailsDelegate{

    
    func goToPlantDocDetails(id: Int,_ cell: Any?) {
//        if self.plantDocAnswerDic["\(id)"] == nil{
//            self.getPlantDocAnswer(questionId: id)
//            performSegue(withIdentifier: "PlantDocDetail", sender: cell)
//            return
//        }
//
//        if self.plantDocAnswerDic["\(id)"]?.status == true{
//            self.plantDocAnswerDic["\(id)"]?.status = false
////            self.docTableView.reloadData()
//            return
//        }
//
//        self.getPlantDocAnswer(questionId: id)
        performSegue(withIdentifier: "PlantDocDetail", sender: cell)
    }
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        if segue.identifier == "PlantDocDetail"{
            let docDetailView = segue.destination as! PlantDocDetailViewController
            
            let cell = sender  as! PlantDocViewCell
            
            docDetailView.plantDocData = cell.plantDocData
            
            
            
//            docDetailView.questionId = (sender as! Int)
        }
    }
    
}
