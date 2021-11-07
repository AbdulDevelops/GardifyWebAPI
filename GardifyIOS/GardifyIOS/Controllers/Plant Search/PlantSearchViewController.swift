//
//  PlantSearchViewController.swift
//  GardifyIOS
//
//  Created by Netzlab on 04.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit
import Alamofire

protocol plantSearchDelegate {
    func searchPlantWithParam(params: Parameters)
    
    func updateSearchParam(params: Parameters)
    
    func updateNameSearch(name: String)
}

class PlantSearchViewController: UIViewController, UIScrollViewDelegate {
    
    @IBOutlet weak var plantSearchTableView: UITableView!
    
    @IBOutlet weak var searchTagCollectionView: FullCollectionView!
    
    @IBOutlet weak var mainScrollView: UIScrollView!
    @IBOutlet weak var plantGroupSelectButton: UIButton!
    @IBOutlet weak var plantFamilySelectButton: UIButton!
    
    @IBOutlet weak var plantGroupDropdownTable: UITableView!
    @IBOutlet weak var plantGroupDropdownHeight: NSLayoutConstraint!
    @IBOutlet weak var plantFamilyDropdownTable: UITableView!
    @IBOutlet weak var plantFamilyDropdownHeight: NSLayoutConstraint!
    
    @IBOutlet weak var searchResultView: UIView!
    
    @IBOutlet weak var nameFilterView: UIView!
    @IBOutlet weak var ecoFilterView: UIView!
    @IBOutlet weak var freezingFilterView: UIView!
    @IBOutlet weak var detailFilterView: UIView!
    
    @IBOutlet weak var ecoFilterButton: UIButton!
    @IBOutlet weak var ecoFilterDropdownHeight: NSLayoutConstraint!
    @IBOutlet weak var ecoFilterMainTableView: UITableView!
    
    @IBOutlet weak var detailFilter1TableView: UITableView!
    @IBOutlet weak var detailFilter2TableView: FullTableView!
    @IBOutlet weak var detailFilterDropdownHeight: NSLayoutConstraint!
    
    @IBOutlet weak var freezingFilterButton: UIButton!
    @IBOutlet weak var detailFilterButton: UIButton!
    @IBOutlet weak var plantSearchButton: UIButton!
    @IBOutlet weak var plantSearchButtonEco: UIButton!
    @IBOutlet weak var plantNameInfoButton: UIButton!
    
    @IBOutlet weak var searchView: UIView!
    @IBOutlet weak var searchNumberLabel: UILabel!
    @IBOutlet weak var searchNumberLabelEco: UILabel!
    
    @IBOutlet weak var nameSearchButton: UIButton!
    
    @IBOutlet weak var pageNumberLabel: UILabel!
    
    @IBOutlet weak var searchBarView: UIView!
    
    var plantSearchResult: PlantSearch?
    var plantSearchCount: Int = 0
    
    var maxPage: Int = 1
    var currentPage: Int = 1
    
    var plantSearchList: [PlantDetailModel] = []
    
    var plantImageList: [UIImage] = []
    
    var tagCollectionData: [String: (t: String, id: Int, type: FilterType)] = [:]
    var collectionData: [String] = ["plant tag1", "plant tag long 2", "plant tag 3", "plant tag 4"]
    
    var isEcoDropdownExtend = false
    var isGroupDropdownExtend = false
    var isFamilyDropdownExtend = false
    
    var searchParams: Parameters = ["skip": 0,
                                    "take": 20,
                                    "selHmin": 0,
                                    "selHmax": 800,
                                    "selMinMonth": 1,
                                    "selMaxMonth": 12,
                                    "freezes": ""
    ]
    
    // filter params
    
    
    @IBOutlet weak var nameSearchField: MinimalTextField!
    
    var searchDelegate: plantSearchDelegate?
    
    let ecoCategoryContainer = CheckBoxFilter()
    let filterContainer = FilterTableView()
    // filter Parameters
    
    var paramEcoTags: [Int] = []
    var cookieTags: [Int] = []
    var colorsTags: [Int] = []
    var excludeTags: [Int] = []
    var leafColorsTags: [Int] = []
    var autumnColorsTags: [Int] = []
    var plantGroups: [Int] = []
    var plantFamilies: [String] = []
    
    var monthTags: [Int] = []
    var heightTags: [Int] = []
    var frostTag: Int = 0
    
    
    var isDropdownFilterToggle = false
    
    // plant display params
    
    var ecoDropdownTableView = EcoDropdownObject()
    var detailFilterDropdown1 = DetailDropdown1()
    var detailFilterDropdown2 = DetailDropdown2()
    
    var groupDropdownObject = GroupFamilyDropdown()
    var familyDropdownObject = GroupFamilyDropdown()
    
    // raw params
    var ecoTags: [(t: String, id: Int)] = []
    
    var tags: [String: [(t: String, id: Int)]] = [:]
    
    var isSearching = false
    
    var isQueueSearch = false
    
    var isUpdatePage = false
    
    var categoryTags: [String: Int] = [
        "Ausschlusskriterien": 0,
        "Besonderheiten": 0,
        "Blattrand": 0,
        "Blattfarbe": 0,
        "Blattform": 0,
        "Blattstellung": 0,
        "Blütenfarben": 0,
        "Blüten": 0,
        "Blütenform": 0,
        "Blütengröße": 0,
        "Blütenstand": 0,
        "Boden": 0,
        "Früchte": 0,
        "Fruchtfarbe": 0,
        "Dekoaspekte": 0,
        "Herbstfärbung": 0,
        "Licht": 0,
        "Laubrythmus": 0,
        "Düngung": 0,
        "Wasserbedarf": 0,
        "Vermehrung": 0,
        "Winterhärte": 0,
        "Schnitt": 0,
        "Verwendung": 0,
        "Wuchs": 0,
        "Nutzpflanzen": 0
    ]
    
    var reverseCategoryTags: [Int: String] = [:]
    
    
    override func viewDidLoad() {
        super.viewDidLoad()
        self.configurePadding()
        
        dropdownHeightUpdate()
        tableViewConfiguration()
        self.updateParamsIndicator()
        self.prepareParameters()

//        self.showPartialSpinner(onView: self.plantSearchTableView)
        queueSearchPlant(true, true)

        
        DispatchQueue.global(qos: .background).async {
//            self.searchPlant(true)
            self.fillParameters()
        }
        // Do any additional setup after loading the view.
        
//        self.navigationItem.addHomeNavigation(width: width!)
            
    }
    
    override func viewWillAppear(_ animated: Bool) {
        super.viewWillAppear(false)
        pageConfiguration()
        
        self.updateNavigationBar(isMain: false, "PFLANZEN", "SUCHE", "main_plantSearch")

    }
    
    func tableViewConfiguration(){
        
        self.plantSearchTableView.backgroundColor = .clear
    }
    
