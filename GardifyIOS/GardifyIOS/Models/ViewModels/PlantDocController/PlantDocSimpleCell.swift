//
//  PlantDocSimpleCell.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 21.10.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class PlantDocSimpleCell: UITableViewCell {
    
    
    @IBOutlet weak var moreInfoButton: UIButton!
    @IBOutlet weak var docTitleText: UILabel!
    @IBOutlet weak var outerView: UIView!
    @IBOutlet weak var frageLabel: UILabel!
    @IBOutlet weak var frageFirstAnswerLabel: UILabel!
    @IBOutlet weak var answerTableView: UITableView!
    @IBOutlet weak var plantDocAnswerHeight: NSLayoutConstraint!
    
    @IBOutlet weak var bottomPadding: NSLayoutConstraint!
    @IBOutlet weak var notifImage: UIImageView!
    
    var descriptionDelegate: PlantDocDetailsDelegate?
    var plantDocData: PlantDoc?
    var plantDocAnswer: PlantDocDetailModel?
    
    var isDetailExtended: Bool = false
    
    var parent: PlantDocViewController?
    
    var parentMyPost: PlantDocMyPostViewController?
    var isAlertOn: Bool = false

    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }
    
    func onConfigure(data: PlantDoc){
        pageConfiguration(data: data)
    }
    
    func pageConfiguration(data: PlantDoc){
        self.plantDocData = data
        docTitleText.text = data.Thema
        
        self.backgroundColor = .clear
        self.contentView.backgroundColor = .clear
        outerView.addBorderRadius()
        self.notifImage.tintColor = UIColor(named: "GardifyGreen")

        answerTableView.backgroundColor = .clear
        updateNotifMark()
        updateDropdown()
    }
    
    func updateNotifMark(){
        if !isAlertOn{
            notifImage.image = nil
            return
        }
        
        notifImage.image = UIImage(systemName: "exclamationmark.circle.fill")
        
    }
    
    func getParentHeight() -> CGFloat{
        if self.parent == nil{
            return self.parentMyPost?.view.frame.height ?? 0
        }
        return self.parent?.view.frame.height ?? 0
    }
    
    func updateDropdown(){
        if isDetailExtended{
            frageLabel.text = "Frage"
            frageFirstAnswerLabel.text = self.plantDocAnswer?.PlantDocViewModel.QuestionText
            let height = max(CGFloat( (self.plantDocAnswer?.PlantDocAnswerList.count)! * 210), getParentHeight() * 0.3 )
            plantDocAnswerHeight.constant = height
            self.bottomPadding.constant = 10
            self.answerTableView.reloadData()
            //            self.parent?.docTableView.reloadData()
        }
        else{
            frageLabel.text = ""
            frageFirstAnswerLabel.text = ""
            plantDocAnswerHeight.constant = 0
            self.bottomPadding.constant = -5
        }
        
    }
    
    @IBAction func onMoreInfo(_ sender: Any) {
        if (descriptionDelegate != nil && self.plantDocData != nil){
            descriptionDelegate?.goToPlantDocDetails(id: self.plantDocData!.QuestionId, self)
        }
    }
    
    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }
}

extension PlantDocSimpleCell: UITableViewDelegate, UITableViewDataSource{
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        if isDetailExtended{
            return (self.plantDocAnswer?.PlantDocAnswerList.count) ?? 0
        }
        else{
            return 0
        }
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        
        let cell = tableView.dequeueReusableCell(withIdentifier: "plantDocAnswerCell", for: indexPath) as! PlantDocAnswerTableViewCell
        let data = self.plantDocAnswer?.PlantDocAnswerList[indexPath.row]
//        cell.parent = self
        cell.dateLabel.text = data?.Date!.toDateString(output: "dd.MM.yyyy")
        cell.senderLabel.text = "Antwort von " + data!.AutorName
        cell.contentLabel.text = data?.AnswerText
        
        if data!.IsAdminAnswer{
            cell.outerView.backgroundColor = UIColor(named: "GardifyGreen")
            cell.dateLabel.textColor = .white
            cell.senderLabel.textColor = .white
            cell.contentLabel.textColor = .white
        }
        
        cell.configureCell()
        
        return cell
    }
    
    
    
    
}
