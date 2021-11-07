//
//  VideoTableViewCell.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 26.10.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit
import WebKit
import youtube_ios_player_helper

class VideoTableViewCell: UITableViewCell {

    @IBOutlet weak var videoView: WKWebView!
    
    @IBOutlet weak var videoTitle: UILabel!
    @IBOutlet weak var outerView: UIView!
    
    @IBOutlet weak var thumbnailImage: UIImageView!
    
    var videoURL: String = ""
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }
    
    func onConfigure(){
        self.backgroundColor = .clear
        self.contentView.backgroundColor = .clear
        
        outerView.addBorderRadius()
    }

    @IBAction func onLinkClicked(_ sender: Any) {
        
        if let url = URL(string: self.videoURL) {
            UIApplication.shared.open(url)
        }
        
    }
    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}