    func pageConfiguration(){
        self.applyTheme()
       
        self.nameFilterView.addBorderRadius()
        self.nameFilterView.addShadow()
        self.ecoFilterView.addBorderRadius()
        self.ecoFilterView.addShadow()
        self.freezingFilterView.addBorderRadius()
        self.freezingFilterView.addShadow()
        self.detailFilterView.addBorderRadius()
        self.detailFilterView.addShadow()
        self.plantSearchButtonEco.setGreenButton()
        
        self.searchTagCollectionView.backgroundColor = .clear
        self.ecoFilterButton.contentHorizontalAlignment = .left
         self.freezingFilterButton.contentHorizontalAlignment = .left
         self.detailFilterButton.contentHorizontalAlignment = .left

        
//        self.nameSearchField
//        self.nameSearchField.layer.borderColor =  CGColor(srgbRed: 1, green: 1, blue: 1, alpha: 0)
//        self.nameSearchField.layer.addBorder(toSide: .bottom, withColor: CGColor(srgbRed: 0, green: 0, blue: 0, alpha: 1), andThickness: 1)
        
        self.plantGroupSelectButton.setCustomSearchButton()
        self.plantFamilySelectButton.setCustomSearchButton()
        self.nameSearchButton.setCustomSearchButton()

//
//        self.plantGroupSelectButton.layer.addBorder(toSide: .bottom, withColor: CGColor(srgbRed: 0, green: 0, blue: 0, alpha: 1), andThickness: 1)
//        self.plantFamilySelectButton.layer.addBorder(toSide: .bottom, withColor: CGColor(srgbRed: 0, green: 0, blue: 0, alpha: 1), andThickness: 1)
//
//        self.plantGroupSelectButton.contentHorizontalAlignment = .left
//        self.plantFamilySelectButton.contentHorizontalAlignment = .left
        

    }
    
    @objc func nameAuto(gesture: UIGestureRecognizer)
    {
        self.performSegue(withIdentifier: "nameSearch", sender: nil)
    }
    
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(true)
        self.updateParamsIndicator()
    }
    
    func getAllImages(){
        
        let imageUrlList = plantSearchResult?.Plants!.reduce(into: [String]()){output, input in
            var raw = input.Images![0].SrcAttr
            output.append((raw?.toURL(false, false))!)
        }
        
        
        getImageFiles(files: imageUrlList!){images in
            self.plantImageList = images
            self.plantSearchTableView.reloadData()
        }
        
    }
    
    func queueSearchPlant(_ updatePage: Bool = false, _ fullLoading: Bool = false){
        
        isUpdatePage = updatePage
        updateParameters()

        if isSearching{
            isQueueSearch = true
            return
        }
        
        isSearching = true
        if fullLoading{
            self.showSpinner(onView: self.view)

        }
        else{
            self.showPartialSpinner(onView: self.plantSearchTableView)

        }
        DispatchQueue.global(qos: .background).async {
            self.searchPlant(updatePage)
        }
    }
    
    func searchPlant(_ updatePage: Bool = false){
        print("params is", self.searchParams, APP_URL.PLANT_SEARCH_API+self.searchParams.getRawBody())
        getPlantCount(updatePage)
        
        self.plantSearchTableView.setContentOffset(.zero, animated: false)
        NetworkManager().requestDataAsync(type: PlantSearch.self, APP_URL.PLANT_SEARCH_API+self.searchParams.getRawBody(), printRequest: false, encoding: JSONEncoding.default){response in
            
            if !response.success{
                
                self.ShowAlert(message: response.result as! String)
                self.removeSpinner()
                return
            }
            
            self.plantSearchResult = (response.result as! PlantSearch)
            
            DispatchQueue.main.async {
                self.plantSearchTableView.reloadData()
                self.getAllImages()
            }
           
            
            
            if !self.isQueueSearch{
                self.removeSpinner()
                self.isSearching = false

                return
            }
            self.isQueueSearch = false
            self.searchPlant(self.isUpdatePage)
        }
        
      
    }
    
    func getPlantCount(_ updatePage: Bool = false){
        var countParams = self.searchParams
        countParams.removeValue(forKey: "take")
        print("count params is", countParams)
        NetworkManager().requestDataAsync(type: Int.self, APP_URL.PLANT_SEARCH_COUNT+countParams.getRawBody(), printRequest: true, encoding: JSONEncoding.default){response in
            if !response.success{
                self.ShowAlert(message: response.result as! String)
                return
            }
            self.plantSearchCount = response.result as! Int
            if updatePage{
                self.updateSearchNumber()
                
            }
            
        }
    }
    
    func scrollSearchPlant(){
        
    }
    
    func updateSearchNumber(){
        
        self.searchNumberLabel.text = "\(self.plantSearchCount) Treffer"
        self.searchNumberLabelEco.text = "\(self.plantSearchCount) Treffer"
        self.currentPage = 1
        self.maxPage = self.plantSearchCount / 20 + 1
        updateSearchPageLabel()
    }
    
    func updateSearchPageLabel(){
        var pageArray: [String] = []

        if self.currentPage == 1{
            for n in 1...min(1 + 2, maxPage){
                pageArray.append("\(n)")
            }
            
            
        }else if self.currentPage == maxPage{
            for n in max(1, currentPage - 2)...maxPage{
                pageArray.append("\(n)")
            }
        }else{
            for n in max(1, currentPage - 1)...min(maxPage, currentPage + 1){
                pageArray.append("\(n)")
            }
        }
        
        var rawLabel = pageArray.joined(separator: " ") as NSString
        
        let attributedString = NSMutableAttributedString(string: rawLabel as String)
        
        
        attributedString.addAttribute(.font, value: UIFont.boldSystemFont(ofSize: 17), range: rawLabel.range(of: "\(currentPage)"))
            
            
        
        self.pageNumberLabel.attributedText = attributedString
    }
    
    @IBAction func onFirstPage(_ sender: Any) {
        if isSearching{
            return
        }
        
        if currentPage <= 1 {
            return
        }
        
        currentPage = 1
        print("page is pressed", currentPage)

        
        updatePagingParameter()
//        self.showPartialSpinner(onView: self.plantSearchTableView)
//        DispatchQueue.main.async {
//            self.searchPlant()
//        }
        
        queueSearchPlant()
    }
    
    @IBAction func onLastPage(_ sender: Any) {
        if isSearching{
            return
        }
        
        if currentPage >= maxPage {
            return
        }
        
        currentPage = maxPage
        print("page is pressed", currentPage)

        
        updatePagingParameter()
//        self.showPartialSpinner(onView: self.plantSearchTableView)
//        DispatchQueue.main.async {
//            self.searchPlant()
//        }
        
        queueSearchPlant()
    }
    
    
    @IBAction func onPrevPage(_ sender: Any) {
        print("page is pressed")
        
        if isSearching{
            return
        }
        
        if currentPage <= 1 {
            return
        }
        
        currentPage -= 1
        print("page is pressed", currentPage)

        
        updatePagingParameter()
//        self.showPartialSpinner(onView: self.plantSearchTableView)
//        DispatchQueue.main.async {
//            self.searchPlant()
//        }
        
        queueSearchPlant()
    }
    
    @IBAction func onNextPage(_ sender: Any) {

        if isSearching{
            return
        }
        
        if currentPage >= maxPage {
            return
        }
        
        currentPage += 1
        print("page is pressed", currentPage)

        
        updatePagingParameter()
//        self.showPartialSpinner(onView: self.plantSearchTableView)
//        DispatchQueue.main.async {
//            self.searchPlant()
//        }
        
        queueSearchPlant()

    }
    
    func updatePagingParameter(){
        updateSearchPageLabel()
        searchParams["skip"] = (currentPage - 1) * 20
    }
    
    override func touchesBegan(_ touches: Set<UITouch>, with event: UIEvent?) {
        view.endEditing(true    )
    }
    
    @IBAction func startSearchFromMain(_ sender: Any) {
         view.endEditing(true    )
        updateParameters()
        self.searchPlantFromParams()
    }
    
    @IBAction func startPlantSearch(_ sender: Any) {
        view.endEditing(true    )
        updateParameters()
        self.searchPlantFromParams(true)
    }
    
    @IBAction func startPlantSearchEco(_ sender: Any) {
        view.endEditing(true    )
        updateParameters()
        self.searchPlantFromParams(true)
    }
    @IBAction func showPlantNameInfo(_ sender: Any) {
        self.ShowAlert(message: "Botanische Namen/Deutsche Namen/Synonyme")
    }
    
    @IBAction func onEcoFilterDropdownClick(_ sender: Any) {
        print("is clicked")
        self.ecoDropdownToggle()
    }
    
    @IBAction func onDropdownToggleClicked(_ sender: Any) {
        isDropdownFilterToggle = !isDropdownFilterToggle
        dropdownHeightUpdate()
    }
    
    @IBAction func onPlantGroupClicked(_ sender: Any) {
        isGroupDropdownExtend = !isGroupDropdownExtend
        
        updateGroupAndFamilyDropdownHeight()
    }
    
    @IBAction func onPlantFamilyClicked(_ sender: Any) {
        isFamilyDropdownExtend = !isFamilyDropdownExtend
        updateGroupAndFamilyDropdownHeight()
    }
    
    func dropdownHeightUpdate(){
        if isDropdownFilterToggle{
            detailFilterDropdownHeight.constant = 2000
            UIView.animate(withDuration: 0.5, animations: {
                            self.mainScrollView.layoutIfNeeded()
                self.detailFilter1TableView.alpha = 1
                self.detailFilter2TableView.alpha = 1
                
            }, completion: { [self]_ in
                updateDetailDropdown1()

                detailFilterDropdownHeight.isActive = false
            })
            
        }
        else{
            DispatchQueue.main.async {
                self.detailFilterDropdownHeight.isActive = true
                while self.detailFilterDropdownHeight == nil {
                    print("not yet")
                }
                self.detailFilter1TableView.alpha = 0
                self.detailFilter2TableView.alpha = 0
//                self.detailFilterView.alpha = 0
                self.detailFilterDropdownHeight.constant = 40
            }
            
        }
    }
    
    @IBAction func onDeleteAllTags(_ sender: Any) {
        self.deleteAllTags()
    }
    
    func scrollViewWillBeginDragging(_ scrollView: UIScrollView) {
        view.endEditing(true    )
    }
    
}

