//
//  VideoViewController.swift
//  GardifyIOS
//
//  Created by Rifat Hussain on 09.09.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation
import WebKit
import youtube_ios_player_helper


class VideoViewController: UIViewController{
    
    @IBOutlet weak var videoPlayerView: WKWebView!
    
    override func viewDidLoad() {
        super.viewDidLoad()
        getVideo()
        
    }
    
    override func viewWillAppear(_ animated: Bool) {
        super.viewWillAppear(animated)
        self.updateNavigationBar(isMain: false, "", "Video", "main_video")

    }
    
    func getVideo(){
        let videoId = "zOc18eOb9Fg"
//        guard let gardifyURL = URL(string: "https://www.youtube.com/watch?time_continue=20&v=zOc18eOb9Fg&feature=emb_logo")
//            else{
//                return
//        }
        
        guard let gardifyURL = URL(string: "https://www.youtube.com/embed/zOc18eOb9Fg")
        else{
            return
        }
        videoPlayerView.load(URLRequest(url: gardifyURL))
    }
    
    
}




