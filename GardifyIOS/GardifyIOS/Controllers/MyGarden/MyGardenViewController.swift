//
//  MyGardenViewController.swift
//  GardifyIOS
//
//  Created by Netzlab on 14.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

enum MyGardenPage {
    case plant
    case ecoCriteria
    case device
}

class MyGardenViewController: UIViewController {

    @IBOutlet weak var myGardenTableView: FullTableView!
    @IBOutlet weak var FilterDropdownTableView: UITableView!
    @IBOutlet weak var plantsButton: UIButton!
    @IBOutlet weak var plantsButtonView: UIView!
    @IBOutlet weak var plantsButtonAddition: UIButton!
    @IBOutlet weak var additionButton: UIButton!
    @IBOutlet weak var additionButtonView: UIView!
    
    @IBOutlet weak var ecoCriteriaButton: UIButton!
    @IBOutlet weak var listenButton: UIButton!
    @IBOutlet weak var listenButtonView: UIView!
    
    @IBOutlet weak var oekoscanButton: UIButton!
    @IBOutlet weak var ecoCategoryCollectionView: UICollectionView!
    @IBOutlet weak var dropdownMainView: UIView!
    @IBOutlet weak var dropdownItemView: UIView!
    @IBOutlet weak var dropdownHeight: NSLayoutConstraint!
    
    @IBOutlet weak var ecoFilterCollectionHeight: NSLayoutConstraint!
    @IBOutlet weak var ecoFilterButtonExtend: UIButton!
    
    
    @IBOutlet weak var dropdownTitleImageView: UIImageView!
    @IBOutlet weak var selectedDropdownLabel: UILabel!
    @IBOutlet weak var pflanzenTitle: UILabel!
    
    
    @IBOutlet weak var plantDropdownImage: UIImageView!
    @IBOutlet weak var ecoElementDropdownImage: UIImageView!
    @IBOutlet weak var deviceDropdownImage: UIImageView!
    @IBOutlet weak var listenDropdownImage: UIImageView!
    
    var myGardenData: [MyGardenModel] = []
    var gardenMainData: MyGardenLightModel?
    var userListData: [UserPlantModel] = []
    
    var myGardenDataFiltered: [MyGardenModel] = []
    
    var ecoElementData: [MyGardenEcoListModel] = []
    var deviceListData: [MyGardenDeviceListModel] = []
    var deviceListImages: [UIImage] = []
    
    var plantImageList: [UIImage] = []
    var ecoFilterList: [Int: String] = [:]
    var extraEcoFilterList: [Int: String] = [:]
    var dropdownFilterList: [Int: String] = [:]
    
    var ecoFilterNumber: [String: Int] = [:]
    var ecoFilterContent: [String: [MyGardenModel]] = [:]
    var filterKey: [String] = []
    var listFilter: Int = -1
    var pageType: MyGardenPage = .plant
    var dropdownHeightCalc: CGFloat = 100
    
    
    var filterDropdownIsOpen: Bool = false
    var selectedDropdown : String = "Pflanzen filtern"
    var tempSelectedDropdown: [String] = []
    
    var ecoFilterIsExtended : Bool = false
    
    @IBOutlet weak var plantDropdownView: UIView!
    @IBOutlet weak var plantDropMainView: UIView!
    @IBOutlet weak var plantDropdownHeight: NSLayoutConstraint!
    
    @IBOutlet weak var listenDropdownHeight: NSLayoutConstraint!
    @IBOutlet weak var dropdownAddPlantView: UIView!
    @IBOutlet weak var listenDropdownItemView: UIView!
    @IBOutlet weak var listenUserListTableView: FullTableView!
    
    @IBOutlet weak var deviceDropdownHeight: NSLayoutConstraint!
    @IBOutlet weak var deviceDropdownTableView: UITableView!
    
    @IBOutlet weak var deviceDropdownView: UIView!
    
    @IBOutlet weak var ecoElementMainDropdownView: UIView!
    
    @IBOutlet weak var ecoElementInnerHeight: NSLayoutConstraint!
    @IBOutlet weak var ecoElementOuterHeight: NSLayoutConstraint!
    
    @IBOutlet weak var ecoElementCollectionView: UICollectionView!
    @IBOutlet weak var ecoElementDescLabel: UILabel!
    
    @IBOutlet weak var gridModeImage: UIImageView!
    
    @IBOutlet weak var cardModeImage: UIImageView!
    
    var isPlantDropdown: Bool = false
    var isListenDropdown: Bool = false
    var isDeviceDropdown: Bool = false
    var isEcoElementDropdown: Bool = false
    var isGridViewModel: Bool = true
    
