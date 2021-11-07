//
//  PlantSearchFilterViewController.swift
//  GardifyIOS
//
//  Created by Netzlab on 05.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit
import Alamofire

protocol FilterUpdateDelegate {
    func updateTags(value: Int, name: String, sender: Any?, type: FilterType, tableType: TableType)
    
    func updateTagsString(value: String, sender: Any?, type: FilterType, tableType: TableType)
    
    func updateSliderValue(value: [Int], sender: Any?, type: SliderType)
}

enum FilterType {
    case cookieTag
    case ecoTag
    case colorsTag
    case excludeTag
    case leafColorsTag
    case autumnColorsTag
    case plantGroup
    case plantFamily
    case slider
}

enum TableType {
    case checkbox
    case filterGeneral
    case ecoFilter
}

enum SliderType {
    case height
    case month
    case freezing
}

var menuList: [(category: String, name: String, type: FilterType, ismulti: Bool)] = [
//    (category: "Ökologischen", name: "Ökologischen", type: FilterType.cookieTag, ismulti: true),
//    (category: "Frosthärte", name: "Frosthärte", type: FilterType.slider, ismulti: false),
    (category: "Licht", name: "Standort", type: FilterType.cookieTag, ismulti: true),
    (category: "Laubrythmus", name: "Laubrythmus", type: FilterType.cookieTag, ismulti: false),
    (category: "Blühdauer", name: "Blühdauer", type: FilterType.slider, ismulti: false),
    (category: "Wuchshöhe", name: "Wuchshöhe", type: FilterType.slider, ismulti: false),
    (category: "Blütenfarben", name: "Blütenfarbe", type: FilterType.colorsTag, ismulti: true),
    (category: "Verwendung", name: "Verwendung", type: FilterType.cookieTag, ismulti: true),
    (category: "Ausschlusskriterien", name: "Ausschlusskriterien", type: FilterType.excludeTag, ismulti: true),
    (category: "Besonderheiten", name: "Besonderheiten", type: FilterType.cookieTag, ismulti: true),
    (category: "Herbstfärbung", name: "Herbstfärbung", type: FilterType.autumnColorsTag, ismulti: true),

    (category: "Wuchs", name: "Wuchs", type: FilterType.cookieTag, ismulti: true),
    (category: "Nutzpflanzen", name: "Nutzpflanzen", type: FilterType.cookieTag, ismulti: true),
    (category: "Dekoaspekte", name: "Dekoaspekte", type: FilterType.cookieTag, ismulti: true),
    (category: "Blüten", name: "Blüten", type: FilterType.cookieTag, ismulti: true),
    (category: "Blütenform", name: "Blütenform", type: FilterType.cookieTag, ismulti: true),
    (category: "Blütengröße", name: "Blütengröße", type: FilterType.cookieTag, ismulti: false),
    (category: "Fruchtfarbe", name: "Fruchtfarbe", type: FilterType.cookieTag, ismulti: true),
    (category: "Früchte", name: "Früchte", type: FilterType.cookieTag, ismulti: false),
    (category: "Blütenstand", name: "Blütenstand", type: FilterType.cookieTag, ismulti: false),
    (category: "Blattfarbe", name: "Blattfarbe", type: FilterType.leafColorsTag, ismulti: true),

    (category: "Blattrand", name: "Blattrand", type: FilterType.cookieTag, ismulti: false),
    (category: "Blattstellung", name: "Blattstellung", type: FilterType.cookieTag, ismulti: false),
    (category: "Blattform", name: "Blattform", type: FilterType.cookieTag, ismulti: false),
    (category: "Boden", name: "Boden", type: FilterType.cookieTag, ismulti: true),
    (category: "Licht", name: "Licht", type: FilterType.cookieTag, ismulti: true),
    (category: "Düngung", name: "Düngung", type: FilterType.cookieTag, ismulti: true),
    (category: "Schnitt", name: "Schnitt", type: FilterType.cookieTag, ismulti: true),
    (category: "Vermehrung", name: "Vermehrung", type: FilterType.cookieTag, ismulti: true),
    (category: "Wasserbedarf", name: "Wasserbedarf", type: FilterType.cookieTag, ismulti: false),
]

