//
//  InstaVideoViewController.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 11.11.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit
import WebKit

class InstaVideoViewController: UIViewController {

    var videoURL: String = ""
    
    @IBOutlet weak var videoView: WKWebView!
    override func viewDidLoad() {
        super.viewDidLoad()

        self.configurePadding()
       
        // Do any additional setup after loading the view.
    }
    
    override func viewDidAppear(_ animated: Bool) {
        self.loadVideo()
    }
    
    
    
    func loadVideo()
    {
        let videoUrl:URL = URL(string: self.videoURL)!
        let request:URLRequest = URLRequest(url: videoUrl)
        videoView.load(request)
        
        return
    }
    
    
    

    /*
    // MARK: - Navigation

    // In a storyboard-based application, you will often want to do a little preparation before navigation
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        // Get the new view controller using segue.destination.
        // Pass the selected object to the new view controller.
    }
    */

}