extension PlantSearchViewController: UITableViewDataSource, UITableViewDelegate{
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return plantSearchResult?.Plants!.count ?? 0
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: "plantSearchViewCell", for: indexPath) as! PlantSearchViewCell
        
        cell.onConfigure(plant: (plantSearchResult?.Plants![indexPath.row])!)
        
        cell.viewController = self
        
        if self.plantImageList.count >= (plantSearchResult?.Plants!.count)!{
            cell.plantImageView.image = self.plantImageList[indexPath.row]
        }
        //        cell.textLabel?.text = plantSearchResult?.Plants![indexPath.row].NameLatin
        return cell
    }
    
    
    
}

extension PlantSearchViewController: plantSearchDelegate{
    func updateNameSearch(name: String) {
        self.nameSearchField.text = name
        queueSearchPlant(true)

        updateNameSearchButtonText()
    }
    
    func updateNameSearchButtonText(){
        self.nameSearchButton.setTitle(self.nameSearchField.text ?? "", for: .normal)
    }
    
    func updateSearchParam(params: Parameters) {
        
        self.searchParams = params
    }
    
    func searchPlantWithParam(params: Parameters) {
        self.searchParams = params
        self.searchParams["skip"] = 0
        self.searchParams["take"] = 20
//        self.showPartialSpinner(onView: self.plantSearchTableView)
//        DispatchQueue.main.async {
//            self.searchPlant()
//        }
        
        queueSearchPlant(true)

    }
    
    
}

// Filter Part
extension PlantSearchViewController: FilterUpdateDelegate{
    func updateSliderValue(value: [Int], sender: Any?, type: SliderType) {
        switch type {
        case .month:
            self.monthTags = value
            updateMonthTags()
        case .height:
            self.heightTags = value
            updateHeightTags()
        case .freezing:
            self.frostTag = value[0]
            updateFreezingTag()
        default:
            return
        }
    }
    
    func updateFreezingTag(){
        let tagKey = "freezing"
        
        if self.frostTag <= 1{
            self.tagCollectionData.removeValue(forKey: tagKey)

        }
        else{
            let temperature = 10 - (self.frostTag - 1) * 5
            self.tagCollectionData[tagKey] = (t: "Frosthärte \(temperature)°C", id: -4, type: .slider)
        }
        queueSearchPlant(true)

//        queueSearchPlant()
        self.searchTagCollectionView.reloadData()

    }
    
    func updateMonthTags(){
        
        let tagKey = "month"

        if self.monthTags[0] <= 1 && self.monthTags[1] >= 12{
            self.tagCollectionData.removeValue(forKey: tagKey)

        }
        else{
            self.tagCollectionData[tagKey] = (t: "Blühdauer Monat \(self.monthTags[0]) bis \(self.monthTags[1])", id: -2, type: .slider)

        }
        queueSearchPlant(true)

        self.searchTagCollectionView.reloadData()

        
    }
    func resetMonthTag(){
          self.tagCollectionData.removeValue(forKey: "month")
        self.monthTags = [1,12]
        queueSearchPlant(true)

        self.searchTagCollectionView.reloadData()

    }
    
    func resetFreezingTag(){
        self.tagCollectionData.removeValue(forKey: "freezing")
        self.frostTag = 1
        queueSearchPlant(true)

        self.searchTagCollectionView.reloadData()
        
    }
    
    func resetHeightTag(){
          self.tagCollectionData.removeValue(forKey: "height")
        self.heightTags = [0,800]
        queueSearchPlant(true)

        self.searchTagCollectionView.reloadData()

    }
    