let ColorSort: [String: Int] = [
    "weiß" : 0,
    "gelb" : 1,
    "rot" : 2, "purpur" : 3, "lachsfarben" : 4, "gelbgrün" : 5, "hellblau" : 6,
    "silbrigweiß" : 7, "orange" : 8, "dunkelrot" : 9, "violett" : 10, "rosa" : 11, "grün" : 12, "blau" : 13,
    "creme" : 14, "apricot" : 15, "rotbraun" : 16, "braun" : 17, "pink" : 18, "blaugrün" : 19, "blauviolett" : 20, "schwarz" : 21,
    "fliederfarben" : 22
]

let freezingId: [String] = [
    "",
    "286,287,288,289,290,291,292,293,294",
    "286,287,288,289,290,291,292,293",
    "286,287,288,289,290,291,292",
    "286,287,288,289,290,291",
    "286,287,288,289,290",
    "286,287,288,289,290",
    "286,287,288,289",
    "286,287,288",
    "286,287",
    "286",
    "285"
]


extension PlantSearchFilterViewController: FilterUpdateDelegate{
    
    func updateSliderValue(value: [Int], sender: Any?, type: SliderType) {
        
        switch type {
        case .month:
            self.parentView!.monthTags = value
            updateMonthTags()

        case .height:
            self.parentView!.heightTags = value
            updateHeightTags()

        case .freezing:
            self.parentView!.frostTag = value[0]
        default:
            return
        }
        
        self.updateParameters()

    }
    
    func updateMonthTags(){
        
        let tagKey = "month"

        
        if self.parentView!.monthTags[0] <= 1 && self.parentView!.monthTags[1] >= 12{
            self.parentView!.tagCollectionData.removeValue(forKey: tagKey)

        }
        else{
            self.parentView!.tagCollectionData[tagKey] = (t: "Blühdauer Monat \(self.parentView!.monthTags[0]) bis \(self.parentView!.monthTags[1])", id: -2, type: .slider)

        }
        self.parentView!.searchTagCollectionView.reloadData()
    }
    

    
    func updateHeightTags(){
        
        let tagKey = "height"

        if self.parentView!.heightTags[0] <= 0 && self.parentView!.heightTags[1] >= 800{
            self.parentView!.tagCollectionData.removeValue(forKey: tagKey)

        }
        else{
            self.parentView!.tagCollectionData[tagKey] = (t: "\(self.parentView!.heightTags[0]) cm - \(self.parentView!.heightTags[1]) cm", id: -3, type: .slider)

        }
        self.parentView!.searchTagCollectionView.reloadData()
        print("collection data is", self.parentView!.tagCollectionData)
    }
    
    func updateTagsString(value: String, sender: Any?, type: FilterType, tableType: TableType) {
        
        self.parentView!.plantFamilies.AddOrRemove(value: value)
        
        let object = sender as! PlantSearchFilterSelectTableViewController
        object.outputTagsString = self.parentView!.plantFamilies
        self.updateParameters()

        
    }
    