    override func viewDidLoad() {
        super.viewDidLoad()
        self.updateNavigationBar(isMain: false, "MEIN", "GARTEN", "main_myGardenNormal")
        self.myGardenTableView.reloadData()
        pageConfiguration()
        ecoDropdownPageConfiguration()
        updateDropdown()
        updateListenDropdown()
        updateDeviceDropdown()
        updateEcoElementDropdown()
        self.configurePadding()
        
        self.gridModeImage.tintColor = .systemGray
        self.cardModeImage.tintColor = .systemGray3

         self.showSpinner(onView: self.view)

        DispatchQueue.global(qos: .background).async {
            self.getMyGardenData()

        }
        // Do any additional setup after loading the view.
    }
    
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(false)
        

    }
    

    @IBAction func okoscanGesture(_ sender: Any) {
        self.ShowTitleAlert(title: "Ökoscan", message: "Mit deinem persönlichen Ökoscan bekommst du ein Gefühl dafür, wie ökologisch dein Garten ist und in welchem Bereich du noch etwas für die Natur tun kannst. Hierbei spielen die Verwendung deiner Außenfläche, die Auswahl und Blühdauer der Pflanzen sowie die Anzahl der Öko-Elemente eine Rolle.")
    }
    
    @IBAction func ecoCategoryClick(_ sender: Any) {
        
    }
    
    @IBAction func onEcoToggle(_ sender: Any) {
        print("toggle is clicked 2")
        ecoFilterIsExtended = !ecoFilterIsExtended
        updateEcoFilterCollectionViewLayout()
    }
    
    @IBAction func onEcoFilterToggle(_ sender: Any) {
        print("toggle is clicked")
        ecoFilterIsExtended = !ecoFilterIsExtended
        updateEcoFilterCollectionViewLayout()

    }
    
    func updateEcoFilterCollectionViewLayout(){
        
//        self.ecoFilterCollectionHeight.constant = (ecoFilterIsExtended ? 325 : 115)
//
//        self.ecoFilterButtonExtend.setTitle(ecoFilterIsExtended ? "Weniger Suchkriterien ..." : "Mehr Suchkriterien ...", for: .normal)
//        UIView.animate(withDuration: 0.5, animations: {
//            self.view.layoutIfNeeded()
//        })
//        
        self.ecoCategoryCollectionView.reloadData()
    }
    
    func pageConfiguration(){
        
        getPlantNumber()
        self.applyTheme()
        self.FilterDropdownTableView.backgroundColor = .clear
        self.myGardenTableView.backgroundColor = .clear
//        self.plantsButtonView.setWhiteButtonView()
//        self.plantsButton.setWhiteButton()
//        self.plantsButtonView.alpha = 0.2
//        self.plantsButton.setTitleColor(.label, for: .normal)
//        self.plantsButton.contentHorizontalAlignment = .left
//        self.plantsButton.titleLabel?.font = UIFont.systemFont(ofSize: 17, weight: .bold)
//                self.additionButtonView.alpha = 0.2

        self.additionButton.setTitleColor(.label, for: .normal)
        self.additionButtonView.setWhiteButtonView()
        self.additionButton.contentHorizontalAlignment = .left
//        self.ecoCriteriaButton.setClearWhiteButton()
        self.ecoCriteriaButton.contentHorizontalAlignment = .left
        
//        self.ecoFilterButtonExtend.setTitleColor(.label, for: .normal)

        self.listenButtonView.setWhiteButtonView()
        self.listenButton.setTitleColor(.label, for: .normal)
        self.listenButton.contentHorizontalAlignment = .left
        
//        self.oekoscanButton.setGreenButton()
//        self.oekoscanButton.titleLabel?.numberOfLines = 2
//        self.oekoscanButton.setTitle("GARDIFY\nÖKO-SCAN ", for: .normal)
        configurePlantDropdown()
        
        self.dropdownMainView.addBorderRadius()
        self.dropdownItemView.addBorderRadius()
    }
    
    func ecoDropdownPageConfiguration(){
        self.ecoElementMainDropdownView.setWhiteButtonView()
    }
    
    
    func configurePlantDropdown(){
        self.plantDropdownHeight.constant = 240
        
        self.plantDropMainView.addBorderRadius()
        self.plantDropdownView.addBorderRadius()
        
        self.plantDropdownView.backgroundColor = rgb(232, 239, 238)
        
    }
    
    
    
    @IBAction func onPlantDropdownClick(_ sender: Any) {
        isPlantDropdown = !isPlantDropdown
        
        updateDropdown()
    }

    
    @IBAction func toggleDropdown(_ sender: Any) {
        setToggleDropdown()
    }
    
    
    @IBAction func onplantCategoryInfoButtonClicked(_ sender: Any) {
        ShowAlert(message: "Die Zahlen in der Klammer stehen für \"Menge der Arten\"/\"Menge der Einzelpflanzen\"")
    }
    
    func setToggleDropdown(){
        filterDropdownIsOpen = !filterDropdownIsOpen
        //        filterDropdownIsOpen = false
        updateDropdown()
    }
    
    func configureListenDropdown(){
        if isDeviceDropdown{
            
        }else{
            
        }
    }
    
    func updateEcoElementDropdown(){
        if isEcoElementDropdown{
            
            getEcoElementList()
            
            ecoElementOuterHeight.constant = 440
            ecoElementInnerHeight.constant = 130
            
            ecoElementDescLabel.alpha = 1
            ecoElementDropdownImage.flipXAxis()
        }else{
            ecoElementOuterHeight.constant = 40
            ecoElementInnerHeight.constant = 40
            ecoElementDescLabel.alpha = 0
            ecoElementDropdownImage.revertFlip()
        }
    }
    
    func updateListenDropdown(){
        if isListenDropdown{
            listenDropdownHeight.constant = CGFloat(90 + (self.userListData.count * 40))
            listenDropdownItemView.alpha = 1
            listenDropdownImage.flipXAxis()
            self.listenUserListTableView.reloadData()
        }
        else{
            listenDropdownHeight.constant = 40
            listenDropdownItemView.alpha = 0
            listenDropdownImage.revertFlip()

        }
    }
    
    func updateDeviceDropdown(){
        if isDeviceDropdown{
            deviceDropdownImage.flipXAxis()
            getDeviceList()

        }
        else
        {
            deviceDropdownHeight.constant = 40
            deviceDropdownView.alpha = 0
            deviceDropdownImage.revertFlip()
        }
    }
    
    func extendDeviceDropdown(){
        deviceDropdownHeight.constant = CGFloat(90 + 127 * self.deviceListData.count)
        
        deviceDropdownView.alpha = 1

    }
    
    func updateDropdown(){
        
        if filterDropdownIsOpen{
            dropdownHeight.constant = dropdownHeightCalc
        }
        else{
            dropdownHeight.constant = 10
        }
        
        var alphaValue = 0
        if isPlantDropdown{
            self.plantDropdownHeight.constant = 200 + (filterDropdownIsOpen ? dropdownHeightCalc : 20)

            alphaValue = 1
            plantDropdownImage.flipXAxis()
        }
        else{
            self.plantDropdownHeight.constant = 40
            
            plantDropdownImage.revertFlip()
        }
        

        UIView.animate(withDuration: 0.5, animations: {
            self.dropdownItemView.alpha = CGFloat(alphaValue)
            self.dropdownMainView.alpha = CGFloat(alphaValue)
            self.dropdownAddPlantView.alpha = CGFloat(alphaValue)
            self.ecoCategoryCollectionView.alpha = CGFloat(alphaValue)
            self.view.layoutIfNeeded()
        })
    }
    
    func getPlantNumber(){
        
        updatePlantCount(){result in
            if !result{
                return
            }
            self.pflanzenTitle.text = "Pflanzen (\(getPlantCount()))"
            
        }
    }
    
    func configureEcoFilter(){

        
        ecoFilterList = ecoCategoryKeys.reduce(into: [Int: String]()){ output, input in
            if !output.values.contains(input.value)
            {
                output[input.key] = input.value
            }
        }
        
        configureDropdownFilter()
        
        extraEcoFilterList = filterCategoryKeys.reduce(into: [Int: String]()){ output, input in
            if !output.values.contains(input.value)
            {
                output[input.key] = input.value
            }
        }
        
        ecoFilterNumber = ecoFilterList.reduce(into: [String: Int]()){ output, input in
            output[input.value] = 0
        }
        
        ecoFilterContent = dropdownFilterList.reduce(into: [String: [MyGardenModel]]()){ output, input in
            output[input.value] = []
        }
        
        
        countEcoFilter()
        self.ecoCategoryCollectionView.backgroundColor = .clear
        self.ecoCategoryCollectionView.reloadData()
    }
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        if segue.identifier == "ecoElement"{
            let controller = segue.destination as! MyGardenEcoElementDetailViewController
           
            controller.ecoPageId = self.ecoElementData[(sender as! Int)].Name
        }
        
        if segue.identifier == "addGarden"{
            let controller =  segue.destination as! MyGardenAddNewGardenViewController
            controller.gardenId = self.userListData[0].GardenId
            controller.gardenDelegate = self
        }
        
    }
    
    func configureDropdownFilter(){
        dropdownFilterList = [:]
        
        for ecoList in ecoFilterList{
            dropdownFilterList[ecoList.key] = ecoList.value
        }
        
        for dropList in filterCategoryKeys{
            if !dropdownFilterList.values.contains(dropList.value)
            {
                dropdownFilterList[dropList.key] = dropList.value
            }
        }
        
        self.FilterDropdownTableView.reloadData()
        
        
    }
    
    func countEcoFilter(){
        
        for plant in self.myGardenData{
            
            let badges = plant.UserPlant.Badges
            
            if (badges?.count ?? 0) < 1{
                continue
            }
            
            for badge in badges!{
                if let exist = ecoCategoryKeys[badge.Id]{
                    ecoFilterNumber[exist] = ecoFilterNumber[exist]! + plant.UserPlant.Count
                    ecoFilterContent[exist]?.append(plant)
                }
                if let exist = filterCategoryKeys[badge.Id]{
                    ecoFilterContent[exist]?.append(plant)
                }
            }
        }
    }
    
    
    func getMyGardenData(){
       
        NetworkManager().requestDataAsync(type: [MyGardenModel].self, APP_URL.MY_GARDEN_USER_PLANTS, printRequest: true){response in
            
            if !response.success{
                
                self.ShowAlert(message: response.result as! String)
                self.removeSpinner()
                return
            }
            self.removeSpinner()
            self.myGardenData = (response.result as! [MyGardenModel]).sorted{$0.UserPlant.NameLatin < $1.UserPlant.NameLatin}
            
            DispatchQueue.main.async {
                self.filterGardenData()
                
                
                self.myGardenTableView.reloadData()
                self.configureEcoFilter()

                self.getAllImages()
            }
            
        }
        
        self.loadUserList()
        
        
    }
    
    func resetGardenFilterData(){
        filterKey = []
        listFilter = -1
    }
    
    func loadUserList(_ useSpinner: Bool = false){
            
            NetworkManager().requestDataAsync(type: [UserPlantModel].self, APP_URL.USER_LIST){response in
                
                if !response.success{
                    
                    self.ShowAlert(message: response.result as! String)
                    if useSpinner{
                        self.removeSpinner()
                    }
    //
                    return
                }
                if useSpinner{
                    self.removeSpinner()
                }
    //            self.removeSpinner()
                self.userListData = response.result as! [UserPlantModel]
                print("user list data is", self.userListData)
                
                DispatchQueue.main.async {
                    self.listenUserListTableView.reloadData()

                }

            }
        }
    
    func filterGardenData(){
        
        var currentGardenData = self.myGardenData
        
        if listFilter != -1{
            currentGardenData = currentGardenData.filter({$0.ListIds[0] == listFilter})
            
        }
        
        if filterKey.count < 1{
            self.myGardenDataFiltered = currentGardenData
            return
        }
        
        var hasKey = false
        var plantCollection: [MyGardenModel] = []
        for key in filterKey{
            
            if let filterValue = self.ecoFilterContent[key] {
                plantCollection += filterValue
                hasKey = true
            }
        }
        
        if !hasKey{
            if listFilter != -1{
    
                self.myGardenDataFiltered = currentGardenData.filter({$0.ListId == listFilter})
                return

            }
            self.myGardenDataFiltered = currentGardenData
            return
        }
        
        plantCollection = Array(Set(plantCollection))
        
        if listFilter != -1{
            plantCollection = plantCollection.filter({$0.ListId == listFilter})

        }
//        if plantCollection.count < 1{
//            self.myGardenDataFiltered = self.myGardenData
//            return
//        }
        
        self.myGardenDataFiltered = plantCollection
//        guard let filterValue = self.ecoFilterContent[filterKey] else {
//            self.myGardenDataFiltered = self.myGardenData
//            return
//
//        }
//        self.myGardenDataFiltered = filterValue
        

    }
    
    
    func getAllImages(){
        
        let imageUrlList = self.myGardenDataFiltered.reduce(into: [String]()){output, input in
            var raw = input.UserPlant.Images![0].SrcAttr
            output.append((raw?.toURL(false, false))!)
        }
        
        
        getImageFiles(files: imageUrlList){images in
            self.plantImageList = images
            self.myGardenTableView.reloadData()
            
        }
        
    }
    
    func getAllDeviceImages(){
        
        let imageUrlList = self.deviceListData.reduce(into: [String]()){output, input in
            var raw = input.EntryImages?[0].SrcAttr
            output.append((raw?.toURL(false, false))!)
        }
        
        
        getImageFiles(files: imageUrlList){images in
            self.deviceListImages = images
            self.myGardenTableView.reloadData()
            
        }
        
    }
    
    func getEcoElementList(){
        
        self.showSpinner(onView: self.view)
        DispatchQueue.global(qos: .background).async {
            NetworkManager().requestDataAsync(type: [MyGardenEcoListModel].self, APP_URL.USER_GARDEN_ECO_ELEMENT_LIST, printRequest: true){response in
                if !response.success{
                    
                    self.ShowAlert(message: response.result as! String)
                    self.removeSpinner()
                    return
                }
                self.removeSpinner()
                
                self.ecoElementData = response.result as! [MyGardenEcoListModel]
                DispatchQueue.main.async {
                    self.ecoElementCollectionView.reloadData()

                }
    //            self.myGardenTableView.reloadData()
                
            }
        }
        
    }
    
    func getDeviceList(){
        
        self.showSpinner(onView: self.view)
        
        DispatchQueue.global(qos: .background).async {
            NetworkManager().requestDataAsync(type: [MyGardenDeviceListModel].self, APP_URL.DEVICE_USER_LIST){response in
                if !response.success{
                    
                    self.ShowAlert(message: response.result as! String)
                    self.removeSpinner()
                    return
                }
                self.removeSpinner()
                
                self.deviceListData = response.result as! [MyGardenDeviceListModel]
                DispatchQueue.main.async {
                    self.extendDeviceDropdown()
    //                self.myGardenTableView.reloadData()
                    self.deviceDropdownTableView.reloadData()
                }

                
            }
        }
        
    }
    
    
    @IBAction func onMyPlantClicked(_ sender: Any) {
        isPlantDropdown = true
        
        updateDropdown()
    }
    
    @IBAction func onDeviceButtonClicked(_ sender: Any) {
        
        isDeviceDropdown = true
        
        updateDeviceDropdown()
    }
    
    
    @IBAction func onWarningClicked(_ sender: Any) {
        let tabBar = self.tabBarController as! MainTabBarController
        
        tabBar.navigateToOtherPage(pageId: .warningPage)
    }
    
    @IBAction func goToEcoScan(_ sender: Any) {
        
        self.goToController(storyBoard: "EcoScan", controllerName: "ecoScanView", type: EcoScanViewController.self)
    }
    
    
    @IBAction func onPlantButtonClick(_ sender: Any) {
        pageType = .plant
        self.listFilter = -1
        self.filterGardenData()
 self.getAllImages()
        self.myGardenTableView.reloadData()
    }
    
    @IBAction func onEcoCriteriaClick(_ sender: Any) {
        isEcoElementDropdown = !isEcoElementDropdown
        updateEcoElementDropdown()
//        pageType = .ecoCriteria
//            self.getEcoElementList()
    
    }
    
    @IBAction func onAdditionDeviceClicked(_ sender: Any) {
        isDeviceDropdown = !isDeviceDropdown
        
        updateDeviceDropdown()
//        pageType = .device
//
//            self.getDeviceList()
 
    }
    
    @IBAction func onPlantAdditionPressed(_ sender: Any) {
        
        showAddNewPlantOption()
    }
    
    @IBAction func onPlantAdditionFromDropdown(_ sender: Any) {
    
        showAddNewPlantOption()
    }
    func showAddNewPlantOption(){
        let selectViewController = (storyboard?.instantiateViewController(identifier: "selectOption"))! as MyGardenSelectOptionViewController
        
        selectViewController.modalPresentationStyle = .overCurrentContext
        selectViewController.selectDelegate = self
        selectViewController.isPlants = true
        selectViewController.mainTabBar = self.tabBarController as! MainTabBarController
        present(selectViewController, animated: true)
    }
    

    
    @IBAction func onDeviceAddPressed(_ sender: Any) {
        self.goToDeviceList()
    }
    
    @IBAction func onGridPressed(_ sender: Any) {
        isGridViewModel = true
        self.gridModeImage.tintColor = .systemGray
        self.cardModeImage.tintColor = .systemGray3
        self.myGardenTableView.reloadData()
    }
    
    @IBAction func onCardPressed(_ sender: Any) {
        isGridViewModel = false
        self.gridModeImage.tintColor = .systemGray3
        self.cardModeImage.tintColor = .systemGray
        self.myGardenTableView.reloadData()

    }
    
    
    @IBAction func onAdditionDevicePressed(_ sender: Any) {
        isDeviceDropdown = !isDeviceDropdown
        
        updateDeviceDropdown()
//        let selectViewController = (storyboard?.instantiateViewController(identifier: "selectOption"))! as MyGardenSelectOptionViewController
//
//        selectViewController.modalPresentationStyle = .overCurrentContext
//        selectViewController.selectDelegate = self
//        selectViewController.isPlants = false
//
//        selectViewController.mainTabBar = self.tabBarController as! MainTabBarController
//        present(selectViewController, animated: true)
    }
    
    
    
    @IBAction func listenButtonPressed(_ sender: Any) {
        isListenDropdown = !isListenDropdown
        
        updateListenDropdown()
//        let selectViewController = (storyboard?.instantiateViewController(identifier: "MyGardenListen"))! as MyGardenListenViewController
//
//        selectViewController.modalPresentationStyle = .overCurrentContext
//        selectViewController.selectDelegate = self
//
//        selectViewController.userListData = self.userListData
////        selectViewController.isPlants = false
//
//        selectViewController.mainTabBar = self.tabBarController as! MainTabBarController
//        present(selectViewController, animated: true)
    }
    
}


