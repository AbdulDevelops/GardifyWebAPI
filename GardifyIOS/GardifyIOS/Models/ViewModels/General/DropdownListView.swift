//
//  DropdownListView.swift
//  GardifyIOS
//
//  Created by Netzlab on 09.12.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class DropdownListView: UIView {

    let kCONTENT_XIB_NAME = "DropdownListView"
    
    @IBOutlet var contentView: UIView!
    
    
    @IBOutlet weak var displayTextField: UITextField!
    @IBOutlet weak var dropdownButton: UIButton!
    var displayText: [String] = []
    var selectedIndex: Int = 0
    var cellHeight: CGFloat = 30
    
    @IBOutlet weak var dropdownTableView: UITableView!
    var isExtended = false
    
    @IBOutlet weak var tableHeight: NSLayoutConstraint!
    
    override init(frame: CGRect) {
       super.init(frame: frame)
        dropdownTableView.dataSource = self
        dropdownTableView.delegate = self
        
     
       commonInit()
   }
    
    required init?(coder aDecoder: NSCoder) {
            super.init(coder: aDecoder)
            commonInit()
        }
        
        func commonInit() {
            Bundle.main.loadNibNamed(kCONTENT_XIB_NAME, owner: self, options: nil)
            contentView.fixInView(self)
        }
    
    func resetTable(){
//        selectedIndex = 0
        dropdownTableView.register(UINib(nibName: "DropdownListViewTableViewCell", bundle: nil), forCellReuseIdentifier: "dropdownListCell")
        
        isExtended = false
        updateTable()
    }
    
    func updateTable(){
        if displayText.count < 1{
            return
        }
        print("displayed text is", displayText[selectedIndex])
        displayTextField.text = displayText[selectedIndex]
        isExtended = false
        updateDropdownView()
        dropdownTableView.reloadData()
    }

    func updateDropdownView(){
        if isExtended{
            tableHeight.constant = CGFloat(displayText.count) * cellHeight
        }
        else{
            tableHeight.constant = 0
        }
    }

    @IBAction func onDropdownClicked(_ sender: Any) {
        
        isExtended = !isExtended
        updateDropdownView()
    }
}

extension UIView
{
    func fixInView(_ container: UIView!) -> Void{
        self.translatesAutoresizingMaskIntoConstraints = false;
        self.frame = container.frame;
        container.addSubview(self);
        NSLayoutConstraint(item: self, attribute: .leading, relatedBy: .equal, toItem: container, attribute: .leading, multiplier: 1.0, constant: 0).isActive = true
        NSLayoutConstraint(item: self, attribute: .trailing, relatedBy: .equal, toItem: container, attribute: .trailing, multiplier: 1.0, constant: 0).isActive = true
        NSLayoutConstraint(item: self, attribute: .top, relatedBy: .equal, toItem: container, attribute: .top, multiplier: 1.0, constant: 0).isActive = true
        NSLayoutConstraint(item: self, attribute: .bottom, relatedBy: .equal, toItem: container, attribute: .bottom, multiplier: 1.0, constant: 0).isActive = true
    }
}

extension DropdownListView: UITableViewDelegate, UITableViewDataSource{
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return displayText.count
    }
    
    func tableView(_ tableView: UITableView, heightForRowAt indexPath: IndexPath) -> CGFloat {
        return cellHeight
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: "dropdownListCell", for: indexPath) as! DropdownListViewTableViewCell
        
        cell.tag = indexPath.row
        
        cell.parent = self
        
        
        cell.clearBackground()
        
        cell.cellLabel.text = displayText[indexPath.row]
        
        if indexPath.row == selectedIndex{
            cell.backgroundColor = .Background
        }
        
        return cell
    }
    
    @objc func selected(){
        print("is shown")
//        self.viewController?.goToPlantDetails(plantId: plantData?.UserPlant.Id ?? 0)
    }
    
    func tableView(_ tableView: UITableView, willSelectRowAt indexPath: IndexPath) -> IndexPath? {
        return indexPath
    }
    
    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        print("selected")
        selectedIndex = indexPath.row
        updateTable()
    }
}