    func updateTags(value: Int, name: String, sender: Any?, type: FilterType, tableType: TableType) {
        
        print("go to update tags", type)
        let tagKey = name+"-\(value)"

        if self.parentView!.tagCollectionData[tagKey] != nil {
            self.parentView!.tagCollectionData.removeValue(forKey: tagKey)
        }
        else{
            self.parentView!.tagCollectionData[tagKey] = (t: name, id: value, type: type)
        }
        
        self.parentView!.searchTagCollectionView.reloadData()
        
        var changedTags: [Int] = []
        switch type {
        case .cookieTag:
            self.parentView!.cookieTags.AddOrRemove(value: value)
            changedTags = self.parentView!.cookieTags
        case .colorsTag:
            self.parentView!.colorsTags.AddOrRemove(value: value)
            changedTags = self.parentView!.colorsTags
        case .excludeTag:
            self.parentView!.excludeTags.AddOrRemove(value: value)
            changedTags = self.parentView!.excludeTags
        case .leafColorsTag:
            self.parentView!.leafColorsTags.AddOrRemove(value: value)
            changedTags = self.parentView!.leafColorsTags
        case .autumnColorsTag:
            self.parentView!.autumnColorsTags.AddOrRemove(value: value)
            changedTags = self.parentView!.autumnColorsTags
        case .plantGroup:
            self.parentView!.plantGroups.AddOrRemove(value: value)
            changedTags = self.parentView!.plantGroups
            print("plant group is", self.parentView!.plantGroups)
            
        default:
            return
        }
        
        self.updateParameters()
        
        switch tableType {
        case .checkbox:
            let object = sender as! CheckBoxFilter
            object.outputTags = changedTags
        case .filterGeneral:
            let object = sender as! PlantSearchFilterSelectTableViewController
            object.outputTags = changedTags
        default:
            return
        }
        
    }
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        
        if segue.identifier == "searchFilterSelect"{
            let menuItem = menuList.first{$0.category == (sender as! String)}
            
            let searchFilterSelectView = segue.destination as! PlantSearchFilterSelectTableViewController
            if (sender as! String) == "Blütenfarben"{
                var filterColorTags = self.parentView!.tags[sender as! String]!.filter{
                    let key = $0.t
                    let value = ColorsKey[key]
                    return value != nil
                }
                
                filterColorTags = filterColorTags.sorted(by: {ColorSort[$0.t] ?? 0 < ColorSort[$1.t] ?? 0})
                searchFilterSelectView.tagSelectList = filterColorTags
                searchFilterSelectView.isColor = true

            }else{
                searchFilterSelectView.tagSelectList = self.parentView!.tags[sender as! String]!

            }
            
            
            searchFilterSelectView.filterDelegate = self
            
            searchFilterSelectView.tableView.allowsMultipleSelection = (menuItem?.ismulti ?? false)
            switch menuItem?.type {
            case .cookieTag:
                searchFilterSelectView.outputTags = self.parentView!.cookieTags
                searchFilterSelectView.filterType = FilterType.cookieTag
                
            case .colorsTag:
                searchFilterSelectView.outputTags = self.parentView!.colorsTags
                searchFilterSelectView.filterType = FilterType.colorsTag
            case .excludeTag:
                searchFilterSelectView.outputTags = self.parentView!.excludeTags
                searchFilterSelectView.filterType = FilterType.excludeTag
            case .leafColorsTag:
                searchFilterSelectView.outputTags = self.parentView!.leafColorsTags
                searchFilterSelectView.filterType = FilterType.leafColorsTag
            case .autumnColorsTag:
                searchFilterSelectView.outputTags = self.parentView!.autumnColorsTags
                searchFilterSelectView.filterType = FilterType.autumnColorsTag
            default:
                return
            }
        }
        else if segue.identifier == "plantGroup"{
            let searchFilterSelectView = segue.destination as! PlantSearchFilterSelectTableViewController

            searchFilterSelectView.tagSelectList = self.parentView!.tags["Pflanzengruppe"]!
            searchFilterSelectView.filterDelegate = self
            searchFilterSelectView.tableView.allowsMultipleSelection = false
            searchFilterSelectView.outputTags = self.parentView!.plantGroups
            searchFilterSelectView.filterType = FilterType.plantGroup
            
        }
        
        else if segue.identifier == "plantFamily"{
            let searchFilterSelectView = segue.destination as! PlantSearchFilterSelectTableViewController
            
            searchFilterSelectView.tagSelectList = self.parentView!.tags["Pflanzenfamilie"]!
            searchFilterSelectView.filterDelegate = self
            searchFilterSelectView.tableView.allowsMultipleSelection = false

            searchFilterSelectView.outputTagsString = self.parentView!.plantFamilies
            searchFilterSelectView.filterType = FilterType.plantFamily
            
        }
        
