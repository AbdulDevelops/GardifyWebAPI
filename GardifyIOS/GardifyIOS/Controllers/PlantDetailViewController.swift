//
//  PlantDetailViewController.swift
//  GardifyIOS
//
//  Created by Netzlab on 03.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class PlantDetailViewController: UIViewController {

    
    @IBOutlet weak var plantImageView: UIImageView!
    @IBOutlet weak var nameLatin: UILabel!
    @IBOutlet weak var nameGerman: UILabel!
    @IBOutlet weak var plantGroupLabel: UILabel!
    @IBOutlet weak var saveToGardenButton: UIButton!
    @IBOutlet weak var outerView: UIView!
    @IBOutlet weak var familyLabel: UILabel!
    @IBOutlet weak var originLabel: UILabel!
    @IBOutlet weak var descriptionLabel: UILabel!
    
    @IBOutlet weak var categoryNameLabel: UILabel!
    @IBOutlet weak var categoryValueLabel: UILabel!
    
    @IBOutlet weak var plantDetailsList: FullTableView!
    
    @IBOutlet weak var toDoTableView: UITableView!
    
    @IBOutlet weak var plantSiblingTableView: FullTableView!
    @IBOutlet weak var plantTodoTableView: FullTableView!
    
    @IBOutlet weak var todoLabel: UILabel!
    
    
    
    var categoryDictionary = [String:[String]]()
    
    
    let headingList: [String] = ["Eigenschaften","Blüten","Standort & Pflege","Blätter","Früchte"]
    
    let sectionCategoryList: [String: [String]] = ["Eigenschaften":["Verwendung","Besonderheiten","Wuchs","Nutzpflanzen","Laubrythmus","Winterhärte"],"Blüten":["Blüten","Blütenfarben","Blütenform","Blütengröße","Blütenstand"], "Standort & Pflege":["Licht","Boden","Schnitt","Wasserbedarf","Vermehrung"], "Blätter":["Blattfarbe","Blattform","Blattrand","Blattstellung"], "Früchte":["Früchte"]]

    
    var plantId: Int?
    var plantModel: PlantDetailModel?
    var plantSiblingDetailList : [PlantSiblingDetailModel]?
    var siblingsImageUrlList : [String:UIImage?] = [:]
    var imageListSiblings : [UIImage] = [UIImage()]
    
    override func viewDidLoad() {
        super.viewDidLoad()

        self.showSpinner(onView: self.view)
        self.configurePadding()
        DispatchQueue.main.async {
            self.getPlantDetail()
            self.getPlantSiblings()

        }
        // Do any additional setup after loading the view.
    }
    
    override func viewWillAppear(_ animated: Bool) {
        super.viewWillAppear(false)
        self.updateNavigationBar(isMain: false)
        pageConfiguration()
    }
    
    func pageConfiguration(){
        
        self.applyTheme()
        self.outerView.addBorderRadius()
        self.saveToGardenButton.setGreenButton()
    }
    
    
    func getPlantDetail(){
        NetworkManager().requestDataAsync(type: PlantDetailModel.self, APP_URL.PLANT_SEARCH_BY_ID + "\(plantId!)"){response in
            if !response.success{
                self.ShowAlert(message: response.result as! String)
                self.removeSpinner()
                self.navigationController?.popViewController(animated: true)
                return
            }
            
            self.plantModel = (response.result as! PlantDetailModel)
            self.configurePlantDetails()
            self.removeSpinner()
            self.plantDetailsList.reloadData()
            self.plantTodoTableView.reloadData()
         
//            self.toDoTableView.reloadData()
        }
    }
    
    func getPlantSiblings(){
        NetworkManager().requestDataAsync(type: [PlantSiblingDetailModel].self, APP_URL.PLANT_SIBLING_BY_ID + "\(plantId!)"){response in
            if !response.success{
                self.ShowAlert(message: response.result as! String)
                self.removeSpinner()
                self.navigationController?.popViewController(animated: true)
                return
            }
            self.plantSiblingDetailList = (response.result as! [PlantSiblingDetailModel])
            //print(self.plantSiblingDetailList?.count)
            
            
            self.siblingsImageUrlList = (self.plantSiblingDetailList?.reduce(into: [String: UIImage?]()){output, input in
                var raw = input.Images![0].SrcAttr
                var outputUrl = raw!.toURL(false, false)
                print("url ", raw)
                output.updateValue(nil, forKey:outputUrl)
                //self.siblingsImageUrlList[raw!] = nil
                //print("siblings url count ", self.siblingsImageUrlList.count)
            })!
            
            
            
            var cnt = 0
            DispatchQueue.main.async {
                
                for url in self.siblingsImageUrlList{
                    let currentCnt = cnt
                    getImageFiles(files: [url.0]){image in
                        self.siblingsImageUrlList[url.0] = image[0]
                        self.plantSiblingTableView.reloadData()
                        print("image file ",self.siblingsImageUrlList)
                    }
                    cnt += 1
                }
            }
            self.plantSiblingTableView.separatorStyle = UITableViewCell.SeparatorStyle.none
            self.plantSiblingTableView.reloadData()
    }
    }
    
    func configurePlantDetails(){
        
        var imageUrl = self.plantModel?.Images![0].SrcAttr
        NetworkManager().getImageFromUrl(urlString: imageUrl!.toURL(false, false)){image in
            self.plantImageView.image = image
        }
        
        self.updateNavigationBar(isMain: false, self.plantModel!.NameGerman.uppercased())

        
        self.nameLatin.text = self.plantModel?.NameLatin.removeItalicTag()
        self.nameGerman.text = self.plantModel?.NameGerman
        self.familyLabel.text = self.plantModel?.Family?.removeItalicTag()
        self.originLabel.text = self.plantModel?.Herkunft
        self.descriptionLabel.text = self.plantModel?.Description.removeItalicTag()
        
        
        
        let arr = self.plantModel?.Badges
        print(self.plantModel?.Badges?.count)
        
        let categoryTagName: [(String, Int)] = [
            ("Besonderheiten" , 108),
            ("Ausschlusskriterien" , 128),
            ("Dekoaspekte" , 121),
            ("Verwendung" , 97),
            ("Vermehrung" , 95),
            ("Düngung" , 94),
            ("Schnitt" , 92),
            ("Wasserbedarf" , 91),
            ("Winter Hardness" , 89),
            ("Boden" , 87),
            ("Licht" , 86),
            ("Wuchs" , 74),
            ("Nutzpflanzen" , 137),
            ("Fruchtfarbe" , 71),
            ("Früchte" , 68),
            ("Blütengröße" , 67),
            ("Blütenstand" , 66),
            ("Blütenform" , 65),
            ("Blütenfarben" , 64),
            ("Laubrythmus" , 53),
            ("Blattfarbe" , 55),
            ("Herbstfärbung" , 56),
            ("Blattform" , 57),
            ("Blattrand" , 58),
            ("Blattstellung" , 59),
            ("Blüten" , 60),
            ("Winterhärte" , 89)
        ]
        
        print(self.plantModel)
        
        for tag in categoryTagName{
            categoryDictionary[tag.0] = self.plantModel?.PlantTags?.filter{$0.CategoryId == tag.1}.reduce(into: [String]()){output, input in
                output.append(input.Title!)
            } as [String]
        }
        

        
        print(categoryDictionary.count)
        print(categoryDictionary["particularity"]?.joined(separator: ", "))
        
        
        
        let plantGroupList = self.plantModel?.PlantGroups?.map{$0.Name}
        self.plantGroupLabel.text = plantGroupList?.joined(separator: ", ")
    }
    
    @IBAction func saveToGarden(_ sender: Any) {

        
        if !self.isLoggedIn(){
            return
        }
        
//        self.showSpinner(onView: self.view)
        
        DispatchQueue.main.async { [self] in
            self.goToAddGarden(plantId: plantId!, plantModel: self.plantModel!)
        }
        
//
//        NetworkManager().requestDataAsync(type: [UserPlantModel].self, APP_URL.USER_PLANT_BY_ID + "\(plantId!)"){response in
//
//
//
//            let storyBoard = UIStoryboard(name: "PlantGarden", bundle: nil)
//            let controller = storyBoard.instantiateViewController(withIdentifier: "PlantGardenAddView") as! PlantGardenAddViewController
//
//            controller.userPlantList = (response.result as! [UserPlantModel])
//            controller.plantDetail = self.plantModel
////            let gardenRequest = NetworkManager().requestData(type: [MyGardenLightModel].self, APP_URL.USER_GARDEN_DETAILS)
////
////            if gardenRequest.success{
////                controller.gardenDetail = (gardenRequest.result as! [MyGardenLightModel])[0]
////
////            }
//
//            self.navigationController?.pushViewController(controller, animated: true)
//
//        }
        
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

extension PlantDetailViewController: UITableViewDelegate, UITableViewDataSource{
    
    
    func numberOfSections(in tableView: UITableView) -> Int {
        if tableView == plantDetailsList{
            return sectionCategoryList.keys.count

        }
        else if tableView == plantTodoTableView{
            return 1
        }
        else if tableView == plantSiblingTableView{
            return 1
        }
        return 1
    }
    
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        if tableView == plantDetailsList{
        return self.sectionCategoryList[self.headingList[section]]?.count ?? 0

        }
        else if tableView == plantTodoTableView{
            return self.plantModel?.TodoTemplates?.count ?? 0
        }
        else if tableView == plantSiblingTableView{
            print("sibling count  ", self.plantSiblingDetailList?.count)
            return self.plantSiblingDetailList?.count ?? 0
        }
        return 0
   
        
    }
    
    func tableView(_ tableView: UITableView, titleForHeaderInSection section: Int) -> String? {
        
        if tableView == plantDetailsList{
            return self.headingList[section]

        }
        else if tableView == plantTodoTableView{
            return ""
        }
        else if tableView == plantSiblingTableView{
            
            return ""
        }
        return ""
  
    }

    
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        
        if tableView == plantDetailsList{
            let cell = tableView.dequeueReusableCell(withIdentifier: "plantdetailscell", for: indexPath) as! PlantDetailsTableViewCell
            
            //cell.backgroundColor = .Background
            //cell.contentView.backgroundColor = .Background
            
            //Array(myDict)[index].key
            let heading = self.headingList[indexPath.section]
            //cell.categoryHeadingLabel.text = heading
            //print(self.sectionCategoryList[heading])
            //print(indexPath.count)
            let key = (self.sectionCategoryList[heading])?[indexPath.row]
            cell.categoryNameLabel.text = key
            cell.categoryValueLabel.text = Array(self.categoryDictionary[key!] ?? [""]).joined(separator: ", ")
            //cell.categoryValueLabel.text = (Array(self.categoryDictionary)[indexPath.row].value).joined(separator: ", ")
            
            return cell
        }
        else if tableView == plantTodoTableView{
            let cell = tableView.dequeueReusableCell(withIdentifier: "plantTodoCell", for: indexPath) as! ToDoPlantTableViewCell
            //cell.backgroundColor = .Background
            cell.contentView.backgroundColor = .Background
            
            
            cell.todoTitleLabel.text = self.plantModel?.TodoTemplates?[indexPath.row].Title
            cell.todoDescriptionLabel.text = self.plantModel?.TodoTemplates?[indexPath.row].Description
            
            return cell
        }
        else if tableView == plantSiblingTableView{
//            let cell = UITableViewCell()
//
//            cell.textLabel?.text = "\(indexPath.row)"
//
//
            let cell = tableView.dequeueReusableCell(withIdentifier: "plantSiblingsCell", for: indexPath) as! PlantSiblingsTableViewCell

            cell.contentView.backgroundColor = .Background
            cell.viewController = self
            cell.plantId = self.plantSiblingDetailList?[indexPath.row].Id ?? 0
            
            //print("cur url ", "\(String(describing: self.plantSiblingDetailList?[indexPath.row].Images?[0].SrcAttr))")
            
            var urlKey = self.plantSiblingDetailList?[indexPath.row].Images?[0].SrcAttr
            
            
            cell.image1.image = UIImage(named: "PlantPlaceholder")
            
            let imageKey = urlKey?.toURL(false,false)
            
            let image = self.siblingsImageUrlList["\(imageKey!)"] as? UIImage
            
            print("cell image is", image)
            if(image == nil) {
                return cell
            }
            cell.image1.image = image
            
//            guard let imageData = self.plantSiblingDetailList?[(indexPath.row)].Images?[0] else {
//                return cell
//            }
            
            //let image = self.siblingsImageUrlList[indexPath.row]

            //print("file  ", imageData.SrcAttr)


            return cell
        }
        
        else{
            return UITableViewCell()
        }
        
        

        
        
    }
}
