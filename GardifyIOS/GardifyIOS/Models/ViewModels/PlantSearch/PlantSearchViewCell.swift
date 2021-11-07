//
//  PlantSearchViewCell.swift
//  GardifyIOS
//
//  Created by Netzlab on 05.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class PlantSearchViewCell: UITableViewCell {

    @IBOutlet weak var nameLatinLabel: UILabel!
    
    @IBOutlet weak var nameGermanLabel: UILabel!
    
    @IBOutlet weak var detailsLabel: UILabel!
    @IBOutlet weak var plantImageView: UIImageView!
    @IBOutlet weak var outerView: UIView!
    
    @IBOutlet weak var saveToGardenButton: UIButton!
    @IBOutlet weak var moreDetailImage: UIImageView!
    @IBOutlet weak var moreInfoButton: UIButton!
    
    @IBOutlet weak var colorCollectionView: UICollectionView!
    @IBOutlet weak var ecoCollectionView: UICollectionView!
    
    @IBOutlet weak var plantDetailExtendButton: UIButton!
    
    
    let colorContainer = ColorCollectionList()
    let ecoContainer = EcoCategoryCollectionList()
    var plantData: PlantDetailModel?
    var viewController: UIViewController?
    
    var badgesList : [Int] = []
    var isDetailExtended = false

    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }
    
    func onConfigure(plant: PlantDetailModel){
        
        plantData = plant
        pageConfiguration()
        collectionDetailConfiguration(plant: plant)
//        let detailTap = UITapGestureRecognizer(target: self, action: #selector(self.getDetail(gesture:)))
//
//        moreDetailImage.addGestureRecognizer(detailTap)
//        moreDetailImage.isUserInteractionEnabled = true
        
//        moreInfoButton.setGreenButton()
    }
    
    func collectionDetailConfiguration(plant: PlantDetailModel){
        
        if plant.Badges == nil{
            return
        }
        var badgeList = plant.Badges?.map({ ecoCategoryKeys[$0.Id]})as! [String]
        ecoContainer.badgesId = Array(Set(badgeList))
        
        self.ecoCollectionView.dataSource = ecoContainer
        self.ecoCollectionView.delegate = ecoContainer
        DispatchQueue.main.async {
            self.ecoCollectionView.reloadData()
            
        }
        
        colorContainer.colorData = plant.Colors ?? []
        self.colorCollectionView.dataSource = colorContainer
        self.colorCollectionView.delegate = colorContainer
        DispatchQueue.main.async {
            self.colorCollectionView.reloadData()

        }
        
       
       
    }
    
    func updateDetailExtend(){
        if isDetailExtended{
            self.detailsLabel.numberOfLines = 10
        }
        else{
            self.detailsLabel.numberOfLines = 3
        }
        self.layoutIfNeeded()
    }
    
    @IBAction func onDetailExtend(_ sender: Any) {
        print("detail extended")
        isDetailExtended = !isDetailExtended
        
        let parent = viewController as! PlantSearchViewController
        
        parent.plantSearchTableView.reloadData()
        updateDetailExtend()
    }
    
    
    @IBAction func getPlantDetail(_ sender: Any) {
        self.viewController?.goToPlantDetails(plantId: plantData!.Id)

    }
    @objc func getDetail(gesture: UIGestureRecognizer){
        self.viewController?.goToPlantDetails(plantId: plantData!.Id)
    }
    
    @IBAction func saveToGarden(_ sender: Any) {
        if !((self.viewController?.isLoggedIn()) ?? false){
            return
        }
        
//        self.viewController?.showSpinner(onView: (self.viewController?.view)!)
        
        DispatchQueue.main.async {
            self.viewController?.goToAddGarden(plantId: self.plantData!.Id, plantModel: self.plantData!)

        }
        
    }
    
    
    private func pageConfiguration(){

        nameLatinLabel.text =  self.plantData?.NameLatin.removeItalicTag()
        nameGermanLabel.text =  self.plantData?.NameGerman.removeItalicTag()
        detailsLabel.text = self.plantData?.Description
        self.backgroundColor = .clear
        self.outerView.backgroundColor = .systemBackground
        self.outerView.addBorderRadius()
        self.outerView.addShadow()
        saveToGardenButton.setGreenButton()
//        self.contentView.backgroundColor = .white
//        self.contentView.addBorderRadius()
    }
    
    override func layoutSubviews() {
        super.layoutSubviews()
        
//         contentView.frame = contentView.frame.inset(by: UIEdgeInsets(top: 10, left: 0, bottom: 10, right: 0))
    }
    
    
    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }
    
    

}


