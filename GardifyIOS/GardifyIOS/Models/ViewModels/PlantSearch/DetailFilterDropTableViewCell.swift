//
//  DetailFilterDropTableViewCell.swift
//  GardifyIOS
//
//  Created by Netzlab on 29.09.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

protocol DetailDropSelectDelegate {
    func cellSelected(index: Int)
}

class DetailFilterDropTableViewCell: UITableViewCell, UITableViewDelegate, UITableViewDataSource {
    

    var tagSelectList: [(t: String, id: Int)] = []
    var filterDelegate: FilterUpdateDelegate?
    
    @IBOutlet weak var filterToggleButton: UIButton!
    @IBOutlet weak var filterTitleLabel: UILabel!
    @IBOutlet weak var filterTitleImage: UIImageView!
    @IBOutlet weak var detailFilterTableView: UITableView!
    
    
    
    @IBOutlet weak var filterHeight: NSLayoutConstraint!
    var parent: DetailDropdown2?
    var dropDelegate: DetailDropSelectDelegate?
    var outputTags: [Int] = []
    var filterType: FilterType?
    var isMultiTable: Bool = true
    var isFilterExtended: Bool = false
    
    let cellHeight = CGFloat(40)
    var cellIndex: IndexPath?
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }
    
    func onConfigure(imageName: String){
        if let image = UIImage(named: "cat_"+imageName) {
            self.filterTitleImage.image = image
        }
        else{
            self.filterTitleImage.image = nil
        }
//        detailFilterTableView.reloadData()
        configureFilterHeight()
    }
    
    func configureFilterHeight(){
        self.filterHeight.constant = CGFloat(self.tagSelectList.count) * self.cellHeight

//        DispatchQueue.main.async {
//            if self.isFilterExtended{
//                self.filterHeight.constant = CGFloat(self.tagSelectList.count) * self.cellHeight
//            }
//            else{
//                self.filterHeight.constant = 0
//
//            }
//
//
//        }
//
        self.detailFilterTableView.allowsMultipleSelection = isMultiTable
        self.detailFilterTableView.reloadData()
    }
    
    @IBAction func onHeightToggle(_ sender: Any) {
        self.isFilterExtended = !self.isFilterExtended
        self.dropDelegate?.cellSelected(index: cellIndex!.row)
        configureFilterHeight()
    }
    

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }
    
    func tableView(_ tableView: UITableView, heightForRowAt indexPath: IndexPath) -> CGFloat {

        return cellHeight

    }
    
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
            return tagSelectList.count

    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: "DetailFilterDropItem", for: indexPath) as! DetailFilterDropItemTableViewCell
        cell.selectionStyle = .none
        cell.filterSelectLabel.text = self.tagSelectList[indexPath.row].t
        
        if outputTags.contains(self.tagSelectList[indexPath.row].id)
        {
            tableView.selectRow(at: indexPath, animated: false, scrollPosition: .none)
        }
        
        return cell
    }
    
    func updateMainTags(value: Int, name: String){
        
        self.filterDelegate?.updateTags(value: value , name: name, sender: nil, type: filterType!, tableType: TableType.filterGeneral)

        
        
    }
    
    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        updateMainTags(value: self.tagSelectList[indexPath.row].id, name: self.tagSelectList[indexPath.row].t)
    }
    
    func tableView(_ tableView: UITableView, didDeselectRowAt indexPath: IndexPath) {
        updateMainTags(value: self.tagSelectList[indexPath.row].id, name: self.tagSelectList[indexPath.row].t)

    }

    

}