extension MyGardenViewController: UITableViewDataSource, UITableViewDelegate{
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        if tableView == deviceDropdownTableView{
            return self.deviceListData.count

        }
        if tableView == listenUserListTableView{
            return self.userListData.count
        }
        else if tableView == FilterDropdownTableView{
            tempSelectedDropdown = textDropdownDispList.filter({$0 != selectedDropdown})
            dropdownHeightCalc = CGFloat(tempSelectedDropdown.count * 40 + 60)
            return tempSelectedDropdown.count
        }
        
        if isGridViewModel{
            return (self.myGardenDataFiltered.count / 3 + (self.myGardenDataFiltered.count % 3 == 0 ? 0 : 1))
        }
        
        switch pageType {
        case .plant:
            return myGardenDataFiltered.count

        case .ecoCriteria:
            return ecoElementData.count
        case .device:
            return deviceListData.count
        default:
            return 0
        }
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {

        if tableView == deviceDropdownTableView{
            let cell = tableView.dequeueReusableCell(withIdentifier: "deviceCell", for: indexPath) as! MyGardenDeviceDropdownTableViewCell
            
            cell.onConfigure(data: self.deviceListData[indexPath.row])
            cell.viewController = self
            return cell
        }
        if tableView == listenUserListTableView{
            let cell = tableView.dequeueReusableCell(withIdentifier: "listenCell", for: indexPath) as! ListenListTableViewCell
            
            cell.selectDelegate = self
            cell.gardenId = self.userListData[indexPath.row].GardenId
            cell.gardenData = self.userListData[indexPath.row]
            cell.viewController = self
            cell.titleLabel.text = self.userListData[indexPath.row].Name
            return cell
        }
        else if tableView == FilterDropdownTableView{
            let name = tempSelectedDropdown[indexPath.row]
        
            let cell = tableView.dequeueReusableCell(withIdentifier: "dropdownFilterCell", for: indexPath) as! FilterDropdownTableViewCell
            cell.clearBackground()
            
            cell.filterLabel.textColor = .black
            if name == "Pflanzen filtern"{
                cell.filterLabel.textColor = .red
            }
            cell.filterLabel.text =  name != "Pflanzen filtern" ? name : "Filter zurücksetzen"
            cell.filterImage.image = getEcoImageString(key: textDropdownKeysList[name]!)
            
            return cell
        }
        
        
        
        if isGridViewModel{
            let cell = tableView.dequeueReusableCell(withIdentifier: "plantGridCell", for: indexPath) as! MyGardenGridTableViewCell
            
            cell.viewController = self
            cell.clearBackground()
            let currentIndex = indexPath.row * 3
            
            cell.plantIdList = []
            
            if self.plantImageList.count < (myGardenDataFiltered.count){
                return cell
            }
            
            if (currentIndex + 1) <= self.plantImageList.count && (currentIndex + 1) <= self.myGardenDataFiltered.count
            {
                cell.plantIdList.append(self.myGardenDataFiltered[currentIndex].UserPlant.PlantId)
                cell.gridImage1.image = self.plantImageList[currentIndex]
            }
            
            if (currentIndex + 2) <= self.plantImageList.count && (currentIndex + 2) <= self.myGardenDataFiltered.count
            {
                cell.plantIdList.append(self.myGardenDataFiltered[currentIndex + 1].UserPlant.PlantId)
                cell.gridImage2.image = self.plantImageList[currentIndex + 1]
            }
            
            if (currentIndex + 3) <= self.plantImageList.count && (currentIndex + 3) <= self.myGardenDataFiltered.count
            {
                cell.plantIdList.append(self.myGardenDataFiltered[currentIndex + 2].UserPlant.PlantId)
                cell.gridImage3.image = self.plantImageList[currentIndex + 2]
            }
            
            return cell
            
        }
        
        switch pageType {
        case .plant:
            let cell = tableView.dequeueReusableCell(withIdentifier: "myGardenCell", for: indexPath) as! MyGardenTableViewCell
            cell.viewController = self
            cell.onConfigure(plant: myGardenDataFiltered[indexPath.row])
            
            if self.plantImageList.count >= (myGardenDataFiltered.count){
                cell.gardenPlantImage.image = self.plantImageList[indexPath.row]
            }
            
            return cell
            
        case .ecoCriteria:
            let cell = tableView.dequeueReusableCell(withIdentifier: "ecoCategoryCell", for: indexPath) as! MyGardenEcoListTableViewCell
            
            cell.onConfigure(data: self.ecoElementData[indexPath.row], id: indexPath.row)
         
            
            return cell
            
        case .device:
            let cell = tableView.dequeueReusableCell(withIdentifier: "deviceCell", for: indexPath) as! MyGardenDeviceListTableViewCell
            
            cell.onConfigure(data: self.deviceListData[indexPath.row])
            cell.viewController = self
            return cell
        default:
            return UITableViewCell()
        }
    }

    
    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        
        if tableView == FilterDropdownTableView{
            selectedDropdown = tempSelectedDropdown[indexPath.row]
            dropdownTitleImageView.image = getEcoImageString(key: textDropdownKeysList[selectedDropdown]!)
            selectedDropdownLabel.text = selectedDropdown
            filterKey = [textDropdownKeysList[selectedDropdown]!]
            
            self.filterGardenData()
             self.getAllImages()
        self.myGardenTableView.reloadData()
//        self.ecoCategoryCollectionView.reloadData()
            setToggleDropdown();            FilterDropdownTableView.reloadData()
        }
        

        
    }
    
    
}

