//
//  ShopViewController.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 28.10.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit
import WebKit

class ShopViewController: UIViewController{

    @IBOutlet weak var shopWebView: WKWebView!
    override func viewDidLoad() {
        super.viewDidLoad()

        self.configurePadding()

        
        // Do any additional setup after loading the view.
    }
    
    override func viewWillAppear(_ animated: Bool) {
        super.viewWillAppear(animated)
        self.updateNavigationBar(isMain: false, "GARDIFY", "SHOP", "main_shop")

        pageConfiguration()
    }
    
//    override func loadView() {
//        let webConfiguration = WKWebViewConfiguration()
//        shopWebView = WKWebView(frame: .zero, configuration: webConfiguration)
//        shopWebView.uiDelegate = self
////        view = shopWebView
//    }
    
    func pageConfiguration(){
        let myURL = URL(string: "https://shop.gardify.de")
        let myRequest = URLRequest(url: myURL!)
        shopWebView.load(URLRequest(url: URL(string:"about:blank")!))
        shopWebView.load(myRequest)
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