    func updateHeightTags(){
        
        let tagKey = "height"

        if self.heightTags[0] <= 0 && self.heightTags[1] >= 800{
            self.tagCollectionData.removeValue(forKey: tagKey)

        }
        else{
            self.tagCollectionData[tagKey] = (t: "\(self.heightTags[0]) cm - \(self.heightTags[1]) cm", id: -3, type: .slider)

        }
        queueSearchPlant(true)

        self.searchTagCollectionView.reloadData()

    }
    
    func updateTagsString(value: String, sender: Any?, type: FilterType, tableType: TableType) {
        
        let tagKey = value
        if self.tagCollectionData[tagKey] != nil {
            self.tagCollectionData.removeValue(forKey: tagKey)
        }
        else{
            self.tagCollectionData[tagKey] = (t: value, id: -1, type: type)
        }
        
        self.searchTagCollectionView.reloadData()
        
        self.plantFamilies.AddOrRemove(value: value)
        
        updateParamsIndicator()
        isFamilyDropdownExtend = false
        updateGroupAndFamilyDropdownHeight()
        updateFamilyDropdown()
        queueSearchPlant(true)

        self.mainScrollView.setContentOffset(CGPoint(x: 0, y: 0), animated: true)
        if sender == nil{
            return
        }
        let object = sender as! PlantSearchFilterSelectTableViewController
        object.outputTagsString = self.plantFamilies
        
    }
    
    func updateTags(value: Int, name: String, sender: Any?, type: FilterType, tableType: TableType) {

        var changedTags: [Int] = []
        
        let tagKey = name+"-\(value)"
        
        if self.tagCollectionData[tagKey] != nil {
            self.tagCollectionData.removeValue(forKey: tagKey)
        }
        else{
            self.tagCollectionData[tagKey] = (t: name, id: value, type: type)
        }
        
        self.searchTagCollectionView.reloadData()
        
        self.findTagsName(tag: value)
        switch type {
        case .cookieTag:

            self.cookieTags.AddOrRemove(value: value)

            changedTags = self.cookieTags
            updateDetailDropdown1()
        case .ecoTag:
            self.paramEcoTags.AddOrRemove(value: value )
            changedTags = self.paramEcoTags
            updateEcoDropdown()
        case .colorsTag:
            self.colorsTags.AddOrRemove(value: value)
            changedTags = self.colorsTags
            updateDetailDropdown1()
        case .excludeTag:
            self.excludeTags.AddOrRemove(value: value)
            changedTags = self.excludeTags
        case .leafColorsTag:
            self.leafColorsTags.AddOrRemove(value: value)
            changedTags = self.leafColorsTags
            updateDetailDropdown1()
        case .autumnColorsTag:
            self.autumnColorsTags.AddOrRemove(value: value)
            changedTags = self.autumnColorsTags
            updateDetailDropdown1()
        case .plantGroup:
            self.plantGroups.AddOrRemove(value: value)
            changedTags = self.plantGroups
            
            isGroupDropdownExtend = false
            updateGroupAndFamilyDropdownHeight()
            updateGroupDropdown()
            updateDetailDropdown1()
            
        default:
            return
        }
        updateParamsIndicator()
        
//        self.showPartialSpinner(onView: self.plantSearchTableView)
//
//        DispatchQueue.main.async {
//            self.searchPlant(true)
//        }
        queueSearchPlant(true)

        
        if sender == nil{
            return
        }
        switch tableType {
        case .checkbox:
            let object = sender as! CheckBoxFilter
            object.outputTags = changedTags
        case .filterGeneral:
            let object = sender as! PlantSearchFilterSelectTableViewController
            object.outputTags = changedTags
        case .ecoFilter:
            let object = sender as! EcoDropdownObject
            object.outputTags = changedTags
        default:
            return
        }
        
    }
    
    func findTagsName(tag: Int){
        
//        print(self.tags)
        
        var value = self.tags.reduce(into: Int()){output, inputVal in
//            print(inputVal.value.filter({$0.id == tag}))
            let filter = inputVal.value.filter({$0.id == tag})
            output = 3
        }
    }
    
    
    func updateParamsIndicator(){
        if let selectedGroup = self.plantGroups.first{
            let group = self.tags["Pflanzengruppe"]?.first(where: {$0.id == selectedGroup})
            let groupName = group?.t ?? ""
//            print("group is", group?.t, groupName)

            self.plantGroupSelectButton.setTitle(groupName, for: .normal)
//            self.plantGroupSelectButton.titleLabel?.text = "whyaaaa"

        }
        else{
            self.plantGroupSelectButton.setTitle("z. B. Immergrüne", for: .normal)


        }
        print("family is", self.plantFamilies)
        if self.plantFamilies.first != "" && self.plantFamilies.count > 0{
            var name: String = self.plantFamilies.first ?? ""
            self.plantFamilySelectButton.setTitle(name.removeItalicTag(), for: .normal)
        }
        else{
            self.plantFamilySelectButton.setTitle("z. B. Ericaceae", for: .normal)

        }
//        print(self.tags["Pflanzengruppe"]?.first({$0.id == self.plantGroups.first}))
//        self.plantGroupSelectButton.titleLabel?.text = self.tags["Pflanzengruppe"]!.first({$0.id == self.plantGroups.first})
    }
    
    func fillParameters(){

        self.ecoTags = [(t: "Bienenfreundlich", id: 447),
                        (t: "Insektenfreundlich", id: 321),
                        (t: "Vogelfreundlich", id: 322),
                        (t: "Schmetterlingsfreundlich", id: 531),
                        (t: "Ökologisch wertvoll", id: 445),
                        (t: "Heimische Pflanze", id: 530)
        ]
        
        DispatchQueue.main.async {
            self.configureEcoDropdown()

        }

        
        if UserDefaultKeys.plantSearchCategory.any() != nil {
            print("is saved")
            
            let savedJson = UserDefaultKeys.plantSearchCategory.any() as! Data
            
            do {
                
                let decodedSentences = try JSONDecoder().decode([searchCategoryModel].self, from: savedJson)

                print("saved data is", decodedSentences)
                
                
                DispatchQueue.main.async {
                    self.fillCategoryTags(data: decodedSentences )
                    self.fillPlantTags()

                }
                
                
                NetworkManager().requestDataAsync(type: [searchCategoryModel].self, APP_URL.PLANT_SEARCH_GET_CATS){response in
                    if !response.success{
                        return
                    }
                    do {
                        let jsonData = try JSONEncoder().encode(response.result as! [searchCategoryModel])
                        let jsonString = String(data: jsonData, encoding: .utf8)
                        print("json string is", jsonString)
                        
                        UserDefaultKeys.plantSearchCategory.set(jsonData)
                    } catch { print(error) }
               
                }
                
            } catch { print(error) }
//            let tempDict = UserDefaultKeys.plantSearchCategory.any() as! searchCategoryStorageModel
//            self.fillCategoryTags(data: tempDict.data!)
//            self.fillPlantTags()

        }
        else{
            NetworkManager().requestDataAsync(type: [searchCategoryModel].self, APP_URL.PLANT_SEARCH_GET_CATS){response in
                if !response.success{
                    self.ShowAlert(message: response.result as! String)
                    return
                }
                self.fillCategoryTags(data: response.result as! [searchCategoryModel] )
                
                
                do {
                    let jsonData = try JSONEncoder().encode(response.result as! [searchCategoryModel])
                    let jsonString = String(data: jsonData, encoding: .utf8)
                    print("json string is", jsonString)
                    
                    UserDefaultKeys.plantSearchCategory.set(jsonData)
                } catch { print(error) }
               
                
           
                self.fillPlantTags()
            }
        }
        
        
       
        
    }
    