extension MyGardenViewController: UICollectionViewDelegate, UICollectionViewDataSource, UICollectionViewDelegateFlowLayout{
    
    func numberOfSections(in collectionView: UICollectionView) -> Int {
        
        
        return 1
//        return ecoFilterIsExtended ? 2 : 1
    }
    
    func collectionView(_ collectionView: UICollectionView, numberOfItemsInSection section: Int) -> Int {
        if collectionView == ecoElementCollectionView{
            return self.ecoElementData.count
        }
        
        if section == 0{
            return ecoFilterList.count
        }
        else{
            return extraEcoFilterList.count
            
        }
        return ecoFilterList.count + extraEcoFilterList.count
    }
    
    func collectionView(_ collectionView: UICollectionView, cellForItemAt indexPath: IndexPath) -> UICollectionViewCell {
        
        if collectionView == ecoElementCollectionView{
            let cell = collectionView.dequeueReusableCell(withReuseIdentifier: "ecoElementCell", for: indexPath) as! MyGardenEcoElementDropdownCollectionViewCell
            cell.tag = indexPath.row
            cell.parent = self
            cell.onConfigure(data: self.ecoElementData[indexPath.row], id: indexPath.row)
            
            return cell
        }
        
        if indexPath.section == 0{
            let cell = collectionView.dequeueReusableCell(withReuseIdentifier: "ecoFilterGardenCell", for: indexPath) as! EcoFilterCollectionViewCell
            
            let key = ecoFilterList[Array(ecoFilterList.keys)[indexPath.row]]!
          
            cell.ecoCountLabel.text = "\(ecoFilterNumber[key]!)"
            cell.ecoFilterImage.image = UIImage(named: key)
            cell.ecoFilterSelectedView.addBorderRadius()
            cell.viewController = self
            cell.setGesture(identifier: key)
            cell.ecoFilterSelectedView.backgroundColor = .clear
            
//            if filterKey.contains(key){
//                cell.ecoFilterSelectedView.backgroundColor = .systemBackground
//            }
            
            return cell
        }
        else{
            let cell = collectionView.dequeueReusableCell(withReuseIdentifier: "extraEcoFilterGardenCell", for: indexPath) as! ecoFilterAdditionCollectionViewCell
            let key = extraEcoFilterList[Array(extraEcoFilterList.keys)[indexPath.row]]!
            cell.ecoFilterSelectView.addBorderRadius()
            cell.filterImage.image = UIImage(named: key)
            cell.ecoFilterSelectView.backgroundColor = .clear
            cell.additionFilterLabel.text = key
//            if filterKey.contains(key){
//                cell.ecoFilterSelectView.backgroundColor = .systemBackground
//            }
            
            return cell
        }
        
    }
    
