//
//  NewsTableViewCell.swift
//  GardifyIOS
//
//  Created by Netzlab on 27.10.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class NewsGridViewCell: UITableViewCell {
    
    @IBOutlet var newsImage1: UIImageView!
    
    @IBOutlet var newsImage2: UIImageView!
    
    
    @IBOutlet weak var newsPlayButton1: UIButton!
    
    @IBOutlet weak var newsPlayButton2: UIButton!
    
    var videoUrl1:String=""
    var videoUrl2:String=""
    
    var isDropdown: Bool = false
    var parent: FullTableView?
    var viewController: NewsViewController?
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }
    
    func onConfigure(){
        self.backgroundColor = .clear
        self.contentView.backgroundColor = .clear
        //self.outerView.addBorderRadius()
    }
    
//    func updateDropdownHeight(){
//
//        if isDropdown{
//            contentLabel.numberOfLines = 0
//            arrowImageDown.flipXAxis()
//        }
//        else{
//            contentLabel.numberOfLines = 3
//            arrowImageDown.revertFlip()
//        }
//
//        parent?.reloadData()
//    }
//
//    @IBAction func onDropdownClicked(_ sender: Any) {
//
//        isDropdown = !isDropdown
//        updateDropdownHeight()
//    }
    
    @IBAction func onImage1Click(_ sender: Any) {
        self.viewController?.goToVideoView(videoURL: self.videoUrl1)
    }
    
    
    @IBAction func onImage2Click(_ sender: Any) {
        self.viewController?.goToVideoView(videoURL: self.videoUrl2)
    }
    
    
    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)
        
        // Configure the view for the selected state
    }
    
}