        else if segue.identifier == "plantSearchSlider"{
            let menuItem = menuList.first{$0.category == (sender as! String)}

            let searchFilterSelectView = segue.destination as! PlantSearchFilterSliderViewController
            searchFilterSelectView.filterDelegate = self
            print("menu is", menuItem?.category)
            if menuItem?.category == "Blühdauer"{
                searchFilterSelectView.isItMonth = true
                searchFilterSelectView.rangeData = self.parentView!.monthTags
                searchFilterSelectView.sliderType = SliderType.month
            }
            else if menuItem?.category == "Wuchshöhe"{
                searchFilterSelectView.isItMonth = false
                searchFilterSelectView.rangeData = self.parentView!.heightTags
                searchFilterSelectView.sliderType = SliderType.height

            }else{
                searchFilterSelectView.isItMonth = true
                searchFilterSelectView.rangeData = self.parentView!.monthTags
                searchFilterSelectView.sliderType = SliderType.freezing

            }
            
        }
    }
}

class PlantSearchFilterViewController: UIViewController {
    
    var searchParams: Parameters = [:]
    var searchDelegate: plantSearchDelegate?
    
    let ecoCategoryContainer = CheckBoxFilter()
    let filterContainer = FilterTableView()
    
    var parentView: PlantSearchViewController?
    // filter Parameters
    
//    var cookieTags: [Int] = []
//    var colorsTags: [Int] = []
//    var excludeTags: [Int] = []
//    var leafColorsTags: [Int] = []
//    var autumnColorsTags: [Int] = []
//    var plantGroups: [Int] = []
//    var plantFamilies: [String] = []
//
//    var monthTags: [Int] = []
//    var heightTags: [Int] = []
//    var frostTag: Int = 0
//
//
//    // raw params
//    var ecoTags: [(t: String, id: Int)] = []
//
//    var tags: [String: [(t: String, id: Int)]] = [:]
//
//    var categoryTags: [String: Int] = [
//        "Ausschlusskriterien": 0,
//        "Besonderheiten": 0,
//        "Blattrand": 0,
//        "Blattfarbe": 0,
//        "Blattform": 0,
//        "Blattstellung": 0,
//        "Blütenfarben": 0,
//        "Blüten": 0,
//        "Blütenform": 0,
//        "Blütengröße": 0,
//        "Blütenstand": 0,
//        "Boden": 0,
//        "Früchte": 0,
//        "Fruchtfarbe": 0,
//        "Dekoaspekte": 0,
//        "Herbstfärbung": 0,
//        "Licht": 0,
//        "Laubrythmus": 0,
//        "Düngung": 0,
//        "Wasserbedarf": 0,
//        "Vermehrung": 0,
//        "Winterhärte": 0,
//        "Schnitt": 0,
//        "Verwendung": 0,
//        "Wuchs": 0,
//        "Nutzpflanzen": 0
//    ]
//
//    var reverseCategoryTags: [Int: String] = [:]
    
    // all filter
    @IBOutlet weak var nameSearchField: UITextField!
    @IBOutlet weak var filterTableView: UITableView!
    
    @IBOutlet weak var plantGroupButton: UIButton!
    @IBOutlet weak var plantFamilyButton: UIButton!
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        self.fillParameters()
//        self.prepareParameters()
        self.configurePadding()