    func fillPlantTags(){
        NetworkManager().requestDataAsync(type: [badgeTagsModel].self, APP_URL.PLANT_SEARCH_GET_TAGS){response in
            if !response.success{
                self.ShowAlert(message: response.result as! String)
                return
            }
            
            self.tags = (response.result as! [badgeTagsModel]).reduce(into:[String: [(t: String, id: Int)]]()){output, input in
                if let key = self.reverseCategoryTags[input.CategoryId]{
                    if output[key] != nil{
                        output[key]?.append((id: input.Id, t: input.Title ?? ""))
                    }
                    else{
                        output[key] = [(id: input.Id, t: input.Title ?? "")]
                    }
                }
            }
     
            self.tags["Ökologischen"] = self.ecoTags
            self.configureDetailDropdown1()
            
            print("tags string is", self.tags.customMirror)
            
            let dict = self.tags as NSDictionary
            
            print("transform mirror", dict as! [String: [(t: String, id: Int)]])
            NetworkManager().requestDataAsync(type: plantGroupMainModel.self, APP_URL.PLANT_SEARCH_GET_GROUPS, printRequest: true){response in
                self.tags["Pflanzengruppe"] = []
                if !response.success{
                    self.ShowAlert(message: "Pflanzengruppe")
                    return
                }
                var groupData = (response.result as! plantGroupMainModel)
                
                var groupTags = groupData.Groups!.map{ (t: $0.Name, id: $0.Id ) }
                self.tags["Pflanzengruppe"] = groupTags
                
                DispatchQueue.main.async {
                    self.configureGroupDropdown()

                }
                
               
//                        UserDefaultKeys.plantSearchTag.set(self.tags as NSDictionary)
            }
            NetworkManager().requestDataAsync(type: [String?].self, APP_URL.PLANT_SEARCH_GET_FAMILIES){response in
                self.tags["Pflanzenfamilie"] = []
                if !response.success{
                    self.ShowAlert(message: response.result as! String)
                    return
                }
                var familyTags = (response.result as! [String?]).map{( t: $0 ?? "", id: -1)}
                self.tags["Pflanzenfamilie"] = familyTags
                
                DispatchQueue.main.async {
                    self.configureFamilyDropdown()

                }

//                        UserDefaultKeys.plantSearchTag.set(self.tags)
            }
            //                self.fillFilterTableView()
        }
    }
    
    func configureEcoDropdown(){
        
        self.ecoDropdownTableView.tagSelectList = self.ecoTags
        self.ecoDropdownTableView.filterDelegate = self
        
        self.ecoFilterMainTableView.delegate = self.ecoDropdownTableView
        self.ecoFilterMainTableView.dataSource = self.ecoDropdownTableView
        self.ecoFilterMainTableView.reloadData()

    }
    
    func updateEcoDropdown(){
        self.ecoDropdownTableView.outputTags = self.paramEcoTags
        self.ecoFilterMainTableView.reloadData()
    }
    
    
    func configureDetailDropdown1(){
        
        DispatchQueue.main.async {
            self.detailFilterDropdown1.parent = self
            self.detailFilter1TableView.delegate = self.detailFilterDropdown1
            self.detailFilter1TableView.dataSource = self.detailFilterDropdown1
            self.detailFilter1TableView.reloadData()
            
            self.detailFilterDropdown2.parent = self
            self.detailFilter2TableView.delegate = self.detailFilterDropdown2
            self.detailFilter2TableView.dataSource = self.detailFilterDropdown2
            self.detailFilter2TableView.reloadData()
        }
        
        
    }
    
    func configureFamilyDropdown(){
        self.familyDropdownObject.filterDelegate = self
        self.familyDropdownObject.tagSelectList = self.tags["Pflanzenfamilie"]!
        self.familyDropdownObject.outputTagsString = self.plantFamilies
        self.familyDropdownObject.filterType = FilterType.plantFamily
        
        self.plantFamilyDropdownTable.delegate = self.familyDropdownObject
        self.plantFamilyDropdownTable.dataSource = self.familyDropdownObject
        self.plantFamilyDropdownTable.reloadData()
    }
    
    func configureGroupDropdown(){
        self.groupDropdownObject.filterDelegate = self
        self.groupDropdownObject.tagSelectList = self.tags["Pflanzengruppe"]!
        self.groupDropdownObject.outputTags = self.plantGroups
        self.groupDropdownObject.filterType = FilterType.plantGroup
        
        self.plantGroupDropdownTable.delegate = self.groupDropdownObject
        self.plantGroupDropdownTable.dataSource = self.groupDropdownObject
        self.plantGroupDropdownTable.reloadData()
    }
    
    func updateGroupAndFamilyDropdownHeight(){
        if isGroupDropdownExtend{
            if self.tags["Pflanzengruppe"] != nil{
                plantGroupDropdownHeight.constant = CGFloat(dropdownGroupFilterHeight) * CGFloat(self.tags["Pflanzengruppe"]!.count)
            }
            
            
        }
        else{
            plantGroupDropdownHeight.constant = 0
            
        }
        
        if isFamilyDropdownExtend{
            if self.tags["Pflanzenfamilie"] != nil{
                plantFamilyDropdownHeight.constant = CGFloat(dropdownGroupFilterHeight) * CGFloat(self.tags["Pflanzenfamilie"]!.count)
            }
            
        }
        else{
            plantFamilyDropdownHeight.constant = 0
        }
        
        UIView.animate(withDuration: 0.5, animations: {self.mainScrollView.layoutIfNeeded()})
    }
    
    func updateGroupDropdown(){
        self.groupDropdownObject.outputTags = self.plantGroups
        self.plantGroupDropdownTable.reloadData()
    }
    
    func updateFamilyDropdown(){
        self.familyDropdownObject.outputTagsString = self.plantFamilies
        self.plantFamilyDropdownTable.reloadData()
    }
    
    func updateDetailDropdown1(){
        self.detailFilter1TableView.reloadData()
        self.detailFilter2TableView.reloadData()

    }
    