    func collectionView(_ collectionView: UICollectionView, layout collectionViewLayout: UICollectionViewLayout, sizeForItemAt indexPath: IndexPath) -> CGSize {
        if indexPath.section == 0{
            return CGSize(width: 40, height: 40)
        }
        else{
            return CGSize(width: 100, height: 70)

        }
    }
   
    func goToEcoElementDetailPage(index: Int){
        performSegue(withIdentifier: "ecoElement", sender: index)
    }
    
    func collectionView(_ collectionView: UICollectionView, didSelectItemAt indexPath: IndexPath) {
        
        if collectionView == ecoElementCollectionView{
            return
//            performSegue(withIdentifier: "ecoElement", sender: indexPath.row)
        }
        
        return
        var key = ""
        if indexPath.section == 0{
            key = ecoFilterList[Array(ecoFilterList.keys)[indexPath.row]]!
        }
        else{
            key = extraEcoFilterList[Array(extraEcoFilterList.keys)[indexPath.row]]!
        }
        
        if  filterKey.contains(key){
            filterKey.remove(at: filterKey.firstIndex(of: key)!)
        }
        else{
            filterKey.append(key)
        }
        
        print("filter cat is", filterKey)
        self.filterGardenData()
        getAllImages()
        
        self.pageType = .plant
        self.myGardenTableView.reloadData()
        self.ecoCategoryCollectionView.reloadData()

    }
    
}

