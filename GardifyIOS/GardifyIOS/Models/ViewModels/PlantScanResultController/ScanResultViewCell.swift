//
//  ScanResultViewCell.swift
//  GardifyText
//
//  Created by Netzlab on 30.07.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class ScanResultViewCell: UITableViewCell {

    @IBOutlet weak var nameLatinLabel: UILabel!
    @IBOutlet weak var nameGermanLabel: UILabel!
    @IBOutlet weak var plantImage: UIImageView!
    @IBOutlet weak var moreInfo: UIButton!
    @IBOutlet weak var saveButton: UIButton!
    var viewController: UIViewController?
    var nameGerman: String = ""
    var nameLatin: String = ""
    var isInDb: Bool = false
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
//        plantImage.image = UIImage(named: "PlantPlaceholder")
    }
    
    func onConfigure(nameLatin: String, nameGerman: String){
        
        pageConfiguration(nameLatin: nameLatin, nameGerman: nameGerman)
    }
    
    func pageConfiguration(nameLatin: String, nameGerman: String){
        nameLatinLabel.text = nameLatin.removeItalicTag()
        
        nameGermanLabel.text = nameGerman.removeItalicTag()
        self.nameGerman = nameGerman
        self.nameLatin = nameLatin
        
        moreInfo.setGreenButton()
        saveButton.setGreenButton()
        print(isInDb)
        if !isInDb{
            moreInfo.alpha = 1
            moreInfo.setTitle("In Google öffnen", for: .normal)
            saveButton.alpha = 1
            saveButton.setTitle("Pflanze vorschlagen", for: .normal)
            
        }
        else{
            moreInfo.alpha = 1
            moreInfo.setTitle("Mehr dazu", for: .normal)

            saveButton.alpha = 1
            saveButton.setTitle("Speichern", for: .normal)

        }
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }
    
    @IBAction func onMoreInfo(_ sender: Any) {
        
        if !isInDb{
            
            if let url = URL(string: "https://www.google.com/search?q=\(nameLatin.replacingOccurrences(of: " ", with: "+"))") {
                UIApplication.shared.open(url)
            }
            return
        }
//        let storyBoard = UIStoryboard(name: "PlantDetail", bundle: nil)
//        let controller = storyBoard.instantiateViewController(withIdentifier: "PlantDetailView") as! PlantDetailViewController
//        
//        controller.plantId = self.tag
//        
//        self.viewController?.navigationController?.pushViewController(controller, animated: true)
        
        self.viewController?.goToPlantDetails(plantId: self.tag)

    }
    
    @IBAction func saveToGarden(_ sender: Any) {
        
        if !(self.viewController?.isLoggedIn() ?? false){
            return
        }
        
        if !isInDb{
            let alert = UIAlertController(title: "\(nameGerman) zu Gardify vorschlagen", message: "Bist du der Urheber dieses Bilder?", preferredStyle: .actionSheet)
            
            let yesAction = UIAlertAction(title: "Ja", style: .default){alert in
                print("yes")
            }
            
            let noAction = UIAlertAction(title: "Nein", style: .default){alert in
                print("no")

            }
            
            let cancelAction = UIAlertAction(title: "Abbrechen", style: .cancel){alert in
                print("cancel")

            }
            
            alert.addAction(yesAction)
            alert.addAction(noAction)
            
            alert.addAction(cancelAction)
            
            self.viewController?.present(alert, animated: true, completion: nil)
            
            return
        }
        
        self.viewController?.goToAddGarden(plantId: self.tag, plantModel: nil, self.nameGerman)
    }
    
    func addSuggestPlant(){
        
    }
    
}
