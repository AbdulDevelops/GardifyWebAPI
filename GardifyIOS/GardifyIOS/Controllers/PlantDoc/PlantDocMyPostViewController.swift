//
//  PlantDocMyPostViewController.swift
//  GardifyIOS
//
//  Created by Netzlab on 20.10.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class PlantDocMyPostViewController: UIViewController {

    @IBOutlet weak var myPlantDocTableView: UITableView!
    
    var plantDocList: [PlantDoc]?
    var imageList: [String: UIImage?] = [:]
    var notReadedList: [Int] = []

    @IBOutlet weak var cardImage: UIImageView!
    @IBOutlet weak var listImage: UIImageView!
    
    var plantDocAnswerDic: [String: (data: PlantDocDetailModel, status: Bool)] = [:]

    var isListMode: Bool = false
    
    override func viewDidLoad() {
        super.viewDidLoad()

        self.applyTheme()
        pageConfiguration()
        configurePadding()
        
        self.showSpinner(onView: self.view)
        DispatchQueue.main.async {
            self.getNotReadPost()
            self.loadMyPost()
            
        }
        // Do any additional setup after loading the view.
    }
    
    func pageConfiguration(){
        self.myPlantDocTableView.backgroundColor = .clear
    }
    
    func loadMyPost() {
        NetworkManager().requestDataAsync(type: [PlantDoc].self, APP_URL.PLANT_DOC_MY_ENTRIES){response in
            if !response.success{
                self.ShowAlert(message: response.result as! String)
                self.removeSpinner()
                return
            }
            self.removeSpinner()
            self.plantDocList = (response.result as! [PlantDoc])
  

            
            
            self.myPlantDocTableView.reloadData()
        }
    }
    
    func getNotReadPost(){
        NetworkManager().requestDataAsync(type: [Int].self, APP_URL.PLANT_DOC_GET_NOT_READ){response in
            if !response.success{
//                self.ShowAlert(message: response.result as! String)
                return
            }
            self.notReadedList = response.result as! [Int]
            self.myPlantDocTableView.reloadData()

        }
    }
    
    func updateViewModeIcon(){
        if isListMode {
            listImage.image = UIImage(named: "doc_list_on")
            cardImage.image = UIImage(named: "doc_card_off")
        }
        else{
            listImage.image = UIImage(named: "doc_list_off")
            cardImage.image = UIImage(named: "doc_card_on")
        }
        
        self.myPlantDocTableView.reloadData()
    }
    
    @IBAction func onCardClicked(_ sender: Any) {
        if !isListMode{
            return
        }
        
        isListMode = false
        updateViewModeIcon()
    }
    
    @IBAction func onListClicked(_ sender: Any) {
        if isListMode{
            return
        }
        
        isListMode = true
        updateViewModeIcon()
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
            self.plantDocAnswerDic["\(questionId)"] = (data: response.result as! PlantDocDetailModel, status: true)
            
            self.myPlantDocTableView.reloadData()
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

extension PlantDocMyPostViewController: UITableViewDelegate, UITableViewDataSource{
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return plantDocList?.count ?? 0

    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        if !isListMode{
            let cell = tableView.dequeueReusableCell(withIdentifier: "plantDocCell", for: indexPath) as! PlantDocViewCell
            cell.tag = indexPath.row
            cell.descriptionDelegate = self
            cell.parentMyPost = self
            cell.isDetailExtended = false
            if plantDocAnswerDic["\(plantDocList![indexPath.row].QuestionId)"] != nil{
                cell.plantDocAnswer = plantDocAnswerDic["\(plantDocList![indexPath.row].QuestionId)"]!.data
                cell.isDetailExtended = (plantDocAnswerDic["\(plantDocList![indexPath.row].QuestionId)"]!.status)
            }
            
            cell.isAlertOn = false

            if notReadedList.contains(plantDocList![indexPath.row].QuestionId){
                cell.isAlertOn = true
            }
            
            cell.onConfigure(data: plantDocList![indexPath.row])
    
            cell.docImage.image = UIImage(named: "PlantPlaceholder")
    
    
    
    
            if self.imageList.count <= indexPath.row{
                return cell
            }
    
            guard let image = self.imageList["\(plantDocList![indexPath.row].QuestionId)"] else {
                return cell
            }
    
            cell.docImage.image = image
            return cell

        }

        
        
        let cell = tableView.dequeueReusableCell(withIdentifier: "plantDocSimpleCell", for: indexPath) as! PlantDocSimpleCell
        cell.tag = indexPath.row
        cell.descriptionDelegate = self
        cell.parentMyPost = self
        cell.isDetailExtended = false
        if plantDocAnswerDic["\(plantDocList![indexPath.row].QuestionId)"] != nil{
            cell.plantDocAnswer = plantDocAnswerDic["\(plantDocList![indexPath.row].QuestionId)"]!.data
            cell.isDetailExtended = (plantDocAnswerDic["\(plantDocList![indexPath.row].QuestionId)"]!.status)
        }
        
        cell.isAlertOn = false
        
        if notReadedList.contains(plantDocList![indexPath.row].QuestionId){
            cell.isAlertOn = true
        }
        
        cell.onConfigure(data: plantDocList![indexPath.row])

//        cell.docImage.image = UIImage(named: "PlantPlaceholder")
        
        
        
        return cell
    }
    
    
    
}

extension PlantDocMyPostViewController: PlantDocDetailsDelegate{
//    func goToPlantDocDetails(id: Int, _ cell: PlantDocViewCell?) {
//        if self.plantDocAnswerDic["\(id)"] == nil{
//            self.getPlantDocAnswer(questionId: id)
//            return
//        }
//        
//        if self.plantDocAnswerDic["\(id)"]?.status == true{
//            self.plantDocAnswerDic["\(id)"]?.status = false
//            self.myPlantDocTableView.reloadData()
//            return
//        }
//        
//        self.getPlantDocAnswer(questionId: id)
//    }
    
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
            
   
            if let cell = sender  as? PlantDocViewCell{
                docDetailView.plantDocData = cell.plantDocData

            }
            
            if let cell = sender as? PlantDocSimpleCell{
                docDetailView.plantDocData = cell.plantDocData
            }
            
            
            
            
//            docDetailView.questionId = (sender as! Int)
        }
    }
    

    
    
}