protocol MyGardenOptionDelegate {
    func goToPlantSearch()
    
    func goToPlantScan()
    
    func goToDeviceList()
    
    func updateSingleUserGardenFromExternal(data: GardenUserPlantEditModel)
    
    func goToEditGarden(gardenItem: UserPlantModel)
    
    func goToDeleteGarden(gardenItem: UserPlantModel)
    
    func updateUserGardenFromExternal(data: [UserPlantModel])

    func showEditWindow(data: MyGardenModel)
    
    func updateGardenUserList()
    
    func updateGardenDataFromExternal(data: [MyGardenModel])
    
    func updateGardenPlantFilter(listId: Int)
    
}

extension MyGardenViewController: MyGardenOptionDelegate{
    func showEditWindow(data: MyGardenModel) {
        let alert = UIAlertController()
        let moveAction = UIAlertAction(title: "Pflanze verschieben", style: .default){action in
            self.goToEditMovePlant(data: data)
            
        }
        let notifAction = UIAlertAction(title: "Notizen zur Pflanze", style: .default){action in
            self.goToEditNoticePlant(data: data)
            
        }
        let countAction = UIAlertAction(title: "Anzahl angeben", style: .default){action in
            self.goToEditPlantCount(data: data)
            
        }
        let deleteAction = UIAlertAction(title: "Löschen", style: UIAlertAction.Style.destructive){action in
            self.ShowDeletePopUp(data: data)
        }
        let closeAction = UIAlertAction(title: "Abbrechen", style: UIAlertAction.Style.cancel)
        alert.addAction(moveAction)
        alert.addAction(notifAction)
        alert.addAction(countAction)
        alert.addAction(deleteAction)

        alert.addAction(closeAction)
        present(alert, animated: true, completion: nil)
    }
    
