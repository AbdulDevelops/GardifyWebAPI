//
//  VideoListViewController.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 26.10.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit
import WebKit
import youtube_ios_player_helper


class VideoListViewController: UIViewController {
    

    @IBOutlet weak var scrollView: UIScrollView!
    
    @IBOutlet weak var videoTableView: UITableView!
    
    @IBOutlet weak var searchLabel: UILabel!
    
    @IBOutlet weak var sortLabel: UILabel!
    
    @IBOutlet weak var topicLabel: UILabel!
    
    @IBOutlet weak var sortButton: UIButton!
    
    @IBOutlet weak var topicButton: UIButton!
    
    @IBOutlet weak var searchButton: UIButton!
    
    @IBOutlet weak var topicDropdownTable: UITableView!
    @IBOutlet weak var topicDropdownHeight: NSLayoutConstraint!
    
    @IBOutlet weak var sortDropdownTable: UITableView!
    @IBOutlet weak var sortDropdownHeight: NSLayoutConstraint!
    
    @IBOutlet weak var filterView: UIView!
    @IBOutlet var viewHeight: NSLayoutConstraint!
    
    @IBOutlet weak var topicArrowImage: UIImageView!
    
    @IBOutlet weak var sortArrowImage: UIImageView!
    
    @IBOutlet weak var sortLabelBorder: UIView!
    
    @IBOutlet weak var topicLabelBorder: UIView!
    
    @IBOutlet weak var outerView: UIView!
    
    var isSearchextended : Bool = false
    
    var videoListData: [VideoModel] = []
    var tmpVideoListData: [VideoModel] = []
    
    var videoRequestList: [URLRequest?] = []
    
    var topicsList:[String] = ["-"]
    
    var sortList:[String] = ["-","Beliebtheit","Datum"]
    
    var topicDropdownObject = FilterDropdown()
    var sortDropdownObject = FilterDropdown()
    
    
    var selectedTopic: String = "-"
    var selectedSort: String = "-"
    
    var isTopicDropdownExtended: Bool = false
    var isSortDropdownExtended: Bool = false
    
    enum VideoFilterType {
        case sort
        case topic
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()

        pageConfiguration()
        self.showSpinner(onView: self.view)
        DispatchQueue.global(qos: .background).async {
            self.getAllVideo()

        }
        
        // Do any additional setup after loading the view.
    }
    
    override func viewWillAppear(_ animated: Bool) {
        super.viewWillAppear(animated)
        self.updateNavigationBar(isMain: false, "", "Video", "main_video")

    }
    
    func pageConfiguration(){
        
        self.applyTheme()
        self.configurePadding()
        self.PageConfiguration()
        
        self.updateSearchDropdown()
        
        self.videoTableView.backgroundColor = .clear
        
        
    }
    
    
    
    func PageConfiguration() {
        self.searchLabel.addBorderRadius()
        self.searchLabel.backgroundColor = .ViewBackground
        
        self.sortLabel.addBorderRadius()
        self.sortLabel.backgroundColor = .ViewBackground
        
        self.topicLabel.addBorderRadius()
        self.topicLabel.backgroundColor = .ViewBackground
        
        
        //self.filterView.addBorderRadius()
        self.filterView.backgroundColor = .ViewBackground
        
        self.topicButton.setCustomSearchButton()
        self.sortButton.setCustomSearchButton()
        
        self.outerView.addBorderRadius()
        
//        self.searchLabel.addCustomBorderRadius(topLeft: true, topRight: true, botLeft: false, botRight: false)
//        self.searchButton.addCustomBorderRadius(topLeft: true, topRight: true, botLeft: false, botRight: false)
//        
//        self.filterView.addCustomBorderRadius(topLeft: false, topRight: false, botLeft: true, botRight: true)
        
    }
    
    
    @IBAction func sortButtonClicked(_ sender: Any) {
        isSortDropdownExtended = !isSortDropdownExtended
        if isSortDropdownExtended{
            if self.sortList.count > 0 {
                sortDropdownHeight.constant = CGFloat(dropdownFilterHeight) * CGFloat(self.sortList.count)
            }
        }
        else {
            sortDropdownHeight.constant = 0
        }
    }
    

