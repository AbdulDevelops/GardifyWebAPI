//
//  NewsModel.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 26.10.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation

struct NewsModel: Codable{
    
    let ListEntries: [NewsEntryModel]
}

struct NewsEntryModel: Codable{
    
    let Theme, Title, Text: String
    let Id:Int
    let Date: String
    
    var EntryImages: [ImageFile]?

    

}

struct NewsInstaModel: Codable{
    let data: [NewsInstaDataModel]
    let paging: NewsInstaPagingModel
}

struct NewsInstaDataModel: Codable{
    let id, media_url, media_type, username, timestamp: String
    let thumbnail_url, caption: String?

}

struct NewsInstaPagingModel: Codable{
    let next, prev: String?
}