    func goToEditMovePlant(data: MyGardenModel){
        let controller = storyboard?.instantiateViewController(identifier: "PlantMoveView") as! MyGardenPlantMoveViewController
//        controller.viewController = self
        controller.gardenDelegate = self

        controller.selectedGarden = data
        controller.userGardenList = self.userListData
        self.navigationController?.pushViewController(controller, animated: true)
    }
    
    func goToEditPlantCount(data: MyGardenModel){
        let controller = storyboard?.instantiateViewController(identifier: "PlantCountView") as! MyGardenPlantCountChangeViewController
//        controller.viewController = self
        controller.selectedGarden = data
        controller.userGardenList = self.userListData
        controller.gardenDelegate = self
        self.navigationController?.pushViewController(controller, animated: true)
    }
    
    func goToEditNoticePlant(data: MyGardenModel){
        let controller = storyboard?.instantiateViewController(identifier: "PlantNoticeView") as! MyGardenPlantNoticeViewController
//        controller.viewController = self
        controller.gardenData = data
        controller.gardenDelegate = self

        self.navigationController?.pushViewController(controller, animated: true)
    }
    
    func ShowDeletePopUp(data: MyGardenModel){
        let alert = UIAlertController(title: "\(data.UserPlant.Name) löschen", message: "Willst du diese Pflanze wirklich aus deinem Garten löschen?", preferredStyle: .actionSheet)
        let confirmAction = UIAlertAction(title: "Abbrechen", style: .cancel)
        let deleteAction = UIAlertAction(title: "Ja", style: UIAlertAction.Style.destructive){action in
            self.deleteUserPlant(data: data)
        }
        
//        let closeAction = UIAlertAction(title: "Abbrechen 2", style: UIAlertAction.Style.cancel)
        alert.addAction(deleteAction)
        alert.addAction(confirmAction)
        
//        alert.addAction(closeAction)

        present(alert, animated: true, completion: nil)

    }
    