    func fillCategoryTags(data: [searchCategoryModel]){
        for item in data{
            if let val = self.categoryTags[item.Title]{
                self.categoryTags[item.Title] = item.Id
                self.reverseCategoryTags[item.Id] = item.Title
            }
        }
    }
    
    func prepareParameters(){
        nameSearchField.text = ((self.searchParams["searchText"] ?? "") as! String)
        updateNameSearchButtonText()
        self.cookieTags = ((self.searchParams["cookieTags"] ?? "") as! String).split(separator: ",").reduce(into: [Int]()){result, str in
            result.append(Int(String(str))!)
        }
        
        self.paramEcoTags = ((self.searchParams["ecosTags"] ?? "") as! String).split(separator: ",").reduce(into: [Int]()){result, str in
            result.append(Int(String(str))!)
        }
        
        
        
        self.colorsTags = ((self.searchParams["colors"] ?? "") as! String).split(separator: ",").reduce(into: [Int]()){result, str in
            result.append(Int(String(str))!)
        }
        self.leafColorsTags = ((self.searchParams["leafColors"] ?? "") as! String).split(separator: ",").reduce(into: [Int]()){result, str in
            result.append(Int(String(str))!)
        }
        self.autumnColorsTags = ((self.searchParams["autumnColors"] ?? "") as! String).split(separator: ",").reduce(into: [Int]()){result, str in
            result.append(Int(String(str))!)
        }
        self.plantGroups = ((self.searchParams["groupId"] ?? "") as! String).split(separator: ",").reduce(into: [Int]()){result, str in
            result.append(Int(String(str))!)
        }
        self.plantFamilies = ((self.searchParams["family"] ?? "") as! String).split(separator: ",").reduce(into: [String]()){result, str in
            result.append(String(str))
        }
        let minHeight = self.searchParams["selHmin"] as! Int
        let maxHeight = self.searchParams["selHmax"] as! Int
        let minMonth = self.searchParams["selMinMonth"] as! Int
        let maxMonth = self.searchParams["selMaxMonth"] as! Int
        self.heightTags = [minHeight, maxHeight]
        self.monthTags = [minMonth, maxMonth]
        
        self.frostTag = freezingId.firstIndex(of: (self.searchParams["freezes"] ?? "") as! String) ?? 1
        if self.frostTag == 0{
            self.frostTag = 1
        }
     
        
    }
    
    func updateParameters(){
        // Name Search
        self.searchParams["searchText"] = nameSearchField.text
        self.searchParams["cookieTags"] = self.cookieTags.map { String($0) }.joined(separator: ",")
        self.searchParams["ecosTags"] = self.paramEcoTags.map { String($0) }.joined(separator: ",")
        self.searchParams["colors"] = self.colorsTags.map { String($0) }.joined(separator: ",")
        self.searchParams["leafColors"] = self.leafColorsTags.map { String($0) }.joined(separator: ",")
        self.searchParams["autumnColors"] = self.autumnColorsTags.map { String($0) }.joined(separator: ",")
        self.searchParams["groupId"] = self.plantGroups.map { String($0) }.joined(separator: ",")
        self.searchParams["family"] = self.plantFamilies.map { $0 }.joined(separator: ",")
        self.searchParams["selHmin"] = self.heightTags[0]
        self.searchParams["selHmax"] = self.heightTags[1]
        self.searchParams["selMinMonth"] = self.monthTags[0]
        self.searchParams["selMaxMonth"] = self.monthTags[1]
        self.searchParams["freezes"] = freezingId[self.frostTag - 1]
        
    }
    
    func searchPlantFromParams(_ updatePage: Bool = false) {
//        let params = self.searchParams
//        self.searchParams = params

        queueSearchPlant(updatePage)

        
//        self.showPartialSpinner(onView: self.plantSearchTableView)
//
//        DispatchQueue.main.async {
//            self.searchPlant(updatePage)
//        }
    }
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        view.endEditing(true    )

        if segue.identifier == "plantGroup"{
            let searchFilterSelectView = segue.destination as! PlantSearchFilterSelectTableViewController
            
            searchFilterSelectView.tagSelectList = self.tags["Pflanzengruppe"]!
            searchFilterSelectView.filterDelegate = self
            searchFilterSelectView.isBackAuto = true
            searchFilterSelectView.tableView.allowsMultipleSelection = false
            searchFilterSelectView.outputTags = self.plantGroups
            searchFilterSelectView.filterType = FilterType.plantGroup
            
        }
            
        else if segue.identifier == "plantFamily"{
            let searchFilterSelectView = segue.destination as! PlantSearchFilterSelectTableViewController
            
            searchFilterSelectView.tagSelectList = self.tags["Pflanzenfamilie"]!
            searchFilterSelectView.filterDelegate = self
            searchFilterSelectView.tableView.allowsMultipleSelection = false
            searchFilterSelectView.isBackAuto = true

            searchFilterSelectView.outputTagsString = self.plantFamilies
            searchFilterSelectView.filterType = FilterType.plantFamily
            
        }
        
        else if segue.identifier == "plantFreezeSlider"{
            let searchFilterSelectView = segue.destination as! PlantSearchFilterSliderViewController
            searchFilterSelectView.filterDelegate = self
            
            searchFilterSelectView.isItMonth = true
            searchFilterSelectView.rangeData = [self.frostTag]
            searchFilterSelectView.sliderType = SliderType.freezing
        }
//        else if segue.identifier == "searchFilterDetail"{
//            let searchFilterView = segue.destination as! PlantSearchFilterViewController
//            searchFilterView.parentView = self
//            searchFilterView.searchDelegate = self
//            searchFilterView.searchParams = self.searchParams
//            print("sended search params is")
//        }
        else if segue.identifier == "plantEcologicalFilter"{
            let searchFilterSelectView = segue.destination as! PlantSearchFilterSelectTableViewController
            searchFilterSelectView.tagSelectList = self.tags["Ökologischen"]!
            searchFilterSelectView.isEcoCat = true
            searchFilterSelectView.filterDelegate = self
            searchFilterSelectView.tableView.allowsMultipleSelection = true
            searchFilterSelectView.outputTags = self.paramEcoTags
            searchFilterSelectView.filterType = FilterType.ecoTag
            


        }
        else if segue.identifier == "nameSearch"{
            
            let view = segue.destination as! PlantNameSearchViewController
            
            view.plantName = self.nameSearchField.text ?? ""
            view.plantDelegate = self
            if (self.plantSearchResult?.Plants?.count ?? 0) > 0{
                view.plantNameList = self.plantSearchResult?.Plants?.reduce(into: [String]()){output, input in
                    var text = input.NameGerman.removeItalicTag()
                    output.append(text.removeApostrophe())
                    
                    } as! [String]
            }
            
        }

    }
    
    override func shouldPerformSegue(withIdentifier identifier: String, sender: Any?) -> Bool {
//        if identifier == "plantEcologicalFilter"{
//            return false
//
//
//        }
//
        return true
    }
    
    
}

