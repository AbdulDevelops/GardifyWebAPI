//
//  DetailFilterFixedTableViewCell.swift
//  GardifyIOS
//
//  Created by Netzlab on 25.09.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

protocol DetailTableSelectDelegate {
    func isSelected(id: Int, name: String)
}

class DetailFilterFixedTableViewCell: UITableViewCell, UITableViewDelegate, UITableViewDataSource{
 
    
    var tagSelectList: [(t: String, id: Int)] = []
    var filterDelegate: FilterUpdateDelegate?
    
    var outputTags: [Int] = []
    var filterType: FilterType?
    var isMultiTable: Bool = true
    
    let cellHeight = CGFloat(50)

    @IBOutlet weak var filterHeight: NSLayoutConstraint!
    
    @IBOutlet weak var filterTableView: FullTableView!
    
    
    func collectionView(_ collectionView: UICollectionView, didSelectItemAt indexPath: IndexPath) {

//        updateFilterHeight()
    }
    

    @IBOutlet weak var filterLabel: UILabel!
    
    override func awakeFromNib() {
        super.awakeFromNib()

    }

    
    func cellConfiguration(){
        
        if tagSelectList.count < 1{
            return
        }
        filterTableView.reloadData()
        filterTableView.allowsMultipleSelection = isMultiTable
        updateFilterHeight()
    }
    
    func updateFilterHeight(){
//        filterHeight.constant = cellHeight * CGFloat((tagSelectList.count / 2) + (tagSelectList.count % 2))
    }
    
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
  

        return (tagSelectList.count / 2) + (tagSelectList.count % 2)
        
//        return tagSelectList.count
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
  
//        if indexPath.row < (Int(tagSelectList.count / 2)){
            let cell = tableView.dequeueReusableCell(withIdentifier: "filterCellDouble", for: indexPath) as! DetailFilterFixedDoubleTableViewCell
            cell.filterType = filterType ?? nil
            cell.detailDelegate = self
            
            let indexVal = indexPath.row * 2
            
            cell.filterLabel1.text = self.tagSelectList[indexVal].t
            
            cell.labelId1 = self.tagSelectList[indexVal].id
            
            let select1 = self.outputTags.contains(self.tagSelectList[indexVal].id)
            cell.filterSelection1(isSelected: select1)
        
        if (indexVal + 1) < self.tagSelectList.count{
            cell.isSingle = false
            cell.filterLabel2.alpha = 1
            cell.labelImage2.alpha = 1
            cell.filterLabel2.text = self.tagSelectList[indexVal + 1].t
            cell.labelId2 = self.tagSelectList[indexVal + 1].id
            let select2 = self.outputTags.contains(self.tagSelectList[indexVal + 1].id)
            cell.filterSelection2(isSelected: select2)
        }
        else{
            cell.isSingle = true
            cell.filterLabel2.alpha = 0
            cell.labelImage2.alpha = 0
        }
        
  

            
            return cell
//        }
//        else{
//            let cell = tableView.dequeueReusableCell(withIdentifier: "filterCell", for: indexPath) as! DetailFilterFixedItemTableViewCell
//            print("filter type is", filterType)
//            cell.filterType = filterType ?? nil
//
//            cell.detailDelegate = self
//
//            print("tag select is", self.tagSelectList[tagSelectList.count - 1], self.outputTags)
//            let select = self.outputTags.contains(self.tagSelectList[tagSelectList.count - 1].id)
//            print("select is", select)
//
//            cell.filterSelection(isSelected: select)
//            cell.labelId = self.tagSelectList[tagSelectList.count - 1].id
//            //        print("value is", self.tagSelectList[tagSelectList.count - 1].t)
//            cell.filterLabel.text = self.tagSelectList[tagSelectList.count - 1].t
//
//            return cell
//        }
        
        

    }
    
    func tableView(_ tableView: UITableView, heightForRowAt indexPath: IndexPath) -> CGFloat {
        return cellHeight
    }
    
    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        let indexVal = indexPath.row * 2
        print("detail selected in", indexVal)

        updateMainTags(value: self.tagSelectList[indexVal].id, name: self.tagSelectList[indexVal].t)
 
    }
    
    func updateMainTags(value: Int, name: String){
  
        self.filterDelegate?.updateTags(value: value , name: name, sender: nil, type: filterType!, tableType: TableType.filterGeneral)
        if !isMultiTable{
            
            let menuListExclude = self.tagSelectList.reduce(into: [Int]()){output, input in
                if input.id != value{
                    output.append(input.id)
                }
            }
            
            if outputTags.contains(where: menuListExclude.contains){
                guard let deletedTags = outputTags.first(where: menuListExclude.contains) else{
                    return
                }
                let deletedName = tagSelectList.first(where: {$0.id == deletedTags})?.t
                
                self.filterDelegate?.updateTags(value: deletedTags , name: deletedName!, sender: nil, type: filterType!, tableType: TableType.filterGeneral)
            }
        }
        
        
        
        
    }
    

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}


extension DetailFilterFixedTableViewCell: DetailTableSelectDelegate{

    
    
    func isSelected(id: Int, name: String) {
        updateMainTags(value: id, name: name)
    }
    
    
}