    @IBAction func onSearchButtonClick(_ sender: Any) {
        isSearchextended = !isSearchextended
        self.updateSearchDropdown()
    }
    
    func updateSearchDropdown() {
        if isSearchextended {
            if viewHeight != nil {
                viewHeight.isActive = false
            }
            
            
            setAlphas()
            
        }
        else {
            viewHeight.constant = 0
            viewHeight.isActive = true
            
            setAlphas()
        }
    }
    
    func setAlphas() {
        if isSearchextended {
            self.topicLabel.alpha = 1
            self.sortLabel.alpha = 1
            self.topicArrowImage.alpha = 1
            self.sortArrowImage.alpha = 1
            self.sortLabelBorder.alpha = 1
            self.topicLabelBorder.alpha = 1
        }
        else {
            
            self.topicLabel.alpha = 0
            self.sortLabel.alpha = 0
            self.topicArrowImage.alpha = 0
            self.sortArrowImage.alpha = 0
            self.sortLabelBorder.alpha = 0
            self.topicLabelBorder.alpha = 0
        }
    }
    
    @IBAction func topicButtonClicked(_ sender: Any) {
        isTopicDropdownExtended = !isTopicDropdownExtended
        if isTopicDropdownExtended{
            if self.topicsList.count > 0 {
                topicDropdownHeight.constant = CGFloat(dropdownFilterHeight) * CGFloat(self.topicsList.count)
            }
        }
        else{
            topicDropdownHeight.constant = 0
        }
    }
    
    func configureTopicDropdown(){
        self.topicDropdownObject.filterDelegate = self
        self.topicDropdownObject.tagSelectList = self.topicsList
        self.topicDropdownObject.filterType = VideoListViewController.VideoFilterType.topic
        
        self.topicDropdownTable.delegate = self.topicDropdownObject
        self.topicDropdownTable.dataSource = self.topicDropdownObject
        self.topicDropdownTable.reloadData()
    }
    
    func configureSortDropdown(){
        self.sortDropdownObject.filterDelegate = self
        self.sortDropdownObject.tagSelectList = self.sortList
        self.sortDropdownObject.filterType = VideoListViewController.VideoFilterType.sort
        
        self.sortDropdownTable.delegate = self.sortDropdownObject
        self.sortDropdownTable.dataSource = self.sortDropdownObject
        self.sortDropdownTable.reloadData()
    }

    func getAllVideo(){
            
        NetworkManager().requestDataAsync(type: [VideoModel].self, APP_URL.VIDEO_LIST){response in
            
            if !response.success{
                self.ShowAlert(message: response.result as! String)
                self.removeSpinner()
                return
            }
            self.removeSpinner()
            self.videoListData = response.result as! [VideoModel]
            self.tmpVideoListData = self.videoListData
            
            DispatchQueue.main.async {
                self.loadUrlRequest()
                setLastVideoTime()
                self.fillTopicsList()
                self.configureTopicDropdown()
                self.configureSortDropdown()
                self.videoTableView.reloadData()
            }
            
        }
        
    }
    
    func fillTopicsList() {
        for video in self.videoListData {
            self.topicsList.append(contentsOf: video.Tags)
        }
        self.topicsList = self.topicsList.removingDuplicates()
        self.topicsList = self.topicsList.sorted()
        print("topics ", self.topicsList)
    }
    
    func updateTopicDropdown(value:String) {
        isTopicDropdownExtended = false
        
        if value == "-" {
            self.videoListData = self.tmpVideoListData
        }
        else {
            self.videoListData = self.tmpVideoListData.filter{$0.Tags.contains(value)}
        }
        
        self.topicDropdownObject.selectedItem = value
        self.topicLabel.text = value
        self.topicDropdownTable.reloadData()
        self.videoTableView.reloadData()

        self.topicDropdownHeight.constant = CGFloat(0)
        //self.videoListData = tmp
    }
    
