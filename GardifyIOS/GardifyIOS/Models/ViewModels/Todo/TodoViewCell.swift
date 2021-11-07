//
//  TodoViewCell.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 31.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class TodoViewCell: UITableViewCell {

    @IBOutlet weak var outerView: UIView!
    
    @IBOutlet weak var outerTopCover: UIView!
    @IBOutlet weak var outerBottomCover: UIView!
    @IBOutlet weak var indicatorTypeLine: UIView!
    @IBOutlet weak var titleLabel: UILabel!
    @IBOutlet weak var todoDateLabel: UILabel!
    @IBOutlet weak var indicatorImage: UIImageView!
    
    @IBOutlet weak var dropdownHeight: NSLayoutConstraint!
    @IBOutlet weak var optionButton: UIButton!
    @IBOutlet weak var innerDropdownView: UIView!
    @IBOutlet weak var checkListImage: UIImageView!
    @IBOutlet weak var innerDropdownHeight: NSLayoutConstraint!
    
    @IBOutlet weak var innerImageView: UIImageView!
    @IBOutlet weak var descLabelHeight: NSLayoutConstraint!
    
    @IBOutlet weak var checkImageButton: UIButton!
    @IBOutlet weak var descLabel: UILabel!
    var parent: UITableView?
    var todoType: ToDoType?
    var id: Int?
    var imageURL: String = ""
    var data: TodoCalenderModel?
    var key: Int?
    var index: Int?
    var parentView: TodoViewController?
    var isTop: Bool = false
    var isBottom: Bool = false
    var isExtended: Bool = false
    var isFinished: Bool = false
    var descHeight = CGFloat(50)
    
    override func awakeFromNib() {
        super.awakeFromNib()
        isExtended = false
        // Initialization code
    }
    
    
    
    func loadImage(){
        
        if imageURL == ""{
            return
        }
        
        NetworkManager().getImageFromUrl(urlString: imageURL){image in
            self.innerImageView.image = image
            
        }
    }
    
    func onConfigure(type: ToDoType, title: String, id: Int, date: String, desc: String){
//        updateDropdownPage()
//        descLabelHeight.constant = descHeight
        self.todoType = type
        self.titleLabel.text = title
        self.descLabel.text = desc
//        let dateValue = date.toDate("yyyy-MM-dd'T'hh:mm:ss")
//        let weekDay = String.getDayString(day: dateValue?.dayNumberOfWeek() ?? 0)
//        self.todoDateLabel.text = "ab \(weekDay) \(dateValue!.toString(output: "dd.MM"))"
        self.id = id
        pageConfiguration()
        self.isFinished = false
        if data != nil{
            self.isFinished = self.data!.isChecked

        }
        updateCheckImage()
        
        if !isExtended{
            closeDropdown()

        }
        

    }
    
    func updateCheckImage(){
        if !isFinished{
            self.checkListImage.image = UIImage(named: "todo_check_empty")
        }
        else{
            self.checkListImage.image = UIImage(named: "todo_check")
        }
    }
    
    
    
    @IBAction func onExtendClick(_ sender: Any) {
        
        isExtended = !isExtended

        updateDropdownPage()
    }
    
    @IBAction func onOptionClicked(_ sender: Any) {
        
        let alert = UIAlertController()
        
        let editAction = UIAlertAction(title: "Bearbeiten", style: .default){alert in
            self.parentView?.goToEditTodoPage(todoId: self.id!)
        }
        
        let deleteAction = UIAlertAction(title: "Löschen", style: .destructive){alert in
            self.showDeleteOption()
        }
        
        let closeAction = UIAlertAction(title: "Abbrechen", style: .cancel, handler: nil)
        
        alert.addAction(editAction)
        alert.addAction(deleteAction)
        alert.addAction(closeAction)

        self.parentView?.present(alert, animated: true, completion: nil)
    }
    
    func showDeleteOption(){
        
        let alert = UIAlertController(title: "", message: "Bist du sicher, dass du dieses To-do löschen möchtest?", preferredStyle: .actionSheet)
        
        let singleDelete = UIAlertAction(title: "Diesen Termin löschen", style: .destructive){alert in
            self.onDeleteSingleTodo()
        }
        
        let allDelete = UIAlertAction(title: "Alle Termine löschen", style: .destructive){alert in
            self.onDeleteTodos()
        }
            
        alert.addAction(singleDelete)
        alert.addAction(allDelete)
        
        if todoType == ToDoType.todo{
            alert.addAction(allDelete)

        }
        let closeAction = UIAlertAction(title: "Abbrechen", style: .cancel, handler: nil)
        alert.addAction(closeAction)

        
        self.parentView?.present(alert, animated: true, completion: nil)

    }
    
    func onDeleteSingleTodo(){
        parentView?.deleteSingleTodo(todoId: self.id!)
    }
    
    func onDeleteTodos(){
        parentView?.deleteTodos(todoId: self.id!)

    }
    
    
    @IBAction func onFinishedToggle(_ sender: Any) {
        
        isFinished = !isFinished
        finishedReques()
        if parentView != nil{
            parentView!.todoDateGroupData[parentView!.todoGroupList[self.key!]]?[self.index!].isChecked = isFinished
        }
        updateCheckImage()
        
    }
    
    func finishedReques(){
        NetworkManager().requestDataAsync(type: String.self, APP_URL.TO_DO_LIST+"markfinished/\(isFinished ? "true" : "false")/\(data!.id)", method: .put, printRequest: true){response in
            print("response")
        }
    }
    
    func closeDropdown(){
        dropdownHeight.constant = 50
        innerDropdownHeight.constant = 0
        indicatorImage.revertFlip()
        innerDropdownView.alpha = 0
    }
    
    func updateDropdownPage(){
        var extraHeight = innerDropdownView.frame.width
        
        if self.imageURL == ""{
            extraHeight = CGFloat(0)
        }
//        if todoType != ToDoType.todo {
//            extraHeight = CGFloat(0)
//        }
        
        if isExtended{
            self.loadImage()
            dropdownHeight.constant = 60 + extraHeight + descHeight + 10
            innerDropdownHeight.constant = extraHeight + descHeight + 10
            indicatorImage.flipXAxis()
            innerDropdownView.alpha = 1
        }
        else{
            dropdownHeight.constant = 50
            innerDropdownHeight.constant = 0
            indicatorImage.revertFlip()
            innerDropdownView.alpha = 0

        }
        
        parent?.reloadData()
    }
    
    func borderConfiguration(){
        self.outerTopCover.alpha = 0
        self.outerBottomCover.alpha = 0
        if isTop && isBottom{
            self.outerView.addCustomBorderRadius(topLeft: true, topRight: true, botLeft: true, botRight: true)
            self.outerView.setWhiteButtonView()

        }
        else if isTop{
            self.outerView.addCustomBorderRadius(topLeft: true, topRight: true, botLeft: false, botRight: false)
            self.outerTopCover.alpha = 0
            self.outerBottomCover.alpha = 1
        }else if isBottom{
            self.outerView.addCustomBorderRadius(topLeft: false, topRight: false, botLeft: true, botRight: true)
            self.outerView.addShadow()
            self.outerTopCover.alpha = 1
            self.outerBottomCover.alpha = 0
        }else{
            self.outerView.addCustomBorderRadius(topLeft: false, topRight: false, botLeft: false, botRight: false)
            self.outerTopCover.alpha = 1
            self.outerBottomCover.alpha = 1
        }
    }
    
    func pageConfiguration(){
        
        
        self.backgroundColor = .clear
        self.contentView.backgroundColor = .clear
        self.outerView.backgroundColor = .white
        self.outerView.setWhiteButtonView()
        self.outerView.clearBorderRadius()
        borderConfiguration()
        checkListImage.alpha = 0
        optionButton.alpha = 0
        
        
        switch todoType {
        case .todo:
            self.indicatorImage.image = UIImage(named: "todo_normal")
            checkListImage.alpha = 1
            optionButton.alpha = 1
        case .custom:
            self.indicatorImage.image = UIImage(named: "todo_custom")
            checkListImage.alpha = 1
            optionButton.alpha = 1
        case .diary:
            self.indicatorImage.image = UIImage(named: "todo_diary")
        case .scan:
            self.indicatorImage.image = UIImage(named: "todo_scan")
        default:
            self.indicatorImage.image = nil
        }
//        self.indicatorTypeLine.backgroundColor = todoType?.getTodoColor()
        
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}
