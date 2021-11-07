//
//  MyGardenTableViewCell.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 17.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation
import UIKit

class MyGardenTableViewCell: UITableViewCell {

    @IBOutlet weak var nameLatinLabel: UILabel!
    @IBOutlet weak var nameGermanLabel: UILabel!
    @IBOutlet weak var gardenLocationLabel: UILabel!
    @IBOutlet weak var noteLabel: UILabel!
    @IBOutlet weak var plantDetailButton: UIButton!
    @IBOutlet weak var plantEditButton: UIButton!
    
    @IBOutlet weak var gardenPlantImage: UIImageView!
    @IBOutlet weak var outerView: UIView!
    @IBOutlet weak var ecoCollectionView: UICollectionView!
    @IBOutlet weak var todoTableView: UITableView!
    
    let ecoContainer = EcoCategoryCollectionList()

    var viewController: MyGardenViewController?
    var plantData: MyGardenModel?
    
    var notIgnoredTodo: [CyclicTodosModel] = []
    var todosData: [CyclicTodosModel] = []
    
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }
    
    func onConfigure(plant: MyGardenModel){
        
        plantData = plant
        todoTableView.delegate = self
        todoTableView.dataSource = self
        
        filterCyclicTodo()
        todoTableView.reloadData()
        
        pageConfiguration(plant: plant)
        plantDetailConfiguration()
        collectionDetailConfiguration(plant: plant)
        
    }
    
    func filterCyclicTodo(){
        notIgnoredTodo = (self.plantData?.UserPlant.CyclicTodos ?? []).filter({!$0.Ignored})
        
        todosData = notIgnoredTodo.filter({ ($0.DateEnd.toDate("yyyy-MM-dd'T'HH:mm:ss")! >= Date())})
        
        
        
        print(todosData)
    }
    
    func plantDetailConfiguration(){
        
        let gesture = UIGestureRecognizer(target: self, action: #selector(self.showPlant))
        
        self.gardenPlantImage.addGestureRecognizer(gesture)
        
        self.gardenPlantImage.isUserInteractionEnabled = true
    }
    
    @IBAction func showPlantDetail(_ sender: Any) {

        self.viewController?.goToPlantDetails(plantId: plantData?.UserPlant.PlantId ?? 0)

    }
    @objc func showPlant(){
        print("is shown")
        self.viewController?.goToPlantDetails(plantId: plantData?.UserPlant.Id ?? 0)
    }
    
    @IBAction func onEdit(_ sender: Any) {
        if plantData == nil{
            return
        }
        self.viewController?.showEditWindow(data: plantData!)
    }
    
    
    func collectionDetailConfiguration(plant: MyGardenModel){
        let badgeList = plant.UserPlant.Badges?.map({ ecoCategoryKeys[$0.Id]}).filter({$0 != nil}).map({$0!})

        ecoContainer.badgesId = Array(Set(badgeList!))

        self.ecoCollectionView.dataSource = ecoContainer
        self.ecoCollectionView.delegate = ecoContainer
            self.ecoCollectionView.reloadData()


    }
//
    func pageConfiguration(plant: MyGardenModel){
        
        self.nameLatinLabel.text = plant.UserPlant.NameLatin.removeItalicTag()
        self.nameGermanLabel.text = plant.UserPlant.Name
        
        self.outerView.addShadow()
        
//        self.gardenLocationLabel.text = "im \(plant.ListName), \(plant.UserPlant.IsInPot ? "" : "keine") Toppflanze"
        let userGardenNameList = plant.ListNames.joined(separator: ", ")
        
        self.gardenLocationLabel.text = "im \(userGardenNameList), \(plant.UserPlant.IsInPot ? "" : "keine") Topfpflanze"
        
        self.backgroundColor = .clear
        self.outerView.backgroundColor = .systemBackground
        self.outerView.addBorderRadius()
        
        if let notes = plant.UserPlant.Notes{
            self.noteLabel.text = plant.UserPlant.CreatedNotes ?? "" + " - " + notes
        }
        else{
            self.noteLabel.text = plant.UserPlant.CreatedNotes ?? "Es gibt keine Notizen für diese Pflanze"
        }
        
        
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}

extension MyGardenTableViewCell: UITableViewDelegate, UITableViewDataSource{
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        if todosData.count == 0 && notIgnoredTodo.count > 0{
            return 1
        }
        else if todosData.count == 0{
            return 1
        }
        return todosData.count 
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        if todosData.count == 0 && notIgnoredTodo.count > 0{
            let cell = UITableViewCell()
            
            cell.textLabel?.text = "Ws gibt zur Zeit keine aktiven To-Dos"
            
            cell.textLabel?.font = UIFont.systemFont(ofSize: 15)
            return cell
        }
        else if todosData.count == 0{
            let cell = UITableViewCell()
            
            cell.textLabel?.text = "Es gibt noch keine aktiven To-dos"
            
            cell.textLabel?.font = UIFont.systemFont(ofSize: 15)
            return cell
        }
        
        let cell = tableView.dequeueReusableCell(withIdentifier: "todoCell", for: indexPath) as! ToDoTableViewCell
        cell.viewController = self.viewController
        cell.todoIndex = indexPath
        cell.parent = self
        let todoData = todosData[indexPath.row]
        
        cell.onConfigure(todo: todoData)
        
        return cell
        
    }
    
    func tableView(_ tableView: UITableView, willSelectRowAt indexPath: IndexPath) -> IndexPath? {
        let alert = UIAlertController(title: "", message: "Texte ändern, löschen und als erledigt markieren kannst du im To-Do-Kalender", preferredStyle: .actionSheet)
        
        let todoAction = UIAlertAction(title: "zum To-Do-Kalender", style: .default){alert in
            let tabBar = self.viewController?.tabBarController as! MainTabBarController
            
            tabBar.navigateToLoggedPage(page: .toDoCalender)
            
        }
        
        let closeAction = UIAlertAction(title: "Abbrechen", style: .cancel, handler: nil)
        
        alert.addAction(todoAction)
        alert.addAction(closeAction)
        
        viewController?.present(alert, animated: true, completion: nil)
        
        return nil
    }
    

    
}