    func deleteUserPlant(data: MyGardenModel){
        NetworkManager().requestDataAsync(type: String.self, APP_URL.MY_GARDEN_USER_PLANTS + "deleteUserPlantFromAllUserList/\(data.UserPlant.PlantId)/\(data.UserPlant.Gardenid)", nil ,method: .delete, printRequest: true){response in
            
            var selectedGardenIndex = self.myGardenData.firstIndex(where: {$0.UserPlant.Id == data.UserPlant.Id})
            
            if selectedGardenIndex == nil{
                self.ShowAlert(message: "Es gibt einen fehler")

                return
            }
            
            self.ShowAlert(message: "Speichern")
            self.myGardenData.remove(at: selectedGardenIndex!)
            self.filterGardenData()
            self.myGardenTableView.reloadData()
            self.configureEcoFilter()
        }
    }
    
    func updateSingleUserGardenFromExternal(data: GardenUserPlantEditModel){
        
        var selectedGardenIndex = self.myGardenData.firstIndex(where: {$0.UserPlant.Id == data.Id})
        
        if selectedGardenIndex == nil{
            return
        }
        
        myGardenData[selectedGardenIndex!].UserPlant.Notes = data.Notes
        myGardenData[selectedGardenIndex!].UserPlant.Count = data.Count
        
        self.filterGardenData()
        self.myGardenTableView.reloadData()
        self.configureEcoFilter()
    }
    
    func updateUserGardenFromExternal(data: [UserPlantModel]) {
        self.userListData = data
        isListenDropdown = false
        updateListenDropdown()
//        self.listenUserListTableView.reloadData()

    }
    
    func updateGardenDataFromExternal(data: [MyGardenModel]) {
        self.myGardenData = data
        
        self.loadUserList()
        self.resetGardenFilterData()
        self.filterGardenData()
         self.getAllImages()
        self.myGardenTableView.reloadData()
    }
    
    
    
    
    func updateGardenPlantFilter(listId: Int) {
        self.pageType = .plant
        self.listFilter = listId
        self.filterGardenData()
         self.getAllImages()
        self.myGardenTableView.reloadData()
    }
    
   
    
    func updateGardenUserList() {
        self.showSpinner(onView: self.view)
        DispatchQueue.global(qos: .background).async {
            self.loadUserList(true)

        }
        isListenDropdown = false
        updateListenDropdown()
    }
    
    func goToDeviceList() {
        let controller = storyboard?.instantiateViewController(identifier: "deviceSelectView") as! MyGardenDeviceSelectViewController
        controller.viewController = self
        self.navigationController?.pushViewController(controller, animated: true)
    }
    
    
    func goToPlantSearch() {
        let storyBoard = UIStoryboard(name: "PlantSearch", bundle: nil)
        //            let controller = storyBoard.instantiateViewController(withIdentifier: "PlantSearchView2") as! PlantSearchViewController
        let controller = storyBoard.instantiateViewController(withIdentifier: "PlantSearchView2")
        self.navigationController?.pushViewController(controller, animated: true)
    }
    
    func goToPlantScan() {
        let storyBoard = UIStoryboard(name: "PlantScan", bundle: nil)
        let controller = storyBoard.instantiateViewController(withIdentifier: "PlantScanView") as! PlantScanViewController
        self.navigationController?.pushViewController(controller, animated: true)
    }
    
    func goToEditGarden(gardenItem: UserPlantModel){
        let controller = storyboard?.instantiateViewController(withIdentifier: "ListenEditGarden") as! MyGardenListenEditGardenViewController
        controller.gardenDelegate = self
        controller.userGarden = gardenItem
               self.navigationController?.pushViewController(controller, animated: true)
    }
    
    func goToDeleteGarden(gardenItem: UserPlantModel) {
        let controller = storyboard?.instantiateViewController(withIdentifier: "ListenDeleteGarden") as! MyGardenListenDeleteViewController
        controller.gardenDelegate = self
        controller.userGardenList = self.userListData
        controller.userGarden = gardenItem
        
        controller.modalPresentationStyle = .overFullScreen
//        self.present(controller, animated: true, completion: nil)
//        self.navigationController?.present(controller, animated: true, completion: nil)
        self.navigationController?.pushViewController(controller, animated: true)
    }
    
}