    func updateSortDropdown(value:String) {
        isSortDropdownExtended = false
        if value == "-" {
            self.sortLabel.text = value
            self.sortDropdownHeight.constant = CGFloat(0)
            return
        }
        if value == "Beliebtheit" {
            self.videoListData = self.videoListData.sorted(by: {$0.ViewCount < $1.ViewCount})
        }
        else if value == "Datum" {
            self.videoListData = self.videoListData.sorted(by: {$0.Date < $1.Date})
        }
        
        self.sortDropdownObject.selectedItem = value

        self.sortLabel.text = value
        self.sortDropdownTable.reloadData()
        self.videoTableView.reloadData()

        self.sortDropdownHeight.constant = CGFloat(0)
    }
    
    func loadUrlRequest(){
        
        self.videoRequestList = []
        
//        for data in videoListData{
//            let urlId = data.YTLink.split(separator: "=")[1]
//            guard let urlText = URL(string: "https://www.youtube.com/embed/\(urlId)") else{
//                print("failed to load url for ", data.Title)
//                videoRequestList.append(nil)
//                continue
//            }
//
//            videoRequestList.append(URLRequest(url: urlText))
//        }
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

extension VideoListViewController: UITableViewDelegate, UITableViewDataSource{
    
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return videoListData.count
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        
        let cell = tableView.dequeueReusableCell(withIdentifier: "videoCell", for: indexPath) as! VideoTableViewCell
        
        cell.onConfigure()
        
        cell.videoTitle.text = self.videoListData[indexPath.row].Title
        cell.videoTitle.textColor = .label

        
       
        let urlId = self.videoListData[indexPath.row].YTLink.split(separator: "=")[1]
        
        let thumbnailImage = "https://img.youtube.com/vi/\(urlId)/hqdefault.jpg"
        
        cell.videoURL = self.videoListData[indexPath.row].YTLink
        
        DispatchQueue.global(qos: .background).async {
            NetworkManager().getImageFromUrl(urlString: thumbnailImage){image in
               
                DispatchQueue.main.async {
                    cell.thumbnailImage.image = image

                }
           }
        }
         
//        if self.videoRequestList[indexPath.row] == nil{
//            return cell
//        }
//
//        cell.videoView.load(self.videoRequestList[indexPath.row]!)
//
        return cell
    }
    
    
    
}

let dropdownFilterHeight = 30

class FilterDropdown: NSObject, UITableViewDelegate, UITableViewDataSource{
    
    var filterDelegate: VideoListViewController?
    var tagSelectList: [String] = []
    var filterType: VideoListViewController.VideoFilterType?
    var selectedItem: String = "-"
    
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return tagSelectList.count
    }
    
    func tableView(_ tableView: UITableView, heightForRowAt indexPath: IndexPath) -> CGFloat {
        return CGFloat(dropdownGroupFilterHeight)
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = UITableViewCell()
        
        cell.textLabel?.text = tagSelectList[indexPath.row]
        cell.textLabel?.textColor = .label
        
        print("index is", tableView.indexPathForSelectedRow, tagSelectList[indexPath.row])
        if selectedItem != tagSelectList[indexPath.row] && indexPath.row == 0{
            cell.textLabel?.text = "löschen"
            cell.textLabel?.textColor = .systemRed
        }
        
        return cell
    }
    
    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        print(self.tagSelectList[indexPath.row])
        switch filterType {
        case .topic:
            self.filterDelegate?.updateTopicDropdown(value:self.tagSelectList[indexPath.row])
        case .sort :
            self.filterDelegate?.updateSortDropdown(value:self.tagSelectList[indexPath.row])
        case .none:
            return
        }
        
    }
    
    func tableView(_ tableView: UITableView, didDeselectRowAt indexPath: IndexPath) {
        print(self.tagSelectList[indexPath.row])
        switch filterType {
        case .topic:
            
            self.filterDelegate?.updateTopicDropdown(value:self.tagSelectList[indexPath.row])
        case .sort :
            self.filterDelegate?.updateSortDropdown(value:self.tagSelectList[indexPath.row])
        case .none:
            return
        }
    }
    
//    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
//        switch filterType {
//        case .plantFamily:
//            self.updateMainTags(value: self.tagSelectList[indexPath.row].t, name: self.tagSelectList[indexPath.row].t)
//
//        default:
//            self.updateMainTags(value: self.tagSelectList[indexPath.row].id, name: self.tagSelectList[indexPath.row].t)
//
//        }
//    }
    
    
}
