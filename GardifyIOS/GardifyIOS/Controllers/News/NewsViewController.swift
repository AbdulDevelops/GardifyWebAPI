//
//  NewsViewController.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 26.10.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class NewsViewController: UIViewController {

    var newsData: NewsModel?
    var instaData: [NewsInstaDataModel] = []
    var newsEntries: [NewsEntryModel] = []
    
    var imageListInsta: [String: UIImage?] = [:]
    var imageListNews: [String?: UIImage?] = [:]
    
    @IBOutlet weak var instaNewsButton: UIButton!
    @IBOutlet weak var gardifyNewsButton: UIButton!
    @IBOutlet var newsTableView: FullTableView!
    
    @IBOutlet weak var listViewImage: UIImageView!
    @IBOutlet weak var gridViewImage: UIImageView!
    
    @IBOutlet weak var countImage: UIImageView!
    @IBOutlet weak var countLabel: UILabel!
    
    
    var isGeneralNewsFinished = false
    var isInstaNewsFinished = false
    
    
    var isNormalNews: Bool = false
    var isListMode: Bool = true
    var videoURL: String = ""
    
    
    override func viewDidLoad() {
        super.viewDidLoad()

        self.applyTheme()
        self.configurePadding()
        
        pageConfiguration()
        
        updateCounter()
        
        
        
        
        self.showSpinner(onView: self.view)
        
        DispatchQueue.global(qos: .background).async {
            self.getInstaPost()
            self.getNews()
        }
        
     
        // Do any additional setup after loading the view.
    }
    
    func updateCounter() {
        updateLastNewsTime(completion: {result in
            let newsCount = result
            self.countImage.alpha = 0
            self.countLabel.text = ""
            if newsCount > 1 {
                self.countImage.alpha = 1
                self.countLabel.text = "\(newsCount)"
            }
        })
    }
    
    func pageConfiguration(){
        self.newsTableView.backgroundColor = .systemBackground
        self.newsTableView.addBorderRadius()
        
        updateNewsModeButton()
    }
    
    func updateLoader(){
        if isGeneralNewsFinished && isInstaNewsFinished{
            self.removeSpinner()
        }
    }
    
    override func viewWillAppear(_ animated: Bool) {
        super.viewWillAppear(animated)
        self.updateNavigationBar(isMain: false, "GARDIFY", "NEWS", "main_News")

    }
    
    @IBAction func onGartenInstaClick(_ sender: Any) {
        isNormalNews = false
        updateNewsModeButton()
        self.newsTableView.reloadData()
    }
    
    @IBAction func onNormalGartenClick(_ sender: Any) {
        isNormalNews = true
        updateNewsModeButton()
        updateCounter()
        self.newsTableView.reloadData()
    }
    
    @IBAction func onListClick(_ sender: Any) {
        if isListMode {
            return
        }
        self.listViewImage.image = UIImage(named: "List_View_Dark")
        self.gridViewImage.image = UIImage(named: "Grid_View_Light")
        
        
        isListMode = true
        self.newsTableView.reloadData()
    }
    
    @IBAction func onGridClick(_ sender: Any) {
        if !isListMode {
            return
        }
        self.listViewImage.image = UIImage(named: "List_View_Light")
        self.gridViewImage.image = UIImage(named: "Grid_View_Dark")
        isListMode = false
        self.newsTableView.reloadData()
    }
    
    
    func updateNewsModeButton(){
        
        if isNormalNews{
            self.gardifyNewsButton.setCustomColorButton(color: rgb(119, 159, 121))
//            self.gardifyNewsButton.backgroundColor = rgb(119, 159, 121)
            self.instaNewsButton.setWhiteButton()
            
            
        }
       else{
            self.gardifyNewsButton.setWhiteButton()
        self.instaNewsButton.setCustomColorButton(color: rgb(119, 159, 121))
//            self.instaNewsButton.backgroundColor =
        }
        
        //Manually overwrite border radius for both buttons
        instaNewsButton.layer.cornerRadius = 5
        gardifyNewsButton.layer.cornerRadius = 5
    }
    
    
    func getNews(){
//        let params: [String: Any?] = [
//            "take" : 10
//        ]
//
        NetworkManager().requestDataAsync(type: NewsModel.self, APP_URL.NEWS_LIST+"?take=10", printRequest: false){response in
            
            if !response.success{
                self.ShowAlert(message: response.result as! String)
                self.isGeneralNewsFinished = true
                self.updateLoader()
                return
            }
            self.isGeneralNewsFinished = true
            
            DispatchQueue.main.async {
                self.updateLoader()
                
                self.newsData = response.result as! NewsModel
                setLastNewsTime()
                if self.newsData != nil{
                    self.newsEntries = self.newsData!.ListEntries
                    self.getAllNewsImage()
                    self.newsTableView.reloadData()
                }
            }
            
        }
        
    }
    
    
    
    func getAllInstaImage(){
        
        let imageUrlList = self.instaData.reduce(into: [(String,String)]()){output, input in
            let raw = input.media_type == "VIDEO" ? input.thumbnail_url! : input.media_url
            output.append(((raw),"\(input.id)"))
        }
        
                self.imageListInsta = imageUrlList.reduce(into: [String: UIImage?]()){output, input in
                    output[input.1] = nil
                }
        
        
        var cnt = 0
        DispatchQueue.main.async {
            
            for url in imageUrlList{
                let currentCnt = cnt
                getImageFiles(files: [url.0]){image in
                    self.imageListInsta[url.1] = image[0]
                    self.newsTableView.reloadData()
                    print("image order", currentCnt, "finished")
                }
                cnt += 1
            }
        }
        
    }
    
    func getAllNewsImage(){
        
        let newsImageUrlList = self.newsEntries.reduce(into: [(String,String)]()){output, input in
            var raw = input.EntryImages![0].SrcAttr
            output.append(((raw?.toURL(false, false))!,"\(input.Id)"))
        }
        
        self.imageListNews = newsImageUrlList.reduce(into: [String: UIImage?]()){output, input in
            output[input.1] = nil
        }
        
        
        var cnt = 0
        DispatchQueue.main.async {
            
            for url in newsImageUrlList{
                let currentCnt = cnt
                getImageFiles(files: [url.0]){image in
                    self.imageListNews[url.1] = image[0]
                    self.newsTableView.reloadData()
                    print("image order", currentCnt, "finished")
                }
                cnt += 1
            }
        }
        
    }
    
    func getInstaPost(){
        
        NetworkManager().requestDataAsync(type: NewsInstaModel.self, APP_URL.NEWS_INSTA_LIST, printRequest: true){response in
            
            if !response.success{
                self.ShowAlert(message: response.result as! String)
                self.isInstaNewsFinished = true
                self.updateLoader()
                return
            }
//            self.removeSpinner()

            self.isInstaNewsFinished = true
            
            DispatchQueue.main.async {
                self.updateLoader()
                self.instaData = (response.result as! NewsInstaModel).data
                self.getAllInstaImage()
                
                self.newsTableView.reloadData()
            }
            
        }
    }
    
    func goToVideoView(videoURL: String){
        if videoURL == ""
        {
            return
        }
        performSegue(withIdentifier: "videoView", sender: videoURL)
    }

    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        
        if segue.identifier == "videoView" {
            let controller = segue.destination as! InstaVideoViewController
            controller.videoURL = sender as! String
            
        }
        
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