        // Do any additional setup after loading the view.
    }
    
    override func viewWillDisappear(_ animated: Bool) {
        super.viewWillDisappear(animated)
        print("is dissapear")
        print("before params is", self.searchParams)
        self.updateParameters()

        self.searchDelegate?.updateSearchParam(params: self.searchParams)
    }
    
    @IBAction func onStartSearch(_ sender: Any) {
        self.updateParameters()
        self.searchDelegate?.searchPlantWithParam(params: self.searchParams)
        self.navigationController?.popViewController(animated: true)
        
    }
    
    func fillFilterTableView(){
        self.filterContainer.tags = self.parentView!.tags
        self.filterContainer.filterDelegate = self
        self.filterContainer.mainView = self
        self.filterTableView.dataSource = self.filterContainer
        self.filterTableView.delegate = self.filterContainer
        self.filterTableView.reloadData()
    }
    
    func fillParameters(){
        
//        self.ecoTags = [(t: "Bienenfreundlich", id: 447),
//                        (t: "Insektenfreundlich", id: 321),
//                        (t: "Vogelfreundlich", id: 322),
//                        (t: "Schmetterlingsfreundlich", id: 531),
//                        (t: "Ökologisch wertvoll", id: 445),
//                        (t: "Heimische Pflanze", id: 530)
//        ]
        
//        self.showSpinner(onView: self.view)
//        NetworkManager().requestDataAsync(type: [searchCategoryModel].self, APP_URL.PLANT_SEARCH_GET_CATS){response in
//            if !response.success{
//                self.ShowAlert(message: response.result as! String)
//                self.removeSpinner()
//                return
//            }
//            self.fillCategoryTags(data: response.result as! [searchCategoryModel] )
//            NetworkManager().requestDataAsync(type: [badgeTagsModel].self, APP_URL.PLANT_SEARCH_GET_TAGS){response in
//                if !response.success{
//                    self.ShowAlert(message: response.result as! String)
//                    self.removeSpinner()
//                    return
//                }
//                self.tags = (response.result as! [badgeTagsModel]).reduce(into:[String: [(t: String, id: Int)]]()){output, input in
//                    if let key = self.reverseCategoryTags[input.CategoryId]{
//                        if output[key] != nil{
//                            output[key]?.append((id: input.Id, t: input.Title ?? ""))
//                        }
//                        else{
//                            output[key] = [(id: input.Id, t: input.Title ?? "")]
//                        }
//                    }
//                }
//                NetworkManager().requestDataAsync(type: [plantGroupModel].self, APP_URL.PLANT_SEARCH_GET_GROUPS){response in
//                    self.tags["Pflanzengruppe"] = []
//                    if !response.success{
//                        self.ShowAlert(message: response.result as! String)
//                        self.removeSpinner()
//                        return
//                    }
//                    self.removeSpinner()
//                    var groupTags = (response.result as! [plantGroupModel]).map{ (t: $0.Name, id: $0.Id ) }
//                    self.tags["Pflanzengruppe"] = groupTags
//                }
//                NetworkManager().requestDataAsync(type: [String].self, APP_URL.PLANT_SEARCH_GET_FAMILIES){response in
//                    self.tags["Pflanzenfamilie"] = []
//                    if !response.success{
//                        self.ShowAlert(message: response.result as! String)
//                        self.removeSpinner()
//                        return
//                    }
//                    self.removeSpinner()
//                    var familyTags = (response.result as! [String]).map{( t: $0, id: -1)}
//                    self.tags["Pflanzenfamilie"] = familyTags
//                }
//                self.tags["Ökologischen"] = self.ecoTags
                self.fillFilterTableView()
//            }
//        }
    }
    
//    func fillCategoryTags(data: [searchCategoryModel]){
//        for item in data{
//            if let val = self.parentView!.categoryTags[item.Title]{
//                self.categoryTags[item.Title] = item.Id
//                self.reverseCategoryTags[item.Id] = item.Title
//            }
//        }
//    }
    
