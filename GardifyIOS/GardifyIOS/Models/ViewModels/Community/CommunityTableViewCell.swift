//
//  CommunityTableViewCell.swift
//  GardifyIOS
//
//  Created by Rifat Hussain on 04.09.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class CommunityTableViewCell: UITableViewCell{
    @IBOutlet weak var titleLabel: UILabel!
    @IBOutlet weak var detailsLabel: UILabel!
    @IBOutlet weak var urlLabel: UILabel!
    @IBOutlet weak var moderatorLabel: UILabel!
    @IBOutlet weak var dropdownImage: UIImageView!
    @IBOutlet weak var outerView: UIView!
    @IBOutlet weak var bottomConstraint: NSLayoutConstraint!
    
    var delegate :CommunityViewController?
    var link : String = ""
    
    var detailText: String = ""
    var urlText: NSAttributedString?
    var modText: NSAttributedString?
    var isExtended: Bool = false
    
    var index: Int = -1
    
    var parent: CommunityViewController?
    
    override func awakeFromNib() {
        super.awakeFromNib()
    }
    
    func configureCell(community: CommunityData){
        
        if delegate == nil {
            
            return
        }
        
     
        isExtended = community.isExtended
        titleLabel?.text = "\(community.title)"
//        detailsLabel?.text = "\(community.details)"
        detailText = community.details
        urlText = getGreenLink(text: community.url)
        
        self.link = community.url
//        urlLabel?.attributedText = getGreenLink(text: community.url)
//        moderatorLabel?.text = "\(community.moderator)"
        
//        moderatorLabel.attributedText = getBoldStart(boldText: "Moderatoren:", fullText: community.moderator)
        modText = getBoldStart(boldText: "Moderatoren:", fullText: community.moderator)
        updateExtendedDisplay()
        dropdownImage.image = UIImage(named: community.dropdownImage)
        outerView.addBorderRadius()
        
        
    }
    
    func updateExtendedDisplay(){
        if isExtended{
            urlLabel?.attributedText = urlText
            moderatorLabel.attributedText = modText
            
            detailsLabel?.text = detailText
            dropdownImage.flipXAxis()
            bottomConstraint.constant = 10
        }
        else{
            urlLabel?.attributedText = NSAttributedString(string: "")
            moderatorLabel.attributedText = NSAttributedString(string: "")
            
            detailsLabel?.text = ""
            dropdownImage.revertFlip()
            bottomConstraint.constant = -50
        }
    }
    
    @IBAction func onLinkClicked(_ sender: Any) {
        if let url = URL(string: self.link), UIApplication.shared.canOpenURL(url) {
            UIApplication.shared.openURL(url)
        }
    }
    
    @IBAction func onExtendedClick(_ sender: Any) {
        
        self.parent?.updateItemExtendStatus(index: self.index, status: !isExtended)
        
        isExtended = !isExtended
        self.parent?.communityTableView.reloadData()
        
    }
    
    func getGreenLink(text: String)-> NSAttributedString{
        let range = (text as NSString).range(of: text)
        let attributedString = NSMutableAttributedString(string:text)
        attributedString.addAttribute(NSAttributedString.Key.foregroundColor, value: UIColor(named: "GardifyGreen")! , range: range)
        attributedString.addAttribute(NSAttributedString.Key.underlineColor, value: UIColor(named: "GardifyGreen")! , range: range)
        
        attributedString.addAttribute(NSAttributedString.Key.underlineStyle, value: NSUnderlineStyle.single.rawValue , range: range)
        
        return attributedString
    }
    
    
    func getBoldStart(boldText: String, fullText: String)-> NSAttributedString{
        
        let text = fullText
        let range = (text as NSString).range(of: boldText)
        let attributedString = NSMutableAttributedString(string:text)

        
        attributedString.addAttribute(NSAttributedString.Key.font, value: UIFont.systemFont(ofSize: 14, weight: .semibold) , range: range)
        return attributedString

    }
}