extension NewsViewController: UITableViewDelegate, UITableViewDataSource{
    
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        if isNormalNews{
            if isListMode {
                return self.newsEntries.count
            }
            
            else {
                return self.newsEntries.count/2 + self.newsEntries.count%2
            }
            
        }
        
        if isListMode {
        return self.instaData.count
        }
        else {
            return self.instaData.count/2 + self.instaData.count%2
        }
    }
    

    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        
        if isListMode{
            
                let cell = tableView.dequeueReusableCell(withIdentifier: "newscell", for: indexPath) as! NewsTableViewCell
            
            cell.backgroundColor = .Background
            cell.contentView.backgroundColor = .Background
            self.newsTableView.backgroundColor = .Background

                
            cell.viewController = self
                cell.parent = self.newsTableView
                cell.onConfigure()
            cell.configureMediaType()
                cell.newsImage.image = UIImage(named: "NewsPlaceholder")
            cell.videoButton.setImage(nil, for:.normal)
            cell.videoButton.tintColor = .gray
//            cell.videoButton.setImage(UIImage(systemName: "play.circle"), for:.normal)


                if isNormalNews {
                cell.titleLabel.text = self.newsEntries[indexPath.row].Title
                    cell.titleLabel.font = UIFont.boldSystemFont(ofSize: 16.0)
                cell.contentLabel.text = self.newsEntries[indexPath.row].Text
                if self.imageListNews.count <= indexPath.row{
                    return cell
                }
                    
                    //cell.videoUrl = self.instaData[indexPath.row].media_url == "VIDEO" ? self.instaData[indexPath.row].media_url : ""
                guard let image = self.imageListNews["\(newsEntries[indexPath.row].Id)"] else {
                    return cell
                }
                    cell.newsImage.image = image
                }
                
                else {
                    cell.titleLabel.text = ""
                    cell.contentLabel.text = self.instaData[indexPath.row].caption
                    cell.newsImage.image = UIImage(named: "NewsPlaceholder")
                    cell.videoUrl = self.instaData[indexPath.row].media_type == "VIDEO" ? self.instaData[indexPath.row].media_url : ""
//                    cell.videoButton.setImage(nil, for: .normal)
                    if self.instaData[indexPath.row].media_type == "VIDEO"
                    {
                        cell.videoButton.setImage(UIImage(systemName: "play.circle"), for:.normal)
                    }
                    
                    
                    //cell.videoButton.setImage(UIImage(named: "play.fill")?.withRenderingMode(.alwaysOriginal), for:[])
                    
                    
                    cell.configureMediaType()
//                    if  self.instaData[indexPath.row].media_type == "VIDEO"
//                    {
//                        cell.newsImage.alpha = 0
//                        cell.newsVideoView.alpha = 1
//
//
//                        let url = self.instaData[indexPath.row].media_url
//                        guard let urlText = URL(string: url) else{
//                            print("failed to load url for ", url)
//                            return cell
//                        }
//
//                        cell.newsVideoView.load(URLRequest(url: urlText))
//
//                        return cell
//
//                    }
                    
                    
                    
                    if self.imageListInsta.count <= indexPath.row{
                        return cell
                    }
                    
                    guard let image = self.imageListInsta["\(instaData[indexPath.row].id)"] else {
                        return cell
                    }
                    cell.newsImage.image = image
                }
                
                return cell
            }
            
        
        else{
            
            let cell = tableView.dequeueReusableCell(withIdentifier: "newsGridcell", for: indexPath) as! NewsGridViewCell
            
            self.newsTableView.backgroundColor = .systemBackground
            cell.clearBackground()
            //cell.backgroundColor = .gray
            
            cell.newsPlayButton1.setImage(nil, for:.normal)
            cell.newsPlayButton2.setImage(nil, for:.normal)
            cell.viewController = self
            
            if isNormalNews {
            if self.imageListNews.count <= indexPath.row{
                return cell
            }
            
            guard let image1 = self.imageListNews["\(newsEntries[(indexPath.row)*2].Id)"] else {
                return cell
            }
            
            cell.newsImage1.image = image1
                
            
            if imageListNews.count > indexPath.row*2+2 {
            guard let image2 = self.imageListNews["\(newsEntries[indexPath.row*2+1].Id)"] else {
                return cell
            }
            
            cell.newsImage2.image = image2
            }
            }
            
            else {
                if self.imageListInsta.count <= indexPath.row{
                    return cell
                }
                
                guard let image1 = self.imageListInsta["\(instaData[(indexPath.row)*2].id)"] else {
                    return cell
                }
                
                cell.newsImage1.image = image1
                
                cell.videoUrl1 = self.instaData[(indexPath.row)*2].media_type == "VIDEO" ? self.instaData[(indexPath.row)*2].media_url : ""
                if self.instaData[(indexPath.row)*2].media_type == "VIDEO"
                {
                    cell.newsPlayButton1.setImage(UIImage(systemName: "play.circle"), for:.normal)
                    cell.newsPlayButton1.tintColor = .gray
                }
                
                
                if imageListInsta.count > indexPath.row*2+2 {
                    cell.videoUrl2 = self.instaData[(indexPath.row)*2+1].media_type == "VIDEO" ? self.instaData[(indexPath.row)*2+1].media_url : ""
                    if self.instaData[(indexPath.row)*2+1].media_type == "VIDEO"
                    {
                        cell.newsPlayButton2.setImage(UIImage(systemName: "play.circle"), for:.normal)
                        cell.newsPlayButton2.tintColor = .gray
                    }
                    guard let image2 = self.imageListInsta["\(instaData[indexPath.row*2+1].id)"] else {
                        return cell
                    }
                    
                    cell.newsImage2.image = image2
                }
            }
            
            return cell
        }
    }
}
        
        