//    func prepareParameters(){
//        print("received params is", self.searchParams)
////        nameSearchField.text = ((self.searchParams["searchText"] ?? "") as! String)
//        self.parentView!.cookieTags = ((self.searchParams["cookieTags"] ?? "") as! String).split(separator: ",").reduce(into: [Int]()){result, str in
//            result.append(Int(String(str))!)
//        }
//        self.parentView!.colorsTags = ((self.searchParams["colors"] ?? "") as! String).split(separator: ",").reduce(into: [Int]()){result, str in
//            result.append(Int(String(str))!)
//        }
//        self.parentView!.leafColorsTags = ((self.searchParams["leafColors"] ?? "") as! String).split(separator: ",").reduce(into: [Int]()){result, str in
//            result.append(Int(String(str))!)
//        }
//        self.parentView!.autumnColorsTags = ((self.searchParams["autumnColors"] ?? "") as! String).split(separator: ",").reduce(into: [Int]()){result, str in
//            result.append(Int(String(str))!)
//        }
//        self.parentView!.plantGroups = ((self.searchParams["groupId"] ?? "") as! String).split(separator: ",").reduce(into: [Int]()){result, str in
//            result.append(Int(String(str))!)
//        }
//        self.parentView!.plantFamilies = ((self.searchParams["family"] ?? "") as! String).split(separator: ",").reduce(into: [String]()){result, str in
//            result.append(String(str))
//        }
//        let minHeight = self.searchParams["selHmin"] as! Int
//        let maxHeight = self.searchParams["selHmax"] as! Int
//        let minMonth = self.searchParams["selMinMonth"] as! Int
//        let maxMonth = self.searchParams["selMaxMonth"] as! Int
//        self.parentView!.heightTags = [minHeight, maxHeight]
//        self.parentView!.monthTags = [minMonth, maxMonth]
//
//        self.parentView!.frostTag = freezingId.firstIndex(of: (self.searchParams["freezes"] ?? "") as! String) ?? 1
//        print(self.searchParams["freezes"])
//        print(self.monthTags, self.heightTags, self.frostTag)
//
//    }
    
    func updateParameters(){
        print("cookie before is", self.parentView!.cookieTags)
        // Name Search
        self.searchParams["searchText"] = nameSearchField.text
        self.searchParams["cookieTags"] = self.parentView!.cookieTags.map { String($0) }.joined(separator: ",")
        self.searchParams["colors"] = self.parentView!.colorsTags.map { String($0) }.joined(separator: ",")
        self.searchParams["leafColors"] = self.parentView!.leafColorsTags.map { String($0) }.joined(separator: ",")
        self.searchParams["autumnColors"] = self.parentView!.autumnColorsTags.map { String($0) }.joined(separator: ",")
        self.searchParams["groupId"] = self.parentView!.plantGroups.map { String($0) }.joined(separator: ",")
        self.searchParams["family"] = self.parentView!.plantFamilies.map { $0 }.joined(separator: ",")
        self.searchParams["selHmin"] = self.parentView!.heightTags[0]
        self.searchParams["selHmax"] = self.parentView!.heightTags[1]
        self.searchParams["selMinMonth"] = self.parentView!.monthTags[0]
        self.searchParams["selMaxMonth"] = self.parentView!.monthTags[1]
        print("params is", searchParams)
        
    }
}




class CheckBoxFilter: NSObject, UITableViewDelegate, UITableViewDataSource{
    
    var rawTags: [(t: String, id: Int)] = []
    var filterDelegate: FilterUpdateDelegate?
    var outputTags: [Int] = []
    var filterType: FilterType?
    
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return rawTags.count
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: "ecoCategoryCell", for: indexPath)
        cell.textLabel?.text = rawTags[indexPath.row].t
        
        if self.outputTags.contains(rawTags[indexPath.row].id)
        {
            tableView.selectRow(at: indexPath, animated: true, scrollPosition: .none)
        }
        
        return cell
    }
    
    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        self.filterDelegate?.updateTags(value: rawTags[indexPath.row].id,  name: rawTags[indexPath.row].t, sender: self, type: filterType!, tableType: TableType.checkbox)
    }
    
    func tableView(_ tableView: UITableView, didDeselectRowAt indexPath: IndexPath) {
        self.filterDelegate?.updateTags(value: rawTags[indexPath.row].id, name: rawTags[indexPath.row].t, sender: self, type: filterType!, tableType: TableType.checkbox)
        
    }
}

