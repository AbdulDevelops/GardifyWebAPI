//
//  NewsTableViewCell.swift
//  GardifyIOS
//
//  Created by Netzlab on 27.10.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit
import WebKit

class NewsTableViewCell: UITableViewCell {

    @IBOutlet var outerView: UIView!
    @IBOutlet var newsImage: UIImageView!
    @IBOutlet var titleLabel: UILabel!
    @IBOutlet var contentLabel: UILabel!
    @IBOutlet weak var arrowImageDown: UIImageView!
    //@IBOutlet weak var newsVideoView: WKWebView!
    @IBOutlet weak var dropdownButton: UIButton!
    
  
    @IBOutlet weak var innerView: UIView!
    
    @IBOutlet weak var videoButton: UIButton!
    var isDropdown: Bool = false
    var parent: FullTableView?
    
    var videoUrl: String = ""
    
    var viewController: NewsViewController?
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }
    
    func onConfigure(){
        
        self.innerView.addBorderRadius()
        self.backgroundColor = .clear
        self.contentView.backgroundColor = .clear
        
        //self.outerView.addBorderRadius()
    }
    
    func configureMediaType(){
        self.newsImage.alpha = 1
        //self.newsVideoView.alpha = 0
    }
    
    func updateDropdownHeight(){
        
        if isDropdown{
            contentLabel.numberOfLines = 0
            arrowImageDown.flipXAxis()
        }
        else{
            contentLabel.numberOfLines = 5
            arrowImageDown.revertFlip()
        }
    }
    
    @IBAction func onDropdownClicked(_ sender: Any) {
        
        isDropdown = !isDropdown
        updateDropdownHeight()
        parent?.reloadData()

    }
    
    @IBAction func onImageClick(_ sender: Any) {
        
        self.viewController?.goToVideoView(videoURL: self.videoUrl)
    }
    
    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}