extension PlantSearchViewController: UICollectionViewDelegate, UICollectionViewDataSource{
    
    func collectionView(_ collectionView: UICollectionView, numberOfItemsInSection section: Int) -> Int {
        return tagCollectionData.values.count
    }
    
    func collectionView(_ collectionView: UICollectionView, cellForItemAt indexPath: IndexPath) -> UICollectionViewCell {
        let cell = collectionView.dequeueReusableCell(withReuseIdentifier: "searchTagCell", for: indexPath) as! searchTagCollectionViewCell
        cell.backgroundColor = .clear
        cell.outerView.backgroundColor = .gray
        
        cell.outerView.addBorderRadius()
        
        var tagData = tagCollectionData.values.reduce(into: [(t: String, id: Int, type: FilterType)]()){ output, input in
            output.append((t: input.t, id: input.id, type: input.type))
        }
        cell.onConfigure(title: tagData[indexPath.row].t.removeItalicTag())
        cell.addBorderRadius()
        return cell
    }
    
    
    func collectionView(_ collectionView: UICollectionView, didSelectItemAt indexPath: IndexPath) {
        
        deleteTagsFunc(index: indexPath.row)
     
  
    }
    
    func deleteTagsFunc(index: Int){
        var tagData = tagCollectionData.values.reduce(into: [(t: String, id: Int, type: FilterType)]()){ output, input in
            output.append((t: input.t, id: input.id, type: input.type))
        }
        
        var tagValue = tagData[index]
    
        if tagValue.id == -1{
            self.updateTagsString(value: tagValue.t, sender: nil, type: tagValue.type, tableType: .filterGeneral)
            return
        }
        else if tagValue.id == -2{
            resetMonthTag()
            return
        }
        else if tagValue.id == -3{
            resetHeightTag()
            return
        }
        else if tagValue.id == -4{
            resetFreezingTag()
            return
        }
        self.updateTags(value: tagValue.id, name: tagValue.t, sender: nil, type: tagValue.type, tableType: .filterGeneral)
    }

    func deleteAllTags(){
        
        var tagData = tagCollectionData.values.reduce(into: [(t: String, id: Int, type: FilterType)]()){ output, input in
            output.append((t: input.t, id: input.id, type: input.type))
        }
        
        for tagValue in tagData{
            if tagValue.id == -1{
                self.updateTagsString(value: tagValue.t, sender: nil, type: tagValue.type, tableType: .filterGeneral)
                return
            }
            else if tagValue.id == -2{
                resetMonthTag()
                return
            }
            else if tagValue.id == -3{
                resetHeightTag()
                return
            }
            else if tagValue.id == -4{
                resetFreezingTag()
                return
            }
            self.updateTags(value: tagValue.id, name: tagValue.t, sender: nil, type: tagValue.type, tableType: .filterGeneral)
        }
    }
    
}

extension PlantSearchViewController{
    
    func ecoDropdownToggle(){
        isEcoDropdownExtend = !isEcoDropdownExtend
        ecoDropdownUpdate(animate: true)
    }
    
    func ecoDropdownUpdate(animate: Bool){
        var tableAlphaValue = 0
        if isEcoDropdownExtend{
            self.ecoFilterDropdownHeight.constant = 390
            tableAlphaValue = 1
        }
        else{
            self.ecoFilterDropdownHeight.constant = 40
            tableAlphaValue = 0
        }
        
        if !animate {
            self.searchBarView.alpha = CGFloat(tableAlphaValue)
            self.ecoFilterMainTableView.alpha = CGFloat(tableAlphaValue)
            return
        }
        
        UIView.animate(withDuration: 0.5, animations: {
            self.searchBarView.alpha = CGFloat(tableAlphaValue)
            self.ecoFilterMainTableView.alpha = CGFloat(tableAlphaValue)

            self.mainScrollView.layoutIfNeeded()
            
        })
    }
}


class EcoDropdownObject:  NSObject, UITableViewDelegate, UITableViewDataSource{
    
    var tagSelectList: [(t: String, id: Int)] = []
    var filterDelegate: FilterUpdateDelegate?
    
    var outputTags: [Int] = []
    
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return tagSelectList.count
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        print("tableview is", tableView)
        let cell = tableView.dequeueReusableCell(withIdentifier: "ecoDropdownCell", for: indexPath) as! EcoDropdownTableViewCell
//        let cell = UITableViewCell()
        
        cell.onConfigure(text: tagSelectList[indexPath.row].t)
        
        if self.outputTags.contains(self.tagSelectList[indexPath.row].id)
        {
            tableView.selectRow(at: indexPath, animated: true, scrollPosition: .none)
        }
//        cell.ecoLabel.text = tagSelectList[indexPath.row].t
        
        return cell
    }
    
    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        self.updateMainTags(value: self.tagSelectList[indexPath.row].id, name: self.tagSelectList[indexPath.row].t)
    }
    func updateMainTags(value: Any?, name: String){
  
        self.filterDelegate?.updateTags(value: value as! Int, name: name, sender: self, type: FilterType.ecoTag, tableType: TableType.ecoFilter)
        
        
    }
    
    func tableView(_ tableView: UITableView, didDeselectRowAt indexPath: IndexPath) {
        self.updateMainTags(value: self.tagSelectList[indexPath.row].id, name: self.tagSelectList[indexPath.row].t)
    }
    
}


class DetailDropdown1: NSObject, UITableViewDelegate, UITableViewDataSource{
    
    
    var detailMenu: [String] = ["Licht", "Laubrythmus", "Blühdauer", "Wuchshöhe", "Blütenfarben", "Verwendung"]
    
    var parent: PlantSearchViewController?
    
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return detailMenu.count
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        var selectedMenu = menuList.first(where: {$0.category == detailMenu[indexPath.row]})
        
        if selectedMenu?.type == FilterType.slider{
            let cell = tableView.dequeueReusableCell(withIdentifier: "DetailFilterSlider") as! DetailFilterSliderTableViewCell
            cell.filterDelegate = self.parent

            cell.filterTitleLabel.text = selectedMenu?.name
            if selectedMenu?.name == "Blühdauer"{
                cell.isItMonth = true
                cell.rangeData = self.parent!.monthTags
                cell.sliderType = SliderType.month
            }
            else if selectedMenu?.category == "Wuchshöhe"{
                cell.isItMonth = false
                cell.rangeData = self.parent!.heightTags
                cell.sliderType = SliderType.height
                
            }
            cell.sliderConfiguration()
            return cell
        }
        
        var cell = tableView.dequeueReusableCell(withIdentifier: "DetailFilterFixed", for: indexPath) as! DetailFilterFixedTableViewCell

        cell.filterDelegate = self.parent
        