class FilterTableView: NSObject, UITableViewDelegate, UITableViewDataSource{
    
    var filterDelegate: FilterUpdateDelegate?
    var mainView: UIViewController?
    
    var tags: [String: [(t: String, id: Int)]] = [:]
    
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        //        return tags.keys.count
        return menuList.count
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: "filterCell", for: indexPath) as! FilterCategoryListViewCell
        
        cell.onConfigure(title: menuList[indexPath.row].name, imageName: menuList[indexPath.row].name)
        
//        cell.textLabel?.text = menuList[indexPath.row].name
        cell.selectionStyle = .none
        return cell
    }
    
    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {

        if menuList[indexPath.row].type == .slider{
            self.mainView?.performSegue(withIdentifier: "plantSearchSlider", sender: menuList[indexPath.row].category)
            return
        }
        
        self.mainView?.performSegue(withIdentifier: "searchFilterSelect", sender: menuList[indexPath.row].category)
    }
    
    
}

class PlantSearchFilterSelectTableViewController: UITableViewController{
    
    var tagSelectList: [(t: String, id: Int)] = []
    var filterDelegate: FilterUpdateDelegate?
    
    var outputTags: [Int] = []
    var outputTagsString: [String] = []
    var filterType: FilterType?
    var isColor: Bool = false
    var isEcoCat: Bool = false
    var isBackAuto: Bool = false
    
    override func viewDidLoad() {
        super.viewDidLoad()
        self.configurePadding()

    }
    
    
    override func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return tagSelectList.count
    }
    
    override func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: "filterSelectCell", for: indexPath) as! FilterSelectTableViewCell
        cell.isColor = self.isColor
        cell.isEcoCat = self.isEcoCat
        cell.onConfigure(text: self.tagSelectList[indexPath.row].t.removeItalicTag())
//        cell.filterNameLabel.text = self.tagSelectList[indexPath.row].t.removeItalicTag()
       
        
        switch filterType {
        case .plantFamily:
            if self.outputTagsString.contains(self.tagSelectList[indexPath.row].t)
            {
                tableView.selectRow(at: indexPath, animated: true, scrollPosition: .none)
            }
        default:
            if self.outputTags.contains(self.tagSelectList[indexPath.row].id)
            {
                tableView.selectRow(at: indexPath, animated: true, scrollPosition: .none)
            }
        }
        
        
        return cell
    }
    
    override func tableView(_ tableView: UITableView, willSelectRowAt indexPath: IndexPath) -> IndexPath? {

        
        
        print("selected at", self.tagSelectList[indexPath.row])


        switch filterType {
        case .plantFamily:
            self.updateMainTags(value: self.tagSelectList[indexPath.row].t, name: self.tagSelectList[indexPath.row].t)

        default:
            self.updateMainTags(value: self.tagSelectList[indexPath.row].id, name: self.tagSelectList[indexPath.row].t)

        }
        
        if let selectedIndex = self.tableView.indexPathsForSelectedRows{
            if selectedIndex.contains(indexPath){
                print("allready selected")
                self.tableView.deselectRow(at: indexPath, animated: false)
                return nil
            }
        }
        
    
        return indexPath
    }
    
    
    override func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        if isBackAuto{
            self.navigationController?.popViewController(animated: true)
        }
    }
    
    
    override func tableView(_ tableView: UITableView, didDeselectRowAt indexPath: IndexPath) {
//        self.filterDelegate?.updateTags(value: self.tagSelectList[indexPath.row].id, sender: self, type: filterType!, tableType: TableType.filterGeneral)
        print("deselected at", self.tagSelectList[indexPath.row])
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
             self.filterDelegate?.updateTagsString(value: value as! String, sender: self, type: filterType!, tableType: TableType.filterGeneral)
        default:
             self.filterDelegate?.updateTags(value: value as! Int, name: name, sender: self, type: filterType!, tableType: TableType.filterGeneral)
        }
       
    }
    
}

