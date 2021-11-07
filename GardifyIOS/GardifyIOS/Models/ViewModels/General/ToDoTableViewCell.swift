//
//  ToDoTableViewCell.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 20.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class ToDoTableViewCell: UITableViewCell {

    
    @IBOutlet weak var todoDescriptionLabel: UILabel!
    @IBOutlet weak var todoCheckImage: UIImageView!
    @IBOutlet weak var todoCheckButton: UIButton!
    @IBOutlet weak var todoDetailButton: UIButton!
    
    var todoIndex: IndexPath?
    var viewController: MyGardenViewController?

    var parent: MyGardenTableViewCell?
    var todoData: CyclicTodosModel?
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }
    
    func onConfigure(todo: CyclicTodosModel){
        todoData = todo
        pageConfiguration(todo: todo)
        
    }
    
    func pageConfiguration(todo: CyclicTodosModel)
    {
        self.todoDescriptionLabel.text = todo.Title
    }
    
    @IBAction func onTodoCheck(_ sender: Any) {
        
        let alert = UIAlertController(title: "", message: "Texte ändern, löschen und als erledigt markieren kannst du im To-Do-Kalender", preferredStyle: .actionSheet)
        
        let todoAction = UIAlertAction(title: "zum To-Do-Kalender", style: .default){alert in
            let tabBar = self.viewController?.tabBarController as! MainTabBarController
            
            tabBar.navigateToLoggedPage(page: .toDoCalender)
            
        }
        
        let closeAction = UIAlertAction(title: "Abbrechen", style: .cancel, handler: nil)
        
        alert.addAction(todoAction)
        alert.addAction(closeAction)
        
        viewController?.present(alert, animated: true, completion: nil)
//        if self.isSelected{
//            self.parent?.todoTableView.deselectRow(at: todoIndex!, animated: false)
//
//        }
//        else{
//            self.parent?.todoTableView.selectRow(at: todoIndex, animated: false, scrollPosition: .none)
//
//        }
    }
    
    @IBAction func onTodoDetailPressed(_ sender: Any) {
        let view = self.parent?.viewController
        view!.goToTodoDetails(todoId: todoData!.Id)
    }
    
    override func setSelected(_ selected: Bool, animated: Bool) {
//        if !isCheckClicked{
//            return
//        }
//
        super.setSelected(selected, animated: animated)

        
        if selected{
            self.todoCheckImage.image = UIImage(systemName: "checkmark")?.withTintColor(.label)
        }
        else{
            self.todoCheckImage.image = UIImage(systemName: "circle")?.withTintColor(.gray)

        }
        // Configure the view for the selected state
    }

}