        if selectedMenu?.type == FilterType.colorsTag{
            var tempColor = (self.parent?.tags[detailMenu[indexPath.row]])!
            cell.tagSelectList = tempColor.filter({ColorsKey[$0.t] != nil})

        }
        else{
            print(self.parent?.tags)
            print("detail menu is", detailMenu, indexPath.row)
            
            guard let tagList = self.parent?.tags[detailMenu[indexPath.row]] else{
                return UITableViewCell()
            }
            cell.tagSelectList = tagList

        }
        
        
        cell.isMultiTable = selectedMenu?.ismulti ?? true
        switch selectedMenu?.type {
        case .cookieTag:
            cell.outputTags = self.parent!.cookieTags
                cell.filterType = FilterType.cookieTag
        case .colorsTag:
            cell.outputTags = self.parent!.colorsTags
                cell.filterType = FilterType.colorsTag
        default:
            break
        }
        
        cell.cellConfiguration()
        cell.filterLabel.text = selectedMenu?.name
//
        return cell
    }
    
    
}

class DetailDropdown2: NSObject, UITableViewDelegate, UITableViewDataSource{
    
    var detailMenu: [[String]] = [["Ausschlusskriterien", "Besonderheiten", "Herbstfärbung", "Wuchs"
                                ,"Nutzpflanzen"
                                ,"Dekoaspekte"],
                                ["Blüten"
                                ,"Blütenform"
                                ,"Blütengröße"
                                ,"Fruchtfarbe"
                                ,"Früchte"
                                ,"Blütenstand"
                                ,"Blattfarbe"
                                ,"Blattrand"
                                ,"Blattstellung"
                                ,"Blattform"]
                                ,["Boden"
                                ,"Licht"
                                ,"Düngung"
                                ,"Schnitt"
                                ,"Vermehrung"
                                ,"Wasserbedarf"]
    ]
    
    var parent: PlantSearchViewController?
    var selectedCell: [Int] = []
    
    var sectionName: [String] = ["Eigenschaften","Blüten, Blätter, Früchte","Standort & Pflege"]
    var sectionCount: [Int] = [6,10,6]

    func tableView(_ tableView: UITableView, heightForRowAt indexPath: IndexPath) -> CGFloat {
        if !selectedCell.contains(indexPath.row)
        {
            return 40

        }
        else{
            return CGFloat((40 + 40 * (self.parent?.tags[detailMenu[indexPath.section][indexPath.row]]!.count)! ?? 0))
        }
    }
    
    func numberOfSections(in tableView: UITableView) -> Int {
        return sectionCount.count
    }
    
    func tableView(_ tableView: UITableView, titleForHeaderInSection section: Int) -> String? {
        return sectionName[section]
    }
    
    func tableView(_ tableView: UITableView, willDisplayHeaderView view: UIView, forSection section: Int) {
        view.tintColor = UIColor.systemBackground
//        view.backgroundColor = UIColor.systemBackground
    }
    
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return sectionCount[section]

    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        
        let cell = tableView.dequeueReusableCell(withIdentifier: "DetailFilterDrop", for: indexPath) as! DetailFilterDropTableViewCell
        
        cell.filterDelegate = self.parent
        cell.tagSelectList = (self.parent?.tags[detailMenu[indexPath.section][indexPath.row]])!
        
        var selectedMenu = menuList.first(where: {$0.name == detailMenu[indexPath.section][indexPath.row]})
        
        cell.isMultiTable = selectedMenu?.ismulti ?? true
        switch selectedMenu?.type {
        case .cookieTag:
            cell.outputTags = self.parent!.cookieTags
                cell.filterType = FilterType.cookieTag
        case .colorsTag: 
            cell.outputTags = self.parent!.colorsTags
                cell.filterType = FilterType.colorsTag
        case .excludeTag:
            cell.outputTags = self.parent!.excludeTags
            cell.filterType = FilterType.excludeTag
        case .leafColorsTag:
            cell.outputTags = self.parent!.leafColorsTags
            cell.filterType = FilterType.leafColorsTag
        case .autumnColorsTag:
            cell.outputTags = self.parent!.autumnColorsTags
            cell.filterType = FilterType.autumnColorsTag
        default:
            break
        }
        cell.cellIndex = indexPath
        cell.parent = self
        cell.dropDelegate = self
        cell.filterTitleLabel.text = detailMenu[indexPath.section][indexPath.row]
        cell.onConfigure(imageName: selectedMenu?.name ?? "")
        return cell
    }
    
    
    
    
}

extension DetailDropdown2: DetailDropSelectDelegate{
    func cellSelected(index: Int) {
        if self.selectedCell.contains(index){
            self.selectedCell.removeAll(where: {$0 == index})
        }
        else{
            self.selectedCell.append(index)
        }
        
        self.parent?.detailFilter2TableView.reloadData()
    }
    
    
}

let dropdownGroupFilterHeight = 30

class GroupFamilyDropdown: NSObject, UITableViewDelegate, UITableViewDataSource{
   
    
    
    var filterDelegate: FilterUpdateDelegate?
    var tagSelectList: [(t: String, id: Int)] = []
    var outputTags: [Int] = []
    var outputTagsString: [String] = []
    var filterType: FilterType?
    
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return tagSelectList.count
    }
    
    func tableView(_ tableView: UITableView, heightForRowAt indexPath: IndexPath) -> CGFloat {
        return CGFloat(dropdownGroupFilterHeight)
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = UITableViewCell()
        
        cell.textLabel?.text = tagSelectList[indexPath.row].t.removeItalicTag()
        
        switch filterType {
        case .plantFamily:
            if outputTagsString.contains(tagSelectList[indexPath.row].t){
                tableView.selectRow(at: indexPath, animated: false, scrollPosition: .none)
            }
        default:
            if outputTags.contains(tagSelectList[indexPath.row].id){
                tableView.selectRow(at: indexPath, animated: false, scrollPosition: .none)
            }
        }
        
        return cell
    }
    
    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        switch filterType {
        case .plantFamily:
            self.updateMainTags(value: self.tagSelectList[indexPath.row].t, name: self.tagSelectList[indexPath.row].t)
            
        default:
            self.updateMainTags(value: self.tagSelectList[indexPath.row].id, name: self.tagSelectList[indexPath.row].t)
            
        }
    }
    
    func tableView(_ tableView: UITableView, didDeselectRowAt indexPath: IndexPath) {
        switch filterType {
        case .plantFamily:
            self.updateMainTags(value: self.tagSelectList[indexPath.row].t, name: self.tagSelectList[indexPath.row].t)
            
        default:
            self.updateMainTags(value: self.tagSelectList[indexPath.row].id, name: self.tagSelectList[indexPath.row].t)
            
        }
    }
    

    
    func updateMainTags(value: Any?, name: String){
        switch filterType {
        case .plantFamily:
            self.filterDelegate?.updateTagsString(value: value as! String, sender: nil, type: filterType!, tableType: TableType.filterGeneral)
        default:
            self.filterDelegate?.updateTags(value: value as! Int, name: name, sender: nil, type: filterType!, tableType: TableType.filterGeneral)
        }
        
    }
    
}




