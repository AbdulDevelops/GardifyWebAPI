//
//  PlantDocAnswerTableViewCell.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 15.10.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class PlantDocAnswerTableViewCell: UITableViewCell {

    @IBOutlet weak var dateLabel: UILabel!
    @IBOutlet weak var senderLabel: UILabel!
    @IBOutlet weak var contentLabel: UILabel!
    @IBOutlet weak var outerView: UIView!
    
    @IBOutlet weak var adminImage: UIImageView!
    var isExtended: Bool = false
    var parent: PlantDocViewCell?
    var tableParent: UITableView?
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }

    func configureCell(){
        self.backgroundColor = .clear
        self.contentView.backgroundColor = .clear
        self.outerView.addBorderRadius()
    }
    
    @IBAction func onAnswerExtend(_ sender: Any) {
        isExtended = !isExtended
        if isExtended{
            self.contentLabel.numberOfLines = 0
        }
        else{
            self.contentLabel.numberOfLines = 3
        }
        
        self.parent?.answerTableView.reloadData()
        self.tableParent?.reloadData()
//        self.parent?.ShowMessageAlert(title: senderLabel.text ?? "", message:  contentLabel.text ?? "" )
    }
    
    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}
